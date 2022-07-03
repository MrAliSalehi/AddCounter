using AddCounter.DataLayer.Context;
using AddCounter.DataLayer.Models;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace AddCounter.DataLayer.Controllers;

public static class LinkController
{
    public static async ValueTask<List<Link>> GetAllLinksAsync(long groupId, CancellationToken ct = default)
    {
        try
        {
            await using var db = new CounterContext();
            var group = await db.Groups.Include(p => p.GroupLinks).FirstOrDefaultAsync(x => x.GroupId == groupId, ct);
            return group?.GroupLinks.ToList() ?? new List<Link>();
        }
        catch (Exception e)
        {
            Log.Error(e, nameof(GetAllLinksAsync));
            return new List<Link>();
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="groupId">THIS IS TELEGRAM ID .</param>
    /// <param name="link"></param>
    /// <param name="ct"></param>
    /// <returns>return 0 on success .1 if already exists and 2 on error</returns>
    public static async ValueTask<ushort> AddLinkAsync(long groupId, string link, CancellationToken ct = default)
    {
        try
        {
            await using var db = new CounterContext();

            var group = await db.Groups.Include(x => x.GroupLinks).SingleOrDefaultAsync(p => p.GroupId == groupId, ct);

            if (group!.GroupLinks.Any(p => p.Url == link))
                return 1;

            group.GroupLinks.Add(new Link() { Group = group, Url = link });
            await db.SaveChangesAsync(ct);
            return 0;
        }
        catch (Exception e)
        {
            Log.Error(e, nameof(AddLinkAsync));
            return 2;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="groupId">THIS IS TELEGRAM ID .</param>
    /// <param name="link"></param>
    /// <param name="ct"></param>
    /// <returns>return 0 on success .1 if not found and 2 on error</returns>
    public static async ValueTask<ushort> RemoveLinkAsync(long groupId, string link, CancellationToken ct = default)
    {
        try
        {
            await using var db = new CounterContext();

            var group = await db.Groups.Include(x => x.GroupLinks).SingleOrDefaultAsync(p => p.GroupId == groupId, ct);

            var findLink = group!.GroupLinks.FirstOrDefault(p => p.Url == link);
            if (findLink is null)
                return 1;

            db.Links.Remove(findLink);
            await db.SaveChangesAsync(ct);
            return 0;
        }
        catch (Exception e)
        {
            Log.Error(e, nameof(AddLinkAsync));
            return 2;
        }
    }
}