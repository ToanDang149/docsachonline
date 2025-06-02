using System.ComponentModel.DataAnnotations;

namespace doantn.Models
{
    public class KhachHang
    {
        public string MaKhachHang { get; set; }
        public string TaiKhoan { get; set; }
        public string MatKhau { get; set; }
        public string HoTen { get; set; }
        public string Email { get; set; }
        public string? SoDT { get; set; }
        public int GioiTinh { get; set; }
        public string MaCap {  get; set; }
    }
}
