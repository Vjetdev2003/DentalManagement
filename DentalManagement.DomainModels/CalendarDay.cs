using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DentalManagement.DomainModels
{
    public class CalendarDay
    {
        public DateTime Date { get; set; }              // Ngày cụ thể
        public bool IsToday { get; set; }
    }
}
