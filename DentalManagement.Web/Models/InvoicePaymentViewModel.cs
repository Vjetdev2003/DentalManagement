using DentalManagement.DomainModels;

namespace DentalManagement.Web.Models
{
    public class InvoicePaymentViewModel
    {
        public int InvoiceId { get; set; } // ID của hóa đơn
        public decimal TotalAmount { get; set; } // Tổng tiền hóa đơn sau khi áp dụng giảm giá
        public decimal Discount { get; set; } // Mức giảm giá (nếu có)
        public string PatientName { get; set; } // Tên bệnh nhân (tuỳ chọn)
        public int Status { get; set; } // Trạng thái hóa đơn (Chưa thanh toán, Đang xử lý, Đã thanh toán)
        public DateTime DateUpdated { get; set; }
    }
}
