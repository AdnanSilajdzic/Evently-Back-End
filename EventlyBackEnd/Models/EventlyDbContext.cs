using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.Globalization;

namespace EventlyBackEnd.Models
{
    public class EventlyDbContext: DbContext
    {
        public EventlyDbContext(DbContextOptions<EventlyDbContext> options)
       : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Event> Events { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasKey(u => u.Id);

            modelBuilder.Entity<Event>()
                .HasKey(u => u.EventId);

            modelBuilder.Entity<Event>()
                .HasOne(e => e.User)
                .WithMany(e => e.CreatedEvents)
                .HasForeignKey(e => e.UserId);
        }
    }
}
