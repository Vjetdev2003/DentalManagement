﻿using DentalManagement.DomainModels;

namespace DentalManagement.Web.Models
{
    public class PrescriptionDetailModel
    {
        public List<PrescriptionMedicineViewModel> Details { get; set; }
        public Prescription Prescriptions { get; set; }
        public Dentist Dentists { get; set; }
        public Patient Patients { get; set; }
    }
}