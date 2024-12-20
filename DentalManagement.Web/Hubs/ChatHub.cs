using DentalManagement.DomainModels;
using DentalManagement.Web.Data;
using DentalManagement.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;


namespace DentalManagement.Web.Hubs
{
    public class ChatHub : Hub
    {
      
        private static int newMessageCount = 0;
        private readonly DentalManagementDbContext _context;
        public ChatHub(DentalManagementDbContext context)
        {
            _context = context;
        }
        // Tham gia nhóm nhân viên khi kết nối
        public async Task JoinEmployeeGroup()
            {
            var userRoles = GetUserRoles(Context.User);
            if (userRoles.Contains(WebUserRoles.Employee) || userRoles.Contains(WebUserRoles.Administrator)) 
            {
                // Thêm kết nối vào nhóm "Employees"
                await Groups.AddToGroupAsync(Context.ConnectionId, "Employees");
                Console.WriteLine($"Employee {Context.User.Identity.Name} joined the employee group.");
            }
        }

        //// Rời nhóm nhân viên
        //public async Task LeaveEmployeeGroup()
        //{
        //    var userRoles = GetUserRoles(Context.User);
        //    if (userRoles.Contains(WebUserRoles.Employee)) // Kiểm tra nếu người dùng là nhân viên
        //    {
        //        // Xóa kết nối khỏi nhóm "Employees"
        //        await Groups.RemoveFromGroupAsync(Context.ConnectionId, "Employees");
        //        Console.WriteLine($"Employee {Context.User.Identity.Name} left the employee group.");
        //    }
        //}

        // Gửi tin nhắn tới nhân viên
        public async Task SendMessageToEmployees(string name, string phone, string message)
        {
            try
            {
                

                // Lưu tin nhắn vào cơ sở dữ liệu
                var msg = new Message
                {
                    Name = name,
                    Phone = phone,
                    Messages = message,
                    Timestamp = DateTime.Now,
                    NewMessage = true

                };
                _context.Messages.Add(msg);
                await _context.SaveChangesAsync();

                int newMessageCount = await _context.Messages
                     .Where(msg => msg.NewMessage == true)  // Chỉ chọn các tin nhắn có NewMessage = true
                     .CountAsync();

                string latestMessageContent = $"Bệnh nhân {name} với số Điện thoại {phone} với tin nhắn: {message} cần tư vấn và hỗ trợ.";
                
                // Gửi thông báo và tin nhắn tới tất cả kết nối
                await Clients.All.SendAsync("NotifyNewMessageCount", newMessageCount, latestMessageContent);
                await Clients.All.SendAsync("ReceiveMessage", name, phone, message, msg.Timestamp, msg.NewMessage);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SendMessageToEmployees: {ex.Message}");
                await Clients.Caller.SendAsync("ReceiveError", ex.Message);
                throw;
            }

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
        public async Task<List<MessageHelp>> GetMessages()
        {
            try
            {
                var cutoffDate = DateTime.Now.AddDays(-2);

                // Get messages from the database
                return await _context.Messages
                    .Where(m => m.Timestamp >= cutoffDate) // Filter by recent messages
                    .OrderByDescending(m => m.Timestamp)
                    .Select(m => new MessageHelp
                    {
                        Name = m.Name,
                        Phone = m.Phone,
                        Message = m.Messages,
                        Timestamp = m.Timestamp,
                        NewMessage = m.NewMessage  // Include the NewMessage flag
                    })
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetMessages: {ex.Message}");
                return new List<MessageHelp>();
            }
        }

        public override async Task OnConnectedAsync()
        {
            try
            {
                // Lấy danh sách tin nhắn từ cơ sở dữ liệu, sắp xếp theo thời gian giảm dần
                var messages = await _context.Messages
                    .OrderByDescending(m => m.Timestamp) // Sắp xếp giảm dần theo thời gian
                    .Select(m => new MessageHelp
                    {
                        Name = m.Name,
                        Phone = m.Phone,
                        Message = m.Messages,
                        Timestamp = m.Timestamp,
                        NewMessage = m.NewMessage
                    })
                    .ToListAsync();

                // Gửi danh sách tin nhắn tới client (mới nhất trước)
                foreach (var msg in messages)
                {
                    await Clients.Caller.SendAsync("ReceiveMessage", msg.Name, msg.Phone, msg.Message, msg.Timestamp, msg.NewMessage);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in OnConnectedAsync: {ex.Message}");
            }

            await base.OnConnectedAsync();
        }
        public async Task MarkMessagesAsRead()
        {
            try
            {
                var unreadMessages = await _context.Messages
                    .Where(m => m.NewMessage == true) // Lọc tin nhắn chưa đọc
                    .ToListAsync();

                foreach (var message in unreadMessages)
                {
                    message.NewMessage = false; // Đánh dấu là đã đọc
                }

                await _context.SaveChangesAsync(); // Lưu thay đổi vào database

                // Cập nhật lại số lượng tin nhắn mới cho tất cả client
                int newMessageCount = 0; // Không còn tin nhắn mới
                await Clients.All.SendAsync("NotifyNewMessageCount", newMessageCount);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in MarkMessagesAsRead: {ex.Message}");
                throw;
            }
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
