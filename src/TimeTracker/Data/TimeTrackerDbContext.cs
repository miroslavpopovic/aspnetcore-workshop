using System;
using Microsoft.EntityFrameworkCore;
using TimeTracker.Domain;

namespace TimeTracker.Data
{
    public class TimeTrackerDbContext : DbContext
    {
        public TimeTrackerDbContext(DbContextOptions<TimeTrackerDbContext> options) : base(options)
        {
        }

        public DbSet<Client> Clients { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<TimeEntry> TimeEntries { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, Name = "John Doe", HourRate = 25m },
                new User { Id = 2, Name = "Joan Doe", HourRate = 30m });

            modelBuilder.Entity<Client>().HasData(
                new Client { Id = 1, Name = "Client 1" },
                new Client { Id = 2, Name = "Client 2" });

            modelBuilder.Entity<Project>().HasData(
                new { Id = 1L, Name = "Project 1", ClientId = 1L },
                new { Id = 2L, Name = "Project 2", ClientId = 1L },
                new { Id = 3L, Name = "Project 3", ClientId = 2L });

            modelBuilder.Entity<TimeEntry>().HasData(
                new
                {
                    Id = 1L,
                    UserId = 1L,
                    ProjectId = 1L,
                    EntryDate = new DateTime(2019, 7, 1),
                    Hours = 5,
                    HourRate = 25m,
                    Description = "Time entry description 1"
                },
                new
                {
                    Id = 2L,
                    UserId = 1L,
                    ProjectId = 2L,
                    EntryDate = new DateTime(2019, 7, 1),
                    Hours = 2,
                    HourRate = 25m,
                    Description = "Time entry description 2"
                },
                new
                {
                    Id = 3L,
                    UserId = 1L,
                    ProjectId = 3L,
                    EntryDate = new DateTime(2019, 7, 1),
                    Hours = 1,
                    HourRate = 25m,
                    Description = "Time entry description 3"
                },
                new
                {
                    Id = 4L,
                    UserId = 2L,
                    ProjectId = 3L,
                    EntryDate = new DateTime(2019, 7, 1),
                    Hours = 8,
                    HourRate = 30m,
                    Description = "Time entry description 4"
                });
        }
    }
}
