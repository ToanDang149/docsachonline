using doantn.Services;
using doantn.ViewModel;
using Microsoft.AspNetCore.Mvc;

public class HomeController : Controller
{
    private readonly SachService _sachService;
    private readonly KhachHangService _khachHangService;
    private readonly TuSachService _tuSachService;
    private readonly DanhGiaService _danhGiaService;

    public HomeController(SachService sachService, KhachHangService khachHangService, TuSachService tuSachService, DanhGiaService danhGiaService)
    {
        _sachService = sachService;
        _khachHangService = khachHangService;
        _tuSachService = tuSachService;
        _danhGiaService = danhGiaService;
    }

    public IActionResult Index()
    {
        var model = new TrangChuViewModel
        {
            EbookHot = _sachService.GetSachHot(),
            EbookTaiNhieu = _sachService.GetSachTaiNhieu(),
            EbookMoi = _sachService.GetSachMoi()
        };

        return View(model);
    }
    [HttpGet]
    public async Task<IActionResult> GetSachChiTiet(string maSach)
    {
        var sach = await _sachService.GetChiTiet(maSach);
        if (sach == null)
            return NotFound();

        var danhGias = await _danhGiaService.GetDanhGiaBySach(maSach);

        double trungBinhSao = 0;
        int soLuongDanhGia = 0;

        if (danhGias != null && danhGias.Any())
        {
            trungBinhSao = Math.Round(danhGias.Average(dg => dg.SoSao ?? 0), 1);
            soLuongDanhGia = danhGias.Count();
        }

        return Json(new
        {
            tenSach = sach.TenSach,
            tenTacGia = sach.TenTacGia,
            luotXem = sach.LuotXem,
            luotTai = sach.LuotTai,
            gioiThieu = sach.GioiThieu,
            anh = sach.Anh,
            trungBinhSao = trungBinhSao,
            soLuongDanhGia = soLuongDanhGia
        });
    }

}