using System.ComponentModel.DataAnnotations;

namespace doantn.ViewModel
{
    public class DangNhapViewModel
    {
        [Required(ErrorMessage = "Tài khoản là bắt buộc.")]
        public string TaiKhoan { get; set; }

        [Required(ErrorMessage = "Mật khẩu là bắt buộc.")]
        [DataType(DataType.Password)]
        public string MatKhau { get; set; }
    }
}
