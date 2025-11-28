using Microsoft.EntityFrameworkCore;
using SwipSwapAuth.Models;
using System.Collections.Generic;

namespace SwipSwapAuth.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<ApplicationUser> Users { get; set; }
    }
}
