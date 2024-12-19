using DentalManagement.DomainModels;
using Microsoft.EntityFrameworkCore;
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
        public DbSet<Message>Messages { get; set; } 
        public DbSet<PrescriptionDetails>PrescriptionDetails{ get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Payment>()
                .HasKey(e => e.PaymentId);
            modelBuilder.Entity<Payment>()
    .Property(p => p.PaymentId)
    .ValueGeneratedOnAdd();

            // Cấu hình cột InvoiceId với Foreign Key
            modelBuilder.Entity<Payment>()
                .Property(e => e.InvoiceId)
                .IsRequired();

            modelBuilder.Entity<Payment>()
                  .HasOne(p => p.Invoice)
                  .WithMany()
                  .HasForeignKey(p => p.InvoiceId)
                  .OnDelete(DeleteBehavior.Cascade);

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
     .HasKey(m => m.MedicalRecordId);

            modelBuilder.Entity<MedicalRecord>()
        .Ignore("ServiceId");

            // Quan hệ với Patient
            modelBuilder.Entity<MedicalRecord>()
                .HasOne(m => m.Patient)
                .WithMany(p => p.MedicalRecords)
                .HasForeignKey(m => m.PatientId)
                .IsRequired() // MedicalRecord luôn cần một bệnh nhân
                .OnDelete(DeleteBehavior.Restrict); // Ngăn chặn xóa hàng loạt

            // Quan hệ với Dentist
            modelBuilder.Entity<MedicalRecord>()
                .HasOne(m => m.Dentist)
                .WithMany(d => d.MedicalRecords)
                .HasForeignKey(m => m.DentistId)  // Đảm bảo rằng ForeignKey được chỉ định đúng
                .OnDelete(DeleteBehavior.Restrict);  // Đảm bảo không tạo cột sai tên
   


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

            modelBuilder.Entity<Prescription>()
                .HasOne(p => p.Dentist)
                .WithMany(d => d.Prescriptions)
                .HasForeignKey(p => p.DentistId)
                .OnDelete(DeleteBehavior.ClientSetNull);


            modelBuilder.Entity<Invoice>()
                .HasOne(i => i.Patient)
                .WithMany(p => p.Invoices)
                .HasForeignKey(i => i.PatientId);

            // Cấu hình InvoiceDetails với ServiceId mà không cần ServiceId trong Invoice
            modelBuilder.Entity<InvoiceDetails>()
                .HasKey(id => new { id.InvoiceId, id.ServiceId });

            modelBuilder.Entity<InvoiceDetails>()
                .HasOne(id => id.Invoice)  // Chỉ định mối quan hệ với Invoice
                .WithMany(i => i.InvoiceDetails)
                .HasForeignKey(id => id.InvoiceId);

            modelBuilder.Entity<InvoiceDetails>()
                .HasOne(id => id.Service)
                .WithMany(s => s.InvoiceDetails) // Mỗi Service có thể có nhiều InvoiceDetails
                .HasForeignKey(id => id.ServiceId);

            modelBuilder.Entity<AppointmentStatus>()
                .HasNoKey();
            modelBuilder.Entity<Message>()
            .HasKey(m => m.Id);
            modelBuilder.Entity<Prescription>()
             .HasOne(p => p.Patient) // Prescription có một Patient
             .WithMany(p=>p.Prescriptions) // Patient có thể có nhiều Prescription
             .HasForeignKey(p => p.PatientId) // Khoá ngoại là PatientId trong Prescription
             .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PrescriptionDetails>()
      .HasKey(pd => new { pd.PrescriptionId, pd.MedicineId });




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
