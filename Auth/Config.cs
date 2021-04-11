using IdentityServer4.Models;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Json;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Test;

namespace Auth
{
  public static class Config
  {
    public static List<TestUser> Users
    {
      get
      {
        var address = new
        {
          street_address = "123 4 St SW",
          locality = "Calgary",
          postal_code = "A1A1A1",
          country = "Canada"
        };

        return new List<TestUser>
        {
          new TestUser
          {
            SubjectId = "139b7b85-3753-4fc1-a79c-1bc46bcd53f9",
            Username = "Anne",
            Password = "Anne",
            Claims =
            {
              new Claim(JwtClaimTypes.Name, "Anne Annerson"),
              new Claim(JwtClaimTypes.GivenName, "Anne"),
              new Claim(JwtClaimTypes.FamilyName, "Annerson"),
              new Claim(JwtClaimTypes.Email, "Anne@Website.Something"),
              new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
              new Claim(JwtClaimTypes.Role, "admin"),
              new Claim(JwtClaimTypes.WebSite, "http://anne.website.something"),
              new Claim(JwtClaimTypes.Address, JsonSerializer.Serialize(address),
                IdentityServerConstants.ClaimValueTypes.Json)
            }
          },
          new TestUser
          {
            SubjectId = "8ab55d52-60c9-4319-944b-0abec05b21c1",
            Username = "Don",
            Password = "Don",
            Claims =
            {
              new Claim(JwtClaimTypes.Name, "Don Donston"),
              new Claim(JwtClaimTypes.GivenName, "Don"),
              new Claim(JwtClaimTypes.FamilyName, "Donston"),
              new Claim(JwtClaimTypes.Email, "Don@Website.Something"),
              new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
              new Claim(JwtClaimTypes.Role, "user"),
              new Claim(JwtClaimTypes.WebSite, "http://don.website.something"),
              new Claim(JwtClaimTypes.Address, JsonSerializer.Serialize(address),
                IdentityServerConstants.ClaimValueTypes.Json)
            }
          }
        };
      }
    }

    public static IEnumerable<IdentityResource> IdentityResources =>
      new []
      {
        new IdentityResources.OpenId(),
        new IdentityResources.Profile(),
        new IdentityResource
        {
          Name = "role",
          UserClaims = new List<string> {"role"}
        }
      };

    public static IEnumerable<ApiScope> ApiScopes =>
      new []
      {
        new ApiScope("weatherapi.read"),
        new ApiScope("weatherapi.write"),
      };
    public static IEnumerable<ApiResource> ApiResources => new[]
    {
      new ApiResource("weatherapi")
      {
        Scopes = new List<string> {"weatherapi.read", "weatherapi.write"},
        ApiSecrets = new List<Secret> {new Secret("ScopeSecret".Sha256())},
        UserClaims = new List<string> {"role"}
      }
    };

    public static IEnumerable<Client> Clients =>
      new[]
      {
        // m2m client credentials flow client
        new Client
        {
          ClientId = "m2m.client",
          ClientName = "Client Credentials Client",

          AllowedGrantTypes = GrantTypes.ClientCredentials,
          ClientSecrets = {new Secret("SuperSecretPassword".Sha256())},

          AllowedScopes = {"weatherapi.read", "weatherapi.write"}
        },

        // interactive client using code flow + pkce
        new Client
        {
          ClientId = "interactive",
          ClientSecrets = {new Secret("SuperSecretPassword".Sha256())},

          AllowedGrantTypes = GrantTypes.Code,

          AllowOfflineAccess = true,
          AllowedScopes = {"openid", "profile", "weatherapi.read"},
          RequirePkce = true,
          RequireConsent = true,
          AllowPlainTextPkce = false
        },
      };
  }
}