using AddCounter.DataLayer.Context;
using AddCounter.DataLayer.Models;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace AddCounter.DataLayer.Controllers;

public static class UserController
{
    public static async ValueTask<User> AddUserAsync(long userId, long chatId, CancellationToken ct = default, bool checkIfAlreadyExists = true)
    {
        try
        {
            await using var db = new CounterContext();
            if (checkIfAlreadyExists)
            {
                var checkExists = await db.Users.SingleOrDefaultAsync(p => p.UserId == userId && p.ChatId == chatId, ct);
                if (checkExists is not null)
                    return checkExists;
                goto Create;
                //await db.Users.AddAsync(defaultNewUser, ct);
                // await db.SaveChangesAsync(ct);
                // return defaultNewUser;
            }
        Create:

            var defaultNewUser = new User { UserId = userId, ChatId = chatId, AddCount = 0 };
            await db.Users.AddAsync(defaultNewUser, ct);
            await db.SaveChangesAsync(ct);
            return defaultNewUser;
        }
        catch (Exception e)
        {
            Log.Error(e, nameof(AddUserAsync));
            return new User();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="chatId"></param>
    /// <param name="update"></param>
    /// <param name="ct"></param>
    /// <returns>return 0 on success and 1 if not exists and 2 on error</returns>
    public static async ValueTask<ushort> UpdateUserAsync(long userId, long chatId, Action<User> update, CancellationToken ct = default)
    {
        try
        {
            await using var db = new CounterContext();
            var findUser = await db.Users.FirstOrDefaultAsync(p => p.UserId == userId && p.ChatId == chatId, ct);
            if (findUser is null)
                return 1;

            update(findUser);
            await db.SaveChangesAsync(ct);
            return 0;
        }
        catch (Exception e)
        {
            Log.Error(e, nameof(UpdateUserAsync));
            return 2;
        }
    }

    public static async ValueTask<User> GetUserAsync(long userId, long chatId, CancellationToken ct = default)
    {
        try
        {
            await using var db = new CounterContext();
            var user = await db.Users.FirstOrDefaultAsync(p => p.UserId == userId && p.ChatId == chatId, ct);
            return user ?? await AddUserAsync(userId, chatId, ct);
        }
        catch (Exception e)
        {
            Log.Error(e, nameof(GetUserAsync));
            return new User();
        }
    }

    public static async ValueTask<List<User>> GetUserByGroupIdAsync(long chatId, CancellationToken ct)
    {
        try
        {
            await using var db = new CounterContext();
            return await db.Users.Where(p => p.ChatId == chatId).ToListAsync(ct);

        }
        catch (Exception e)
        {
            Log.Error(e, nameof(GetUserByGroupIdAsync));
            return new List<User>();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="chatId"></param>
    /// <param name="ct"></param>
    /// <returns>return 0 on success . 1 on error</returns>
    public static async ValueTask<ushort> ResetAllUserAddsAsync(long chatId, CancellationToken ct = default)
    {
        try
        {
            await using var db = new CounterContext();
            var users = await db.Users.ToListAsync(ct);
            foreach (var user in users)
            {
                user.AddCount = 0;
            }

            await db.SaveChangesAsync(ct);
            return 0;
        }
        catch (Exception e)
        {
            Log.Error(e, nameof(ResetAllUserAddsAsync));
            return 1;
        }
    }
}