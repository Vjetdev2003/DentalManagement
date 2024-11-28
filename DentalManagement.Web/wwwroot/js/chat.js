document.addEventListener("DOMContentLoaded", function () {
    const chatButton = document.getElementById("chatButton");
    const chatWindow = document.getElementById("chatWindow");
    const closeChatButton = document.getElementById("closeChatButton");
    const sendMessageBtn = document.getElementById("submitRequest");
    const chatMessageInput = document.querySelector("[name='message']");
    const nameInput = document.querySelector("[name='name']");
    const phoneInput = document.querySelector("[name='phone']");
    const chatMessages = document.getElementById("chatMessages");

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
                chatButton.style.display = "flex" ;
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
                            console.log("Message sent successfully" );
                            alert("Tin nhắn đã được gửi!");
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


    // Lắng nghe thông báo có tin nhắn mới và cập nhật số lượng tin nhắn
    chatConnection.on("NotifyNewMessageCount", function (newMessageCount, latestMessageContent) {
        const messageCountElement = document.getElementById("newMessageCount");
        const messageHeader = document.getElementById("messageHeader");
        const msgItems = document.getElementById("messageItems");
        if (messageCountElement) {
            messageCountElement.textContent = newMessageCount;  // Cập nhật số lượng tin nhắn mới
        }
        if (messageHeader) {
            messageHeader.textContent = `You have ${newMessageCount} new messages`; // Cập nhật tiêu đề
        }
        // Tạo một phần tử danh sách hiển thị tin nhắn mới
        const listItem = document.createElement("li");
        listItem.innerHTML = `
        <li>
            <h4>
                <span>${latestMessageContent}</span>
            </h4>
            </li>
        `;
        msgItems.appendChild(listItem);
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
