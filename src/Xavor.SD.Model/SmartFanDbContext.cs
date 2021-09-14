using System;
using System.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Xavor.SD.Model
{
    public partial class SmartFanDbContext : DbContext
    {
        public SmartFanDbContext()
        {
        }

        public SmartFanDbContext(DbContextOptions<SmartFanDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Alarmsandwarnings> Alarmsandwarnings { get; set; }
        public virtual DbSet<Bom> Bom { get; set; }
        public virtual DbSet<Commandhistory> Commandhistory { get; set; }
        public virtual DbSet<Configurations> Configurations { get; set; }
        public virtual DbSet<Customer> Customer { get; set; }
        public virtual DbSet<Defaultsettings> Defaultsettings { get; set; }
        public virtual DbSet<Device> Device { get; set; }
        public virtual DbSet<Devicealarms> Devicealarms { get; set; }
        public virtual DbSet<Devicealarmshistory> Devicealarmshistory { get; set; }
        public virtual DbSet<Devicebatchnumber> Devicebatchnumber { get; set; }
        public virtual DbSet<Devicecommand> Devicecommand { get; set; }
        public virtual DbSet<Devicegroup> Devicegroup { get; set; }
        public virtual DbSet<Devicestatus> Devicestatus { get; set; }
        public virtual DbSet<Devicestatushistory> Devicestatushistory { get; set; }
        public virtual DbSet<Email> Email { get; set; }
        public virtual DbSet<EmailTemplate> EmailTemplate { get; set; }
        public virtual DbSet<Environmentsensors> Environmentsensors { get; set; }
        public virtual DbSet<Environmentstandards> Environmentstandards { get; set; }
        public virtual DbSet<Firmware> Firmware { get; set; }
        public virtual DbSet<Form> Form { get; set; }
        public virtual DbSet<Groupcommand> Groupcommand { get; set; }
        public virtual DbSet<Groups> Groups { get; set; }
        public virtual DbSet<Inverter> Inverter { get; set; }
        public virtual DbSet<License> License { get; set; }
        public virtual DbSet<Permission> Permission { get; set; }
        public virtual DbSet<Persona> Persona { get; set; }
        public virtual DbSet<Personapermission> Personapermission { get; set; }
        public virtual DbSet<Refreshtokens> Refreshtokens { get; set; }
        public virtual DbSet<Role> Role { get; set; }
        public virtual DbSet<Rolepermission> Rolepermission { get; set; }
        public virtual DbSet<Ruleengine> Ruleengine { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<Userdevice> Userdevice { get; set; }
        public virtual DbSet<Userrole> Userrole { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=tcp:autosfsqlserver.database.windows.net,1433;Initial Catalog=autosfdatabase;Persist Security Info=False;User ID=xadmin;Password=Xavor12345678;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.6-servicing-10079");

            modelBuilder.Entity<Alarmsandwarnings>(entity =>
            {
                entity.ToTable("alarmsandwarnings");

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Description).HasMaxLength(200);

                entity.Property(e => e.EncryptedCode)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Language).HasMaxLength(50);

                entity.Property(e => e.ReasonAnalysis).HasMaxLength(500);

                entity.Property(e => e.Timestamp).HasMaxLength(50);

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.HasOne(d => d.Inverter)
                    .WithMany(p => p.Alarmsandwarnings)
                    .HasForeignKey(d => d.InverterId)
                    .HasConstraintName("FK_alarmsandwarnings_inverter");
            });

            modelBuilder.Entity<Bom>(entity =>
            {
                entity.ToTable("bom");

                entity.Property(e => e.BatchId).HasMaxLength(50);

                entity.Property(e => e.BomType).HasMaxLength(50);

                entity.Property(e => e.CustomerId).HasMaxLength(50);

                entity.Property(e => e.FileFormat).HasMaxLength(50);

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<Commandhistory>(entity =>
            {
                entity.ToTable("commandhistory");

                entity.HasIndex(e => e.DeviceId)
                    .HasName("FK_DeviceCommandHistory_Device_idx");

                entity.HasIndex(e => e.GroupId)
                    .HasName("FK_GroupCommandHistory_Groups_idx");

                entity.Property(e => e.AutoEndTime)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.AutoStartTime)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.CommandId)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.CommandType)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime2(3)");

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime2(3)");

                entity.Property(e => e.TimeZone)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.HasOne(d => d.Device)
                    .WithMany(p => p.Commandhistory)
                    .HasForeignKey(d => d.DeviceId)
                    .HasConstraintName("FK_DeviceCommandHistory_CommandHistory");

                entity.HasOne(d => d.Group)
                    .WithMany(p => p.Commandhistory)
                    .HasForeignKey(d => d.GroupId)
                    .HasConstraintName("FK_GroupCommandHistory_CommandHistory");
            });

            modelBuilder.Entity<Configurations>(entity =>
            {
                entity.ToTable("configurations");

                entity.HasIndex(e => e.CustomerId)
                    .HasName("FK_Configuration_Customer_idx");

                entity.Property(e => e.Name)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.Value)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Configurations)
                    .HasForeignKey(d => d.CustomerId)
                    .HasConstraintName("FK_Configuration_Customer");
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.ToTable("customer");

                entity.Property(e => e.Address).IsUnicode(false);

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime2(3)");

                entity.Property(e => e.CustomerId)
                    .HasColumnName("CustomerID")
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerType)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ModifiedBy)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime2(3)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.RecordId).HasDefaultValueSql("(newid())");
            });

            modelBuilder.Entity<Defaultsettings>(entity =>
            {
                entity.ToTable("defaultsettings");

                entity.Property(e => e.AutoEndTime)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.AutoStartTime)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime2(0)");

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime2(0)");

                entity.Property(e => e.TimeZone)
                    .HasMaxLength(200)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Device>(entity =>
            {
                entity.ToTable("device");

                entity.HasIndex(e => e.CustomerId)
                    .HasName("FK_Device_Customer");

                entity.HasIndex(e => e.DeviceId)
                    .HasName("Device_ID_Unique")
                    .IsUnique();

                entity.Property(e => e.Appassword)
                    .HasColumnName("APPassword")
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.Apssid)
                    .HasColumnName("APSSID")
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.BatchId)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime2(3)");

                entity.Property(e => e.CurrentFirmwareVersion).HasMaxLength(50);

                entity.Property(e => e.DeviceCode)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.DeviceId)
                    .IsRequired()
                    .HasColumnName("DeviceID")
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.LastMaintenanceDate).HasColumnType("datetime2(3)");

                entity.Property(e => e.LatestFirmwareVersion).HasMaxLength(50);

                entity.Property(e => e.ModifiedBy)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime2(3)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.RecordId).HasDefaultValueSql("(newid())");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Device)
                    .HasForeignKey(d => d.CustomerId)
                    .HasConstraintName("FK_Device_Customer");

                entity.HasOne(d => d.Inverter)
                    .WithMany(p => p.Device)
                    .HasForeignKey(d => d.InverterId)
                    .HasConstraintName("FK_device_inverter");
            });

            modelBuilder.Entity<Devicealarms>(entity =>
            {
                entity.ToTable("devicealarms");

                entity.Property(e => e.Alarm)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerId)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.DeviceId)
                    .IsRequired()
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.Rpm).HasColumnName("RPM");

                entity.Property(e => e.Timestamp)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.Warning)
                    .HasMaxLength(200)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Devicealarmshistory>(entity =>
            {
                entity.ToTable("devicealarmshistory");

                entity.HasIndex(e => e.DeviceId)
                    .HasName("FK_DeviceAlarmsHistory_Device_idx");

                entity.Property(e => e.Alarm)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime2(0)")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime2(0)");

                entity.Property(e => e.Rpm).HasColumnName("RPM");

                entity.Property(e => e.Timestamp)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.Warning)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.HasOne(d => d.Device)
                    .WithMany(p => p.Devicealarmshistory)
                    .HasForeignKey(d => d.DeviceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DeviceAlarmsHistory_Device");
            });

            modelBuilder.Entity<Devicebatchnumber>(entity =>
            {
                entity.ToTable("devicebatchnumber");

                entity.Property(e => e.BatchId).HasMaxLength(50);

                entity.Property(e => e.BatchName).HasMaxLength(50);
            });

            modelBuilder.Entity<Devicecommand>(entity =>
            {
                entity.ToTable("devicecommand");

                entity.HasIndex(e => e.CommandHistoryId)
                    .HasName("FK_DeviceCommand_CommandHistory_idx");

                entity.HasIndex(e => e.DeviceId)
                    .HasName("FK_DeviceCommand_Device_idx");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime2(3)");

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime2(3)");

                entity.HasOne(d => d.CommandHistory)
                    .WithMany(p => p.Devicecommand)
                    .HasForeignKey(d => d.CommandHistoryId)
                    .HasConstraintName("FK_DeviceCommand_CommandHistory");

                entity.HasOne(d => d.Device)
                    .WithMany(p => p.Devicecommand)
                    .HasForeignKey(d => d.DeviceId)
                    .HasConstraintName("FK_DeviceCommand_Device");
            });

            modelBuilder.Entity<Devicegroup>(entity =>
            {
                entity.ToTable("devicegroup");

                entity.HasIndex(e => e.DeviceId)
                    .HasName("FK_DeviceGroup_Device");

                entity.HasIndex(e => e.GroupId)
                    .HasName("FK_DeviceGroup_Group");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime2(3)");

                entity.Property(e => e.ModifiedBy)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime2(3)");

                entity.HasOne(d => d.Device)
                    .WithMany(p => p.Devicegroup)
                    .HasForeignKey(d => d.DeviceId)
                    .HasConstraintName("FK_DeviceGroup_Device");

                entity.HasOne(d => d.Group)
                    .WithMany(p => p.Devicegroup)
                    .HasForeignKey(d => d.GroupId)
                    .HasConstraintName("FK_DeviceGroup_Group");
            });

            modelBuilder.Entity<Devicestatus>(entity =>
            {
                entity.ToTable("devicestatus");

                entity.HasIndex(e => e.DeviceId)
                    .HasName("FK_DeviceDeviceStatus_DeviceStatus_idx");

                entity.Property(e => e.Alarm)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.AutoEndTime)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.AutoStartTime)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.CommandType)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime2(3)");

                entity.Property(e => e.TimeZone)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.Warnings)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.HasOne(d => d.Device)
                    .WithMany(p => p.Devicestatus)
                    .HasForeignKey(d => d.DeviceId)
                    .HasConstraintName("FK_DeviceDeviceStatus_DeviceStatus");
            });

            modelBuilder.Entity<Devicestatushistory>(entity =>
            {
                entity.ToTable("devicestatushistory");

                entity.HasIndex(e => e.DeviceId)
                    .HasName("FK_DeviceStatusHistory_Device");

                entity.Property(e => e.AutoEndTime)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.AutoStartTime)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.CommandType)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime2(3)");

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime2(3)");

                entity.Property(e => e.TimeZone)
                    .HasMaxLength(200)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Email>(entity =>
            {
                entity.ToTable("email");

                entity.Property(e => e.Body).HasMaxLength(750);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime2(3)");

                entity.Property(e => e.From).HasMaxLength(250);

                entity.Property(e => e.Subject).HasMaxLength(250);

                entity.Property(e => e.To).HasMaxLength(250);
            });

            modelBuilder.Entity<EmailTemplate>(entity =>
            {
                entity.Property(e => e.Body).HasMaxLength(750);

                entity.Property(e => e.Code).HasMaxLength(50);

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime2(3)")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");

                entity.Property(e => e.Language).HasMaxLength(50);

                entity.Property(e => e.Subject).HasMaxLength(250);
            });

            modelBuilder.Entity<Environmentsensors>(entity =>
            {
                entity.ToTable("environmentsensors");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Unit).HasMaxLength(50);
            });

            modelBuilder.Entity<Environmentstandards>(entity =>
            {
                entity.ToTable("environmentstandards");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Unit).HasMaxLength(50);
            });

            modelBuilder.Entity<Firmware>(entity =>
            {
                entity.ToTable("firmware");

                entity.Property(e => e.BatchId).HasMaxLength(50);

                entity.Property(e => e.CustomerId).HasMaxLength(50);

                entity.Property(e => e.FileFormat).HasMaxLength(50);

                entity.Property(e => e.FirmwareVersion).HasMaxLength(50);

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<Form>(entity =>
            {
                entity.ToTable("form");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime2(3)")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.FormId).HasMaxLength(50);

                entity.Property(e => e.FormName).HasMaxLength(50);

                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<Groupcommand>(entity =>
            {
                entity.ToTable("groupcommand");

                entity.HasIndex(e => e.CommandHistoryId)
                    .HasName("FK_CommandHistory_GroupCommand_idx");

                entity.HasIndex(e => e.GroupId)
                    .HasName("FK_GroupGroupCommand_GroupCommand_idx");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime2(3)");

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime2(3)");

                entity.HasOne(d => d.CommandHistory)
                    .WithMany(p => p.Groupcommand)
                    .HasForeignKey(d => d.CommandHistoryId)
                    .HasConstraintName("FK_CommandHistory_GroupCommand");

                entity.HasOne(d => d.Group)
                    .WithMany(p => p.Groupcommand)
                    .HasForeignKey(d => d.GroupId)
                    .HasConstraintName("FK_Group_GroupCommand");
            });

            modelBuilder.Entity<Groups>(entity =>
            {
                entity.ToTable("groups");

                entity.HasIndex(e => e.CustomerId)
                    .HasName("FK_Groups_Customer");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime2(3)");

                entity.Property(e => e.GroupId)
                    .HasMaxLength(45)
                    .IsUnicode(false);

                entity.Property(e => e.ModifiedBy)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime2(3)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.RecordId).HasDefaultValueSql("(newid())");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Groups)
                    .HasForeignKey(d => d.CustomerId)
                    .HasConstraintName("FK_Groups_Customer");
            });

            modelBuilder.Entity<Inverter>(entity =>
            {
                entity.ToTable("inverter");

                entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.InverterId).HasMaxLength(50);

                entity.Property(e => e.InverterName).HasMaxLength(50);

                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");

                entity.Property(e => e.ZhInverterName)
                    .HasColumnName("zh_InverterName")
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<License>(entity =>
            {
                entity.ToTable("license");

                entity.Property(e => e.LicenseName).HasMaxLength(50);

                entity.Property(e => e.LicenseType).HasMaxLength(50);

                entity.Property(e => e.RecordId).HasDefaultValueSql("(newid())");
            });

            modelBuilder.Entity<Permission>(entity =>
            {
                entity.ToTable("permission");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime2(3)");

                entity.Property(e => e.Description)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.ModifiedBy)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime2(3)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.Value)
                    .HasMaxLength(500)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Persona>(entity =>
            {
                entity.ToTable("persona");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime2(3)")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");

                entity.Property(e => e.PersonaId).HasMaxLength(50);

                entity.Property(e => e.PersonaName).HasMaxLength(50);
            });

            modelBuilder.Entity<Personapermission>(entity =>
            {
                entity.ToTable("personapermission");

                entity.HasOne(d => d.Form)
                    .WithMany(p => p.Personapermission)
                    .HasForeignKey(d => d.FormId)
                    .HasConstraintName("FK_personapermission_form");

                entity.HasOne(d => d.Persona)
                    .WithMany(p => p.Personapermission)
                    .HasForeignKey(d => d.PersonaId)
                    .HasConstraintName("FK_personapermission_persona");
            });

            modelBuilder.Entity<Refreshtokens>(entity =>
            {
                entity.ToTable("refreshtokens");

                entity.HasIndex(e => e.UserId)
                    .HasName("FK_REFRESHTOKENS_USER_idx");

                entity.Property(e => e.RefreshToken)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Refreshtokens)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_REFRESHTOKENS_USER");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("role");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime2(3)");

                entity.Property(e => e.Description)
                    .HasMaxLength(300)
                    .IsUnicode(false);

                entity.Property(e => e.ModifiedBy)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime2(3)");

                entity.Property(e => e.Role1)
                    .IsRequired()
                    .HasColumnName("Role")
                    .HasMaxLength(200)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Rolepermission>(entity =>
            {
                entity.ToTable("rolepermission");

                entity.HasIndex(e => e.FormId)
                    .HasName("FK_RolePermission_Role");

                entity.HasIndex(e => e.RoleId)
                    .HasName("FK_RolePermission_Permission");

                entity.Property(e => e.CanDelete).HasDefaultValueSql("((0))");

                entity.Property(e => e.CanExport).HasDefaultValueSql("((0))");

                entity.Property(e => e.CanInsert).HasDefaultValueSql("((0))");

                entity.Property(e => e.CanUpdate).HasDefaultValueSql("((0))");

                entity.Property(e => e.CanView).HasDefaultValueSql("((0))");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime2(3)")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");

                entity.HasOne(d => d.Form)
                    .WithMany(p => p.Rolepermission)
                    .HasForeignKey(d => d.FormId)
                    .HasConstraintName("FK_rolepermission_form");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Rolepermission)
                    .HasForeignKey(d => d.RoleId)
                    .HasConstraintName("FK_rolepermission_role");
            });

            modelBuilder.Entity<Ruleengine>(entity =>
            {
                entity.ToTable("ruleengine");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime2(3)")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");

                entity.Property(e => e.ModifiedBy)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime2(3)");

                entity.Property(e => e.RecordId).HasDefaultValueSql("(newid())");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Ruleengine)
                    .HasForeignKey(d => d.CustomerId)
                    .HasConstraintName("FK_RuleEngine_customer");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("user");

                entity.HasIndex(e => e.CustomerId)
                    .HasName("FK_User_Customer");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime2(3)");

                entity.Property(e => e.DeviceIdentifier)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.DeviceType)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.DomainUserName)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.EmailAddress)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.FirstName).HasMaxLength(200);

                entity.Property(e => e.IdentityProvider)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.LastName).HasMaxLength(200);

                entity.Property(e => e.ModifiedBy)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime2(3)");

                entity.Property(e => e.Password).HasMaxLength(200);

                entity.Property(e => e.ProfilePicture).IsUnicode(false);

                entity.Property(e => e.RecordId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasColumnName("UserID")
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.User)
                    .HasForeignKey(d => d.CustomerId)
                    .HasConstraintName("FK_User_Customer");
            });

            modelBuilder.Entity<Userdevice>(entity =>
            {
                entity.ToTable("userdevice");

                entity.HasIndex(e => e.DeviceId)
                    .HasName("FK_UserDevice_Device");

                entity.HasIndex(e => e.UserId)
                    .HasName("FK_UserDevice_User");

                entity.HasOne(d => d.Device)
                    .WithMany(p => p.Userdevice)
                    .HasForeignKey(d => d.DeviceId)
                    .HasConstraintName("FK_UserDevice_Device");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Userdevice)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_UserDevice_User");
            });

            modelBuilder.Entity<Userrole>(entity =>
            {
                entity.ToTable("userrole");

                entity.HasIndex(e => e.RoleId)
                    .HasName("FK_UserRoleMapping_Role");

                entity.HasIndex(e => e.UserId)
                    .HasName("FK_UserRoleMapping_User");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Userrole)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserRoleMapping_Role");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Userrole)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserRoleMapping_User");
            });
        }
    }
}
