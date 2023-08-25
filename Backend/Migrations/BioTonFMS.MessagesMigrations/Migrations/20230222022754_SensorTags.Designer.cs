﻿// <auto-generated />
using System;
using System.Collections;
using BioTonFMS.Infrastructure.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BioTonFMS.MessagesMigrations.Migrations
{
    [DbContext(typeof(MessagesDBContext))]
    [Migration("20230222022754_SensorTags")]
    partial class SensorTags
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.9")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("BioTonFMS.Domain.TrackerMessages.MessageTag", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<bool>("IsFallback")
                        .HasColumnType("boolean")
                        .HasColumnName("is_fallback");

                    b.Property<int?>("SensorId")
                        .HasColumnType("integer")
                        .HasColumnName("sensor_id");

                    b.Property<byte>("TagType")
                        .HasColumnType("smallint")
                        .HasColumnName("tag_type");

                    b.Property<long>("TrackerMessageId")
                        .HasColumnType("bigint")
                        .HasColumnName("tracker_message_id");

                    b.Property<int?>("TrackerTagId")
                        .HasColumnType("integer")
                        .HasColumnName("tracker_tag_id");

                    b.HasKey("Id")
                        .HasName("pk_message_tags");

                    b.HasIndex("TrackerMessageId")
                        .HasDatabaseName("ix_message_tags_tracker_message_id");

                    b.ToTable("message_tags", (string)null);

                    b.HasDiscriminator<byte>("TagType");
                });

            modelBuilder.Entity("BioTonFMS.Domain.TrackerMessages.TrackerMessage", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<int?>("CoolantTemperature")
                        .HasColumnType("integer")
                        .HasColumnName("coolant_temperature");

                    b.Property<int?>("CoordCorrectness")
                        .HasColumnType("integer")
                        .HasColumnName("coord_correctness");

                    b.Property<double?>("Direction")
                        .HasColumnType("double precision")
                        .HasColumnName("direction");

                    b.Property<int?>("EngineSpeed")
                        .HasColumnType("integer")
                        .HasColumnName("engine_speed");

                    b.Property<int?>("FuelLevel")
                        .HasColumnType("integer")
                        .HasColumnName("fuel_level");

                    b.Property<double?>("Height")
                        .HasColumnType("double precision")
                        .HasColumnName("height");

                    b.Property<string>("Imei")
                        .IsRequired()
                        .HasMaxLength(15)
                        .HasColumnType("character varying(15)")
                        .HasColumnName("imei");

                    b.Property<double?>("Latitude")
                        .HasColumnType("double precision")
                        .HasColumnName("latitude");

                    b.Property<double?>("Longitude")
                        .HasColumnType("double precision")
                        .HasColumnName("longitude");

                    b.Property<int?>("SatNumber")
                        .HasColumnType("integer")
                        .HasColumnName("sat_number");

                    b.Property<DateTime>("ServerDateTime")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("server_date_time");

                    b.Property<double?>("Speed")
                        .HasColumnType("double precision")
                        .HasColumnName("speed");

                    b.Property<int>("TrId")
                        .HasColumnType("integer")
                        .HasColumnName("tr_id");

                    b.Property<DateTime>("TrackerDateTime")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("tracker_date_time");

                    b.HasKey("Id")
                        .HasName("pk_tracker_messages");

                    b.ToTable("tracker_messages", (string)null);
                });

            modelBuilder.Entity("BioTonFMS.Domain.TrackerMessages.MessageTagBits", b =>
                {
                    b.HasBaseType("BioTonFMS.Domain.TrackerMessages.MessageTag");

                    b.Property<BitArray>("Value")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("bit varying(32)")
                        .HasColumnName("value");

                    b.HasDiscriminator().HasValue((byte)2);
                });

            modelBuilder.Entity("BioTonFMS.Domain.TrackerMessages.MessageTagBoolean", b =>
                {
                    b.HasBaseType("BioTonFMS.Domain.TrackerMessages.MessageTag");

                    b.Property<bool>("Value")
                        .HasColumnType("boolean")
                        .HasColumnName("message_tag_boolean_value");

                    b.HasDiscriminator().HasValue((byte)5);
                });

            modelBuilder.Entity("BioTonFMS.Domain.TrackerMessages.MessageTagByte", b =>
                {
                    b.HasBaseType("BioTonFMS.Domain.TrackerMessages.MessageTag");

                    b.Property<byte>("Value")
                        .HasColumnType("smallint")
                        .HasColumnName("message_tag_byte_value");

                    b.HasDiscriminator().HasValue((byte)3);
                });

            modelBuilder.Entity("BioTonFMS.Domain.TrackerMessages.MessageTagDateTime", b =>
                {
                    b.HasBaseType("BioTonFMS.Domain.TrackerMessages.MessageTag");

                    b.Property<DateTime>("Value")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("message_tag_date_time_value");

                    b.HasDiscriminator().HasValue((byte)7);
                });

            modelBuilder.Entity("BioTonFMS.Domain.TrackerMessages.MessageTagDouble", b =>
                {
                    b.HasBaseType("BioTonFMS.Domain.TrackerMessages.MessageTag");

                    b.Property<double>("Value")
                        .HasColumnType("double precision")
                        .HasColumnName("message_tag_double_value");

                    b.HasDiscriminator().HasValue((byte)4);
                });

            modelBuilder.Entity("BioTonFMS.Domain.TrackerMessages.MessageTagInteger", b =>
                {
                    b.HasBaseType("BioTonFMS.Domain.TrackerMessages.MessageTag");

                    b.Property<int>("Value")
                        .HasColumnType("integer")
                        .HasColumnName("message_tag_integer_value");

                    b.HasDiscriminator().HasValue((byte)1);
                });

            modelBuilder.Entity("BioTonFMS.Domain.TrackerMessages.MessageTagString", b =>
                {
                    b.HasBaseType("BioTonFMS.Domain.TrackerMessages.MessageTag");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)")
                        .HasColumnName("message_tag_string_value");

                    b.HasDiscriminator().HasValue((byte)6);
                });

            modelBuilder.Entity("BioTonFMS.Domain.TrackerMessages.MessageTag", b =>
                {
                    b.HasOne("BioTonFMS.Domain.TrackerMessages.TrackerMessage", "TrackerMessage")
                        .WithMany("Tags")
                        .HasForeignKey("TrackerMessageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_message_tags_tracker_messages_tracker_message_id");

                    b.Navigation("TrackerMessage");
                });

            modelBuilder.Entity("BioTonFMS.Domain.TrackerMessages.TrackerMessage", b =>
                {
                    b.Navigation("Tags");
                });
#pragma warning restore 612, 618
        }
    }
}