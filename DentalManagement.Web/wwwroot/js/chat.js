document.addEventListener("DOMContentLoaded", function () {
    const chatButton = document.getElementById("chatButton");
    const chatWindow = document.getElementById("chatWindow");
    const closeChatButton = document.getElementById("closeChatButton");
    const sendMessageBtn = document.getElementById("submitRequest");
    const chatMessageInput = document.querySelector("[name='message']");
    const nameInput = document.querySelector("[name='name']");
    const phoneInput = document.querySelector("[name='phone']");
    const messageItems = document.getElementById("messageItems");
    const messageCountElement = document.getElementById("newMessageCount");
    const messageHeader = document.getElementById("messageHeader");
    const openMessagesButton = document.getElementById("messageDropdown");

    // Kiểm tra nếu các phần tử tồn tại rồi mới gán sự kiện
    if (chatButton) {
        chatButton.addEventListener("click", function () {
            if (chatWindow) {
                chatButton.style.display = chatWindow.style.display != "none" ? "flex" : "none";
                chatWindow.style.display = chatWindow.style.display === "none" ? "flex" : "none";
            }
        
        });
    }

    if (closeChatButton) {
        closeChatButton.addEventListener("click", function () {
            if (chatWindow) {
                chatButton.style.display = "flex";
                chatWindow.style.display = "none";
            }
        });
    }

    if (sendMessageBtn) {
        sendMessageBtn.addEventListener("click", function () {
            const name = nameInput.value.trim();
            const phone = phoneInput.value.trim();
            const message = chatMessageInput.value.trim();

            if (name && phone && message) {
                if (isConnected) {
                    // Gửi tin nhắn đến nhân viên
                    chatConnection.invoke("SendMessageToEmployees", name, phone, message)
                        .then(() => {
                            console.log("Message sent successfully");
                            alert("Yêu cầu được gửi thành công! Vui lòng chờ nhân viên gọi hỗ trợ bạn.\n Xin cảm ơn!");
                            // Reset form input sau khi gửi
                            nameInput.value = "";
                            phoneInput.value = "";
                            chatMessageInput.value = "";
                        })
                        .catch(err => {
                            console.error("Error sending message: " + err.toString());
                            alert("Gửi tin nhắn không thành công. Vui lòng thử lại.");
                        });
                } else {
                    alert("Kết nối SignalR không thành công. Vui lòng thử lại.");
                }
            } else {
                alert("Vui lòng nhập đầy đủ thông tin.");
            }
        });
    }

    // Tạo kết nối SignalR
    let chatConnection = new signalR.HubConnectionBuilder()
        .withUrl("/chatHub", {
            transport: signalR.HttpTransportType.WebSockets | signalR.HttpTransportType.LongPolling
        })
        .build();

    // Trạng thái kết nối
    let isConnected = false;

    // Khởi tạo kết nối và tham gia nhóm nhân viên
    chatConnection.start()
        .then(function () {
            console.log("Connected to SignalR Hub");
            isConnected = true;  // Cập nhật trạng thái kết nối
            chatConnection.invoke("JoinEmployeeGroup").catch(err => console.error(err.toString()));
        })
        .catch(function (err) {
            console.error("Error while starting connection: " + err.toString());
            setTimeout(() => {  // Tự động thử kết nối lại sau một khoảng thời gian ngắn
                chatConnection.start().then(() => {
                    console.log("Reconnected to SignalR Hub");
                    isConnected = true;
                }).catch(err => console.error("Error reconnecting: " + err.toString()));
            }, 5000);
        });

    // Xử lý sự kiện khi nhận được tin nhắn mới
    chatConnection.on("ReceiveMessage", function (name, phone, message, timestamp, isNewMessage) {
        // Tạo một phần tử danh sách mới cho tin nhắn
        const listItem = document.createElement("li");

        // Kiểm tra xem đây có phải là tin nhắn mới không
        if (isNewMessage) {
            // Thêm lớp CSS hoặc thay đổi màu nền để làm nổi bật tin nhắn mới
            listItem.classList.add("new-message"); // Thêm lớp CSS "new-message"
            listItem.style.backgroundColor = "#e0f7fa"; // Ví dụ: Thay đổi màu nền
        }

        // Tạo cấu trúc HTML cho tin nhắn
        listItem.innerHTML = `
    <div>
        Bệnh nhân <strong>${name}</strong> với số Điện thoại <strong>${phone}</strong> với tin nhắn: 
        <em>${message}</em> cần tư vấn và hỗ trợ. 
        <small>(${new Date(timestamp).toLocaleString()})</small>
    </div>`;

        // Định dạng timestamp để so sánh (trong trường hợp chuỗi thời gian)
        const messageTimestamp = new Date(timestamp).getTime();

        // Thêm tin nhắn mới vào danh sách và sắp xếp theo timestamp (giảm dần)
        let inserted = false;
        const items = messageItems.children;
        for (let i = 0; i < items.length; i++) {
            const currentTimestamp = new Date(items[i].querySelector("small").textContent).getTime();
            if (messageTimestamp > currentTimestamp) {
                messageItems.insertBefore(listItem, items[i]);
                inserted = true;
                break;
            }
        }

        // Nếu không có phần tử nào nhỏ hơn, thêm vào cuối danh sách
        if (!inserted) {
            messageItems.appendChild(listItem);
        }

        // Sau một thời gian, loại bỏ lớp "new-message" (có thể sau 5 giây hoặc khi người dùng đã đọc tin nhắn)
        setTimeout(() => {
            if (isNewMessage) {
                listItem.classList.remove("new-message");
                listItem.style.backgroundColor = ""; // Xóa màu nền
            }
        }, 5000); // Tin nhắn sẽ không còn "mới" sau 5 giây

        // Sau khi tin nhắn được hiển thị, đánh dấu nó là đã đọc
        markMessageAsRead(message, timestamp);
    });

    // Hàm đánh dấu tin nhắn là đã đọc
    function markMessageAsRead(message, timestamp) {
        fetch("/api/markMessageAsRead", {
            method: "POST",
            body: JSON.stringify({ message, timestamp }),
            headers: {
                "Content-Type": "application/json"
            }
        })
            .then(response => {
                if (response.ok) {
                    console.log("Tin nhắn đã được đánh dấu là đã đọc.");
                } else {
                    console.log("Không thể đánh dấu tin nhắn là đã đọc.");
                }
            })
            .catch(error => {
                console.error("Lỗi khi đánh dấu tin nhắn đã đọc: ", error);
            });
    }


    // Cập nhật số lượng tin nhắn mới
    chatConnection.on("NotifyNewMessageCount", function (newMessageCount, latestMessageContent) {
        if (messageCountElement) {
            messageCountElement.textContent = newMessageCount;  // Cập nhật số lượng tin nhắn mới
        }
        if (messageHeader) {
            messageHeader.textContent = `You have ${newMessageCount} new messages`; // Cập nhật tiêu đề
        }
        // Thêm một mục thông báo về số lượng tin nhắn mới
      //  const notificationItem = document.createElement("li");
       // notificationItem.innerHTML = `<span><strong>New messages:</strong> ${latestMessageContent}</span>`;
        //messageItems.appendChild(notificationItem);
    });

    // Xử lý sự cố mất kết nối và thử kết nối lại
    chatConnection.onclose(function () {
        console.log("Connection lost. Reconnecting...");
        isConnected = false;
        // Tự động thử kết nối lại sau 5 giây
        setTimeout(() => {
            chatConnection.start()
                .then(function () {
                    console.log("Reconnected to SignalR Hub");
                    isConnected = true;
                })
                .catch(function (err) {
                    console.error("Error reconnecting: " + err.toString());
                });
        }, 5000);  // Cố gắng kết nối lại sau 5 giây
    });
});
