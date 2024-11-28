using DentalManagement.DomainModels;
using Microsoft.EntityFrameworkCore;
using static Dapper.SqlMapper;

namespace DentalManagement.Web.Data
{
    public class DentalManagementDbContext : DbContext
    {
        public DentalManagementDbContext(DbContextOptions<DentalManagementDbContext> options) : base(options)
        {
        }

        public DbSet<Patient> Patients { get; set; }
        public DbSet<Dentist> Dentists { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<MedicalRecord> MedicalRecords { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<Prescription> Prescriptions { get; set; }
        public DbSet<Medicine> Medicines { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<UserAccount> UserAccounts { get; set; }
        public DbSet<AppointmentStatus> AppointmentStatuses { get; set; }
        public DbSet<InvoiceDetails> InvoiceDetails { get; set; }
        public DbSet<Payment>Payments { get; set; }
        public DbSet<MessageHelp>Messages { get; set; } 
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Payment>()
                .HasKey(e => e.PaymentId);

            // Cấu hình cột InvoiceId với Foreign Key
            modelBuilder.Entity<Payment>()
                .Property(e => e.InvoiceId)
                .IsRequired();

            modelBuilder.Entity<Appointment>()
         .HasOne(a => a.Patient)  
         .WithMany(p => p.Appointments)  
         .HasForeignKey(a => a.PatientId)
         .OnDelete(DeleteBehavior.ClientSetNull); 

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Dentist)  
                .WithMany(d => d.Appointments)  
                .HasForeignKey(a => a.DentistId)
                .OnDelete(DeleteBehavior.ClientSetNull);


            modelBuilder.Entity<MedicalRecord>()
                   .HasOne(m => m.Service)
                   .WithMany(s => s.MedicalRecords)
                   .HasForeignKey(m => m.ServiceId)
                   .OnDelete(DeleteBehavior.Restrict); 

            modelBuilder.Entity<MedicalRecord>()
                .HasOne(m => m.Patient)
                .WithMany(p => p.MedicalRecords)
                .HasForeignKey(m => m.PatientId)
                .OnDelete(DeleteBehavior.Restrict); 

            modelBuilder.Entity<MedicalRecord>()
                .HasOne(m => m.Dentist)
                .WithMany(d => d.MedicalRecords)
                .HasForeignKey(m => m.DentistId)
                .OnDelete(DeleteBehavior.Restrict);

            // Thiết lập quan hệ giữa Invoice và Patient (Một bệnh nhân có nhiều hóa đơn)
            modelBuilder.Entity<Invoice>()
              .HasKey(i => i.InvoiceId); // Set the primary key

            // Configure any relationships as needed
            modelBuilder.Entity<Invoice>()
                .HasOne(i => i.Patient)
                .WithMany(p => p.Invoices) // Assuming Patient has a collection of Invoices
                .HasForeignKey(i => i.PatientId);

            modelBuilder.Entity<Patient>()
                .Property(p => p.PatientName)
                .HasMaxLength(100)
                .IsRequired();

            modelBuilder.Entity<Medicine>()
                .Property(m => m.MedicineName)
                .HasMaxLength(100)
                .IsRequired();

            // Thiết lập bảng Prescription liên kết với Patient và Dentist
            modelBuilder.Entity<Prescription>()
                .HasOne(p => p.Patient)
                .WithMany(pa => pa.Prescriptions)
                .HasForeignKey(p => p.PatientId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            modelBuilder.Entity<Prescription>()
                .HasOne(p => p.Dentist)
                .WithMany(d => d.Prescriptions)
                .HasForeignKey(p => p.DentistId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            modelBuilder.Entity<InvoiceDetails>()
                .HasKey(id => new {id.InvoiceId, id.ServiceId});

            modelBuilder.Entity<InvoiceDetails>()
                .HasOne(id => id.Invoice)
                .WithMany(i => i.InvoiceDetails)
                .HasForeignKey(i => i.InvoiceId);
            modelBuilder.Entity<InvoiceDetails>()
               .HasOne(id => id.Service)
               .WithMany(i => i.InvoiceDetails)
               .HasForeignKey(i => i.ServiceId);

            modelBuilder.Entity<AppointmentStatus>()
                .HasNoKey();
            modelBuilder.Entity<MessageHelp>()
                .HasNoKey();
           


            // Tùy chỉnh tên bảng (nếu cần)
            modelBuilder.Entity<Patient>().ToTable("Patients");
            modelBuilder.Entity<Dentist>().ToTable("Dentists");
            modelBuilder.Entity<Appointment>().ToTable("Appointments");
            modelBuilder.Entity<MedicalRecord>().ToTable("MedicalRecords");
            modelBuilder.Entity<Prescription>().ToTable("Prescriptions");
            modelBuilder.Entity<Service>().ToTable("Services");
            modelBuilder.Entity<Medicine>().ToTable("Medicines");
            modelBuilder.Entity<Invoice>().ToTable("Invoices");
            modelBuilder.Entity<Employee>().ToTable("Employees");
           
        }
    }
}
