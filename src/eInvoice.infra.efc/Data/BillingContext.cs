using eInvoice.domain.Common;
using eInvoice.infra.efc.Infra.Models;
using eInvoice.infra.efc.Models;
using System.Net.NetworkInformation;

namespace eInvoice.infra.efc.Data;

internal class BillingContext : DbContext
{
    public DbSet<Configuration> Configurations { get; set; }
    public DbSet<Company> Companies { get; set; }
    public DbSet<CustomerMaster> CustomerMasters { get; set; }
    public DbSet<EdiinvoicingTo> EdiInvocingTos { get; set; }
    public DbSet<Invoice> Invoices { get; set; }
    public DbSet<SapInvoice> Sapinvoices { get; set; }
    public DbSet<InvoiceItem> InvoiceItems { get; set; }
    public DbSet<InvoiceEvent> InvoiceEvents { get; set; }
    public DbSet<InvoicePool> InvoicePools { get; set; }
    public DbSet<InvoiceStorage> InvoiceStorages { get; set; }
    public DbSet<VslVoyLine> VslVoyLines { get; set; }
    public DbSet<VslVoyage> VslVoyages { get; set; }
    public DbSet<BusinessSpaceArea> BusinessSpaceAreas { get; set; }
    public DbSet<BankData> BankData { get; set; }
    public DbSet<TariffDesc> TariffDescs { get; set; }
    public DbSet<CommodityClassification> CommodityClassifications { get; set; }
    public DbSet<InvoiceAdded> InvoiceAdded { get; set; }
    public DbSet<InvoiceAttachment> InvoiceAttachment { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<BusinessProcessProfile> BusinessProcessProfiles { get; set; }
    public DbSet<InvoiceDetail> InvoiceDetails { get; set; }
    public DbSet<CustomerDetails> CustomerDetails { get; set; }
    public DbSet<Vatclause> Vatclauses { get; set; }

    public BillingContext(DbContextOptions<BillingContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("SQL_Latin1_General_CP1_CI_AS");

        modelBuilder.Entity<Configuration>()
            .ToTable("Configuration");

        modelBuilder.Entity<User>()
            .ToTable("Users")
            .HasKey(u => u.UserId);

        modelBuilder.Entity<User>()
            .HasOne(u => u.UserChangerNavigation)
            .WithMany()
            .HasForeignKey(u => u.UserChanger)
            .HasPrincipalKey(u => u.UserId)
            .IsRequired(false);

        modelBuilder.Entity<User>()
            .HasOne(u => u.UserCreatorNavigation)
            .WithMany()
            .HasForeignKey(u => u.UserCreator)
            .HasPrincipalKey(u => u.UserId)
            .IsRequired(false);

        modelBuilder.Entity<InvoiceEvent>(entity =>
        {
            entity.ToTable("InvoiceEvents");
            entity.Property(ie => ie.LineOperatorId)
                  .HasColumnName("line_operator_id");
            entity.Property(ie => ie.VesselId)
                  .HasColumnName("vessel_id");
            entity.HasKey(ie => ie.Gkey);
        });

        modelBuilder.Entity<VslVoyLine>()
            .HasNoKey()
            .ToView("VslVoyLines");

        modelBuilder.Entity<Invoice>()
            .HasMany(s => s.SapInvoices)
            .WithOne(i => i.Invoice)
            .HasForeignKey(s => s.InvoiceNumber)
            .HasPrincipalKey(i => i.InvoiceNumber)
            .IsRequired(false);

        modelBuilder.Entity<Invoice>()
            .HasOne(i => i.InvoicedUserNavigation)
            .WithMany()
            .HasForeignKey(i => i.InvoicedUser)
            .HasPrincipalKey(u => u.UserId)
            .IsRequired(false);

        modelBuilder.Entity<CustomerMaster>()
            .HasNoKey()
            .ToView(nameof(CustomerMaster));

        modelBuilder.Entity<Invoice>()
                    .HasOne(i => i.EdiinvoicingTo)
                    .WithMany()
                    .HasForeignKey(i => i.BillToParty)
                    .HasPrincipalKey(e => e.BillToParty)
                    .IsRequired(false);

        modelBuilder.Entity<Invoice>()
            .HasOne(i => i.InvoiceTypeNavigation)
            .WithMany()
            .HasForeignKey(i => i.InvoiceType)
            .HasPrincipalKey(e => e.InvoiceTypeId)
            .IsRequired(false);

        modelBuilder.Entity<Invoice>()
            .HasMany(i => i.InvoiceAttachments)
            .WithOne(i => i.Invoice)
            .HasForeignKey(a => a.InvoiceNumber)
            .HasPrincipalKey(i => i.InvoiceNumber)
            .IsRequired();

        modelBuilder.Entity<InvoiceTypeEntity>()
                    .HasOne(i => i.VatClauseNavigation)
                    .WithMany()
                    .HasForeignKey(i => i.VatClause)
                    .HasPrincipalKey(v => v.Id)
                    .IsRequired(false);

        modelBuilder.Entity<InvoiceTypeEntity>()
            .HasMany(it => it.Invoices)
            .WithOne()
            .HasForeignKey(it => it.InvoiceNumber)
            .HasPrincipalKey(i => i.InvoiceTypeId);

        modelBuilder.Entity<SapInvoice>(enitity =>
        {
            enitity.HasKey(s => s.AltInvoiceNumber)
                   .HasName("PK_SepInvoice");
            enitity.Property(s => s.Sapinvoice1)
                   .HasColumnName("SAPinvoice");
            enitity.ToTable("SapInvoices");
        });

        modelBuilder.Entity<SapInvoice>()
            .HasMany(s => s.InvoiceItems)
            .WithOne(ii => ii.SapInvoice)
            .HasForeignKey(s => s.AltInvoiceNumber)
            .HasPrincipalKey(i => i.AltInvoiceNumber)
            .IsRequired(false);

        modelBuilder.Entity<BankData>()
            .ToTable("BankData")
            .HasQueryFilter(bd => bd.IsValid);

        modelBuilder.Entity<Tariff>()
                    .ToTable("tariffs")
                    .HasKey(t => new { t.Tariff1, t.Event })
                    .HasName("PK_Tariffs1");

        modelBuilder.Entity<Tariff>()
                    .Property(t => t.Tariff1)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("Tariff");

        modelBuilder.Entity<CommodityClassification>()
            .HasOne(t => t.TariffNavigation)
            .WithMany()
            .HasForeignKey(t => new { t.Tariff, t.Event })
            .HasPrincipalKey(c => new { c.Tariff1, c.Event })
            .IsRequired();

        modelBuilder.Entity<CommodityClassification>()
            .HasAlternateKey(c => new { c.Event, c.Tariff });

        modelBuilder.Entity<Invoice>()
            .Property(i => i.BillingMethod)
            .HasConversion<short>()
            .HasColumnType("smallint")
            .HasColumnName("billing_method");

        modelBuilder.Entity<TariffDesc>()
            .ToTable("TariffDesc")
            .HasKey(td => new { td.Tariff, td.Event, td.Language })
            .HasName("PK_TariffDesc");

        modelBuilder.Entity<TariffDesc>()
            .HasOne(td => td.TariffNavigation)
            .WithMany()
            .HasForeignKey(td => new { td.Tariff, td.Event })
            .HasPrincipalKey(t => new { t.Tariff1, t.Event });

        modelBuilder.Entity<BusinessProcessProfile>()
            .HasData(
                new BusinessProcessProfile { Id = 1, Created = new DateTimeOffset(new DateTime(2025, 10, 6, 11, 10, 1, 256, DateTimeKind.Unspecified).AddTicks(7379), new TimeSpan(0, 1, 0, 0, 0)), Updated = new DateTimeOffset(2025, 10, 6, 11, 10, 1, TimeSpan.FromHours(1)), Name = nameof(BusinessProcessProfileQualifier.P1), Description = "izdavanje računa za isporuke robe i usluga prema narudžbenicama, na temelju ugovora" },
                new BusinessProcessProfile { Id = 2, Created = new DateTimeOffset(new DateTime(2025, 10, 6, 11, 10, 1, 256, DateTimeKind.Unspecified).AddTicks(7379), new TimeSpan(0, 1, 0, 0, 0)), Updated = new DateTimeOffset(2025, 10, 6, 11, 10, 1, TimeSpan.FromHours(1)), Name = nameof(BusinessProcessProfileQualifier.P2), Description = "periodično izdavanje računa za isporuke robe i usluga na temelju ugovora" },
                new BusinessProcessProfile { Id = 3, Created = new DateTimeOffset(new DateTime(2025, 10, 6, 11, 10, 1, 256, DateTimeKind.Unspecified).AddTicks(7379), new TimeSpan(0, 1, 0, 0, 0)), Updated = new DateTimeOffset(2025, 10, 6, 11, 10, 1, TimeSpan.FromHours(1)), Name = nameof(BusinessProcessProfileQualifier.P3), Description = "izdavanje računa za isporuku prema samostalnoj narudžbenici" },
                new BusinessProcessProfile { Id = 4, Created = new DateTimeOffset(new DateTime(2025, 10, 6, 11, 10, 1, 256, DateTimeKind.Unspecified).AddTicks(7379), new TimeSpan(0, 1, 0, 0, 0)), Updated = new DateTimeOffset(2025, 10, 6, 11, 10, 1, TimeSpan.FromHours(1)), Name = nameof(BusinessProcessProfileQualifier.P4), Description = "plaćanje unaprijed (avansno plaćanje)" },
                new BusinessProcessProfile { Id = 5, Created = new DateTimeOffset(new DateTime(2025, 10, 6, 11, 10, 1, 256, DateTimeKind.Unspecified).AddTicks(7379), new TimeSpan(0, 1, 0, 0, 0)), Updated = new DateTimeOffset(2025, 10, 6, 11, 10, 1, TimeSpan.FromHours(1)), Name = nameof(BusinessProcessProfileQualifier.P5), Description = "plaćanje na licu mjesta (engl. Sport payment)" },
                new BusinessProcessProfile { Id = 6, Created = new DateTimeOffset(new DateTime(2025, 10, 6, 11, 10, 1, 256, DateTimeKind.Unspecified).AddTicks(7379), new TimeSpan(0, 1, 0, 0, 0)), Updated = new DateTimeOffset(2025, 10, 6, 11, 10, 1, TimeSpan.FromHours(1)), Name = nameof(BusinessProcessProfileQualifier.P6), Description = "plaćanje prije isporuke, na temelju narudžbenice" },
                new BusinessProcessProfile { Id = 7, Created = new DateTimeOffset(new DateTime(2025, 10, 6, 11, 10, 1, 256, DateTimeKind.Unspecified).AddTicks(7379), new TimeSpan(0, 1, 0, 0, 0)), Updated = new DateTimeOffset(2025, 10, 6, 11, 10, 1, TimeSpan.FromHours(1)), Name = nameof(BusinessProcessProfileQualifier.P7), Description = "izdavanje računa s referencama na otpremnicu" },
                new BusinessProcessProfile { Id = 8, Created = new DateTimeOffset(new DateTime(2025, 10, 6, 11, 10, 1, 256, DateTimeKind.Unspecified).AddTicks(7379), new TimeSpan(0, 1, 0, 0, 0)), Updated = new DateTimeOffset(2025, 10, 6, 11, 10, 1, TimeSpan.FromHours(1)), Name = nameof(BusinessProcessProfileQualifier.P8), Description = "izdavanje računa s referencama na otpremnicu i primku" },
                new BusinessProcessProfile { Id = 9, Created = new DateTimeOffset(new DateTime(2025, 10, 6, 11, 10, 1, 256, DateTimeKind.Unspecified).AddTicks(7379), new TimeSpan(0, 1, 0, 0, 0)), Updated = new DateTimeOffset(2025, 10, 6, 11, 10, 1, TimeSpan.FromHours(1)), Name = nameof(BusinessProcessProfileQualifier.P9), Description = "odobrenja ili računi s negativnim iznosima, izdani zbog raznih razloga, uključujući i povrat prazne ambalaže" },
                new BusinessProcessProfile { Id = 10, Created = new DateTimeOffset(new DateTime(2025, 10, 6, 11, 10, 1, 256, DateTimeKind.Unspecified).AddTicks(7379), new TimeSpan(0, 1, 0, 0, 0)), Updated = new DateTimeOffset(2025, 10, 6, 11, 10, 1, TimeSpan.FromHours(1)), Name = nameof(BusinessProcessProfileQualifier.P10), Description = "izdavanje korektivnog računa (storniranje/ispravak računa)" },
                new BusinessProcessProfile { Id = 11, Created = new DateTimeOffset(new DateTime(2025, 10, 6, 11, 10, 1, 256, DateTimeKind.Unspecified).AddTicks(7379), new TimeSpan(0, 1, 0, 0, 0)), Updated = new DateTimeOffset(2025, 10, 6, 11, 10, 1, TimeSpan.FromHours(1)), Name = nameof(BusinessProcessProfileQualifier.P11), Description = "izdavanje djelomičnoga i konačnog računa" },
                new BusinessProcessProfile { Id = 12, Created = new DateTimeOffset(new DateTime(2025, 10, 6, 11, 10, 1, 256, DateTimeKind.Unspecified).AddTicks(7379), new TimeSpan(0, 1, 0, 0, 0)), Updated = new DateTimeOffset(2025, 10, 6, 11, 10, 1, TimeSpan.FromHours(1)), Name = nameof(BusinessProcessProfileQualifier.P12), Description = "samoizdavanje računa" },
                new BusinessProcessProfile { Id = 13, Created = new DateTimeOffset(new DateTime(2025, 10, 6, 11, 10, 1, 256, DateTimeKind.Unspecified).AddTicks(7379), new TimeSpan(0, 1, 0, 0, 0)), Updated = new DateTimeOffset(2025, 10, 6, 11, 10, 1, TimeSpan.FromHours(1)), Name = nameof(BusinessProcessProfileQualifier.P99), Description = "poslovni proces koji definira Kupac" }
                );

        modelBuilder.Entity<InvoiceDetail>(entity =>
        {
            entity.Property(id => id.Created)
                  .HasDefaultValueSql("SYSDATETIMEOFFSET()")
                  .IsRequired();

            entity.HasOne(id => id.InvoiceNavigation)
                  .WithMany()
                  .HasForeignKey(id => id.InvoiceNumber)
                  .HasPrincipalKey(i => i.InvoiceNumber)
                  .IsRequired(false)
                  .OnDelete(DeleteBehavior.Cascade);


            entity.HasOne(id => id.BusinessProcessProfileNavigation)
                  .WithMany()
                  .HasForeignKey(id => id.BusinessProcessProfileId)
                  .HasPrincipalKey(b => b.Id)
                  .IsRequired();

            entity.Property(id => id.BusinessProcessProfileId)
            .HasDefaultValue(BusinessProcessProfileQualifier.P2); 

            entity.Property(id => id.SendingStatus)
                  .HasDefaultValue(SendingStatusQualifier.NotReady);
        });

        modelBuilder.Entity<InvoiceAttachment>(entity => 
        {
            entity.Property(ia => ia.Created)
                  .HasDefaultValueSql("SYSDATETIMEOFFSET()")
                  .IsRequired();
        });

        modelBuilder.Entity<CustomerDetails>(entity =>
        {
            entity.Property(cd => cd.Created)
                  .HasDefaultValueSql("SYSDATETIMEOFFSET()")
                  .IsRequired();
        });
    }
}