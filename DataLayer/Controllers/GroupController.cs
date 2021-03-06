using AddCounter.DataLayer.Context;
using AddCounter.DataLayer.Models;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace AddCounter.DataLayer.Controllers;

internal static class GroupController
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="groupId"></param>
    /// <param name="ct"></param>
    /// <returns>returns 0 on success. 1 on exception and error</returns>
    public static async ValueTask<ushort> AddGroupAsync(long groupId, CancellationToken ct = default)
    {
        try
        {
            await using var db = new CounterContext();
            _ = await db.Groups.AddAsync(new Group { GroupId = groupId }, ct);
            _ = await db.SaveChangesAsync(ct);
            return 0;
        }
        catch (Exception e)
        {
            Log.Error(e, nameof(AddGroupAsync));
            return 1;
        }
    }
    public static async ValueTask<Group?> GetGroupAsync(long groupId, CancellationToken ct = default)
    {
        try
        {
            await using var db = new CounterContext();
            return await db.Groups.AsNoTracking().SingleOrDefaultAsync(p => p.GroupId == groupId, ct);
        }
        catch (Exception e)
        {
            Log.Error(e, nameof(GetGroupAsync));
            return null;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="update"></param>
    /// <param name="groupId"></param>
    /// <param name="ct"></param>
    /// <returns>return 0 on success and 1 on error</returns>
    public static async ValueTask<ushort> UpdateGroupAsync(Action<Group> update, long groupId, CancellationToken ct = default)
    {
        try
        {
            await using var db = new CounterContext();
            var group = await db.Groups.FirstOrDefaultAsync(p => p.GroupId == groupId, ct);
            if (group is null)
                return 1;
            update(group);
            _ = await db.SaveChangesAsync(ct);
            return 0;

        }
        catch (Exception e)
        {
            Log.Error(e, nameof(UpdateGroupAsync));
            return 1;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="chatId"></param>
    /// <param name="ct"></param>
    /// <returns>0 on success , 1 if not exists and 2 on error</returns>
    public static async ValueTask<ushort> ResetGroupSettingAsync(long chatId, CancellationToken ct = default)
    {
        try
        {
            await using var db = new CounterContext();
            var group = await db.Groups.FirstOrDefaultAsync(p => p.GroupId == chatId, ct);
            if (group is null)
                return 1;
            group.RequiredAddCount = 0;
            group.WelcomeMessage = "";
            group.AddPrice = 0;
            await db.SaveChangesAsync(ct);
            return 0;
        }
        catch (Exception e)
        {
            Log.Error(e, nameof(ResetGroupSettingAsync));
            return 2;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="groupId"></param>
    /// <param name="ct"></param>
    /// <returns>0 on success . 1 if not exists and 2 on error</returns>
    public static async ValueTask<ushort> RemoveGroupAsync(long groupId, CancellationToken ct = default)
    {
        try
        {
            await using var db = new CounterContext();
            var group = await db.Groups.FirstOrDefaultAsync(p => p.GroupId == groupId, ct);
            if (group is null)
                return 1;
            db.Groups.Remove(group);
            await db.SaveChangesAsync(ct);
            return 0;
        }
        catch (Exception e)
        {
            Log.Error(e, nameof(RemoveGroupAsync));
            return 2;
        }
    }
}