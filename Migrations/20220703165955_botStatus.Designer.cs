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
    [Migration("20220703165955_botStatus")]
    partial class botStatus
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

                    b.Property<long>("AddPrice")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("BotStatus")
                        .HasColumnType("INTEGER");

                    b.Property<long>("GroupId")
                        .HasColumnType("INTEGER");

                    b.Property<ushort>("MessageDeleteTimeInMinute")
                        .HasColumnType("INTEGER");

                    b.Property<int>("RequiredAddCount")
                        .HasColumnType("INTEGER");

                    b.Property<string>("WelcomeMessage")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Groups");
                });

            modelBuilder.Entity("AddCounter.DataLayer.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<uint>("AddCount")
                        .HasColumnType("INTEGER");

                    b.Property<long>("ChatId")
                        .HasColumnType("INTEGER");

                    b.Property<long>("UserId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });
#pragma warning restore 612, 618
        }
    }
}
