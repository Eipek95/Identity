using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace AspNetCoreIdentityApp.Web.Requirements
{
    public class ViolonceRequirement : IAuthorizationRequirement
    {
        public int ThresholdAge { get; set; }//belirlenen kural yaşı
    }

    public class ViolonceRequirementHandler : AuthorizationHandler<ViolonceRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ViolonceRequirement requirement)
        {
            if (!context.User.HasClaim(x => x.Type == "birthdate"))
            {
                context.Fail();
                return Task.CompletedTask;
            }

            Claim birthDateClaim = context.User.FindFirst("birthdate")!;

            var today = DateTime.Now;
            var birthDate = Convert.ToDateTime(birthDateClaim.Value);
            var age = today.Year - birthDate.Year;//yaş hesabı

            if (birthDate > today.AddYears(-age)) age--; //atık yıl için yaş hesabı

            if (requirement.ThresholdAge > age)
            {
                context.Fail();
                return Task.CompletedTask;
            }

            context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }
}
