using DentalManagement.DomainModels;
using System.ComponentModel.DataAnnotations;

namespace DentalManagement.Web.Models
{
    public class PrescriptionMedicineViewModel
    {
        public int PrescriptionId { get; set; }
        public int MedicineId { get; set; }
        public string MedicineName { get; set; }
        public int Quantity {  get; set; }
        public decimal MedicinePrice {  get; set; }
        public decimal TotalMedicine { get; set; }
    }
}
