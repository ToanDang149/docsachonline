document.querySelectorAll('.book-card1').forEach(card => {
    const popup = card.querySelector('.book-hover-info');
    card.addEventListener('mouseenter', async function () {
        const maSach = this.dataset.masach;
        if (!maSach) return;
        const rect = this.getBoundingClientRect();
        const spaceRight = window.innerWidth - rect.right;
        const popupWidth = 400;
        if (spaceRight < popupWidth + 20) {
            popup.style.left = 'auto';
            popup.style.right = '105%';
        } else {
            popup.style.right = 'auto';
            popup.style.left = '105%';
        }
        if (!this.dataset.loaded) {
            try {
                const response = await fetch(`/Home/GetSachChiTiet?maSach=${maSach}`);
                if (response.ok) {
                    const data = await response.json();

                    popup.querySelector('.hover-img').src = data.anh || '/images/default.png';
                    popup.querySelector('.hover-title').innerText = data.tenSach || 'Không có tên';
                    popup.querySelector('.hover-author-name').innerText = data.tenTacGia || 'Không rõ';
                    popup.querySelector('.hover-luotxem').innerText = data.luotXem || '0';
                    popup.querySelector('.hover-luottai').innerText = data.luotTai || '0';
                    popup.querySelector('.hover-trungbinhsao').innerText = data.trungBinhSao || '0';
                    popup.querySelector('.hover-soluongdanhgia').innerText = data.soLuongDanhGia || '0';
                    popup.querySelector('.hover-description').innerText = (data.gioiThieu || '').substring(0, 200) + "...";

                    this.dataset.loaded = "true";
                }
            } catch (error) {
                console.error('Không thể lấy chi tiết sách:', error);
            }
        }
        popup.style.display = 'flex';
        popup.style.transition = 'opacity 0.3s ease, visibility 0.3s ease, transform 0.3s ease';
        popup.style.opacity = '1';
        popup.style.visibility = 'visible';
        popup.style.transform = 'translateX(0) scale(1)';
    });
    card.addEventListener('mouseleave', function () {
        const popup = this.querySelector('.book-hover-info');
        popup.style.display = 'none';
        popup.style.transition = 'none';
        popup.style.opacity = '0';
        popup.style.visibility = 'hidden';
        popup.style.transform = 'translateX(20px) scale(0.95)';
    });
    popup.addEventListener('mouseenter', function (e) {
        popup.style.display = 'flex';
        popup.style.transition = 'none';
        popup.style.opacity = '0';
        popup.style.visibility = 'hidden';
        popup.style.transform = 'translateX(20px) scale(0.95)';
        e.stopPropagation();
    });
});