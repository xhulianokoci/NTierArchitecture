using System.Security.Principal;

namespace NTierTemplate.Extentions;

public class BasicAuthenticatedUser : IIdentity
{
    public BasicAuthenticatedUser(string authenticationType, bool isAuthenticated, string name)
    {
        AuthenticationType = authenticationType;
        IsAuthenticated = isAuthenticated;
        Name = name;
    }

    public string AuthenticationType { get; }
    public bool IsAuthenticated { get; }
    public string Name { get; }
}
