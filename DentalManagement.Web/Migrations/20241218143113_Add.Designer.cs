﻿// <auto-generated />
using System;
using DentalManagement.Web.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace DentalManagement.Web.Migrations
{
    [DbContext(typeof(DentalManagementDbContext))]
    [Migration("20241218143113_Add")]
    partial class Add
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("DentalManagement.DomainModels.Appointment", b =>
                {
                    b.Property<int>("AppointmentId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("AppointmentId"));

                    b.Property<DateTime?>("AppointmentDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DateUpdated")
                        .HasColumnType("datetime2");

                    b.Property<int>("DentistId")
                        .HasColumnType("int");

                    b.Property<string>("DentistName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<TimeSpan?>("EndTime")
                        .HasColumnType("time");

                    b.Property<DateTime?>("FinishedTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("Notes")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("PatientId")
                        .HasColumnType("int");

                    b.Property<string>("PatientName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ServiceID")
                        .HasColumnType("int");

                    b.Property<string>("ServiceName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<TimeSpan?>("StartTime")
                        .HasColumnType("time");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<string>("UserIdCreate")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserIdUpdated")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("AppointmentId");

                    b.HasIndex("DentistId");

                    b.HasIndex("PatientId");

                    b.HasIndex("ServiceID");

                    b.ToTable("Appointments", (string)null);
                });

            modelBuilder.Entity("DentalManagement.DomainModels.AppointmentStatus", b =>
                {
                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.ToTable("AppointmentStatuses");
                });

            modelBuilder.Entity("DentalManagement.DomainModels.Dentist", b =>
                {
                    b.Property<int>("DentistId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("DentistId"));

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Avatar")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DateUpdated")
                        .HasColumnType("datetime2");

                    b.Property<string>("DentistName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Specialization")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserIdCreate")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserIdUpdated")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("WorkSchedule")
                        .HasColumnType("datetime2");

                    b.HasKey("DentistId");

                    b.ToTable("Dentists", (string)null);
                });

            modelBuilder.Entity("DentalManagement.DomainModels.Employee", b =>
                {
                    b.Property<int>("EmployeeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("EmployeeId"));

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Avatar")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DateHired")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DateOfBirth")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DateUpdated")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FullName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RoleName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("Salary")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("UserIdCreate")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserIdUpdated")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("EmployeeId");

                    b.ToTable("Employees", (string)null);
                });

            modelBuilder.Entity("DentalManagement.DomainModels.Invoice", b =>
                {
                    b.Property<int>("InvoiceId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("InvoiceId"));

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DateUpdated")
                        .HasColumnType("datetime2");

                    b.Property<decimal>("Discount")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("EmployeeId")
                        .HasColumnType("int");

                    b.Property<string>("EmployeeName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("FinishTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("PatientAddress")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("PatientId")
                        .HasColumnType("int");

                    b.Property<string>("PatientName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PaymentMethod")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("PrescriptionId")
                        .HasColumnType("int");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<decimal>("TotalPrice")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("UserIdCreate")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserIdUpdated")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("InvoiceId");

                    b.HasIndex("EmployeeId");

                    b.HasIndex("PatientId");

                    b.ToTable("Invoices", (string)null);
                });

            modelBuilder.Entity("DentalManagement.DomainModels.InvoiceDetails", b =>
                {
                    b.Property<int>("InvoiceId")
                        .HasColumnType("int");

                    b.Property<int>("ServiceId")
                        .HasColumnType("int");

                    b.Property<decimal>("Discount")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("PrescriptionId")
                        .HasColumnType("int");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.Property<decimal>("SalePrice")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("ServiceName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("InvoiceId", "ServiceId");

                    b.HasIndex("ServiceId");

                    b.ToTable("InvoiceDetails");
                });

            modelBuilder.Entity("DentalManagement.DomainModels.MedicalRecord", b =>
                {
                    b.Property<int>("MedicalRecordId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("MedicalRecordId"));

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DateOfTreatment")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DateUpdated")
                        .HasColumnType("datetime2");

                    b.Property<int?>("DentistId")
                        .IsRequired()
                        .HasColumnType("int");

                    b.Property<string>("Diagnosis")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("NextAppointmentDate")
                        .HasColumnType("datetime2");

                    b.Property<int?>("PatientId")
                        .IsRequired()
                        .HasColumnType("int");

                    b.Property<int?>("ServiceId")
                        .HasColumnType("int");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<string>("Symptoms")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Treatment")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TreatmentOutcome")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserIdCreate")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserIdUpdated")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("MedicalRecordId");

                    b.HasIndex("DentistId");

                    b.HasIndex("PatientId");

                    b.HasIndex("ServiceId");

                    b.ToTable("MedicalRecords", (string)null);
                });

            modelBuilder.Entity("DentalManagement.DomainModels.Medicine", b =>
                {
                    b.Property<int>("MedicineId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("MedicineId"));

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DateUpdated")
                        .HasColumnType("datetime2");

                    b.Property<string>("DosageInstructions")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Duration")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Frequency")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("MedicineName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Photo")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("StockQuantity")
                        .HasColumnType("int");

                    b.Property<string>("Unit")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Usage")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserIdCreate")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserIdUpdated")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("MedicineId");

                    b.ToTable("Medicines", (string)null);
                });

            modelBuilder.Entity("DentalManagement.DomainModels.Message", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Messages")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("NewMessage")
                        .HasColumnType("bit");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("Messages");
                });

            modelBuilder.Entity("DentalManagement.DomainModels.Patient", b =>
                {
                    b.Property<int>("PatientId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("PatientId"));

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DateUpdated")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("Gender")
                        .HasColumnType("bit");

                    b.Property<double>("Height")
                        .HasColumnType("float");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("PatientDOB")
                        .HasColumnType("datetime2");

                    b.Property<string>("PatientName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Photo")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RoleName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserIdCreate")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserIdUpdated")
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("Weight")
                        .HasColumnType("float");

                    b.HasKey("PatientId");

                    b.ToTable("Patients", (string)null);
                });

            modelBuilder.Entity("DentalManagement.DomainModels.Payment", b =>
                {
                    b.Property<int>("PaymentId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("PaymentId"));

                    b.Property<decimal>("AmountPaid")
                        .HasColumnType("decimal(18,2)");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DateUpdated")
                        .HasColumnType("datetime2");

                    b.Property<int>("InvoiceId")
                        .HasColumnType("int");

                    b.Property<string>("Notes")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PaymentMethod")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PaymentStatus")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ServiceId")
                        .HasColumnType("int");

                    b.Property<string>("ServiceName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserIdCreate")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserIdUpdated")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("PaymentId");

                    b.HasIndex("InvoiceId");

                    b.ToTable("Payments");
                });

            modelBuilder.Entity("DentalManagement.DomainModels.Prescription", b =>
                {
                    b.Property<int>("PrescriptionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("PrescriptionId"));

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DateUpdated")
                        .HasColumnType("datetime2");

                    b.Property<int>("DentistId")
                        .HasColumnType("int");

                    b.Property<string>("Diagnosis")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Dosage")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Duration")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Frequency")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Notes")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("PatientId")
                        .HasColumnType("int");

                    b.Property<string>("UserIdCreate")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserIdUpdated")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("PrescriptionId");

                    b.HasIndex("DentistId");

                    b.HasIndex("PatientId");

                    b.ToTable("Prescriptions", (string)null);
                });

            modelBuilder.Entity("DentalManagement.DomainModels.PrescriptionDetails", b =>
                {
                    b.Property<int>("PrescriptionId")
                        .HasColumnType("int");

                    b.Property<int>("MedicineId")
                        .HasColumnType("int");

                    b.Property<string>("MedicineName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.Property<decimal>("SalePrice")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("PrescriptionId", "MedicineId");

                    b.HasIndex("MedicineId");

                    b.ToTable("PrescriptionDetails");
                });

            modelBuilder.Entity("DentalManagement.DomainModels.Service", b =>
                {
                    b.Property<int>("ServiceId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ServiceId"));

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DateUpdated")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Photo")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("ServiceName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserIdCreate")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserIdUpdated")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ServiceId");

                    b.ToTable("Services", (string)null);
                });

            modelBuilder.Entity("DentalManagement.DomainModels.UserAccount", b =>
                {
                    b.Property<string>("UserID")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("FullName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Photo")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RoleNames")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("UserID");

                    b.ToTable("UserAccounts");
                });

            modelBuilder.Entity("DentalManagement.DomainModels.Appointment", b =>
                {
                    b.HasOne("DentalManagement.DomainModels.Dentist", "Dentist")
                        .WithMany("Appointments")
                        .HasForeignKey("DentistId")
                        .IsRequired();

                    b.HasOne("DentalManagement.DomainModels.Patient", "Patient")
                        .WithMany("Appointments")
                        .HasForeignKey("PatientId")
                        .IsRequired();

                    b.HasOne("DentalManagement.DomainModels.Service", "Service")
                        .WithMany()
                        .HasForeignKey("ServiceID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Dentist");

                    b.Navigation("Patient");

                    b.Navigation("Service");
                });

            modelBuilder.Entity("DentalManagement.DomainModels.Invoice", b =>
                {
                    b.HasOne("DentalManagement.DomainModels.Employee", "Employee")
                        .WithMany()
                        .HasForeignKey("EmployeeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DentalManagement.DomainModels.Patient", "Patient")
                        .WithMany("Invoices")
                        .HasForeignKey("PatientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Employee");

                    b.Navigation("Patient");
                });

            modelBuilder.Entity("DentalManagement.DomainModels.InvoiceDetails", b =>
                {
                    b.HasOne("DentalManagement.DomainModels.Invoice", "Invoice")
                        .WithMany("InvoiceDetails")
                        .HasForeignKey("InvoiceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DentalManagement.DomainModels.Service", "Service")
                        .WithMany("InvoiceDetails")
                        .HasForeignKey("ServiceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Invoice");

                    b.Navigation("Service");
                });

            modelBuilder.Entity("DentalManagement.DomainModels.MedicalRecord", b =>
                {
                    b.HasOne("DentalManagement.DomainModels.Dentist", "Dentist")
                        .WithMany("MedicalRecords")
                        .HasForeignKey("DentistId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("DentalManagement.DomainModels.Patient", "Patient")
                        .WithMany("MedicalRecords")
                        .HasForeignKey("PatientId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("DentalManagement.DomainModels.Service", null)
                        .WithMany("MedicalRecords")
                        .HasForeignKey("ServiceId");

                    b.Navigation("Dentist");

                    b.Navigation("Patient");
                });

            modelBuilder.Entity("DentalManagement.DomainModels.Payment", b =>
                {
                    b.HasOne("DentalManagement.DomainModels.Invoice", "Invoice")
                        .WithMany()
                        .HasForeignKey("InvoiceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Invoice");
                });

            modelBuilder.Entity("DentalManagement.DomainModels.Prescription", b =>
                {
                    b.HasOne("DentalManagement.DomainModels.Dentist", "Dentist")
                        .WithMany("Prescriptions")
                        .HasForeignKey("DentistId")
                        .IsRequired();

                    b.HasOne("DentalManagement.DomainModels.Patient", "Patient")
                        .WithMany("Prescriptions")
                        .HasForeignKey("PatientId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Dentist");

                    b.Navigation("Patient");
                });

            modelBuilder.Entity("DentalManagement.DomainModels.PrescriptionDetails", b =>
                {
                    b.HasOne("DentalManagement.DomainModels.Medicine", "Medicine")
                        .WithMany("PrescriptionMedicines")
                        .HasForeignKey("MedicineId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DentalManagement.DomainModels.Prescription", null)
                        .WithMany("PrescriptionDetails")
                        .HasForeignKey("PrescriptionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Medicine");
                });

            modelBuilder.Entity("DentalManagement.DomainModels.Dentist", b =>
                {
                    b.Navigation("Appointments");

                    b.Navigation("MedicalRecords");

                    b.Navigation("Prescriptions");
                });

            modelBuilder.Entity("DentalManagement.DomainModels.Invoice", b =>
                {
                    b.Navigation("InvoiceDetails");
                });

            modelBuilder.Entity("DentalManagement.DomainModels.Medicine", b =>
                {
                    b.Navigation("PrescriptionMedicines");
                });

            modelBuilder.Entity("DentalManagement.DomainModels.Patient", b =>
                {
                    b.Navigation("Appointments");

                    b.Navigation("Invoices");

                    b.Navigation("MedicalRecords");

                    b.Navigation("Prescriptions");
                });

            modelBuilder.Entity("DentalManagement.DomainModels.Prescription", b =>
                {
                    b.Navigation("PrescriptionDetails");
                });

            modelBuilder.Entity("DentalManagement.DomainModels.Service", b =>
                {
                    b.Navigation("InvoiceDetails");

                    b.Navigation("MedicalRecords");
                });
#pragma warning restore 612, 618
        }
    }
}
