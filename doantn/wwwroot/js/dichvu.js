function muaGoi(maCap) {
    fetch('/Mua/MuaGoi', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ maCap: maCap })
    })
        .then(res => res.json())
        .then(data => {
            if (data.success) {
                Swal.fire({
                    icon: 'success',
                    title: 'Mua gói thành công!',
                    showConfirmButton: false,
                    timer: 2000,
                });
                setTimeout(() => location.reload(), 2200);
            } else {
                Swal.fire({
                    icon: 'error',
                    title: 'Lỗi!',
                    text: data.message
                });
            }
        });
}
function hienThongBaoDangNhap() {
    Swal.fire({
        title: 'Bạn cần đăng nhập!',
        text: 'Vui lòng đăng nhập để tiếp tục.',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: 'Ok',
        cancelButtonText: 'Hủy',
    }).then((result) => {
        if (result.isConfirmed) {
            const returnUrl = encodeURIComponent(window.location.pathname + window.location.search);
            window.location.href = `/TaiKhoan/DangNhap?returnUrl=${returnUrl}`;
        }
    });
}
