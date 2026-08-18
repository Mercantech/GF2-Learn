using System.Security.Claims;
using GF2Learn.Web.Auth;
using GF2Learn.Web.Data;
using GF2Learn.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace GF2Learn.Web.Services;

public interface IAppUserService
{
    Task<AppUser?> EnsureCurrentUserAsync(
        ClaimsPrincipal principal,
        bool markLogin = false,
        CancellationToken cancellationToken = default);

    Task TouchActivityAsync(
        string userSub,
        DateTimeOffset occurredAt,
        CancellationToken cancellationToken = default);
}

public sealed class AppUserService(
    Gf2LearnDbContext db,
    IAuthorizationService authorization) : IAppUserService
{
    public async Task<AppUser?> EnsureCurrentUserAsync(
        ClaimsPrincipal principal,
        bool markLogin = false,
        CancellationToken cancellationToken = default)
    {
        var userSub = UserIdentity.GetSubject(principal);
        if (userSub is null || principal.Identity?.IsAuthenticated != true)
            return null;

        var now = DateTimeOffset.UtcNow;
        var isEducator = (await authorization.AuthorizeAsync(
            principal,
            resource: null,
            AdminAuthorizationPolicies.Educator)).Succeeded;
        var isSuperAdmin = (await authorization.AuthorizeAsync(
            principal,
            resource: null,
            AdminAuthorizationPolicies.SuperAdmin)).Succeeded;
        var appUser = await db.AppUsers
            .FirstOrDefaultAsync(user => user.UserSub == userSub, cancellationToken);

        if (appUser is null)
        {
            appUser = new AppUser
            {
                Id = Guid.NewGuid(),
                UserSub = userSub,
                FirstSeenAt = now,
                LastLoginAt = now,
                IsEducator = isEducator,
                IsSuperAdmin = isSuperAdmin
            };
            db.AppUsers.Add(appUser);

            try
            {
                await db.SaveChangesAsync(cancellationToken);
                return appUser;
            }
            catch (DbUpdateException)
            {
                // Parallel requests can both be the first authenticated request.
                db.ChangeTracker.Clear();
                appUser = await db.AppUsers
                    .FirstOrDefaultAsync(user => user.UserSub == userSub, cancellationToken);
                if (appUser is null)
                    throw;
            }
        }

        var changed = false;
        if (appUser.IsEducator != isEducator || appUser.IsSuperAdmin != isSuperAdmin)
        {
            appUser.IsEducator = isEducator;
            appUser.IsSuperAdmin = isSuperAdmin;
            changed = true;
        }

        if (markLogin)
        {
            appUser.LastLoginAt = now;
            changed = true;
        }

        if (changed)
            await db.SaveChangesAsync(cancellationToken);

        return appUser;
    }

    public async Task TouchActivityAsync(
        string userSub,
        DateTimeOffset occurredAt,
        CancellationToken cancellationToken = default)
    {
        await db.AppUsers
            .Where(user => user.UserSub == userSub
                           && (user.LastActivityAt == null || user.LastActivityAt < occurredAt))
            .ExecuteUpdateAsync(
                setters => setters.SetProperty(user => user.LastActivityAt, occurredAt),
                cancellationToken);
    }
}

public static class UserIdentity
{
    public static string? GetSubject(ClaimsPrincipal principal) =>
        principal.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? principal.FindFirstValue("sub");
}
