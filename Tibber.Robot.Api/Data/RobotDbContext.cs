using Microsoft.EntityFrameworkCore;

namespace Tibber.Robot.Api.Data;

public class RobotDbContext : DbContext
{
    public RobotDbContext(DbContextOptions<RobotDbContext> options) : base(options)
    {
    }

    public DbSet<CleanResult> CleanResults { get; set; }
}
