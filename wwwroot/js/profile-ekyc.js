// Thêm hiệu ứng xuất hiện dần dần
document.addEventListener("DOMContentLoaded", function() {
    const card = document.querySelector('.glass-card');
    card.style.opacity = 0;
    card.style.transform = "translateY(20px)";
    
    setTimeout(() => {
        card.style.transition = "all 0.8s ease-out";
        card.style.opacity = 1;
        card.style.transform = "translateY(0)";
    }, 100);
});