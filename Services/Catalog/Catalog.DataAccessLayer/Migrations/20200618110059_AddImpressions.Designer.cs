﻿// <auto-generated />
using System;
using Catalog.DataAccessLayer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Catalog.DataAccessLayer.Migrations
{
    [DbContext(typeof(CatalogContext))]
    [Migration("20200618110059_AddImpressions")]
    partial class AddImpressions
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Catalog.Models.Login", b =>
                {
                    b.Property<Guid>("LoginId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Token")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TokenId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("LoginId");

                    b.HasIndex("UserId");

                    b.ToTable("Logins");
                });

            modelBuilder.Entity("Catalog.Models.ObjectFreeProperties", b =>
                {
                    b.Property<int>("ObjectId")
                        .HasColumnType("int");

                    b.Property<DateTime>("OfferedFreeAtUtc")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("TakenAtUtc")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("TakerId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("ObjectId");

                    b.HasIndex("TakerId");

                    b.ToTable("ObjectFreeProperties");
                });

            modelBuilder.Entity("Catalog.Models.ObjectImpression", b =>
                {
                    b.Property<int>("ObjectId")
                        .HasColumnType("int");

                    b.Property<Guid>("LoginId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("ViewedAtUtc")
                        .HasColumnType("datetime2");

                    b.HasKey("ObjectId", "LoginId", "ViewedAtUtc");

                    b.HasIndex("LoginId");

                    b.ToTable("ObjectImpressions");
                });

            modelBuilder.Entity("Catalog.Models.ObjectLoan", b =>
                {
                    b.Property<Guid>("ObjectLoanId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("LoanEndAt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("LoanedAt")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("LoginId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int?>("ObjectLoanPropertiesObjectId")
                        .HasColumnType("int");

                    b.Property<float?>("Rating")
                        .HasColumnType("real");

                    b.HasKey("ObjectLoanId");

                    b.HasIndex("LoginId");

                    b.HasIndex("ObjectLoanPropertiesObjectId");

                    b.ToTable("ObjectLoans");
                });

            modelBuilder.Entity("Catalog.Models.ObjectLoanProperties", b =>
                {
                    b.Property<int>("ObjectId")
                        .HasColumnType("int");

                    b.HasKey("ObjectId");

                    b.ToTable("ObjectsLoanProperties");
                });

            modelBuilder.Entity("Catalog.Models.ObjectPhoto", b =>
                {
                    b.Property<int>("ObjectPhotoId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("AddedAtUtc")
                        .HasColumnType("datetime2");

                    b.Property<string>("AdditionalInformation")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FilePath")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ObjectId")
                        .HasColumnType("int");

                    b.HasKey("ObjectPhotoId");

                    b.HasIndex("ObjectId");

                    b.ToTable("ObjectPhoto");
                });

            modelBuilder.Entity("Catalog.Models.ObjectTag", b =>
                {
                    b.Property<int>("ObjectId")
                        .HasColumnType("int");

                    b.Property<int>("TagId")
                        .HasColumnType("int");

                    b.HasKey("ObjectId", "TagId");

                    b.HasIndex("TagId");

                    b.ToTable("ObjectTags");
                });

            modelBuilder.Entity("Catalog.Models.OfferedObject", b =>
                {
                    b.Property<int>("OfferedObjectId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("CurrentTransactionType")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("EndsAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("OwnerId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("PublishedAt")
                        .HasColumnType("datetime2");

                    b.HasKey("OfferedObjectId");

                    b.HasIndex("OwnerId");

                    b.ToTable("Objects");
                });

            modelBuilder.Entity("Catalog.Models.Tag", b =>
                {
                    b.Property<int>("TagId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("TagId");

                    b.ToTable("Tags");
                });

            modelBuilder.Entity("Catalog.Models.User", b =>
                {
                    b.Property<Guid>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("OriginalUserId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Catalog.Models.Login", b =>
                {
                    b.HasOne("Catalog.Models.User", "User")
                        .WithMany("Logins")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Catalog.Models.ObjectFreeProperties", b =>
                {
                    b.HasOne("Catalog.Models.OfferedObject", "Object")
                        .WithOne("ObjectFreeProperties")
                        .HasForeignKey("Catalog.Models.ObjectFreeProperties", "ObjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Catalog.Models.User", "Taker")
                        .WithMany("TakenObjects")
                        .HasForeignKey("TakerId");
                });

            modelBuilder.Entity("Catalog.Models.ObjectImpression", b =>
                {
                    b.HasOne("Catalog.Models.Login", "Login")
                        .WithMany("Impressions")
                        .HasForeignKey("LoginId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Catalog.Models.OfferedObject", "Object")
                        .WithMany("Impressions")
                        .HasForeignKey("ObjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Catalog.Models.ObjectLoan", b =>
                {
                    b.HasOne("Catalog.Models.Login", "Login")
                        .WithMany("Loans")
                        .HasForeignKey("LoginId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Catalog.Models.ObjectLoanProperties", "ObjectLoanProperties")
                        .WithMany("ObjectLoans")
                        .HasForeignKey("ObjectLoanPropertiesObjectId");
                });

            modelBuilder.Entity("Catalog.Models.ObjectLoanProperties", b =>
                {
                    b.HasOne("Catalog.Models.OfferedObject", "Object")
                        .WithOne("ObjectLoanProperties")
                        .HasForeignKey("Catalog.Models.ObjectLoanProperties", "ObjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Catalog.Models.ObjectPhoto", b =>
                {
                    b.HasOne("Catalog.Models.OfferedObject", "Object")
                        .WithMany("Photos")
                        .HasForeignKey("ObjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Catalog.Models.ObjectTag", b =>
                {
                    b.HasOne("Catalog.Models.OfferedObject", "Object")
                        .WithMany("Tags")
                        .HasForeignKey("ObjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Catalog.Models.Tag", "Tag")
                        .WithMany("Objects")
                        .HasForeignKey("TagId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Catalog.Models.OfferedObject", b =>
                {
                    b.HasOne("Catalog.Models.User", "Owner")
                        .WithMany("OfferedObjects")
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
