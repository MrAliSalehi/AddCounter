using AddCounter.DataLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace AddCounter.DataLayer.Context;
#nullable disable
public class CounterContext : DbContext
{
    public DbSet<Group> Groups { get; set; } = default;
    public DbSet<Link> Links { get; set; } = default;

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

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Group>(p =>
        {
            p.HasMany(x => x.GroupLinks);
        });
        modelBuilder.Entity<Link>(p =>
        {
            p.HasOne(a => a.Group).WithMany(x => x.GroupLinks);
        });
        base.OnModelCreating(modelBuilder);
    }
}