document.addEventListener("DOMContentLoaded", function () {
    // 1. Tạo mã QR tự động từ link xuất Excel
    const computerIP = "192.168.1.118:5099"; 
    const actionPath = document.getElementById("btnExport").getAttribute("href");
    const exportUrl = "http://" + computerIP + actionPath;
    console.log("Link QR sẽ là: " + exportUrl);
    new QRCode(document.getElementById("qrcode"), {
        text: exportUrl,
        width: 140,
        height: 140,
        colorDark: "#000000",
        colorLight: "#ffffff",
        correctLevel: QRCode.CorrectLevel.H
    });

    // 2. Logic Tìm kiếm
    const searchInput = document.getElementById("searchInput");
    const filterAmount = document.getElementById("filterAmount");
    const tableRows = document.querySelectorAll("#historyTable tbody tr");

    function filterTable() {
        const searchText = searchInput.value.toLowerCase();
        const amountRange = filterAmount.value;

        tableRows.forEach(row => {
            const note = row.querySelector(".note-cell").textContent.toLowerCase();
            const postId = row.querySelector(".badge").textContent.toLowerCase();
            const amount = parseFloat(row.querySelector(".amount-val").getAttribute("data-amount"));

            let matchesSearch = note.includes(searchText) || postId.includes(searchText);
            let matchesAmount = true;

            if (amountRange === "1") matchesAmount = amount < 500000;
            else if (amountRange === "2") matchesAmount = amount >= 500000 && amount <= 2000000;
            else if (amountRange === "3") matchesAmount = amount > 2000000;

            if (matchesSearch && matchesAmount) {
                row.style.display = "";
            } else {
                row.style.display = "none";
            }
        });
    }

    searchInput.addEventListener("keyup", filterTable);
    filterAmount.addEventListener("change", filterTable);
});

function resetFilters() {
    document.getElementById("searchInput").value = "";
    document.getElementById("filterAmount").value = "";
    const rows = document.querySelectorAll("#historyTable tbody tr");
    rows.forEach(r => r.style.display = "");
}