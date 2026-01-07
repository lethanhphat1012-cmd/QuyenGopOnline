document.addEventListener("DOMContentLoaded", function () {
    // Tooltip khởi tạo nếu có dùng Bootstrap Tooltips
    var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'))
    var tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl)
    });

    console.log("Admin Post Manager Loaded.");
});
function confirmDelete(id, title) {
    Swal.fire({
        title: 'Bạn có chắc chắn?',
        text: `Chiến dịch "${title}" sẽ bị xóa vĩnh viễn!`,
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#ff4757',
        cancelButtonColor: '#2f3542',
        confirmButtonText: 'Vâng, xóa nó!',
        cancelButtonText: 'Hủy'
    }).then((result) => {
        if (result.isConfirmed) {
            $.post('/Post/Delete/' + id, function (data) {
                if (data.success) {
                    Swal.fire('Đã xóa!', data.message, 'success').then(() => {
                        location.reload(); // Load lại trang để cập nhật danh sách
                    });
                } else {
                    Swal.fire('Lỗi!', data.message, 'error');
                }
            });
        }
    });
}