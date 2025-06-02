namespace doantn.ViewModel
{
    public class SachQuanLyViewModel
    {
        public string MaSach { get; set; }
        public string TenSach { get; set; }
        public string TenTacGia { get; set; }
        public string TenLoai { get; set; }
        public string Anh { get; set; }
        public int SoChuong { get; set; }
    }
    public class SachThemMoiViewModel
    {
        public string TenSach { get; set; }
        public string TenTacGia { get; set; }
        public string? Nguon { get; set; }
        public string GioiThieu { get; set; }
        public string MaLoai { get; set; } 

        public IFormFile AnhBia { get; set; }
        public string Anh { get; set; }
        public List<ChuongViewModel> Chuongs { get; set; } = new List<ChuongViewModel>();
        public List<IFormFile>? FilesSach { get; set; } = new List<IFormFile>();
        public int LuotXem { get; set; } = 0;
        public int LuotTai { get; set; } = 0;
        public List<FileSachViewModel> FilesDaTai { get; set; } = new List<FileSachViewModel>();
        public string? DanhSachFileXoa { get; set; }
        public string? MaCap {  get; set; }
    }
    public class ChuongViewModel
    {
        public string TenChuong { get; set; }
        public string NoiDung { get; set; }
    }
    public class FileSachViewModel
    {
        public string MaFile { get; set; }
        public string TenFile { get; set; }
        public string LoaiFile { get; set; }
        public string DuongDan { get; set; }
    }
    public class CapDoViewModel
    {
        public string MaCap { get; set; }
        public string TenCap { get; set; }
    }
}
