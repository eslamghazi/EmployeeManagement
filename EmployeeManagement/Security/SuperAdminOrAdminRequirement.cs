using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace EmployeeManagement.Security
{
    public class SuperAdminOrAdminRequirement : IAuthorizationRequirement
    {
    }

    public class SuperAdminOrAdminHandler : AuthorizationHandler<SuperAdminOrAdminRequirement>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            SuperAdminOrAdminRequirement requirement)
        {
            if (context.User.IsInRole("Super Admin") || context.User.IsInRole("Admin"))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
