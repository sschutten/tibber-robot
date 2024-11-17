using Microsoft.EntityFrameworkCore;
using Tibber.Robot.Api.Models;

namespace Tibber.Robot.Api.Data;

public class RobotDbContext : DbContext
{
    public RobotDbContext(DbContextOptions<RobotDbContext> options) : base(options)
    {
    }

    public DbSet<Execution> Executions { get; set; }
}
