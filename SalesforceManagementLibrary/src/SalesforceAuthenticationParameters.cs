namespace SalesforceManagementLibrary;

using System.Security.Cryptography;

public class SalesforceAuthenticationParameters
{
    private string? sub;
    private string? aud;
    private string? iss;
    private string? clientSecret;
    private string? clientId;
    private string? privKey;
    private string? pubKey;
    public bool ValidForJWT {
        get => null != this.privKey && null != this.sub && null != this.aud;
    }
    public bool ValidForClientCredentials
    {
        get => null != this.aud && null != this.clientSecret && null != this.clientId;
    }

    public static SalesforceAuthenticationParameters NewInstance()
    {
        return new SalesforceAuthenticationParameters();
    }
    public SalesforceAuthenticationParameters SetPrivateKey(string? key)
    {
        this.privKey = key;
        return this;
    }
    public RSA GetPrivateKey()
    {
        var rsa = RSA.Create();
        rsa.ImportFromPem(this.privKey!.ToCharArray());
        return rsa;
    }
    public SalesforceAuthenticationParameters SetPublicKey(string? key)
    {
        this.pubKey = key;
        return this;
    }
    public RSA GetPublicKey()
    {
        var rsa = RSA.Create();
        rsa.ImportFromPem(this.pubKey!.ToCharArray());
        return rsa;
    }
    public SalesforceAuthenticationParameters SetClientId(string? clientId)
    {
        this.clientId = clientId;
        return this;
    }
    public SalesforceAuthenticationParameters SetIssuer(string? issuer)
    {
        this.iss = issuer;
        return this;
    }
    public SalesforceAuthenticationParameters SetClientSecret(string? clientSecret)
    {
        this.clientSecret = clientSecret;
        return this;
    }
    public string? GetIssuer()
    {
        return this.iss;
    }
    public string? GetClientId()
    {
        return this.clientId;
    }
    public string? GetClientSecret()
    {
        return this.clientSecret;
    }
    public SalesforceAuthenticationParameters SetAudience(Uri? uri)
    {
        if (null != uri) this.aud = uri.ToString();
        return this;
    }
    public string? GetAudience()
    {
        return this.aud;
    }
    public SalesforceAuthenticationParameters SetUsername(string? username)
    {
        this.sub = username;
        return this;
    }
    public string? GetSubject()
    {
        return this.sub;
    }
}