using DentalManagement.DomainModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DentalManagement.Web.Models
{
    public class InvoiceViewModel
    {
        public int InvoiceId { get; set; }
        public int PatientId {  get; set; }
        public int AppointmentId { get; set; }
        public string PatientName { get; set; }
        public int EmployeeId {  get; set; }
        public int ServiceId { get; set; }
        public string ServiceName { get; set; }
        public string EmployeeName {  get; set; }
        public string PatientAddress {  get; set; }
        public int SalePrice {  get; set; }
        public int Quantity {  get; set; }
        public decimal TotalPrice { get; set; }

        public string PaymentMethod { get; set; }

        public int Status { get; set; }

        public string StatusDescription { get; set; }
        public decimal Discount {  get; set; }

        public DateTime InvoiceDate { get; set; }

        public DateTime? FinishTime { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
        public string? UserIdCreate { get; set; } = string.Empty;
        public string? UserIdUpdated { get; set; } = string.Empty;
        public IEnumerable<SelectListItem> PatientList { get; set; }
        public IEnumerable<SelectListItem> ServiceList { get; set; }
        public IEnumerable<SelectListItem> DentistList { get; set; }
        public IEnumerable<SelectListItem> EmployeeList { get; set; }
        public IEnumerable<Patient> Patients { get; set; }
        public IEnumerable<Service> Services { get; set; }
        public IEnumerable<Dentist> Dentists { get; set; }
        public IEnumerable<Employee> Employees { get; set; }
    }
}
