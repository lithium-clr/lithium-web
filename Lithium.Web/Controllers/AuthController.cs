using System.Security.Claims;
using Lithium.Web.Infrastructure.Data;
using Lithium.Web.Infrastructure.Data.Collections;
using Lithium.Web.Infrastructure.Data.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace Lithium.Web.Controllers;

[Route("auth")]
public sealed class AuthController(UserCollection userCollection) : ControllerBase
{
    [HttpGet("login")]
    public IActionResult Login(string returnUrl = "/")
    {
        return Challenge(new AuthenticationProperties { RedirectUri = Url.Action("Callback", new { returnUrl }) },
            "Discord");
    }

    [HttpGet("callback")]
    public async Task<IActionResult> Callback(string returnUrl = "/")
    {
        var result = await HttpContext.AuthenticateAsync("External");

        if (!result.Succeeded)
            return Redirect("/");

        var discordIdStr = result.Principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!ulong.TryParse(discordIdStr, out var discordId) || discordId is 0)
            return Redirect("/");

        var username = result.Principal.FindFirst(ClaimTypes.Name)?.Value;
        var email = result.Principal.FindFirst(ClaimTypes.Email)?.Value;

        // Get the avatar hash from the claim mapped in Program.cs
        var avatarHash = result.Principal.FindFirst("urn:discord:avatar:url")?.Value;

        // Construct the full avatar URL if the hash exists
        string? avatarUrl = null;

        if (!string.IsNullOrEmpty(avatarHash))
        {
            var extension = avatarHash.StartsWith("a_") ? "gif" : "png";
            avatarUrl = $"https://cdn.discordapp.com/avatars/{discordId}/{avatarHash}.{extension}";
        }

        if (string.IsNullOrEmpty(username))
            return Redirect("/");

        var user = await userCollection.FirstAsync(u => u.Discord.Id == discordId);

        if (user is null)
        {
            user = new User
            {
                Discord = new DiscordUser
                {
                    Id = discordId,
                    Username = username,
                    Email = email,
                    AvatarUrl = avatarUrl
                }
            };

            await userCollection.InsertAsync(user);
        }
        else
        {
            // Update user info if changed
            if (user.Discord.Username != username || user.Discord.AvatarUrl != avatarUrl || user.Discord.Email != email)
            {
                user.Discord.Username = username;
                user.Discord.Email = email;
                user.Discord.AvatarUrl = avatarUrl;

                await userCollection.UpdateAsync(user);
            }
        }

        var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
        identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
        identity.AddClaim(new Claim(ClaimTypes.Name, user.Discord.Username));

        if (!string.IsNullOrEmpty(user.Discord.Email))
            identity.AddClaim(new Claim(ClaimTypes.Email, user.Discord.Email));

        if (!string.IsNullOrEmpty(user.Discord.AvatarUrl))
            identity.AddClaim(new Claim("AvatarUrl", user.Discord.AvatarUrl));

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
        await HttpContext.SignOutAsync("External");

        return Redirect(returnUrl);
    }

    [HttpGet("logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Redirect("/");
    }
}