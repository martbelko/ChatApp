﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using wa_api.Data;

#nullable disable

namespace wa_api.Migrations
{
    [DbContext(typeof(WaDbContext))]
    [Migration("20221022231613_Init")]
    partial class Init
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("ConversationUser", b =>
                {
                    b.Property<int>("ConversationsId")
                        .HasColumnType("integer");

                    b.Property<int>("MembersId")
                        .HasColumnType("integer");

                    b.HasKey("ConversationsId", "MembersId");

                    b.HasIndex("MembersId");

                    b.ToTable("ConversationUser");
                });

            modelBuilder.Entity("wa_api.Models.Conversation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.HasKey("Id");

                    b.ToTable("Conversation");
                });

            modelBuilder.Entity("wa_api.Models.Message", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("AuthorId")
                        .HasColumnType("integer");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int?>("ConversationId")
                        .HasColumnType("integer");

                    b.Property<DateTime>("Time")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.HasIndex("ConversationId");

                    b.ToTable("Messages");
                });

            modelBuilder.Entity("wa_api.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("ConversationUser", b =>
                {
                    b.HasOne("wa_api.Models.Conversation", null)
                        .WithMany()
                        .HasForeignKey("ConversationsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("wa_api.Models.User", null)
                        .WithMany()
                        .HasForeignKey("MembersId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("wa_api.Models.Message", b =>
                {
                    b.HasOne("wa_api.Models.User", "Author")
                        .WithMany("Messages")
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("wa_api.Models.Conversation", null)
                        .WithMany("Messages")
                        .HasForeignKey("ConversationId");

                    b.Navigation("Author");
                });

            modelBuilder.Entity("wa_api.Models.Conversation", b =>
                {
                    b.Navigation("Messages");
                });

            modelBuilder.Entity("wa_api.Models.User", b =>
                {
                    b.Navigation("Messages");
                });
#pragma warning restore 612, 618
        }
    }
}
