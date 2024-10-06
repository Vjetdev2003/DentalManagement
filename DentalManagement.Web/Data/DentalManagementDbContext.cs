using DentalManagement.DomainModels;
using Microsoft.EntityFrameworkCore;

namespace DentalManagement.Web.Data
{
    public class DentalManagementDbContext : DbContext
    {
        public DentalManagementDbContext(DbContextOptions<DentalManagementDbContext> options) : base(options) { 
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
        public DbSet<DentistPatient> DentistPatients { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Appointment>()
         .HasOne(a => a.Patient)  // Each appointment is related to one patient
         .WithMany(p => p.Appointments)  // A patient can have many appointments
         .HasForeignKey(a => a.PatientId)
         .OnDelete(DeleteBehavior.ClientSetNull);  // Define the delete behavior

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Dentist)  // Each appointment is related to one dentist
                .WithMany(d => d.Appointments)  // A dentist can have many appointments
                .HasForeignKey(a => a.DentistId)
                .OnDelete(DeleteBehavior.ClientSetNull);
            modelBuilder.Entity<Appointment>()
        .HasOne(a => a.Employee) // Một Appointment có một Employee
        .WithMany(e => e.Appointments) // Một Employee có nhiều Appointment
        .HasForeignKey(a => a.EmployeeId) // Khóa ngoại là EmployeeId
        .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<MedicalRecord>()
                   .HasOne(m => m.Service)
                   .WithMany(s => s.MedicalRecords)
                   .HasForeignKey(m => m.ServiceId)
                   .OnDelete(DeleteBehavior.Restrict);  // Prevent cascade delete

            modelBuilder.Entity<MedicalRecord>()
                .HasOne(m => m.Patient)
                .WithMany(p => p.MedicalRecords)
                .HasForeignKey(m => m.PatientId)
                .OnDelete(DeleteBehavior.Restrict);  // Prevent cascade delete

            modelBuilder.Entity<MedicalRecord>()
                .HasOne(m => m.Dentist)
                .WithMany(d => d.MedicalRecords)
                .HasForeignKey(m => m.DentistId)
                .OnDelete(DeleteBehavior.Restrict);

            // Thiết lập quan hệ giữa Invoice và Patient (Một bệnh nhân có nhiều hóa đơn)
            modelBuilder.Entity<Invoice>()
                .HasOne(i => i.Patient)
                .WithMany(p => p.Invoices)
                .HasForeignKey(i => i.PatientId)
                .OnDelete(DeleteBehavior.ClientSetNull);


            // Các cấu hình khác (Tên bảng, độ dài trường, v.v.) nếu cần
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
            modelBuilder.Entity<DentistPatient>()
            .HasKey(dp => new { dp.DentistID, dp.PatientID });
            modelBuilder.Entity<DentistPatient>()
           .HasOne(dp => dp.Dentist)
           .WithMany(d => d.DentistPatients)
           .HasForeignKey(dp => dp.DentistID);
         
            modelBuilder.Entity<DentistPatient>()
                .HasOne(dp => dp.Patient)
                .WithMany(p => p.DentistPatients)
                .HasForeignKey(dp => dp.PatientID);
                



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
            modelBuilder.Entity<DentistPatient>().ToTable("DentistPatients");
        }
    }
}
