using System;
using System.Security.Claims;
using System.Threading.Tasks;
using ARunner.Auth;
using ARunner.DataLayer.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace ARunner.Auth
{
    public class AccessRequired : IAuthorizationRequirement
    {
        public UserAccess Access { get; }

        public AccessRequired(UserAccess access)
        {
            Access = access;
        }
    }
}

public class AccessRequiredHandler : AuthorizationHandler<AccessRequired>
{
    private readonly UserManager<User> _manager;

    public AccessRequiredHandler(UserManager<User> manager)
    {
        _manager = manager;
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AccessRequired requirement)
    {
        var user = _manager.GetUserAsync(context.User).Result;
        
        if (requirement.Access <= user.Access)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}