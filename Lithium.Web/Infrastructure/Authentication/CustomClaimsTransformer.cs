using System.Security.Claims;
using Lithium.Web.Infrastructure.Data.Collections;
using Microsoft.AspNetCore.Authentication;

namespace Lithium.Web.Infrastructure.Authentication;

public sealed class CustomClaimsTransformer(UserCollection users) : IClaimsTransformation
{
    public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        // Clone the principal to avoid modifying the original
        var clone = principal.Clone();
        var newIdentity = (ClaimsIdentity)clone.Identity!;

        // Check if the user is already populated with our roles
        if (newIdentity.HasClaim(c => c.Type == "internal_id"))
            return principal;

        var idStr = principal.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(idStr)) return principal;

        var id = Guid.Parse(idStr);

        var user = await users.FirstAsync(u => u.Id == id);
        if (user is null) return principal;

        // Add roles as claims
        foreach (var role in user.Roles)
        {
            if (!newIdentity.HasClaim(ClaimTypes.Role, role))
                newIdentity.AddClaim(new Claim(ClaimTypes.Role, role));
        }

        // Add an internal marker to prevent re-fetching on subsequent calls
        newIdentity.AddClaim(new Claim("internal_id", user.Id.ToString()));

        return clone;
    }
}