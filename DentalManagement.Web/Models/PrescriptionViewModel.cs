using DentalManagement.DomainModels;
using System.Web.Mvc;

namespace DentalManagement.Web.Models
{
    public class PrescriptionViewModel
    {
        public int PrescriptionId { get; set; }
        public int DentistId {  get; set; }
        public int PatientId { get; set; }  
        public string Notes { get; set; }
        public string Diagnosis { get; set; }
        public string Dosage { get; set; }
        public string Frequency { get; set; }
        public string Duration { get; set; }
        public List<PrescriptionDetails > PrescriptionMedicines { get; set; }
        public List<SelectListItem> ListMedicine { get; set; }
        public List<Medicine> Medicines { get; set; }
        public MedicineSearchInput MedicineSearchInput { get; set; } = new MedicineSearchInput();
        public PrescriptionCreateModel PrescriptionCreateModel { get; set; } = new PrescriptionCreateModel();

    }
}
