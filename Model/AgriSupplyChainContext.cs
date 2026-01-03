using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Model;

public partial class AgriSupplyChainContext : DbContext
{
    public AgriSupplyChainContext()
    {
    }

    public AgriSupplyChainContext(DbContextOptions<AgriSupplyChainContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AuditLog> AuditLogs { get; set; }

    public virtual DbSet<Batch> Batches { get; set; }

    public virtual DbSet<DigitalSignature> DigitalSignatures { get; set; }

    public virtual DbSet<Distributor> Distributors { get; set; }

    public virtual DbSet<Farm> Farms { get; set; }

    public virtual DbSet<Inspection> Inspections { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<QrCode> QrCodes { get; set; }

    public virtual DbSet<Retailer> Retailers { get; set; }

    public virtual DbSet<RetailerStock> RetailerStocks { get; set; }

    public virtual DbSet<Shipment> Shipments { get; set; }

    public virtual DbSet<ShipmentItem> ShipmentItems { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Warehouse> Warehouses { get; set; }

    public virtual DbSet<WarehouseStock> WarehouseStocks { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=ADMIN-PC;Database=agri_supplychain;Trusted_Connection=True;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__audit_lo__3213E83F20342D53");

            entity.ToTable("audit_logs");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Action)
                .HasMaxLength(50)
                .HasColumnName("action");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("created_at");
            entity.Property(e => e.NewValue).HasColumnName("new_value");
            entity.Property(e => e.OldValue).HasColumnName("old_value");
            entity.Property(e => e.RowId)
                .HasMaxLength(128)
                .HasColumnName("row_id");
            entity.Property(e => e.TableName)
                .HasMaxLength(128)
                .HasColumnName("table_name");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.AuditLogs)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_audit_user");
        });

        modelBuilder.Entity<Batch>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__batches__3213E83F2A8D995A");

            entity.ToTable("batches");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BatchCode)
                .HasMaxLength(100)
                .HasColumnName("batch_code");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedByUserId).HasColumnName("created_by_user_id");
            entity.Property(e => e.ExpiryDate).HasColumnName("expiry_date");
            entity.Property(e => e.FarmId).HasColumnName("farm_id");
            entity.Property(e => e.HarvestDate).HasColumnName("harvest_date");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.Quantity)
                .HasColumnType("decimal(12, 3)")
                .HasColumnName("quantity");
            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .HasDefaultValue("pending")
                .HasColumnName("status");
            entity.Property(e => e.Unit)
                .HasMaxLength(50)
                .HasDefaultValue("kg")
                .HasColumnName("unit");

            entity.HasOne(d => d.CreatedByUser).WithMany(p => p.Batches)
                .HasForeignKey(d => d.CreatedByUserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_batches_createdby");

            entity.HasOne(d => d.Farm).WithMany(p => p.Batches)
                .HasForeignKey(d => d.FarmId)
                .HasConstraintName("fk_batches_farm");

            entity.HasOne(d => d.Product).WithMany(p => p.Batches)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("fk_batches_product");
        });

        modelBuilder.Entity<DigitalSignature>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__digital___3213E83F9460C9CC");

            entity.ToTable("digital_signatures");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Notes).HasColumnName("notes");
            entity.Property(e => e.ReferenceDocument)
                .HasMaxLength(255)
                .HasColumnName("reference_document");
            entity.Property(e => e.SignatureMethod)
                .HasMaxLength(100)
                .HasColumnName("signature_method");
            entity.Property(e => e.SignatureValue).HasColumnName("signature_value");
            entity.Property(e => e.SignedAt)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("signed_at");
            entity.Property(e => e.SignerUserId).HasColumnName("signer_user_id");

            entity.HasOne(d => d.SignerUser).WithMany(p => p.DigitalSignatures)
                .HasForeignKey(d => d.SignerUserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_digsig_user");
        });

        modelBuilder.Entity<Distributor>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__distribu__3213E83FA542E57C");

            entity.ToTable("distributors");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ContactInfo)
                .HasMaxLength(500)
                .HasColumnName("contact_info");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("created_at");
            entity.Property(e => e.Name)
                .HasMaxLength(200)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Farm>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__farms__3213E83F8140FE3F");

            entity.ToTable("farms");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Certifications).HasColumnName("certifications");
            entity.Property(e => e.ContactInfo)
                .HasMaxLength(500)
                .HasColumnName("contact_info");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("created_at");
            entity.Property(e => e.Location)
                .HasMaxLength(500)
                .HasColumnName("location");
            entity.Property(e => e.Name)
                .HasMaxLength(200)
                .HasColumnName("name");
            entity.Property(e => e.OwnerName)
                .HasMaxLength(200)
                .HasColumnName("owner_name");
        });

        modelBuilder.Entity<Inspection>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__inspecti__3213E83FF0E42187");

            entity.ToTable("inspections");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BatchId).HasColumnName("batch_id");
            entity.Property(e => e.ChemicalResidue)
                .HasColumnType("decimal(8, 4)")
                .HasColumnName("chemical_residue");
            entity.Property(e => e.Humidity)
                .HasColumnType("decimal(6, 2)")
                .HasColumnName("humidity");
            entity.Property(e => e.InspectionDate)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("inspection_date");
            entity.Property(e => e.InspectorUserId).HasColumnName("inspector_user_id");
            entity.Property(e => e.Notes).HasColumnName("notes");
            entity.Property(e => e.QualityScore).HasColumnName("quality_score");
            entity.Property(e => e.ReportFile)
                .HasMaxLength(255)
                .HasColumnName("report_file");
            entity.Property(e => e.SignatureId).HasColumnName("signature_id");
            entity.Property(e => e.Temperature)
                .HasColumnType("decimal(6, 2)")
                .HasColumnName("temperature");

            entity.HasOne(d => d.Batch).WithMany(p => p.Inspections)
                .HasForeignKey(d => d.BatchId)
                .HasConstraintName("fk_inspections_batch");

            entity.HasOne(d => d.InspectorUser).WithMany(p => p.Inspections)
                .HasForeignKey(d => d.InspectorUserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_inspections_inspector");

            entity.HasOne(d => d.Signature).WithMany(p => p.Inspections)
                .HasForeignKey(d => d.SignatureId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_inspections_signature");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__products__3213E83F0D98FC2F");

            entity.ToTable("products");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Category)
                .HasMaxLength(100)
                .HasColumnName("category");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("created_at");
            entity.Property(e => e.Name)
                .HasMaxLength(200)
                .HasColumnName("name");
            entity.Property(e => e.Sku)
                .HasMaxLength(100)
                .HasColumnName("sku");
            entity.Property(e => e.StorageInstructions)
                .HasMaxLength(500)
                .HasColumnName("storage_instructions");
            entity.Property(e => e.TypicalShelfLifeDays).HasColumnName("typical_shelf_life_days");
        });

        modelBuilder.Entity<QrCode>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__qr_codes__3213E83FC7C693CC");

            entity.ToTable("qr_codes");

            entity.HasIndex(e => e.Token, "UQ__qr_codes__CA90DA7A4AE431FC").IsUnique();

            entity.HasIndex(e => e.BatchId, "UQ__qr_codes__DBFC0430F8FB8967").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BatchId).HasColumnName("batch_id");
            entity.Property(e => e.GeneratedAt)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("generated_at");
            entity.Property(e => e.Token)
                .HasMaxLength(255)
                .HasColumnName("token");
            entity.Property(e => e.Url)
                .HasMaxLength(500)
                .HasColumnName("url");

            entity.HasOne(d => d.Batch).WithOne(p => p.QrCode)
                .HasForeignKey<QrCode>(d => d.BatchId)
                .HasConstraintName("fk_qr_batch");
        });

        modelBuilder.Entity<Retailer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__retailer__3213E83F0F1CA312");

            entity.ToTable("retailers");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ContactInfo)
                .HasMaxLength(500)
                .HasColumnName("contact_info");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("created_at");
            entity.Property(e => e.Location)
                .HasMaxLength(500)
                .HasColumnName("location");
            entity.Property(e => e.Name)
                .HasMaxLength(200)
                .HasColumnName("name");
        });

        modelBuilder.Entity<RetailerStock>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__retailer__3213E83FEC67A16A");

            entity.ToTable("retailer_stock");

            entity.HasIndex(e => new { e.RetailerId, e.BatchId }, "uq_retailer_batch").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BatchId).HasColumnName("batch_id");
            entity.Property(e => e.LastUpdated)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("last_updated");
            entity.Property(e => e.Quantity)
                .HasColumnType("decimal(12, 3)")
                .HasColumnName("quantity");
            entity.Property(e => e.RetailerId).HasColumnName("retailer_id");
            entity.Property(e => e.Unit)
                .HasMaxLength(50)
                .HasDefaultValue("kg")
                .HasColumnName("unit");

            entity.HasOne(d => d.Batch).WithMany(p => p.RetailerStocks)
                .HasForeignKey(d => d.BatchId)
                .HasConstraintName("fk_retailerstock_batch");

            entity.HasOne(d => d.Retailer).WithMany(p => p.RetailerStocks)
                .HasForeignKey(d => d.RetailerId)
                .HasConstraintName("fk_retailerstock_retailer");
        });

        modelBuilder.Entity<Shipment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__shipment__3213E83FBA74B1BD");

            entity.ToTable("shipments");

            entity.HasIndex(e => e.ShipmentCode, "UQ__shipment__CB13E6245299CEBE").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ArrivalTime).HasColumnName("arrival_time");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("created_at");
            entity.Property(e => e.DepartureTime).HasColumnName("departure_time");
            entity.Property(e => e.DistributorId).HasColumnName("distributor_id");
            entity.Property(e => e.DriverInfo)
                .HasMaxLength(255)
                .HasColumnName("driver_info");
            entity.Property(e => e.FromId).HasColumnName("from_id");
            entity.Property(e => e.FromType)
                .HasMaxLength(50)
                .HasColumnName("from_type");
            entity.Property(e => e.Notes).HasColumnName("notes");
            entity.Property(e => e.PlannedByUserId).HasColumnName("planned_by_user_id");
            entity.Property(e => e.ShipmentCode)
                .HasMaxLength(100)
                .HasColumnName("shipment_code");
            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .HasDefaultValue("pending")
                .HasColumnName("status");
            entity.Property(e => e.ToId).HasColumnName("to_id");
            entity.Property(e => e.ToType)
                .HasMaxLength(50)
                .HasColumnName("to_type");
            entity.Property(e => e.VehicleInfo)
                .HasMaxLength(255)
                .HasColumnName("vehicle_info");

            entity.HasOne(d => d.Distributor).WithMany(p => p.Shipments)
                .HasForeignKey(d => d.DistributorId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_shipments_distributor");

            entity.HasOne(d => d.PlannedByUser).WithMany(p => p.Shipments)
                .HasForeignKey(d => d.PlannedByUserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_shipments_planner");
        });

        modelBuilder.Entity<ShipmentItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__shipment__3213E83F09748E8A");

            entity.ToTable("shipment_items");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BatchId).HasColumnName("batch_id");
            entity.Property(e => e.Quantity)
                .HasColumnType("decimal(12, 3)")
                .HasColumnName("quantity");
            entity.Property(e => e.ShipmentId).HasColumnName("shipment_id");
            entity.Property(e => e.Unit)
                .HasMaxLength(50)
                .HasDefaultValue("kg")
                .HasColumnName("unit");

            entity.HasOne(d => d.Batch).WithMany(p => p.ShipmentItems)
                .HasForeignKey(d => d.BatchId)
                .HasConstraintName("fk_shipmentitems_batch");

            entity.HasOne(d => d.Shipment).WithMany(p => p.ShipmentItems)
                .HasForeignKey(d => d.ShipmentId)
                .HasConstraintName("fk_shipmentitems_shipment");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__users__3213E83F7C45A38F");

            entity.ToTable("users");

            entity.HasIndex(e => e.Username, "UQ__users__F3DBC5726231BA84").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.FullName)
                .HasMaxLength(200)
                .HasColumnName("full_name");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .HasColumnName("password_hash");
            entity.Property(e => e.PasswordResetExpiry).HasColumnName("password_reset_expiry");
            entity.Property(e => e.PasswordResetToken)
                .HasMaxLength(100)
                .HasColumnName("password_reset_token");
            entity.Property(e => e.Phone)
                .HasMaxLength(50)
                .HasColumnName("phone");
            entity.Property(e => e.Role)
                .HasMaxLength(20)
                .HasColumnName("role");
            entity.Property(e => e.Username)
                .HasMaxLength(100)
                .HasColumnName("username");
        });

        modelBuilder.Entity<Warehouse>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__warehous__3213E83FD1C73684");

            entity.ToTable("warehouses");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ContactInfo)
                .HasMaxLength(500)
                .HasColumnName("contact_info");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("created_at");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.Location)
                .HasMaxLength(500)
                .HasColumnName("location");
            entity.Property(e => e.Name)
                .HasMaxLength(200)
                .HasColumnName("name");
        });

        modelBuilder.Entity<WarehouseStock>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__warehous__3213E83FA301CEF4");

            entity.ToTable("warehouse_stock");

            entity.HasIndex(e => new { e.WarehouseId, e.BatchId }, "uq_warehouse_batch").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BatchId).HasColumnName("batch_id");
            entity.Property(e => e.LastUpdated)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("last_updated");
            entity.Property(e => e.Quantity)
                .HasColumnType("decimal(12, 3)")
                .HasColumnName("quantity");
            entity.Property(e => e.Unit)
                .HasMaxLength(50)
                .HasDefaultValue("kg")
                .HasColumnName("unit");
            entity.Property(e => e.WarehouseId).HasColumnName("warehouse_id");

            entity.HasOne(d => d.Batch).WithMany(p => p.WarehouseStocks)
                .HasForeignKey(d => d.BatchId)
                .HasConstraintName("fk_stock_batch");

            entity.HasOne(d => d.Warehouse).WithMany(p => p.WarehouseStocks)
                .HasForeignKey(d => d.WarehouseId)
                .HasConstraintName("fk_stock_warehouse");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
