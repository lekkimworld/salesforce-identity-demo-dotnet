using System.Text.Json;
using System.Net.Http.Json;
using SalesforceManagementLibrary.Models;

namespace SalesforceManagementLibrary;

class SalesforceTokenInfo {
    public string? id { get; set; }
    public string? access_token { get; set; }
    public string? instance_url { get; set; }
}

public class SalesforceAPIClient {
    public static string ApiVersion = "v55.0";
    public static Dictionary<string,IEnumerable<string>> ObjectFields;
    public string? InstanceUrl {
        get => this.tokenInfo?.instance_url;
    }
    private SalesforceAuthenticationParameters? param;
    private SalesforceTokenInfo? tokenInfo;
    private HttpClient? client;

    static SalesforceAPIClient() {
        SalesforceAPIClient.ObjectFields = new Dictionary<string,IEnumerable<string>>{
            {"User", new List<string>{"IndividualId","ContactId","AccountId","Email","FirstName","LastName","LanguageLocaleKey","LocaleSidKey","TimeZoneSidKey"}},
        };
    }

    public static async Task<SalesforceAPIClient> BuildFromParams(SalesforceAuthenticationParameters param) {
        var client = new SalesforceAPIClient(param);
        await client.RefreshAccessToken();    
        return client;
    }

    private SalesforceAPIClient(SalesforceAuthenticationParameters param) {
        this.param = param;
    }

    private async Task RefreshAccessToken() {
        FormUrlEncodedContent? content = null;

        if (this.param!.ValidForJWT) {
            // get via jwt
            var jwt = JWTHelper.IssueJWT(this.param!);

            // build content
            content = new FormUrlEncodedContent(new Dictionary<string, string> {
                {"grant_type", "urn:ietf:params:oauth:grant-type:jwt-bearer"},
                {"assertion", jwt}
            });
        } else if (this.param!.ValidForClientCredentials) {
            // get via client credentials
            content = new FormUrlEncodedContent(new Dictionary<string,string> {
                {"grant_type", "client_credentials"},
                {"client_id", this.param!.GetClientId()!},
                {"client_secret", this.param!.GetClientSecret()!}
            });
        } else {
            throw new Exception("Salesforce parameters are not valid for JWT or client_credentials");
        }

        // exchange
        var client = new HttpClient();
        var res = await client.PostAsync($"{this.param!.GetAudience()}/services/oauth2/token", content);
        var body = await res.Content.ReadAsStringAsync();
        
        // parse response and create client
        this.tokenInfo = JsonSerializer.Deserialize<SalesforceTokenInfo>(body);
        this.client = new HttpClient();
        this.client.DefaultRequestHeaders.Add("Authorization", $"Bearer {this.tokenInfo!.access_token}");
    
    }

    private string getBaseDataUri() {
        return $"{this.tokenInfo!.instance_url!}/services/data/{SalesforceAPIClient.ApiVersion}";
    }

    private string getJoinedFieldsForObject(string objectName) {
        var fieldset = SalesforceAPIClient.ObjectFields.ContainsKey(objectName) ? SalesforceAPIClient.ObjectFields[objectName] : null;
        var fields = null == fieldset ? new HashSet<string>() : new HashSet<string>(fieldset);
        fields.Add("Id");
        fields.Add("Name");
        fields.Add("CreatedById");
        if (!objectName.Equals("User")) fields.Add("OwnerId");
        fields.Add("LastModifiedById");
        var strfields = string.Join(',', fields);
        return strfields;
    }

    public async Task<JsonDocument> GetAsync(string uri) {
        if (null == this.client) throw new Exception("No client set");
        var res = await this.client.GetAsync(uri);
        res.EnsureSuccessStatusCode();
        var body = await res.Content.ReadAsStringAsync();
        return JsonDocument.Parse(body);
    }

    public async Task<JsonDocument> PostAsync(string uri, BaseModelObject obj)
    {
        if (null == this.client) throw new Exception("No client set");
        var content = JsonContent.Create(obj.Values);
        var res = await this.client.PostAsync(uri, content);
        res.EnsureSuccessStatusCode();
        var body = await res.Content.ReadAsStringAsync();
        return JsonDocument.Parse(body);
    }

    public async Task<Models.User> GetRunningAsUser() {
        // extract record id
        var fields = this.getJoinedFieldsForObject("User");
        var id = this.tokenInfo!.id!.Substring(this.tokenInfo!.id!.LastIndexOf('/') + 1);
        var json = await this.GetAsync($"{this.getBaseDataUri()}/query?q=SELECT {fields} FROM User WHERE Id='{id}'");

        // get records and just movenext as we always have a current user
        var records = json.RootElement.GetProperty("records").EnumerateArray();
        records.MoveNext();
        return new Models.User(records.Current);
    }

    public async Task<Models.AllRecords<Models.User>> GetAllUsers() {
        // execute query for all users
        var fields = this.getJoinedFieldsForObject("User");
        var uri = $"{this.getBaseDataUri()!}/query?q=SELECT {fields} FROM User";
        var json = await this.GetAsync(uri);
        return AllRecords<User>.FromJson(json);
    }

    public async Task<Models.AllRecords<Models.Account>> GetAllAccounts()
    {
        // execute query for all users
        var fields = this.getJoinedFieldsForObject("Account");
        var uri = $"{this.getBaseDataUri()!}/query?q=SELECT {fields} FROM Account";
        var json = await this.GetAsync(uri);
        return AllRecords<Account>.FromJson(json);
    }

    public async Task Upsert(BaseModelObject obj) {
        Dictionary<string,object>? values = obj.Values;
        if (null == values) return;
        
        if (null != obj.Id) {
            // update
            Console.WriteLine("update " + obj);
        } else {
            // insert
            Console.WriteLine("insert " + obj);
            await this.PostAsync($"{this.getBaseDataUri()!}/sobjects/Account", obj);
        }
    }
}
