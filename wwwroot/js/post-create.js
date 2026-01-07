document.addEventListener("DOMContentLoaded", function () {
    const fileInput = document.getElementById("ImageFile");
    const previewImage = document.getElementById("image-preview");
    const uploadText = document.querySelector(".upload-text");

    fileInput.addEventListener("change", function () {
        const file = this.files[0];
        if (file) {
            const reader = new FileReader();
            reader.onload = function (e) {
                previewImage.src = e.target.result;
                previewImage.style.display = "block";
                uploadText.innerHTML = `<i class="fa-solid fa-circle-check text-success"></i> Đã chọn: <b>${file.name}</b>`;
            };
            reader.readAsDataURL(file);
        }
    });
});