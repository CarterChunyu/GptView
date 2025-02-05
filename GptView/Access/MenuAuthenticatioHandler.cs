using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace GptView.Access
{
    public class MenuAuthenticatioHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public MenuAuthenticatioHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder) : base(options, logger, encoder)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            // Claim[] claims = new Claim[] { new Claim(ClaimTypes.Role,"yu")};
            Claim[] claims = new Claim[] { };
            ClaimsIdentity identity = new(claims);
            ClaimsPrincipal principal = new(identity);
            AuthenticationTicket ticket = new(principal, Scheme.Name);
            AuthenticateResult success = AuthenticateResult.Success(ticket);
            return Task.FromResult(success);
        }
    }
}
