﻿// <auto-generated />
using System;
using EventlyBackEnd.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace EventlyBackEnd.Migrations
{
    [DbContext(typeof(EventlyDbContext))]
    partial class EventlyDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("EventlyBackEnd.Models.Entities.Event", b =>
                {
                    b.Property<long>("EventId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("EventId"));

                    b.Property<long>("CreatorId")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("DateTime")
                        .HasColumnType("datetime2");

                    b.Property<bool>("Featured")
                        .HasColumnType("bit");

                    b.Property<string>("ImageURL")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Location")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Organizer")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Type")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("EventId");

                    b.HasIndex("CreatorId");

                    b.ToTable("Events");
                });

            modelBuilder.Entity("EventlyBackEnd.Models.Entities.Post", b =>
                {
                    b.Property<long>("PostId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("PostId"));

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<long>("EventId")
                        .HasColumnType("bigint");

                    b.Property<string>("ImageURL")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<long?>("PosterId")
                        .HasColumnType("bigint");

                    b.HasKey("PostId");

                    b.HasIndex("EventId");

                    b.HasIndex("PosterId");

                    b.ToTable("Posts");
                });

            modelBuilder.Entity("EventlyBackEnd.Models.Entities.User", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Location")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Username")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("EventlyBackEnd.Models.Entities.UserSavedEvent", b =>
                {
                    b.Property<long>("UserId")
                        .HasColumnType("bigint");

                    b.Property<long>("EventId")
                        .HasColumnType("bigint");

                    b.HasKey("UserId", "EventId");

                    b.HasIndex("EventId");

                    b.ToTable("UserSavedEvents");
                });

            modelBuilder.Entity("EventlyBackEnd.Models.Entities.Event", b =>
                {
                    b.HasOne("EventlyBackEnd.Models.Entities.User", "Creator")
                        .WithMany("CreatedEvents")
                        .HasForeignKey("CreatorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Creator");
                });

            modelBuilder.Entity("EventlyBackEnd.Models.Entities.Post", b =>
                {
                    b.HasOne("EventlyBackEnd.Models.Entities.Event", "Event")
                        .WithMany("Posts")
                        .HasForeignKey("EventId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("EventlyBackEnd.Models.Entities.User", "Poster")
                        .WithMany("Posts")
                        .HasForeignKey("PosterId");

                    b.Navigation("Event");

                    b.Navigation("Poster");
                });

            modelBuilder.Entity("EventlyBackEnd.Models.Entities.UserSavedEvent", b =>
                {
                    b.HasOne("EventlyBackEnd.Models.Entities.Event", "Event")
                        .WithMany("SavedByUsers")
                        .HasForeignKey("EventId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("EventlyBackEnd.Models.Entities.User", "User")
                        .WithMany("SavedEvents")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Event");

                    b.Navigation("User");
                });

            modelBuilder.Entity("EventlyBackEnd.Models.Entities.Event", b =>
                {
                    b.Navigation("Posts");

                    b.Navigation("SavedByUsers");
                });

            modelBuilder.Entity("EventlyBackEnd.Models.Entities.User", b =>
                {
                    b.Navigation("CreatedEvents");

                    b.Navigation("Posts");

                    b.Navigation("SavedEvents");
                });
#pragma warning restore 612, 618
        }
    }
}
