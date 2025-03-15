using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagementSystem.Core.Entities;

namespace TaskManagementSystem.Data
{
    /// <summary>
    /// EF Core DbContext integrating ASP.NET Core Identity and TaskItem entity.
    /// </summary>
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        // DbSet for tasks
        public DbSet<TaskItem> Tasks { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            //// Configure TaskItem entity if needed (indexes, constraints, etc.)
            //builder.Entity<TaskItem>()
            //    .HasIndex(t => t.UserId);

            // Stored Procedure Mappings
            builder.Entity<TaskItem>().ToTable("Tasks");
            builder.Entity<TaskItem>().HasKey(t => t.Id);

            // Configure relationships
            builder.Entity<TaskItem>()
                .HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(t => t.UserId);


        }
    }
}
