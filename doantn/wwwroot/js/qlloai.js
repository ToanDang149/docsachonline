$(document).ready(function () {
    $('.table').DataTable({
        lengthChange: false,
        searching: true,
        info: false,
        paging: true,
        pageLength: 10,
        language: {
            search: "Tìm kiếm:",
            searchPlaceholder: "Nhập tên loại sách...",
            paginate: {
                previous: "<i class='fa-solid fa-chevron-left'></i>",
                next: "<i class='fa-solid fa-chevron-right'></i>"
            },
            zeroRecords: "Không tìm thấy loại sách phù hợp",
        }
    });

    $(".dataTables_filter").append(`
                <button type="button" class="btn-add-new" onclick="openModalThemLoai()">
                    <i class="fa-solid fa-plus"></i> Thêm mới
                </button>
            `);
});
function openModalThemLoai() {
    $('#tenLoaiMoi').val('');
    $('#modalThemLoai').modal('show');
}

$('#formThemLoai').submit(function (e) {
    e.preventDefault();
    const tenLoai = $('#tenLoaiMoi').val().trim();
    if (!tenLoai) {
        Swal.fire({
            icon: 'warning',
            title: 'Thiếu thông tin',
            text: 'Tên loại sách không được để trống.'
        });
        return;
    }

    $.ajax({
        url: '/QuanLyLoai/ThemLoaiSach',
        method: 'POST',
        data: { tenLoai },
        success: function (res) {
            if (res.isSuccess) {
                $('#modalThemLoai').modal('hide');
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
        },
        error: function () {
            Swal.fire({
                icon: 'error',
                title: 'Lỗi server',
                text: 'Không thể thêm loại sách.'
            });
        }
    });
});
function openModalSuaLoai(maLoai, tenLoai) {
    $('#maLoaiSua').val(maLoai);
    $('#tenLoaiSua').val(tenLoai);
    $('#modalSuaLoai').modal('show');
}

$('#formSuaLoai').submit(function (e) {
    e.preventDefault();

    const maLoai = $('#maLoaiSua').val();
    const tenLoai = $('#tenLoaiSua').val().trim();

    if (!tenLoai) {
        Swal.fire({
            icon: 'warning',
            title: 'Thiếu thông tin',
            text: 'Tên loại sách không được để trống.'
        });
        return;
    }

    $.ajax({
        url: '/QuanLyLoai/CapNhatLoai',
        method: 'POST',
        data: { maLoai, tenLoai },
        success: function (res) {
            if (res.isSuccess) {
                $('#modalSuaLoai').modal('hide');
                Swal.fire({
                    icon: 'success',
                    title: 'Cập nhật thành công!',
                    timer: 1500,
                    showConfirmButton: false
                }).then(() => location.reload());
            } else {
                Swal.fire({
                    icon: 'error',
                    title: 'Cập nhật thất bại!',
                    text: res.message
                });
            }
        },
        error: function () {
            Swal.fire({
                icon: 'error',
                title: 'Lỗi server',
                text: 'Không thể cập nhật loại sách.'
            });
        }
    });
});
function xoaLoaiSach(maLoai, tenLoai) {
    Swal.fire({
        title: 'Xác nhận xóa?',
        text: `Bạn có chắc chắn muốn xóa loại sách "${tenLoai}"?`,
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#d33',
        cancelButtonColor: '#aaa',
        confirmButtonText: 'Xóa',
        cancelButtonText: 'Hủy'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: '/QuanLyLoai/XoaLoaiSach',
                method: 'POST',
                data: { maLoai },
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
                        text: 'Không thể xóa loại sách.'
                    });
                }
            });
        }
    });
}