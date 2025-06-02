using System.ComponentModel.DataAnnotations;

namespace doantn.ViewModel
{
    public class DangKyViewModel
    {
        [Required(ErrorMessage = "Tài khoản là bắt buộc.")]
        public string TaiKhoan { get; set; }
        [Required(ErrorMessage = "Mật khẩu là bắt buộc.")]
        public string MatKhau { get; set; }
        [Required(ErrorMessage = "Xác nhận mật khẩu là bắt buộc.")]
        [Compare("MatKhau", ErrorMessage = "Mật khẩu xác nhận không khớp.")]
        public string XacNhanMatKhau { get; set; }
        [Required(ErrorMessage = "Họ tên là bắt buộc.")]
        public string HoTen { get; set; }
        [Required(ErrorMessage = "Email là bắt buộc.")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Số điện thoại là bắt buộc.")]
        public string SoDT { get; set; }
        [Required(ErrorMessage = "Giới tính là bắt buộc.")]
        public int GioiTinh { get; set; } 
    }
}
