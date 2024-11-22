using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DentalManagement.DomainModels
{
    public static class Constants
    {
        public const int APPOINTMENT_INIT = 1;          // Lịch hẹn đã đặt. Đang chờ xác nhận.
        public const int APPOINTMENT_CONFIRMED = 2;     // Lịch hẹn đã xác nhận. Đang chờ đến ngày hẹn.
        public const int APPOINTMENT_IN_PROGRESS = 3;   // Lịch hẹn đang diễn ra.
        public const int APPOINTMENT_FINISHED = 4;      // Lịch hẹn đã hoàn tất.
        public const int APPOINTMENT_CANCELLED = -1;    // Lịch hẹn đã hủy.
        public const int APPOINTMENT_NO_SHOW = -2;      // Bệnh nhân không đến khám.

    }

    public static class Constants_Invoice
    {
        public const int INVOICE_PAID = 3; //Đã thanh toán
        public const int INVOICE_UNPAID = 1; //Chưa thanh toán
        public const int INVOICE_PROCESSING = 2;//Đang xử lí
        public const int INVOICE_FAILED = -2;//Thanh toán thất bại
        public const int INVOICE_CANCELLED = -1; //Thanh toán bị huỷ
    }
    public static class Paymethod
    {
        public const string CASH = "Tiền mặt";
        public const string CARD = "Thẻ";
        public const string BANKING = "Chuyển khoản";

    }
}
