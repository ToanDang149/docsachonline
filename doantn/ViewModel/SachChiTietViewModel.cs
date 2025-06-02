namespace doantn.ViewModel
{
    public class SachChiTietViewModel
    {
        public string MaSach { get; set; }
        public string TenSach { get; set; }
        public string TenTacGia { get; set; }
        public string? GioiThieu { get; set; }
        public string? TenLoai { get; set; }
        public int? LuotXem { get; set; }
        public int? LuotTai { get; set; }
        public string? Anh { get; set; }

        public List<TrangNoiDungVM> DanhSachTrang { get; set; }
        public List<ChuongNoiDungVM> ChuongNoiDung { get; set; }
    }

    public class TrangNoiDungVM
    {
        public string TenChuong { get; set; }
        public int SoTrang { get; set; }
        public string NoiDung { get; set; }
    }
    public class ChuongNoiDungVM
    {
        public string TenChuong { get; set; }
        public string NoiDung { get; set; }
    }
    public class AddBookmarkRequest
    {
        public string MaSach { get; set; }
        public string TenChuong { get; set; }
        public int SoTrang { get; set; }
    }
    public class BookMarkIndex
    {
        public string TenChuong { get; set; }
        public int SoTrang { get; set; }
    }
    public class DeleteBookmarkRequest
    {
        public string MaSach { get; set; }
        public int SoTrang { get; set; }
    }
    public class LuuTuSachRequest
    {
        public string MaSach { get; set; }
    }
}
