function xoaKhoiTuSach(maSach, button) {
    Swal.fire({
        title: 'Bạn chắc chắn muốn xóa sách này?',
        text: "Sách sẽ bị xóa khỏi tủ sách của bạn!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#dc3545',
        cancelButtonColor: '#6c757d',
        confirmButtonText: 'Gỡ sách',
        cancelButtonText: 'Hủy'
    }).then((result) => {
        if (result.isConfirmed) {
            fetch('/TuSach/Xoa', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({ maSach: maSach })
            })
                .then(response => {
                    if (response.ok) {
                        button.closest('.book-card1').remove();
                        if (document.querySelectorAll('.book-card1').length === 0) {
                            document.querySelector('.book-grid1').innerHTML = `
                                            <p class="chua1" padding: 81px 447px>Bạn chưa lưu sách nào.</p>
                                `;
                        }
                        Swal.fire(
                            'Đã xóa!',
                            'Sách đã được gỡ khỏi tủ sách của bạn.',
                            'success'
                        );
                    } else {
                        Swal.fire('Lỗi', 'Xóa thất bại. Vui lòng thử lại!', 'error');
                    }
                })
                .catch(error => {
                    console.error('Error:', error);
                    Swal.fire('Lỗi', 'Đã xảy ra lỗi, thử lại sau!', 'error');
                });
        }
    });
}