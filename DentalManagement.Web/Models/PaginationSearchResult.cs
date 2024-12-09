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
    public class PrescriptionSearchResult : PaginationSearchResult
    {
        public List<Prescription> Prescriptions = new List<Prescription>();
        public IEnumerable<Prescription> Data { get; set; }
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
        public  List<Medicine> Medicines { get; set; }
        public IEnumerable<Medicine> Data { get; set; }

    }
    public class AppointmentSearchResult : PaginationSearchResult
    {
        public  List<Appointment> Appointments = new List<Appointment>();
        public decimal Price { get; set; }
        public int Unit {  get; set; }
        public IEnumerable<Appointment> Data { get; set; }
        public IEnumerable<InvoiceDetails> Details { get; set; }
        public int Status { get; set; }
        public string TimeRange { get; set; } = "";
    }
    public class MedicalRecordSearchResult : PaginationSearchResult
    {
        public required List<MedicalRecord> MedicalRecords { get; set; }
    }
    public class InvoiceSearchResult : PaginationSearchResult
    {
        public List<Invoice> Invoices = new List<Invoice>();
        public int Status { get; set; }
        public string TimeRange { get; set; } = "";
        
    }
    public class ServiceSearchResult : PaginationSearchResult
    {
        public  List<Service> Services { get; set; } = new List<Service>();
        public IEnumerable<Service> Data { get; set; }
    }
}

