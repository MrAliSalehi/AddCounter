using AddCounter.DataLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace AddCounter.DataLayer.Context;
#nullable disable
public class CounterContext : DbContext
{
    public DbSet<Group> Groups { get; set; } = default;
    public DbSet<User> Users { get; set; } = default;

    private readonly ILoggerFactory _loggerFactory;

    private const string DbPath = "AddCounter.db";

    public CounterContext() : base() { }
    public CounterContext(ILoggerFactory loggerFactory)
    {
        _loggerFactory = loggerFactory;
    }
    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        _ = options.UseLoggerFactory(_loggerFactory);
        _ = options.UseSqlite($"Data Source={DbPath}");
    }

}