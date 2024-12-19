    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    namespace DentalManagement.DomainModels
    {
        public class PrescriptionDetails
        {
            public int PrescriptionId { get; set; }
            public int MedicineId { get; set; }
            public Medicine Medicine { get; set; }
            public string MedicineName { get; set; } = string.Empty;
            public int Quantity { get; set; }
             public decimal SalePrice { get; set; } = 0;
            //public  Invoice Invoice { get; set; }
            public decimal TotalMedicine { get {return Quantity * SalePrice; }}

    }
    
}
