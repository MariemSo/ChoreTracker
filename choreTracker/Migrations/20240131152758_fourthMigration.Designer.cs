﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using choreTracker.Models;

#nullable disable

namespace choreTracker.Migrations
{
    [DbContext(typeof(MyContext))]
    [Migration("20240131152758_fourthMigration")]
    partial class fourthMigration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("choreTracker.Models.Job", b =>
                {
                    b.Property<int>("JobId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Location")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<int?>("UserId")
                        .HasColumnType("int");

                    b.Property<int?>("WorkerId")
                        .HasColumnType("int");

                    b.HasKey("JobId");

                    b.HasIndex("UserId");

                    b.HasIndex("WorkerId");

                    b.ToTable("Jobs");
                });

            modelBuilder.Entity("choreTracker.Models.User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("FName")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("LName")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime(6)");

                    b.HasKey("UserId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("choreTracker.Models.Job", b =>
                {
                    b.HasOne("choreTracker.Models.User", "Creator")
                        .WithMany("createdJobs")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("choreTracker.Models.User", "Worker")
                        .WithMany("myJobs")
                        .HasForeignKey("WorkerId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("Creator");

                    b.Navigation("Worker");
                });

            modelBuilder.Entity("choreTracker.Models.User", b =>
                {
                    b.Navigation("createdJobs");

                    b.Navigation("myJobs");
                });
#pragma warning restore 612, 618
        }
    }
}
