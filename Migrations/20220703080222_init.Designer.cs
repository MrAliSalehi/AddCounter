﻿// <auto-generated />
using AddCounter.DataLayer.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace AddCounter.Migrations
{
    [DbContext(typeof(CounterContext))]
    [Migration("20220703080222_init")]
    partial class init
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "6.0.6");

            modelBuilder.Entity("AddCounter.DataLayer.Models.Group", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("GroupId")
                        .HasColumnType("INTEGER");

                    b.Property<uint>("RequiredAddCount")
                        .HasColumnType("INTEGER");

                    b.Property<string>("WelcomeMessage")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Groups");
                });

            modelBuilder.Entity("AddCounter.DataLayer.Models.Link", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("GroupId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("GroupId");

                    b.ToTable("Link");
                });

            modelBuilder.Entity("AddCounter.DataLayer.Models.Link", b =>
                {
                    b.HasOne("AddCounter.DataLayer.Models.Group", "Group")
                        .WithMany("GroupLinks")
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Group");
                });

            modelBuilder.Entity("AddCounter.DataLayer.Models.Group", b =>
                {
                    b.Navigation("GroupLinks");
                });
#pragma warning restore 612, 618
        }
    }
}