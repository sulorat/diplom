using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace diplom.Models;

public partial class DiplomContext : DbContext
{
    public DiplomContext()
    {
    }

    public DiplomContext(DbContextOptions<DiplomContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Equipment> Equipments { get; set; }

    public virtual DbSet<Equipmentmovement> Equipmentmovements { get; set; }

    public virtual DbSet<Event> Events { get; set; }

    public virtual DbSet<Eventstatus> Eventstatuses { get; set; }

    public virtual DbSet<Movementtype> Movementtypes { get; set; }

    public virtual DbSet<Priority> Priorities { get; set; }

    public virtual DbSet<Repairrequest> Repairrequests { get; set; }

    public virtual DbSet<Requeststatus> Requeststatuses { get; set; }

    public virtual DbSet<Staff> Staff { get; set; }

    public virtual DbSet<Status> Statuses { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host = 2.26.8.167; database = postgres; username = sasha; password = L1Jj9wYngP; port = 5454");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Equipment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("equipments_pkey");
                
            entity.ToTable("equipments", "dimka");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Dateoflastcheck).HasColumnName("dateoflastcheck");
            entity.Property(e => e.Enddate).HasColumnName("enddate");
            entity.Property(e => e.Imagepath)
                .HasColumnType("character varying")
                .HasColumnName("imagepath");
            entity.Property(e => e.Isdeleted).HasColumnName("isdeleted");
            entity.Property(e => e.Name)
                .HasMaxLength(128)
                .HasColumnName("name");
            entity.Property(e => e.Place)
                .HasMaxLength(128)
                .HasColumnName("place");
            entity.Property(e => e.Productioncapacity).HasColumnName("productioncapacity");
            entity.Property(e => e.StatusId).HasColumnName("status_id");
            entity.Property(e => e.Workhours).HasColumnName("workhours");

            entity.HasOne(d => d.Status).WithMany(p => p.Equipment)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("equipments_status_id_fkey");
        });

        modelBuilder.Entity<Equipmentmovement>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("equipmentmovement_pkey");

            entity.ToTable("equipmentmovement", "dimka");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.EquipmentId).HasColumnName("equipment_id");
            entity.Property(e => e.Fromdate).HasColumnName("fromdate");
            entity.Property(e => e.Fromplace)
                .HasMaxLength(128)
                .HasColumnName("fromplace");
            entity.Property(e => e.Reason)
                .HasMaxLength(256)
                .HasColumnName("reason");
            entity.Property(e => e.Responsibleperson).HasColumnName("responsibleperson");
            entity.Property(e => e.Todate).HasColumnName("todate");
            entity.Property(e => e.Toplace)
                .HasMaxLength(128)
                .HasColumnName("toplace");
            entity.Property(e => e.TypeId).HasColumnName("type_id");

            entity.HasOne(d => d.Equipment).WithMany(p => p.Equipmentmovements)
                .HasForeignKey(d => d.EquipmentId)
                .HasConstraintName("equipmentmovement_equipment_id_fkey");

            entity.HasOne(d => d.ResponsiblepersonNavigation).WithMany(p => p.Equipmentmovements)
                .HasForeignKey(d => d.Responsibleperson)
                .HasConstraintName("equipmentmovement_responsibleperson_fkey");

            entity.HasOne(d => d.Type).WithMany(p => p.Equipmentmovements)
                .HasForeignKey(d => d.TypeId)
                .HasConstraintName("equipmentmovement_type_id_fkey");
        });

        modelBuilder.Entity<Event>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("events_pkey");

            entity.ToTable("events", "dimka");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Dateof).HasColumnName("dateof");
            entity.Property(e => e.Description)
                .HasMaxLength(256)
                .HasColumnName("description");
            entity.Property(e => e.EquipmentId).HasColumnName("equipment_id");
            entity.Property(e => e.EventstatusId).HasColumnName("eventstatus_id");
            entity.Property(e => e.Isdeleted).HasColumnName("isdeleted");

            entity.HasOne(d => d.Equipment).WithMany(p => p.Events)
                .HasForeignKey(d => d.EquipmentId)
                .HasConstraintName("events_equipment_id_fkey");

            entity.HasOne(d => d.Eventstatus).WithMany(p => p.Events)
                .HasForeignKey(d => d.EventstatusId)
                .HasConstraintName("events_eventstatus_id_fkey");
        });

        modelBuilder.Entity<Eventstatus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("eventstatus_pkey");

            entity.ToTable("eventstatus", "dimka");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Description)
                .HasMaxLength(256)
                .HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(64)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Movementtype>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("movementtypes_pkey");

            entity.ToTable("movementtypes", "dimka");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(64)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Priority>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("priorities_pkey");

            entity.ToTable("priorities", "dimka");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(32)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Repairrequest>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("repairrequests_pkey");

            entity.ToTable("repairrequests", "dimka");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Createdat).HasColumnName("createdat");
            entity.Property(e => e.Description)
                .HasMaxLength(256)
                .HasColumnName("description");
            entity.Property(e => e.EquipmentId).HasColumnName("equipment_id");
            entity.Property(e => e.Isdeleted).HasColumnName("isdeleted");
            entity.Property(e => e.Priority).HasColumnName("priority");
            entity.Property(e => e.StatusId).HasColumnName("status_id");

            entity.HasOne(d => d.Equipment).WithMany(p => p.Repairrequests)
                .HasForeignKey(d => d.EquipmentId)
                .HasConstraintName("repairrequests_equipment_id_fkey");

            entity.HasOne(d => d.PriorityNavigation).WithMany(p => p.Repairrequests)
                .HasForeignKey(d => d.Priority)
                .HasConstraintName("repairrequests_priority_fkey");

            entity.HasOne(d => d.Status).WithMany(p => p.Repairrequests)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("repairrequests_status_id_fkey");
        });

        modelBuilder.Entity<Requeststatus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("requeststatuses_pkey");

            entity.ToTable("requeststatuses", "dimka");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(32)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Staff>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("staff_pkey");

            entity.ToTable("staff", "dimka");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Login)
                .HasColumnType("character varying")
                .HasColumnName("login");
            entity.Property(e => e.Name)
                .HasMaxLength(256)
                .HasColumnName("name");
            entity.Property(e => e.Password)
                .HasColumnType("character varying")
                .HasColumnName("password");
            entity.Property(e => e.Post)
                .HasMaxLength(128)
                .HasColumnName("post");
        });

        modelBuilder.Entity<Status>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("status_pkey");

            entity.ToTable("status", "dimka");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(128)
                .HasColumnName("name");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
