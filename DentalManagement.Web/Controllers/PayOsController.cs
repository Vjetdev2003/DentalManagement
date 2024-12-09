using DentalManagement.DomainModels;
using DentalManagement.Web.Data;
using DentalManagement.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.Security.Cryptography;
using System.Text;

namespace DentalManagement.Web.Controllers
{
    [ApiController]
    public class PayOsController : ControllerBase
    {
        private readonly DentalManagementDbContext _context;
        private readonly string checksumKey = "e2c88633b9cb46240d1300376bac12c6077566f54d848d84066909f7e7c2f7b9";
        private readonly ILogger _log;
        public PayOsController(DentalManagementDbContext context,ILogger log)
        {
            _context = context;
            _log = log;
        }
        private string ComputePayOSSignature(PayOSWebhookRequest request)
        {
            // Chuỗi cần ký được tạo từ các thuộc tính quan trọng của payload
            string dataToSign = $"{request.Code}|{request.Desc}|{request.Status}|{request.OrderCode}";
            string secretKey = "YourSecretKey"; // Khóa bí mật của bạn từ PayOS

            using (var hmac = new System.Security.Cryptography.HMACSHA256(Encoding.UTF8.GetBytes(secretKey)))
            {
                byte[] hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(dataToSign));
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }
        [HttpPost]
        [Route("api/payos/webhook")]
        public async Task<IActionResult> PayOSWebhook([FromBody] PayOSWebhookRequest webhookRequest)
        {
            try
            {
                // Kiểm tra chữ ký để xác thực dữ liệu từ PayOS
                string computedSignature = ComputePayOSSignature(webhookRequest);
                if (computedSignature != webhookRequest.Signature)
                {
                    return BadRequest(new { success = false, message = "Invalid signature" });
                }

                // Lấy thông tin giao dịch từ webhook
                var transactionStatus = webhookRequest.Status;
                var orderCode = webhookRequest.OrderCode;

                var invoice = await _context.Invoices.FirstOrDefaultAsync(i => i.InvoiceId == Int32.Parse(orderCode));
                if (invoice == null)
                {
                    return NotFound(new { success = false, message = "Invoice not found" });
                }

                // Xử lý trạng thái thanh toán
                if (transactionStatus == "COMPLETED")
                {
                    invoice.Status = 3; // Thanh toán thành công
                }
                else if (transactionStatus == "CANCELLED")
                {
                    invoice.Status = -1; // Thanh toán bị hủy
                }

                // Lưu thay đổi vào cơ sở dữ liệu
                await _context.SaveChangesAsync();

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                // Ghi log lỗi chi tiết
                Console.WriteLine($"Error processing webhook: {ex.Message}");
                return StatusCode(500, new { success = false, message = "An error occurred", error = ex.Message });
            }
        }
    }
}
