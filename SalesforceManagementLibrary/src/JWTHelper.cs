namespace SalesforceManagementLibrary;

using JWT.Algorithms;
using JWT.Builder;

public class JWTHelper
{
    
    public static string IssueJWT(SalesforceAuthenticationParameters param) {
        // build token
        var token = JwtBuilder.Create()
                      .WithAlgorithm(new RS256Algorithm(param.GetPublicKey(), param.GetPrivateKey()))
                      .ExpirationTime(DateTime.UtcNow.AddMinutes(2))
                      .Audience(param.GetAudience())
                      .Subject(param.GetSubject())
                      .Issuer(param.GetIssuer())
                      .Encode();
        return token;
    }
}
