using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace AspNetCoreIdentityApp.Web.Requirements
{
    public class ExhangeExpireRequirement : IAuthorizationRequirement
    {
    }


    public class ExhangeExpireRequirementHandler : AuthorizationHandler<ExhangeExpireRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ExhangeExpireRequirement requirement)
        {

            if (!context.User.HasClaim(x => x.Type == "ExchangeExpireDate"))
            {
                context.Fail();
                return Task.CompletedTask;
            }

            Claim exchangeExpireDate = context.User.FindFirst("ExchangeExpireDate")!;
            if (DateTime.Now > Convert.ToDateTime(exchangeExpireDate.Value))
            {
                context.Fail();
                return Task.CompletedTask;
            }

            context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }
}
