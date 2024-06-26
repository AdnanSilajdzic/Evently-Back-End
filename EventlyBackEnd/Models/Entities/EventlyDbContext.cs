﻿using Microsoft.EntityFrameworkCore;
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
        public DbSet<Post> Posts { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //define primary keys
            modelBuilder.Entity<User>()
                .HasKey(u => u.Id);

            modelBuilder.Entity<Event>()
                .HasKey(u => u.EventId);

            modelBuilder.Entity<Post>()
                .HasKey(u => u.PostId);

            //one-to-many relationship between event and creator
            modelBuilder.Entity<Event>()
                .HasOne(e => e.Creator)
                .WithMany(e => e.CreatedEvents)
                .HasForeignKey(e => e.CreatorId);

            modelBuilder.Entity<UserSavedEvent>()
                .HasKey(use => new { use.UserId, use.EventId });

            //many-to-many relationship between saved event and users
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

            //one-to-many relationship between posts and users
            modelBuilder.Entity<Post>()
                .HasOne(u => u.Poster)
                .WithMany(u => u.Posts)
                .HasForeignKey(u => u.PosterId);

            //one-to-many relationship between posts and events
            modelBuilder.Entity<Post>()
                .HasOne(u => u.Event)
                .WithMany(u => u.Posts)
                .HasForeignKey(u => u.EventId);

        }
    }
}
