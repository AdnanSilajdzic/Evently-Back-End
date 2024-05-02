using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.Globalization;

namespace EventlyBackEnd.Models.Entities
{
    public class EventlyDbContext : DbContext
    {
        public EventlyDbContext(DbContextOptions<EventlyDbContext> options)
       : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<UserSavedEvent> UserSavedEvents { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasKey(u => u.Id);

            modelBuilder.Entity<Event>()
                .HasKey(u => u.EventId);

            modelBuilder.Entity<Event>()
                .HasOne(e => e.Creator)
                .WithMany(e => e.CreatedEvents)
                .HasForeignKey(e => e.CreatorId);

            modelBuilder.Entity<UserSavedEvent>()
                .HasKey(use => new { use.UserId, use.EventId });

            modelBuilder.Entity<UserSavedEvent>()
                .HasOne(use => use.User)
                .WithMany(u => u.SavedEvents)
                .HasForeignKey(use => use.UserId)
                .OnDelete(DeleteBehavior.Restrict); // Remove cascade delete action

            modelBuilder.Entity<UserSavedEvent>()
                .HasOne(use => use.Event)
                .WithMany(e => e.SavedByUsers)
                .HasForeignKey(use => use.EventId)
                .OnDelete(DeleteBehavior.Cascade); // Keep cascade delete action for this relationship

        }
    }
}
