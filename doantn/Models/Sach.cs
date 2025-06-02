namespace doantn.Models
{
    public class Sach
    {
        public string MaSach { get; set; }
        public string TenSach { get; set; }
        public string TenTacGia { get; set; }
        public string? GioiThieu { get; set; }
        public string TenLoai { get; set; }
        public int? LuotXem {  get; set; }
        public int? LuotTai { get; set; }
        public string? Anh { get; set; }
        public string? Nguon {  get; set; }
        public int? DanhGia { get; set; }
        public string MaLoai { get; set; }
        public DateTime ThoiGianCatNhap {  get; set; }
    }
    public class Loai
    {
        public string MaLoai { get; set; }
        public string TenLoai { get; set; }
    }
}
