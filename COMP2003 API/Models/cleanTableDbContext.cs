using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace COMP2003_API.Models
{
    public partial class cleanTableDbContext : DbContext
    {
        public cleanTableDbContext()
        {
        }

        public cleanTableDbContext(DbContextOptions<cleanTableDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AppBookingsView> AppBookingsView { get; set; }
        public virtual DbSet<AppVenueView> AppVenueView { get; set; }
        public virtual DbSet<BookingAttendees> BookingAttendees { get; set; }
        public virtual DbSet<Bookings> Bookings { get; set; }
        public virtual DbSet<Customers> Customers { get; set; }
        public virtual DbSet<VenueTables> VenueTables { get; set; }
        public virtual DbSet<Venues> Venues { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=cleanTableDb;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AppBookingsView>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("app_bookings_view");

                entity.Property(e => e.AddLineOne)
                    .IsRequired()
                    .HasColumnName("add_line_one")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.AddLineTwo)
                    .IsRequired()
                    .HasColumnName("add_line_two")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.BookingAttended).HasColumnName("booking_attended");

                entity.Property(e => e.BookingId).HasColumnName("booking_id");

                entity.Property(e => e.BookingSize).HasColumnName("booking_size");

                entity.Property(e => e.BookingTime)
                    .HasColumnName("booking_time")
                    .HasColumnType("datetime");

                entity.Property(e => e.City)
                    .IsRequired()
                    .HasColumnName("city")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.County)
                    .IsRequired()
                    .HasColumnName("county")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerId).HasColumnName("customer_id");

                entity.Property(e => e.VenueId).HasColumnName("venue_id");

                entity.Property(e => e.VenueName)
                    .IsRequired()
                    .HasColumnName("venue_name")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.VenuePostcode)
                    .IsRequired()
                    .HasColumnName("venue_postcode")
                    .HasMaxLength(10)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<AppVenueView>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("app_venue_view");

                entity.Property(e => e.AddLineOne)
                    .IsRequired()
                    .HasColumnName("add_line_one")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.AddLineTwo)
                    .IsRequired()
                    .HasColumnName("add_line_two")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.City)
                    .IsRequired()
                    .HasColumnName("city")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.County)
                    .IsRequired()
                    .HasColumnName("county")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.TotalCapacity).HasColumnName("total_capacity");

                entity.Property(e => e.VenueId).HasColumnName("venue_id");

                entity.Property(e => e.VenueName)
                    .IsRequired()
                    .HasColumnName("venue_name")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.VenuePostcode)
                    .IsRequired()
                    .HasColumnName("venue_postcode")
                    .HasMaxLength(10)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<BookingAttendees>(entity =>
            {
                entity.HasKey(e => new { e.BookingId, e.CustomerId })
                    .HasName("PK__booking___1135F909E38196DE");

                entity.ToTable("booking_attendees");

                entity.Property(e => e.BookingId).HasColumnName("booking_id");

                entity.Property(e => e.CustomerId).HasColumnName("customer_id");

                entity.Property(e => e.BookingAttended).HasColumnName("booking_attended");

                entity.HasOne(d => d.Booking)
                    .WithMany(p => p.BookingAttendees)
                    .HasForeignKey(d => d.BookingId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__booking_a__booki__4E53A1AA");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.BookingAttendees)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__booking_a__custo__4F47C5E3");
            });

            modelBuilder.Entity<Bookings>(entity =>
            {
                entity.HasKey(e => e.BookingId)
                    .HasName("PK__bookings__5DE3A5B10A626279");

                entity.ToTable("bookings");

                entity.Property(e => e.BookingId).HasColumnName("booking_id");

                entity.Property(e => e.BookingSize).HasColumnName("booking_size");

                entity.Property(e => e.BookingTime)
                    .HasColumnName("booking_time")
                    .HasColumnType("datetime");

                entity.Property(e => e.VenueId).HasColumnName("venue_id");

                entity.Property(e => e.VenueTableId).HasColumnName("venue_table_id");

                entity.HasOne(d => d.Venue)
                    .WithMany(p => p.Bookings)
                    .HasForeignKey(d => d.VenueId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__bookings__venue___2A164134");

                entity.HasOne(d => d.VenueTable)
                    .WithMany(p => p.Bookings)
                    .HasForeignKey(d => d.VenueTableId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__bookings__venue___2EDAF651");
            });

            modelBuilder.Entity<Customers>(entity =>
            {
                entity.HasKey(e => e.CustomerId)
                    .HasName("PK__customer__CD65CB8511777C82");

                entity.ToTable("customers");

                entity.HasIndex(e => e.CustomerUsername)
                    .HasName("UQ__customer__64E4CB014C3FBEC7")
                    .IsUnique();

                entity.Property(e => e.CustomerId).HasColumnName("customer_id");

                entity.Property(e => e.CustomerContactNumber).HasColumnName("customer_contact_number");

                entity.Property(e => e.CustomerName)
                    .IsRequired()
                    .HasColumnName("customer_name")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerPassword)
                    .IsRequired()
                    .HasColumnName("customer_password")
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerUsername)
                    .IsRequired()
                    .HasColumnName("customer_username")
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<VenueTables>(entity =>
            {
                entity.HasKey(e => e.VenueTableId);

                entity.ToTable("venue_tables");

                entity.Property(e => e.VenueTableId).HasColumnName("venue_table_id");

                entity.Property(e => e.VenueId).HasColumnName("venue_id");

                entity.Property(e => e.VenueTableCapacity).HasColumnName("venue_table_capacity");

                entity.Property(e => e.VenueTableNum).HasColumnName("venue_table_num");
            });

            modelBuilder.Entity<Venues>(entity =>
            {
                entity.HasKey(e => e.VenueId)
                    .HasName("PK__venues__82A8BE8D9F10979F");

                entity.ToTable("venues");

                entity.Property(e => e.VenueId).HasColumnName("venue_id");

                entity.Property(e => e.AddLineOne)
                    .IsRequired()
                    .HasColumnName("add_line_one")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.AddLineTwo)
                    .IsRequired()
                    .HasColumnName("add_line_two")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.City)
                    .IsRequired()
                    .HasColumnName("city")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.County)
                    .IsRequired()
                    .HasColumnName("county")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.VenueName)
                    .IsRequired()
                    .HasColumnName("venue_name")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.VenuePostcode)
                    .IsRequired()
                    .HasColumnName("venue_postcode")
                    .HasMaxLength(10)
                    .IsUnicode(false);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
