// Biến toàn cục lưu dữ liệu để gửi đi
let globalQrData = "";
let globalImageBase64 = "";

document.addEventListener("DOMContentLoaded", function () {
    startCamera();
});

function startCamera() {
    const statusMsg = document.getElementById("status-msg");
    const overlay = document.getElementById("scan-overlay");
    
    statusMsg.classList.remove("d-none");
    statusMsg.innerHTML = '<i class="fas fa-spinner fa-spin me-2"></i>Đang khởi động Camera...';

    const html5QrCode = new Html5Qrcode("reader");
    
    // Cấu hình Camera
    const config = { 
        fps: 10, 
        qrbox: { width: 250, height: 250 }, 
        aspectRatio: 1.0 
    };

    html5QrCode.start({ facingMode: "environment" }, config, 
        (decodedText, decodedResult) => {
            // === KHI PHÁT HIỆN QR ===
            
            // 1. QUAN TRỌNG: Chụp ảnh NGAY LẬP TỨC (khi video còn đang chạy)
            captureSnapshot();

            // 2. Sau khi chụp xong mới dừng Camera
            html5QrCode.stop().then(() => {
                overlay.style.display = "none";
                statusMsg.classList.add("d-none");

                // 3. Hiển thị kết quả ra màn hình
                showResult(decodedText);
            }).catch(err => console.log("Stop failed: ", err));
        },
        (errorMessage) => {
            // Đang quét...
        }
    ).then(() => {
        statusMsg.classList.add("d-none");
        overlay.style.display = "block"; 
    }).catch(err => {
        statusMsg.classList.remove("d-none");
        statusMsg.className = "alert alert-danger";
        statusMsg.innerText = "Lỗi: Không thể truy cập Camera. Hãy cấp quyền!";
    });
}

function captureSnapshot() {
    // Lấy thẻ video
    const videoElement = document.querySelector("#reader video");
    
    // KIỂM TRA AN TOÀN: Nếu không tìm thấy video thì dừng lại
    if (!videoElement) {
        console.error("Không tìm thấy Video để chụp ảnh!");
        return;
    }

    const canvas = document.getElementById("snapshot-canvas");
    const ctx = canvas.getContext("2d");

    canvas.width = videoElement.videoWidth;
    canvas.height = videoElement.videoHeight;

    ctx.drawImage(videoElement, 0, 0, canvas.width, canvas.height);

    globalImageBase64 = canvas.toDataURL("image/png");
    
    document.getElementById("captured-preview").src = globalImageBase64;
}

function showResult(qrText) {
    globalQrData = qrText;
    document.getElementById("qr-result").value = qrText;
    document.getElementById("result-section").style.display = "block";
    document.querySelector(".scanner-container").style.display = "none";
}

function sendDataToBackend() {
    // 1. Kiểm tra xem đã có dữ liệu chưa
    if (!globalQrData || !globalImageBase64) {
        alert("⚠️ Chưa có dữ liệu! Vui lòng quét lại.");
        return;
    }

    // 2. Tạo hiệu ứng "Đang xử lý" cho nút bấm
    const btn = document.getElementById("btn-process");
    const originalText = btn.innerHTML;
    btn.innerHTML = '<i class="fas fa-spinner fa-spin me-2"></i>Đang phân tích AI...';
    btn.disabled = true;

    // 3. Đóng gói dữ liệu vào FormData
    const formData = new FormData();
    formData.append("qrData", globalQrData);
    formData.append("imageFile", globalImageBase64);

    // 4. Gửi yêu cầu về Server (AccountController)
    fetch('/Account/VerifyIdentity', {
        method: 'POST',
        body: formData
    })
    .then(response => response.json())
    .then(data => {
        if (data.success) {
            // Trường hợp 1: AI xác nhận trùng khớp -> Thành công
            Swal.fire({
                title: 'Xác thực thành công!',
                text: data.message,
                icon: 'success'
            }).then(() => {
                window.location.href = "/Home/Index"; // Chuyển hướng về trang chủ
            });
        } else {
            // Trường hợp 2: Dữ liệu không khớp hoặc lỗi
            Swal.fire({
                title: 'Xác thực thất bại',
                text: data.message,
                icon: 'error'
            });
        }
    })
    .catch(error => {
        console.error('Error:', error);
        alert("❌ Lỗi kết nối đến máy chủ!");
    })
    .finally(() => {
        // Trả lại trạng thái nút bấm
        btn.innerHTML = originalText;
        btn.disabled = false;
    });
}

