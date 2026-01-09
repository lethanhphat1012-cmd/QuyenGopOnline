// auth.js
$(document).ready(function () {
    $('form').on('submit', function () {
        if ($(this).valid()) {
            let btn = $(this).find('.btn-auth');
            btn.prop('disabled', true);
            btn.html('<span class="spinner-border spinner-border-sm me-2"></span> Đang xử lý...');
        }
    });
});