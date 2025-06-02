$(document).ready(function () {
    $('.table').DataTable({
        lengthChange: false,
        searching: true,
        info: false,
        paging: true,
        pageLength: 10,
        language: {
            search: "Tìm kiếm:",
            searchPlaceholder: "Nhập từ khóa...",
            paginate: {
                previous: "<i class='fa-solid fa-chevron-left'></i>",
                next: "<i class='fa-solid fa-chevron-right'></i>"
            },
            zeroRecords: "Không tìm thấy sách",
        }
    });
    $(".dataTables_filter").append(`
                    <button type="button" class="btn-add-new" onclick="openModalThemSach()">
                    <i class="fa-solid fa-plus"></i> Thêm mới
                </button>
            `);
});
function addChuong() {
    const container = document.getElementById('chuong-container');
    const index = container.children.length;

    const chuongDiv = document.createElement('div');
    chuongDiv.classList.add('border', 'p-3', 'mb-3', 'position-relative');
    chuongDiv.innerHTML = `
                <button type="button" class="btn btn-danger btn-sm position-absolute" style="top: 8px; right: 8px; z-index: 2;" onclick="this.parentElement.remove()">
                    <i class="fa-solid fa-minus"></i>
                </button>

                <div style="margin-top: 30px;"> 
                    <input name="Chuongs[${index}].TenChuong" class="form-control mb-2" placeholder="Tên chương" required />
                    <textarea name="Chuongs[${index}].NoiDung" class="form-control" placeholder="Nội dung chương" style="min-height: 400px;" required></textarea>
                </div>
            `;

    container.appendChild(chuongDiv);
}
let filesSelected = [];

$('#uploadFilesSach').on('change', function () {
    const files = Array.from(this.files);

    files.forEach(file => {
        const extMoi = file.name.split('.').pop().toLowerCase();
        const indexTrung = filesSelected.findIndex(f =>
            f.name.split('.').pop().toLowerCase() === extMoi
        );
        if (indexTrung !== -1) {
            filesSelected.splice(indexTrung, 1);
        }
        filesSelected.push(file);
    });
    renderFileSachList();
    updateInputFiles();
});


function renderFileSachList() {
    const list = $('#fileSachList');
    list.empty();

    filesSelected.forEach((file, index) => {
        const listItem = `
                    <li class="list-group-item d-flex justify-content-between align-items-center">
                        ${file.name}
                        <button type="button" class="btn btn-danger btn-sm" onclick="removeFileSach(${index})">
                            <i class="fa fa-trash"></i>
                        </button>
                    </li>
                `;
        list.append(listItem);
    });
    updateInputFiles();
}

function removeFileSach(index) {
    filesSelected.splice(index, 1);
    renderFileSachList();
}
function updateInputFiles() {
    const input = document.getElementById('uploadFilesSach');
    const dataTransfer = new DataTransfer();

    filesSelected.forEach(file => {
        dataTransfer.items.add(file);
    });

    input.files = dataTransfer.files;
}

$('#uploadAnhBiaEdit').on('change', function () {
    const file = this.files[0];
    if (file) {
        const reader = new FileReader();
        reader.onload = function (e) {
            $('#previewEditAnhBia').attr('src', e.target.result).show();
        };
        reader.readAsDataURL(file);
    }
});

function openModalThemSach() {
    $('#formThemSach')[0].reset();
    $('#chuong-container').empty();
    $('#previewAnhBiaThem').attr('src', '').hide();
    loadLoaiSach();
    loadCapSach();
    $('#modalThemSach').modal('show');
    $('input[name="AnhBia"]').val('');
    $('input[name="FilesSach"]').val('');

}
function loadCapSach() {
    $.ajax({
        url: '/QuanLy/GetDanhSachCap',
        method: 'GET',
        success: function (data) {
            const select = $('#selectCapSach');
            select.find('option:not([disabled])').remove();

            data.forEach(cap => {
                select.append(`<option value="${cap.maCap}">${cap.tenCap}</option>`);
            });
        },
        error: function () {
            alert('Không tải được danh sách cấp độ.');
        }
    });
}
function loadLoaiSach() {
    $.ajax({
        url: '/QuanLy/GetLoaiSach',
        method: 'GET',
        success: function (data) {
            var select = $('#selectLoaiSach');
            select.find('option:not([disabled])').remove();

            data.forEach(function (item) {
                select.append(`<option value="${item.maLoai}">${item.tenLoai}</option>`);
            });
        },
        error: function () {
            alert('Không tải được danh sách loại sách.');
        }
    });
}
function formatTextBeforeSave(text) {
    if (!text) return "";

    text = text.trim();

    const paragraphs = text.split(/\n\s*\n/);

    return paragraphs.map(p => `<p>${p.trim().replace(/\n/g, '<br>')}</p>`).join('');
}

$(document).on('paste', '#chuong-container textarea', function (e) {
    e.preventDefault();
    var text = (e.originalEvent || e).clipboardData.getData('text/plain');
    text = text.replace(/\r/g, '');
    text = text.split('\n').map(line => line.trim()).join('\n');
    insertTextAtCursor(this, text);
});

function insertTextAtCursor(input, text) {
    const start = input.selectionStart;
    const end = input.selectionEnd;
    const value = input.value;

    input.value = value.substring(0, start) + text + value.substring(end);
    input.selectionStart = input.selectionEnd = start + text.length;
}
$('#formThemSach').submit(async function (e) {
    e.preventDefault();
    const tenSach = $('input[name="TenSach"]').val().trim();
    const tenTacGia = $('input[name="TenTacGia"]').val().trim();
    const gioiThieu = $('textarea[name="GioiThieu"]').val().trim();
    const maLoai = $('#selectLoaiSach').val();
    const anhBia = $('#uploadAnhBiaThem')[0].files[0];
    const soChuong = $('#chuong-container .border').length;
    const soFile = filesSelected.length;
    const maCap = $('#selectCapSach').val();
    if (!maCap) {
        Swal.fire({
            icon: 'warning',
            title: 'Thiếu thông tin',
            text: 'Phân cấp không được để trống.',
            confirmButtonText: 'OK',
            customClass: { confirmButton: 'swal-button-blue' }
        });
        return;
    }
    if (!tenSach) {
        Swal.fire({
            icon: 'warning',
            title: 'Thiếu thông tin',
            text: 'Tên sách không được để trống.',
            confirmButtonText: 'OK',
            customClass: { confirmButton: 'swal-button-blue' }
        });
        return;
    }
    if (!tenTacGia) {
        Swal.fire({
            icon: 'warning',
            title: 'Thiếu thông tin',
            text: 'Tên tác giả không được để trống.',
            confirmButtonText: 'OK',
            customClass: { confirmButton: 'swal-button-blue' }
        });
        return;
    }
    if (!gioiThieu) {
        Swal.fire({
            icon: 'warning',
            title: 'Thiếu thông tin',
            text: 'Giới thiệu không được để trống.',
            confirmButtonText: 'OK',
            customClass: { confirmButton: 'swal-button-blue' }
        });
        return;
    }
    if (!maLoai) {
        Swal.fire({
            icon: 'warning',
            title: 'Thiếu thông tin',
            text: 'Loại sách không được để trống.',
            confirmButtonText: 'OK',
            customClass: { confirmButton: 'swal-button-blue' }
        });
        return;
    }
    if (!anhBia) {
        Swal.fire({
            icon: 'warning',
            title: 'Thiếu thông tin',
            text: 'Sách phải có ảnh bìa.',
            confirmButtonText: 'OK',
            customClass: { confirmButton: 'swal-button-blue' }
        });
        return;
    }
    if (soChuong === 0) {
        Swal.fire({
            icon: 'warning',
            title: 'Thiếu thông tin',
            text: 'Phải có ít nhất một chương sách.',
            confirmButtonText: 'OK',
            customClass: { confirmButton: 'swal-button-blue' }
        });
        return;
    }
    let loiChuong = false;
    $('#chuong-container textarea').each(function () {
        if (!$(this).val().trim()) loiChuong = true;
    });
    if (loiChuong) {
        Swal.fire({
            icon: 'warning',
            title: 'Thiếu nội dung chương',
            text: 'Mỗi chương phải cần có nội dung.',
            confirmButtonText: 'OK',
            customClass: { confirmButton: 'swal-button-blue' }
        });
        return;
    }
    const isTrung = await $.get('/QuanLy/KiemTraTenSachTrung', { tenSach })
        .then(res => res.isTrung)
        .catch(() => false);
    if (isTrung) {
        Swal.fire({
            icon: 'error',
            title: 'Tên sách đã tồn tại',
            text: `Tên sách "${tenSach}" đã có. Vui lòng nhập tên khác.`,
            confirmButtonText: 'OK',
            customClass: {
                confirmButton: 'swal-button-blue'
            }
        });
        return;
    }
    $('#chuong-container textarea').each(function () {
        const content = $(this).val();
        const newContent = formatTextBeforeSave(content);
        $(this).val(newContent);
    });

    var formData = new FormData(this);

    $.ajax({
        url: '/QuanLy/ThemSach',
        method: 'POST',
        data: formData,
        processData: false,
        contentType: false,
        success: function (response) {
            if (response.isSuccess) {
                $('#modalThemSach').modal('hide');
                Swal.fire({
                    icon: 'success',
                    title: 'Thêm sách thành công!',
                    showConfirmButton: false,
                    timer: 2000
                }).then(() => {
                    location.reload();
                });
            } else {
                alert('Thêm sách thất bại: ' + response.message);
            }
        },
        error: function () {
            alert('Có lỗi xảy ra.');
        }
    });
});
$('#uploadAnhBiaThem').on('change', function () {
    const file = this.files[0];
    if (file) {
        const reader = new FileReader();
        reader.onload = function (e) {
            $('#previewAnhBiaThem').attr('src', e.target.result).show();
        };
        reader.readAsDataURL(file);
    }
});

$('#formEditSach').submit(async function (e) {
    e.preventDefault();
    const tenSach = $('#editTenSach').val().trim();
    const tenTacGia = $('#editTenTacGia').val().trim();
    const gioiThieu = $('#editGioiThieu').val().trim();
    const maLoai = $('#editMaLoai').val();
    const anhHienTai = $('#editAnhHienTai').val();
    const anhMoi = $('#uploadAnhBiaEdit')[0].files[0];
    const soChuong = $('#edit-chuong-container .border').length;
    const maCap = $('#editMaCap').val();
    if (!maCap) {
        Swal.fire({
            icon: 'warning',
            title: 'Thiếu thông tin',
            text: 'Phân cấp không được để trống.',
            confirmButtonText: 'OK',
            customClass: { confirmButton: 'swal-button-blue' }
        });
        return;
    }
    if (!tenSach) {
        Swal.fire({
            icon: 'warning',
            title: 'Thiếu thông tin',
            text: 'Tên sách không được để trống.',
            confirmButtonText: 'OK',
            customClass: { confirmButton: 'swal-button-blue' }
        });
        return;
    }
    if (!tenTacGia) {
        Swal.fire({
            icon: 'warning',
            title: 'Thiếu thông tin',
            text: 'Tên tác giả không được để trống.',
            confirmButtonText: 'OK',
            customClass: { confirmButton: 'swal-button-blue' }
        });
        return;
    }
    if (!gioiThieu) {
        Swal.fire({
            icon: 'warning',
            title: 'Thiếu thông tin',
            text: 'Giới thiệu không được để trống.',
            confirmButtonText: 'OK',
            customClass: { confirmButton: 'swal-button-blue' }
        });
        return;
    }
    if (!maLoai) {
        Swal.fire({
            icon: 'warning',
            title: 'Thiếu thông tin',
            text: 'Loại sách không được để trống.',
            confirmButtonText: 'OK',
            customClass: { confirmButton: 'swal-button-blue' }
        });
        return;
    }
    if (!anhMoi && !anhHienTai) {
        Swal.fire({
            icon: 'warning',
            title: 'Thiếu thông tin',
            text: 'Sách phải có ảnh bìa.',
            confirmButtonText: 'OK',
            customClass: { confirmButton: 'swal-button-blue' }
        });
        return;
    }
    if (soChuong === 0) {
        Swal.fire({
            icon: 'warning',
            title: 'Thiếu thông tin',
            text: 'Phải có ít nhất một chương sách.',
            confirmButtonText: 'OK',
            customClass: { confirmButton: 'swal-button-blue' }
        });
        return;
    }
    let loiChuong = false;
    $('#edit-chuong-container textarea').each(function () {
        if (!$(this).val().trim()) loiChuong = true;
    });
    if (loiChuong) {
        Swal.fire({
            icon: 'warning',
            title: 'Thiếu nội dung chương',
            text: 'Mỗi chương cần phải có nội dung.',
            confirmButtonText: 'OK',
            customClass: { confirmButton: 'swal-button-blue' }
        });
        return;
    }
    $('#edit-chuong-container textarea').each(function () {
        const content = $(this).val();
        const newContent = formatTextBeforeSave(content);
        $(this).val(newContent);
    });
    var formData = new FormData(this);
    var maSach = $('#editMaSach').val();
    formData.append("DanhSachFileXoa", fileCuBiXoa.join(','));
    for (var pair of formData.entries()) {
        console.log(pair[0] + ':', pair[1]);
    }
    $.ajax({
        url: '/QuanLy/CapNhatSach?id=' + maSach,
        method: 'POST',
        data: formData,
        processData: false,
        contentType: false,
        success: function (response) {
            if (response.isSuccess) {
                $('#modalEditSach').modal('hide');
                Swal.fire({
                    icon: 'success',
                    title: 'Cập nhật thành công!',
                    showConfirmButton: false,
                    timer: 2000
                }).then(() => {
                    location.reload();
                });
            } else {
                alert('Cập nhật thất bại: ' + response.message);
            }
        },
        error: function () {
            alert('Có lỗi xảy ra.');
        }
    });
});
function addEditChuong() {
    const container = document.getElementById('edit-chuong-container');
    const index = container.children.length;

    const chuongDiv = document.createElement('div');
    chuongDiv.classList.add('border', 'p-3', 'mb-3', 'position-relative');
    chuongDiv.innerHTML = `
                <button type="button" class="btn btn-danger btn-sm position-absolute" style="top: 8px; right: 8px;" onclick="this.parentElement.remove()">
                    <i class="fa-solid fa-minus"></i>
                </button>

                <div style="margin-top: 30px;">
                    <input name="Chuongs[${index}].TenChuong" class="form-control mb-2" placeholder="Tên chương" required />
                    <textarea name="Chuongs[${index}].NoiDung" class="form-control" placeholder="Nội dung chương" style="min-height: 400px;" required></textarea>
                </div>
            `;
    container.appendChild(chuongDiv);
}

function openModalEditSach(maSach) {
    $.ajax({
        url: '/QuanLy/Edit?id=' + maSach,
        method: 'GET',
        success: function (data) {
            console.log("DATA TRẢ VỀ:", data);
            $('#formEditSach')[0].reset();
            $('#editMaSach').val(maSach);
            $('#editTenSach').val(data.tenSach);
            $('#editTenTacGia').val(data.tenTacGia);
            $('#editNguon').val(data.nguon);
            $('#editGioiThieu').val(data.gioiThieu);
            $('#editAnhHienTai').val(data.anh || '');
            if (data.anh) {
                $('#previewEditAnhBia').attr('src', data.anh).show();
                $('#editAnhHienTai').val(data.anh);
            } else {
                $('#previewEditAnhBia').hide();
                $('#editAnhHienTai').val('');
            }
            loadLoaiSachForEdit(data.maLoai);
            loadCapSachForEdit(data.maCap);
            $('#edit-chuong-container').empty();
            if (data.chuongs && data.chuongs.length > 0) {
                data.chuongs.forEach(function (chuong, index) {
                    const noiDungGoc = chuong.noiDung
                        .replace(/<\/p>/gi, '\n\n')
                        .replace(/<br\s*\/?>/gi, '\n')
                        .replace(/<[^>]*>/g, '')
                    const chuongDiv = `
                                <div class="border p-3 mb-3 position-relative">
                                    <button type="button" class="btn btn-danger btn-sm position-absolute" style="top: 8px; right: 8px;" onclick="this.parentElement.remove()">
                                        <i class="fa-solid fa-minus"></i>
                                    </button>
                                    <div style="margin-top: 30px;">
                                        <input name="Chuongs[${index}].TenChuong" class="form-control mb-2" placeholder="Tên chương" value="${chuong.tenChuong}" required />
                                        <textarea name="Chuongs[${index}].NoiDung" class="form-control" style="min-height:400px;" required>${noiDungGoc}</textarea>
                                    </div>
                                </div>
                            `;
                    $('#edit-chuong-container').append(chuongDiv);
                });
            }

            $('#fileListAll').empty();
            fileCuBiXoa = [];

            if (data.filesDaTai && data.filesDaTai.length > 0) {
                data.filesDaTai.forEach(function (file) {
                    const fileItem = `
                                <li class="list-group-item d-flex justify-content-between align-items-center">
                                    ${file.tenFile}.${file.loaiFile}
                                    <button type="button" class="btn btn-sm btn-danger" onclick="xoaFileCu('${file.maFile}', this)">
                                        <i class="fa fa-trash"></i>
                                    </button>
                                </li>
                            `;
                    $('#fileListAll').append(fileItem);
                });
            }

            editFilesSelected = [];
            renderFileMoi();
            $('#uploadFilesSachEdit').val(null);
            $('#modalEditSach').modal('show');
        },
        error: function () {
            alert('Không lấy được thông tin sách.');
        }
    });
}
function loadCapSachForEdit(selectedMaCap) {
    $.ajax({
        url: '/QuanLy/GetDanhSachCap',
        method: 'GET',
        success: function (data) {
            const select = $('#editMaCap');
            select.empty();
            select.append('<option value="" disabled>Chọn cấp độ</option>');

            data.forEach(cap => {
                const option = `<option value="${cap.maCap}">${cap.tenCap}</option>`;
                select.append(option);
            });

            if (selectedMaCap) {
                select.val(selectedMaCap);
            }
        },
        error: function () {
            alert('Không tải được danh sách cấp độ.');
        }
    });
}
function loadLoaiSachForEdit(selectedMaLoai) {
    $.ajax({
        url: '/QuanLy/GetLoaiSach',
        method: 'GET',
        success: function (data) {
            var select = $('#editMaLoai');
            select.empty();
            select.append('<option value="" disabled>Chọn loại sách</option>');

            data.forEach(function (item) {
                var option = `<option value="${item.maLoai}">${item.tenLoai}</option>`;
                select.append(option);
            });

            if (selectedMaLoai) {
                select.val(selectedMaLoai);
            }
        },
        error: function () {
            alert('Không tải được danh sách loại sách.');
        }
    });
}
let editFilesSelected = [];
let fileCuBiXoa = [];
function xoaFileCu(maFile, button) {
    fileCuBiXoa.push(maFile);
    $('#editDanhSachFileXoa').val(fileCuBiXoa.join(','));
    $(button).closest('li').remove();
}

$('#uploadFilesSachEdit').on('change', function () {
    const files = Array.from(this.files);

    files.forEach(file => {
        const extMoi = file.name.split('.').pop().toLowerCase();
        const daCoTrongMoi = editFilesSelected.some(f =>
            f.name.split('.').pop().toLowerCase() === extMoi
        );
        const daCoTrongCu = $('#fileListAll li').toArray().some(li => {
            const fileName = $(li).contents().first().text().trim();
            const ext = fileName.split('.').pop().toLowerCase();
            return ext === extMoi;
        });

        if (daCoTrongMoi || daCoTrongCu) {
            Swal.fire({
                toast: true,
                position: 'top-end',
                icon: 'error',
                title: `Chỉ tồn tại 1 file .${extMoi} trong danh sách!`,
                showConfirmButton: false,
                timer: 3000,
            });
            return;
        }
        editFilesSelected.push(file);
    });

    renderFileMoi();
    updateUploadInput();
});
function renderFileMoi() {
    $('#fileListAll .file-new').remove();

    editFilesSelected.forEach((file, index) => {
        const li = `
                    <li class="list-group-item d-flex justify-content-between align-items-center file-new">
                        ${file.name}
                        <button type="button" class="btn btn-danger btn-sm" onclick="removeFileMoi(${index})">
                            <i class="fa fa-trash"></i>
                        </button>
                    </li>
                `;
        $('#fileListAll').append(li);
    });
}

function removeFileMoi(index) {
    editFilesSelected.splice(index, 1);
    renderFileMoi();
    updateUploadInput();
}

function updateUploadInput() {
    const input = document.getElementById('uploadFilesSachEdit');
    const dt = new DataTransfer();

    editFilesSelected.forEach(file => dt.items.add(file));
    input.files = dt.files;
}
function xoaSach(maSach, tenSach) {
    Swal.fire({
        title: 'Xác nhận xóa?',
        text: `Bạn có chắc muốn xóa sách "${tenSach}"?`,
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#d33',
        cancelButtonColor: '#aaa',
        confirmButtonText: 'Xoá',
        cancelButtonText: 'Huỷ'
    }).then((result) => {
        if (result.isConfirmed) {
            $.post('/QuanLy/XoaSach', { id: maSach }, function (res) {
                if (res.isSuccess) {
                    Swal.fire({
                        icon: 'success',
                        title: 'Đã xoá thành công!',
                        timer: 1500,
                        showConfirmButton: false
                    }).then(() => location.reload());
                } else {
                    Swal.fire({
                        icon: 'error',
                        title: 'Xoá thất bại',
                        text: res.message
                    });
                }
            });
        }
    });
}