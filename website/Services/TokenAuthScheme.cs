using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace website.Services
{

    public class TokenAuthOptions : AuthenticationSchemeOptions
    {

        public string SecretKey { get; set; }

        public TimeSpan Expiary { get; set; }

    }

    public class TokenAuthScheme : AuthenticationHandler<TokenAuthOptions>
    {

        public const string SchemeName = "JWTTokenAuth";

        public TokenAuthScheme(IOptionsMonitor<TokenAuthOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) 
            : base(options, logger, encoder, clock)
        {
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            string auth = Context.Request.Headers["Authorization"];
            if (auth == null) return AuthenticateResult.Fail("No JWT token provided");
            var auths = auth.Split(" ");
            if (auths[0].ToLower() != "bearer") return AuthenticateResult.Fail("Invalid authentication");
            string token = auths[1];

            TokenGenerator generator = new TokenGenerator {
                Expiary = Options.Expiary,
                SecretKey = Options.SecretKey
            };

            try
            {
                var principal = generator.Validate(token);
                return AuthenticateResult.Success(new AuthenticationTicket(principal, SchemeName));
            }
            catch
            {
                return AuthenticateResult.Fail("Failed to validate token");
            }
        }
    }
}
