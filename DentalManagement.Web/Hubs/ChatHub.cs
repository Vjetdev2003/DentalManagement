using DentalManagement.Web.Data;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace DentalManagement.Web.Hubs
{
    public class ChatHub : Hub
    {
        private static int newMessageCount = 0;
        // Tham gia nhóm nhân viên khi kết nối
        public async Task JoinEmployeeGroup()
        {
            var userRoles = GetUserRoles(Context.User);
            if (userRoles.Contains(WebUserRoles.Employee)) // Kiểm tra nếu người dùng là nhân viên
            {
                // Thêm kết nối vào nhóm "Employees"
                await Groups.AddToGroupAsync(Context.ConnectionId, "Employees");
                Console.WriteLine($"Employee {Context.User.Identity.Name} joined the employee group.");
            }
        }

        // Rời nhóm nhân viên
        public async Task LeaveEmployeeGroup()
        {
            var userRoles = GetUserRoles(Context.User);
            if (userRoles.Contains(WebUserRoles.Employee)) // Kiểm tra nếu người dùng là nhân viên
            {
                // Xóa kết nối khỏi nhóm "Employees"
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, "Employees");
                Console.WriteLine($"Employee {Context.User.Identity.Name} left the employee group.");
            }
        }

        // Gửi tin nhắn tới nhân viên
        public async Task SendMessageToEmployees(string name, string phone, string message)
        {
            newMessageCount++;  // Tăng số lượng tin nhắn mới
        

            string latestMessageContent = $"Bệnh nhân {name} với số Điện thoại {phone} với tin nhắn: {message} cần tư vấn và hỗ trợ.";
            // Gửi thông báo về số lượng tin nhắn mới và nội dung tin nhắn mới
            await Clients.All.SendAsync("NotifyNewMessageCount", newMessageCount, latestMessageContent);
            await Clients.All.SendAsync("ReceiveMessage", name, phone, message);

            /*var msg = new MessageHelp
            {
                Name = name,
                Phone = phone,
                Message = message
            };
            listMessage.Add(msg);
            if(listMessage.Count > 0)
            {
                foreach (var item in listMessage)
                {
                    string latestMessageContent = $"Bệnh nhân {item.Name} với số Điện thoại {item.Phone} với tin nhắn: {item.Message} cần tư vấn và hỗ trợ.";
                    await Clients.All.SendAsync("NotifyNewMessageCount", newMessageCount, latestMessageContent);
                    await Clients.All.SendAsync("ReceiveMessage", item.Name, item.Phone, item.Message);
                }
            }*/
        }

        // Lấy vai trò của người dùng từ Claims
        private List<string> GetUserRoles(ClaimsPrincipal user)
        {
            var roles = new List<string>();
            if (user != null)
            {
                roles.AddRange(user.FindAll(ClaimTypes.Role).Select(c => c.Value));
            }
            return roles;
        }
    }
  
}
