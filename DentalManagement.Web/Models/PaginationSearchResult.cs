using DentalManagement.DomainModels;

namespace DentalManagement.Web.Models
{
    public class PaginationSearchResult
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; }
        public string SearchValue { get; set; }
        public int RowCount { get; set; }
        public int PageCount
        {
            get
            {
                if (PageSize == 0)
                    return 1;
                int n = RowCount / PageSize;
                if (RowCount % PageSize > 0)
                {
                    n += 1;
                }
                return n;
            }
        }

    }
    public class EmployeeSearchResult : PaginationSearchResult
    {
        public required List<Employee> Employees { get; set; }
    }
    public class DentistSearchResult : PaginationSearchResult
    {
        public required List<Dentist> Dentists { get; set; }
    }
    public class PatientSearchResult : PaginationSearchResult
    {
        public required List<Patient> Patients { get; set; }
    }
    public class MedicineSearchResult : PaginationSearchResult
    {
        public required List<Medicine> Medicines { get; set; }
    }
    public class AppointmentSearchResult : PaginationSearchResult
    {
        public required List<Appointment> Appointments { get; set; }
    }
    public class MedicalRecordSearchResult : PaginationSearchResult
    {
        public required List<MedicalRecord> MedicalRecords { get; set; }
    }
    public class InvoiceSearchResult : PaginationSearchResult
    {
        public required List<Invoice> Invoices { get; set; }
    }
    public class ServiceSearchResult : PaginationSearchResult
    {
        public required List<Service> Services { get; set; }
    }
}

