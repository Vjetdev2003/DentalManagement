namespace DentalManagement.Web.Models
{
    // Model cho WebhookRequest
    public class PayOSWebhookRequest
    {
        public string Code { get; set; }
        public string Desc { get; set; }
        public bool Success { get; set; }
        public string Status { get; set; }
        public string OrderCode { get; set; }
        public string Signature { get; set; }
    }
}
