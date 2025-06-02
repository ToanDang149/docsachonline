document.querySelector("form").addEventListener("submit", function (e) {
    var input = document.querySelector("input[name='search']").value.trim();
    if (!input) {
        e.preventDefault();
    }
});
const navbar = document.querySelector('.navbar-bottom');
const navbarOffsetTop = navbar.offsetTop;

window.addEventListener('scroll', function () {
    if (window.scrollY >= navbarOffsetTop + navbar.offsetHeight) {
        navbar.classList.add('sticky-show');
    } else {
        navbar.classList.remove('sticky-show');
    }
});
document.addEventListener('DOMContentLoaded', function () {
    fetch('/Loai/GetLoai')
        .then(response => response.json())
        .then(data => {
            const dropdown = document.getElementById('dropdownLibrary');
            dropdown.innerHTML = '';
            data.forEach(loai => {
                const li = document.createElement('li');
                li.innerHTML = `<a href="/Sach/DanhSach?maLoai=${loai.maLoai}">${loai.tenLoai}</a>`;
                dropdown.appendChild(li);
            });
        })
        .catch(error => console.error('Lỗi lấy mã loại:', error));
});        