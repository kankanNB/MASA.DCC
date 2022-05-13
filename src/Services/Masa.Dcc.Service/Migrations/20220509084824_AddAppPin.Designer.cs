﻿// <auto-generated />
using System;
using Masa.Dcc.Service.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Masa.Dcc.Service.Admin.Migrations
{
    [DbContext(typeof(DccDbContext))]
    [Migration("20220509084824_AddAppPin")]
    partial class AddAppPin
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("Masa.BuildingBlocks.Dispatcher.IntegrationEvents.Logs.IntegrationEventLog", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreationTime")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("EventId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("EventTypeName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("ModificationTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("RowVersion")
                        .IsRequired()
                        .HasMaxLength(36)
                        .HasColumnType("nvarchar(36)");

                    b.Property<int>("State")
                        .HasColumnType("int");

                    b.Property<int>("TimesSent")
                        .HasColumnType("int");

                    b.Property<Guid>("TransactionId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex(new[] { "EventId", "RowVersion" }, "index_eventid_version");

                    b.HasIndex(new[] { "State", "ModificationTime" }, "index_state_modificationtime");

                    b.HasIndex(new[] { "State", "TimesSent", "ModificationTime" }, "index_state_timessent_modificationtime");

                    b.ToTable("IntegrationEventLog", (string)null);
                });

            modelBuilder.Entity("Masa.Dcc.Service.Admin.Domain.App.Aggregates.AppConfigObject", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int>("AppId")
                        .HasColumnType("int")
                        .HasComment("AppId");

                    b.Property<int>("ConfigObjectId")
                        .HasColumnType("int")
                        .HasComment("ConfigObjectId");

                    b.Property<DateTime>("CreationTime")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("Creator")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("EnvironmentClusterId")
                        .HasColumnType("int")
                        .HasComment("EnvironmentClusterId");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<DateTime>("ModificationTime")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("Modifier")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("ConfigObjectId")
                        .IsUnique();

                    b.ToTable("AppConfigObjects");
                });

            modelBuilder.Entity("Masa.Dcc.Service.Admin.Domain.App.Aggregates.AppPin", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int>("AppId")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreationTime")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("Creator")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<DateTime>("ModificationTime")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("Modifier")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.ToTable("AppPin");
                });

            modelBuilder.Entity("Masa.Dcc.Service.Admin.Domain.App.Aggregates.AppSecret", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int>("AppId")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreationTime")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("Creator")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("EncryptionSecret")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("EnvironmentId")
                        .HasColumnType("int");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<DateTime>("ModificationTime")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("Modifier")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("Secret")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("AppSecrets");
                });

            modelBuilder.Entity("Masa.Dcc.Service.Admin.Domain.App.Aggregates.ConfigObject", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("ntext");

                    b.Property<DateTime>("CreationTime")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("Creator")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("FormatLabelId")
                        .HasColumnType("int")
                        .HasComment("Format");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<DateTime>("ModificationTime")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("Modifier")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)")
                        .HasComment("Name");

                    b.Property<int>("RelationConfigObjectId")
                        .HasColumnType("int")
                        .HasComment("Relation config object Id");

                    b.Property<string>("TempContent")
                        .IsRequired()
                        .HasColumnType("ntext");

                    b.Property<int>("Type")
                        .HasColumnType("int")
                        .HasComment("Type");

                    b.HasKey("Id");

                    b.ToTable("ConfigObjects");
                });

            modelBuilder.Entity("Masa.Dcc.Service.Admin.Domain.App.Aggregates.ConfigObjectRelease", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Comment")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)")
                        .HasComment("Comment");

                    b.Property<int>("ConfigObjectId")
                        .HasColumnType("int")
                        .HasComment("Config object Id");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasMaxLength(2147483647)
                        .HasColumnType("ntext")
                        .HasComment("Content");

                    b.Property<DateTime>("CreationTime")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("Creator")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("FromReleaseId")
                        .HasColumnType("int")
                        .HasComment("Rollback From Release Id");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<bool>("IsInvalid")
                        .HasColumnType("bit")
                        .HasComment("If it is rolled back, it will be true");

                    b.Property<DateTime>("ModificationTime")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("Modifier")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)")
                        .HasComment("Name");

                    b.Property<int>("ToReleaseId")
                        .HasColumnType("int")
                        .HasComment("Rollback To Release Id");

                    b.Property<byte>("Type")
                        .HasColumnType("tinyint")
                        .HasComment("Release type");

                    b.Property<string>("Version")
                        .IsRequired()
                        .HasColumnType("varchar(20)")
                        .HasComment("Version foramt is yyyyMMddHHmmss");

                    b.HasKey("Id");

                    b.HasIndex("ConfigObjectId");

                    b.ToTable("ConfigObjectReleases");
                });

            modelBuilder.Entity("Masa.Dcc.Service.Admin.Domain.App.Aggregates.PublicConfig", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<DateTime>("CreationTime")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("Creator")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Identity")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<DateTime>("ModificationTime")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("Modifier")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("PublicConfigs");
                });

            modelBuilder.Entity("Masa.Dcc.Service.Admin.Domain.App.Aggregates.PublicConfigObject", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int>("ConfigObjectId")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreationTime")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("Creator")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("EnvironmentClusterId")
                        .HasColumnType("int");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<DateTime>("ModificationTime")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("Modifier")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("PublicConfigId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ConfigObjectId")
                        .IsUnique();

                    b.HasIndex("PublicConfigId");

                    b.ToTable("PublicConfigObjects");
                });

            modelBuilder.Entity("Masa.Dcc.Service.Admin.Domain.Label.Aggregates.Label", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<DateTime>("CreationTime")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("Creator")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)")
                        .HasComment("Description");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<DateTime>("ModificationTime")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("Modifier")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)")
                        .HasComment("Name");

                    b.Property<string>("TypeCode")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)")
                        .HasComment("TypeCode");

                    b.Property<string>("TypeName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)")
                        .HasComment("TypeName");

                    b.HasKey("Id");

                    b.HasIndex(new[] { "TypeCode", "IsDeleted" }, "IX_TypeCode");

                    b.ToTable("Labels");
                });

            modelBuilder.Entity("Masa.Dcc.Service.Admin.Domain.App.Aggregates.AppConfigObject", b =>
                {
                    b.HasOne("Masa.Dcc.Service.Admin.Domain.App.Aggregates.ConfigObject", "ConfigObject")
                        .WithOne("AppConfigObject")
                        .HasForeignKey("Masa.Dcc.Service.Admin.Domain.App.Aggregates.AppConfigObject", "ConfigObjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ConfigObject");
                });

            modelBuilder.Entity("Masa.Dcc.Service.Admin.Domain.App.Aggregates.ConfigObjectRelease", b =>
                {
                    b.HasOne("Masa.Dcc.Service.Admin.Domain.App.Aggregates.ConfigObject", null)
                        .WithMany("ConfigObjectRelease")
                        .HasForeignKey("ConfigObjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Masa.Dcc.Service.Admin.Domain.App.Aggregates.PublicConfigObject", b =>
                {
                    b.HasOne("Masa.Dcc.Service.Admin.Domain.App.Aggregates.ConfigObject", "ConfigObject")
                        .WithOne("PublicConfigObject")
                        .HasForeignKey("Masa.Dcc.Service.Admin.Domain.App.Aggregates.PublicConfigObject", "ConfigObjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Masa.Dcc.Service.Admin.Domain.App.Aggregates.PublicConfig", "PublicConfig")
                        .WithMany("PublicConfigObjects")
                        .HasForeignKey("PublicConfigId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ConfigObject");

                    b.Navigation("PublicConfig");
                });

            modelBuilder.Entity("Masa.Dcc.Service.Admin.Domain.App.Aggregates.ConfigObject", b =>
                {
                    b.Navigation("AppConfigObject")
                        .IsRequired();

                    b.Navigation("ConfigObjectRelease");

                    b.Navigation("PublicConfigObject")
                        .IsRequired();
                });

            modelBuilder.Entity("Masa.Dcc.Service.Admin.Domain.App.Aggregates.PublicConfig", b =>
                {
                    b.Navigation("PublicConfigObjects");
                });
#pragma warning restore 612, 618
        }
    }
}
