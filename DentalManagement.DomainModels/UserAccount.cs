using System.ComponentModel.DataAnnotations;

namespace DentalManagement.DomainModels
{
    /// <summary>
    /// Thông tin tài khoản trong CSDL
    /// </summary>
    public class UserAccount
    {
        [Key]
        public string UserID { get; set; } = ""; // Khóa chính

        [Required]
        [StringLength(50)]
        public string UserName { get; set; } = ""; // Tên người dùng
        public string FullName {  get; set; } = "";
        public string Photo { get; set; } = ""; // Email người dùng (nếu có)

        [StringLength(100)]
        public string Email { get; set; } = "";// Email người dùng (nếu có)
        public string Password { get; set; } = "";
        public string RoleNames { get; set; } = "";
    }

}
