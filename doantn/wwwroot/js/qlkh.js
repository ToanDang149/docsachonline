$(document).ready(function () {
    $('.table').DataTable({
        lengthChange: false,
        searching: true,
        info: false,
        paging: true,
        pageLength: 10,
        language: {
            search: "Tìm kiếm:",
            searchPlaceholder: "Nhập tài khoản, tên, email...",
            paginate: {
                previous: "<i class='fa-solid fa-chevron-left'></i>",
                next: "<i class='fa-solid fa-chevron-right'></i>"
            },
            zeroRecords: "Không tìm thấy người dùng phù hợp",
        }
    });
    $(".dataTables_filter").append(`
                <button type="button" class="btn-add-new" onclick="openModalThemKhachHang()">
                    <i class="fa-solid fa-plus"></i> Thêm mới
                </button>
            `);
});
function openModalThemKhachHang() {
    $('#formThemKhachHang')[0].reset();
    $('#modalThemKhachHang').modal('show');
    loadCapKhachHang();
}

function loadCapKhachHang() {
    $.ajax({
        url: '/QuanLyKhachHang/GetDanhSachCap',
        method: 'GET',
        success: function (data) {
            var select = $('#selectCapKhachHang');
            select.find('option:not([disabled])').remove();
            data.forEach(cap => {
                select.append(`<option value="${cap.maCap}">${cap.tenCap}</option>`);
            });
        },
        error: function () {
            alert('Không thể tải cấp độ.');
        }
    });
}
$('#formThemKhachHang').submit(async function (e) {
    e.preventDefault();
    const taiKhoan = $('input[name="TaiKhoan"]').val().trim();
    const matKhau = $('input[name="MatKhau"]').val().trim();
    const hoTen = $('input[name="HoTen"]').val().trim();
    const email = $('input[name="Email"]').val().trim();
    const soDT = $('input[name="SoDT"]').val().trim();
    const gioiTinh = $('select[name="GioiTinh"]').val();
    const maCap = $('select[name="MaCap"]').val();

    const regexHoTen = /^[^\d]+$/;
    const regexEmail = /^[^\s@@]+@@[^\s@@]+\.[^\s@@]+$/;
    const regexChiToanSo = /^\d+$/;
    const isTrung = await $.get('/QuanLyKhachHang/KiemTraTaiKhoanTrung', { taiKhoan })
        .then(res => res.isTrung)
        .catch(() => false);

    if (!taiKhoan) {
        Swal.fire({
            icon: 'warning',
            title: 'Thiếu thông tin',
            text: 'Tài khoản không được để trống.',
            confirmButtonText: 'OK',
            customClass: { confirmButton: 'swal-button-blue' }
        });
        return;
    }
    if (isTrung) {
        Swal.fire({
            icon: 'error',
            title: 'Tài khoản đã tồn tại',
            text: `Tài khoản "${taiKhoan}" đã có. Vui lòng nhập tài khoản khác.`,
            confirmButtonText: 'OK',
            customClass: { confirmButton: 'swal-button-blue' }
        });
        return;
    }
    if (!matKhau) {
        Swal.fire({
            icon: 'warning',
            title: 'Thiếu thông tin',
            text: 'Mật khẩu không được để trống.',
            confirmButtonText: 'OK',
            customClass: { confirmButton: 'swal-button-blue' }
        });
        return;
    }
    if (!hoTen) {
        Swal.fire({
            icon: 'warning',
            title: 'Thiếu thông tin',
            text: 'Họ tên không được để trống.',
            confirmButtonText: 'OK',
            customClass: { confirmButton: 'swal-button-blue' }
        });
        return;
    }
    if (!email) {
        Swal.fire({
            icon: 'warning',
            title: 'Thiếu thông tin',
            text: 'Email không được để trống.',
            confirmButtonText: 'OK',
            customClass: { confirmButton: 'swal-button-blue' }
        });
        return;
    }
    if (!soDT) {
        Swal.fire({
            icon: 'warning',
            title: 'Thiếu thông tin',
            text: 'Số điện thoại không được để trống.',
            confirmButtonText: 'OK',
            customClass: { confirmButton: 'swal-button-blue' }
        });
        return;
    }
    if (!gioiTinh) {
        Swal.fire({
            icon: 'warning',
            title: 'Thiếu thông tin',
            text: 'Phải chọn giới tính.',
            confirmButtonText: 'OK',
            customClass: { confirmButton: 'swal-button-blue' }
        });
        return;
    }
    if (!maCap) {
        Swal.fire({
            icon: 'warning',
            title: 'Thiếu thông tin',
            text: 'Phân cấp không đươc để trống.',
            confirmButtonText: 'OK',
            customClass: { confirmButton: 'swal-button-blue' }
        });
        return;
    }
    if (taiKhoan.length < 5) {
        Swal.fire({
            icon: 'warning',
            title: 'Tài khoản không hợp lệ',
            text: 'Tài khoản phải có ít nhất 5 ký tự.',
            confirmButtonText: 'OK',
            customClass: { confirmButton: 'swal-button-blue' }
        });
        return;
    }

    if (matKhau.length < 6) {
        Swal.fire({
            icon: 'warning',
            title: 'Mật khẩu không hợp lệ',
            text: 'Mật khẩu phải có ít nhất 6 ký tự.',
            confirmButtonText: 'OK',
            customClass: { confirmButton: 'swal-button-blue' }
        });
        return;
    }

    if (!regexHoTen.test(hoTen)) {
        Swal.fire({
            icon: 'warning',
            title: 'Họ tên không hợp lệ',
            text: 'Họ tên không được chứa số.',
            confirmButtonText: 'OK',
            customClass: { confirmButton: 'swal-button-blue' }
        });
        return;
    }

    if (!regexEmail.test(email)) {
        Swal.fire({
            icon: 'warning',
            title: 'Email không hợp lệ',
            text: 'Vui lòng nhập đúng định dạng email.',
            confirmButtonText: 'OK',
            customClass: { confirmButton: 'swal-button-blue' }
        });
        return;
    }

    if (!regexChiToanSo.test(soDT)) {
        Swal.fire({
            icon: 'warning',
            title: 'Số điện thoại không hợp lệ',
            text: 'Số điện thoại chỉ được chứa số (0–9).',
            confirmButtonText: 'OK',
            customClass: { confirmButton: 'swal-button-blue' }
        });
        return;
    }
    if (!soDT.startsWith('0')) {
        Swal.fire({
            icon: 'warning',
            title: 'Số điện thoại không hợp lệ',
            text: 'Số điện thoại phải bắt đầu bằng số 0.',
            confirmButtonText: 'OK',
            customClass: { confirmButton: 'swal-button-blue' }
        });
        return;
    }
    if (soDT.length < 9 || soDT.length > 10) {
        Swal.fire({
            icon: 'warning',
            title: 'Số điện thoại không hợp lệ',
            text: 'Số điện thoại phải có 9 hoặc 10 số.',
            confirmButtonText: 'OK',
            customClass: { confirmButton: 'swal-button-blue' }
        });
        return;
    }
    var formData = $(this).serialize();
    $.post('/QuanLyKhachHang/ThemNguoiDung', formData, function (res) {
        if (res.isSuccess) {
            $('#modalThemKhachHang').modal('hide');
            Swal.fire({
                icon: 'success',
                title: 'Thêm thành công!',
                timer: 1500,
                showConfirmButton: false
            }).then(() => location.reload());
        } else {
            Swal.fire({
                icon: 'error',
                title: 'Thêm thất bại!',
                text: res.message
            });
        }
    });
});
function openModalSuaKhachHang(maKH) {
    $.get('/QuanLyKhachHang/GetThongTinKhachHang', { maKhachHang: maKH }, function (data) {
        if (!data) {
            alert("Không lấy được dữ liệu.");
            return;
        }

        $('#editMaKhachHang').val(data.maKhachHang);
        $('#editTaiKhoan').val(data.taiKhoan);
        $('#editHoTen').val(data.hoTen);
        $('#editEmail').val(data.email);
        $('#editSoDT').val(data.soDT);
        $('#editGioiTinh').val(data.gioiTinh);

        loadCapEdit(data.maCap);

        $('#modalSuaKhachHang').modal('show');
    });
}

function loadCapEdit(selectedCap) {
    $.get('/QuanLyKhachHang/GetDanhSachCap', function (data) {
        const select = $('#editMaCap');
        select.empty();
        select.append('<option value="">Chọn cấp</option>');
        data.forEach(c => {
            const selected = c.maCap === selectedCap ? 'selected' : '';
            select.append(`<option value="${c.maCap}" ${selected}>${c.tenCap}</option>`);
        });
    });
}

$('#formSuaKhachHang').submit(async function (e) {
    e.preventDefault();
    const maKhachHang = $('#editMaKhachHang').val();
    const taiKhoan = $('#editTaiKhoan').val().trim();
    const hoTen = $('#editHoTen').val().trim();
    const email = $('#editEmail').val().trim();
    const soDT = $('#editSoDT').val().trim();
    const gioiTinh = $('#editGioiTinh').val();
    const maCap = $('#editMaCap').val();

    const regexHoTen = /^[^\d]+$/;
    const regexEmail = /^[^\s@@]+@@[^\s@@]+\.[^\s@@]+$/;
    const regexChiToanSo = /^\d+$/;
    const isTrung = await $.get('/QuanLyKhachHang/KiemTraTaiKhoanTrung', {
        taiKhoan,
        maKhachHang
    })
        .then(res => res.isTrung)
        .catch(() => false);

    if (!taiKhoan) {
        Swal.fire({
            icon: 'warning',
            title: 'Thiếu thông tin',
            text: 'Tài khoản không được để trống.',
            confirmButtonText: 'OK',
            customClass: { confirmButton: 'swal-button-blue' }
        });
        return;
    }
    if (isTrung) {
        Swal.fire({
            icon: 'error',
            title: 'Tài khoản đã tồn tại',
            text: `Tài khoản "${taiKhoan}" đã có. Vui lòng nhập tài khoản khác.`,
            confirmButtonText: 'OK',
            customClass: { confirmButton: 'swal-button-blue' }
        });
        return;
    }
    if (!hoTen) {
        Swal.fire({
            icon: 'warning',
            title: 'Thiếu thông tin',
            text: 'Họ tên không được để trống.',
            confirmButtonText: 'OK',
            customClass: { confirmButton: 'swal-button-blue' }
        });
        return;
    }
    if (!email) {
        Swal.fire({
            icon: 'warning',
            title: 'Thiếu thông tin',
            text: 'Email không được để trống.',
            confirmButtonText: 'OK',
            customClass: { confirmButton: 'swal-button-blue' }
        });
        return;
    }
    if (!soDT) {
        Swal.fire({
            icon: 'warning',
            title: 'Thiếu thông tin',
            text: 'Số điện thoại không được để trống.',
            confirmButtonText: 'OK',
            customClass: { confirmButton: 'swal-button-blue' }
        });
        return;
    }
    if (!gioiTinh) {
        Swal.fire({
            icon: 'warning',
            title: 'Thiếu thông tin',
            text: 'Phải chọn giới tính.',
            confirmButtonText: 'OK',
            customClass: { confirmButton: 'swal-button-blue' }
        });
        return;
    }
    if (!maCap) {
        Swal.fire({
            icon: 'warning',
            title: 'Thiếu thông tin',
            text: 'Phân cấp không đươc để trống.',
            confirmButtonText: 'OK',
            customClass: { confirmButton: 'swal-button-blue' }
        });
        return;
    }
    if (taiKhoan.length < 5) {
        Swal.fire({
            icon: 'warning',
            title: 'Tài khoản không hợp lệ',
            text: 'Tài khoản phải có ít nhất 5 ký tự.',
            confirmButtonText: 'OK',
            customClass: { confirmButton: 'swal-button-blue' }
        });
        return;
    }

    if (!regexHoTen.test(hoTen)) {
        Swal.fire({
            icon: 'warning',
            title: 'Họ tên không hợp lệ',
            text: 'Họ tên không được chứa số.',
            confirmButtonText: 'OK',
            customClass: { confirmButton: 'swal-button-blue' }
        });
        return;
    }

    if (!regexEmail.test(email)) {
        Swal.fire({
            icon: 'warning',
            title: 'Email không hợp lệ',
            text: 'Vui lòng nhập đúng định dạng email.',
            confirmButtonText: 'OK',
            customClass: { confirmButton: 'swal-button-blue' }
        });
        return;
    }

    if (!regexChiToanSo.test(soDT)) {
        Swal.fire({
            icon: 'warning',
            title: 'Số điện thoại không hợp lệ',
            text: 'Số điện thoại chỉ được chứa số (0–9).',
            confirmButtonText: 'OK',
            customClass: { confirmButton: 'swal-button-blue' }
        });
        return;
    }
    if (!soDT.startsWith('0')) {
        Swal.fire({
            icon: 'warning',
            title: 'Số điện thoại không hợp lệ',
            text: 'Số điện thoại phải bắt đầu bằng số 0.',
            confirmButtonText: 'OK',
            customClass: { confirmButton: 'swal-button-blue' }
        });
        return;
    }
    if (soDT.length < 9 || soDT.length > 10) {
        Swal.fire({
            icon: 'warning',
            title: 'Số điện thoại không hợp lệ',
            text: 'Số điện thoại phải có 9 hoặc 10 số.',
            confirmButtonText: 'OK',
            customClass: { confirmButton: 'swal-button-blue' }
        });
        return;
    }

    const formData = $(this).serialize();
    $.post('/QuanLyKhachHang/CapNhatNguoiDung', formData, function (res) {
        if (res.isSuccess) {
            $('#modalSuaKhachHang').modal('hide');
            Swal.fire({ icon: 'success', title: 'Cập nhật thành công!', timer: 1500, showConfirmButton: false })
                .then(() => location.reload());
        } else {
            Swal.fire({ icon: 'error', title: 'Lỗi cập nhật', text: res.message });
        }
    });
});
function xoaNguoiDung(maKhachHang, taiKhoan) {
    Swal.fire({
        title: 'Xác nhận xóa?',
        text: `Bạn có chắc muốn xóa người dùng "${taiKhoan}"?`,
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#d33',
        cancelButtonColor: '#aaa',
        confirmButtonText: 'Xóa',
        cancelButtonText: 'Hủy'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: '/QuanLyKhachHang/XoaNguoiDung',
                method: 'POST',
                data: { maKhachHang },
                success: function (res) {
                    if (res.isSuccess) {
                        Swal.fire({
                            icon: 'success',
                            title: 'Đã xóa thành công!',
                            timer: 1500,
                            showConfirmButton: false
                        }).then(() => location.reload());
                    } else {
                        Swal.fire({
                            icon: 'error',
                            title: 'Xóa thất bại!',
                            text: res.message
                        });
                    }
                },
                error: function () {
                    Swal.fire({
                        icon: 'error',
                        title: 'Lỗi server',
                        text: 'Không thể xóa người dùng.'
                    });
                }
            });
        }
    });
}