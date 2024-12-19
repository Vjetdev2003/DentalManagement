using DentalManagement.Web.Data;
using DentalManagement.Web.Repository;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DentalManagement.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserAccountRepository _userAccountRepository;

        public AccountController(DentalManagementDbContext context)
        {
            _userAccountRepository = new UserAccountRepository(context);
        }
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username = "", string password = "")
        {
            ViewBag.UserName = username;
            
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError("Error", "Nhập tên và mật khẩu!");
                TempData["LoginError"] = "Vui long nhap ten va mat khau!";
                return View();
            }

            var userAccount = await _userAccountRepository.Authorize(username, password);

            if (userAccount == null)
            {
                ModelState.AddModelError("Error", "Đăng nhập thất bại!");
                TempData["LoginError"] = "Ten dang nhap hoac mat khau khong dung!";
                return View();
            }

            // Đăng nhập thành công, tạo dữ liệu để lưu thông tin đăng nhập
            var userData = new WebUserData()
            {
                UserId = userAccount.UserID,
                UserName = userAccount.UserName,
                Email = userAccount.Email,
                DisplayName = userAccount.FullName,
                Photo = userAccount.Photo,
                ClientIP = HttpContext.Connection.RemoteIpAddress?.ToString(),
                SessionId = HttpContext.Session.Id,
                AdditionalData = "",
                Roles = userAccount.RoleNames.Split(',').ToList()
            };

            // Thiết lập phiên đăng nhập cho tài khoản
            await HttpContext.SignInAsync(userData.CreatePrincipal());

            // Lưu thông tin vào Session nếu cần thiết
            HttpContext.Session.SetString("UserName", userAccount.UserName);
            HttpContext.Session.SetString("UserID", userAccount.UserID);
            HttpContext.Session.SetString("UserRoles", userAccount.RoleNames);
            HttpContext.Session.SetString("Photo",userAccount.Photo);
            // Bất kỳ thông tin nào khác bạn cần lưu vào session

            // Redirect về trang chủ sau khi đăng nhập
            return RedirectToAction("Index", "Home");
        }
        public async Task<IActionResult> Logout()
        {
         
            // Xóa session
            HttpContext.Session.Clear();

            // Đăng xuất
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Chuyển hướng về trang đăng nhập
            return RedirectToAction("Login");
        }
        public async Task<IActionResult> ChangePassword(string oldPassword = "", string newPassword = "", string reNewPassword = "")
        {
            ViewBag.Title = "Đổi Mật Khẩu";
            if (Request.Method == "POST")
            {
                // Kiểm tra xem có nhập đầy đủ thông tin không?
                if (string.IsNullOrWhiteSpace(oldPassword) || string.IsNullOrWhiteSpace(newPassword) || string.IsNullOrWhiteSpace(reNewPassword))
                {
                    ModelState.AddModelError("Error", "Vui lòng nhập lại thông tin");
                    return View();
                }
                // Kiểm tra xem mật khẩu có trùng với mật khẩu mới không?
                if (!newPassword.Equals(reNewPassword))
                {
                    ModelState.AddModelError("Error", "Mật khẩu mới không trùng khớp");
                    return View();
                }

                var userData = User.GetUserData(); // Lấy thông tin người dùng từ Claims
                if (userData != null)
                {
                    var result = await _userAccountRepository.ChangePassword(userData.UserName, oldPassword, newPassword);
                    if (!result)
                    {
                        ModelState.AddModelError("Error", "Mật khẩu cũ không đúng");
                        return View();
                    }
                    return RedirectToAction("Logout"); // Chuyển hướng đến trang đăng xuất
                }
            }
            return View();
        }


        public IActionResult AccessDenied()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(string userName, string password, string confirmPassword, string email)
        {
            bool emailExists = await _userAccountRepository.CheckEmailExists(email);
            if (emailExists)
            {
                ViewBag.ErrorMessage = "Email này đã được đăng ký.";
                return View();
            }
            if (password.Length < 8)
            {
                ViewBag.ErrorMessage = "Mật khẩu phải có ít nhất 8 ký tự.";
                return View();
            }
            if (password != confirmPassword)
            {
                ViewBag.ErrorMessage = "Mật khẩu và xác nhận mật khẩu không khớp.";
                return View();
            }

            bool isRegistered = await _userAccountRepository.Register(userName, password, email);
            if (isRegistered)
            {

                // Đăng ký thành công
                ViewBag.SuccessMessage = "Đăng ký thành công! Bạn có thể đăng nhập ngay bây giờ.";
                return View();
            }
            else
            {
                // Tên đăng nhập hoặc email đã tồn tại
                ViewBag.ErrorMessage = "Vui lòng nhập email chính xác.";
                return View();
            }
        }
    }
}

