using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace EventEase_01.Models;

public partial class EventEase01Context : DbContext
{
    public EventEase01Context()
    {
    }

    public EventEase01Context(DbContextOptions<EventEase01Context> options)
        : base(options)
    {
    }

    public virtual DbSet<Booking> Bookings { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Event> Events { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<Seat> Seats { get; set; }

    public virtual DbSet<Ticket> Tickets { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Venue> Venues { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {

    }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.BookingId).HasName("PK__Booking__35ABFDE0CBA755AB");

            entity.ToTable("Booking");

            entity.Property(e => e.BookingId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("Booking_ID");
            entity.Property(e => e.BookingDateTime)
                .HasColumnType("datetime")
                .HasColumnName("Booking_DateTime");
            entity.Property(e => e.NumberOfBookings).HasColumnName("Number_Of_Bookings");
            entity.Property(e => e.TicketId).HasColumnName("Ticket_ID");
            entity.Property(e => e.UserId).HasColumnName("User_ID");

            entity.HasOne(d => d.Ticket).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.TicketId)
                .HasConstraintName("FK__Booking__Ticket___5CD6CB2B");

            entity.HasOne(d => d.User).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Booking__User_ID__5BE2A6F2");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__Category__6DB38D4E558E5175");

            entity.ToTable("Category");

            entity.Property(e => e.CategoryId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("Category_ID");
            entity.Property(e => e.CategoryDescription).HasColumnName("Category_Description");
            entity.Property(e => e.CategoryName)
                .HasMaxLength(50)
                .HasColumnName("Category_Name");
        });

        modelBuilder.Entity<Event>(entity =>
        {
            entity.HasKey(e => e.EventId).HasName("PK__Event__FD6BEFE402244B4C");

            entity.ToTable("Event");

            entity.Property(e => e.EventId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("Event_ID");
            entity.Property(e => e.CategoryId).HasColumnName("Category_ID");
            entity.Property(e => e.EventDate).HasColumnName("Event_Date");
            entity.Property(e => e.EventDescription).HasColumnName("Event_Description");
            entity.Property(e => e.EventName)
                .HasMaxLength(100)
                .HasColumnName("Event_Name");
            entity.Property(e => e.VenueId).HasColumnName("Venue_ID");

            entity.HasOne(d => d.Category).WithMany(p => p.Events)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("FK__Event__Category___5441852A");

            entity.HasOne(d => d.Venue).WithMany(p => p.Events)
                .HasForeignKey(d => d.VenueId)
                .HasConstraintName("FK__Event__Venue_ID__534D60F1");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PK__Order__F1E4639B1DF67503");

            entity.ToTable("Order");

            entity.Property(e => e.OrderId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("Order_ID");
            entity.Property(e => e.OrderDateTime)
                .HasColumnType("datetime")
                .HasColumnName("Order_DateTime");
            entity.Property(e => e.PaymentStatus)
                .HasMaxLength(50)
                .HasColumnName("Payment_Status");
            entity.Property(e => e.UserId).HasColumnName("User_ID");

            entity.HasOne(d => d.User).WithMany(p => p.Orders)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Order__User_ID__60A75C0F");
        });

        modelBuilder.Entity<Seat>(entity =>
        {
            entity.HasKey(e => e.SeatId).HasName("PK__Seat__8B2CE7B635A7C5BC");

            entity.ToTable("Seat");

            entity.Property(e => e.SeatId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("Seat_ID");
            entity.Property(e => e.SeatAvailability).HasColumnName("Seat_Availability");
            entity.Property(e => e.SeatBookingStatus).HasColumnName("Seat_Booking_Status");
            entity.Property(e => e.SeatNumber)
                .HasMaxLength(100)
                .HasColumnName("Seat_Number");
            entity.Property(e => e.SeatPrice)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("Seat_Price");
            entity.Property(e => e.VenueId).HasColumnName("Venue_ID");

            entity.HasOne(d => d.Venue).WithMany(p => p.Seats)
                .HasForeignKey(d => d.VenueId)
                .HasConstraintName("FK__Seat__Venue_ID__6477ECF3");
        });

        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.HasKey(e => e.TicketId).HasName("PK__Ticket__ED7260D95D47BECD");

            entity.ToTable("Ticket");

            entity.Property(e => e.TicketId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("Ticket_ID");
            entity.Property(e => e.EventId).HasColumnName("Event_ID");
            entity.Property(e => e.TicketAvailability).HasColumnName("Ticket_Availability");
            entity.Property(e => e.TicketPrice)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("Ticket_Price");

            entity.HasOne(d => d.Event).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.EventId)
                .HasConstraintName("FK__Ticket__Event_ID__5812160E");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__User__206D9190B4194A1C");

            entity.ToTable("User");

            entity.Property(e => e.UserId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("User_ID");
            entity.Property(e => e.PasswordHash).HasColumnName("Password_hash");
            entity.Property(e => e.PasswordSalt).HasColumnName("Password_Salt");
            entity.Property(e => e.UserAge).HasColumnName("User_Age");
            entity.Property(e => e.UserEmail)
                .HasMaxLength(100)
                .HasColumnName("User_Email");
            entity.Property(e => e.UserName).HasMaxLength(50);
            entity.Property(e => e.UserRole)
                .HasMaxLength(50)
                .HasColumnName("User_Role");
        });

        modelBuilder.Entity<Venue>(entity =>
        {
            entity.HasKey(e => e.VenueId).HasName("PK__Venue__4A99C4B9FB0273EE");

            entity.ToTable("Venue");

            entity.Property(e => e.VenueId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("Venue_ID");
            entity.Property(e => e.VenueAddress).HasColumnName("Venue_Address");
            entity.Property(e => e.VenueCapacity).HasColumnName("Venue_Capacity");
            entity.Property(e => e.VenueName)
                .HasMaxLength(100)
                .HasColumnName("Venue_Name");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
