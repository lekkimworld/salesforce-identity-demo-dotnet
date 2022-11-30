using SalesforceManagementLibrary;
using SalesforceManagementLibrary.Models;

// load envvars
var root = Directory.GetCurrentDirectory();
var dotenv = Path.Combine(root, ".env");
DotEnv.Load(dotenv);

var endpoint = Environment.GetEnvironmentVariable("SALESFORCE_ENDPOINT")!;
var privateKey = null != Environment.GetEnvironmentVariable("JWT_PRIVATE_KEY")
    ? System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(Environment.GetEnvironmentVariable("JWT_PRIVATE_KEY")!))
    : null;
var publicKey = null != Environment.GetEnvironmentVariable("JWT_PUBLIC_KEY")
    ? System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(Environment.GetEnvironmentVariable("JWT_PUBLIC_KEY")!))
    : null;

var client = await SalesforceAPIClient.BuildFromParams(SalesforceAuthenticationParameters.NewInstance()
    .SetAudience(new Uri(endpoint))
    .SetClientId(Environment.GetEnvironmentVariable("SALESFORCE_CLIENT_ID"))
    .SetClientSecret(Environment.GetEnvironmentVariable("SALESFORCE_CLIENT_SECRET"))
    .SetPrivateKey(privateKey)
    .SetPublicKey(publicKey)
    .SetIssuer(Environment.GetEnvironmentVariable("JWT_ISSUER"))
    .SetUsername(Environment.GetEnvironmentVariable("JWT_SUBJECT")));


dynamic user = await client.GetRunningAsUser();
Console.WriteLine($"Running as user: <{user.Name}> <{user.Id}>");

/*
Console.WriteLine("-----");
Console.WriteLine("Listing all User-records");
var users = await client.GetAllUsers();
while (null != users)
{
    foreach (dynamic u in users)
    {
        Console.WriteLine($"\t{u.Name}");
    }
    users = await users.GetNextRecords(client);
}
*/

/*
// extend Account fields
SalesforceAPIClient.ObjectFields["Account"] = new List<string>{"NumberOfEmployees"};
foreach (dynamic a in await client.GetAllAccounts()) {
    Console.WriteLine($"\t{a.Name} {a.NumberOfEmployees}");
}

dynamic ac = new Account();
ac.Name = "Create Account";
ac.NumberOfEmployees = 123;
await client.Upsert(ac);
*/