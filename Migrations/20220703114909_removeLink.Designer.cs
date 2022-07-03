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
    [Migration("20220703114909_removeLink")]
    partial class removeLink
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

                    b.Property<long>("GroupId")
                        .HasColumnType("INTEGER");

                    b.Property<uint>("RequiredAddCount")
                        .HasColumnType("INTEGER");

                    b.Property<string>("WelcomeMessage")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Groups");
                });
#pragma warning restore 612, 618
        }
    }
}