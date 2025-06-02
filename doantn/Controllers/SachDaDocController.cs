using doantn.Services;
using Microsoft.AspNetCore.Mvc;

namespace doantn.Controllers
{
    public class SachDaDocController : Controller
    {
        private readonly KhachHangService _khachHangService;
        public SachDaDocController(KhachHangService khachHangService)
        {
            _khachHangService = khachHangService;
        }
        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("MaKhachHang") == null)
            {
                return RedirectToAction("DangNhap", "TaiKhoan");
            }

            var maKhachHang = HttpContext.Session.GetString("MaKhachHang");
            var danhSachDaDoc = await _khachHangService.GetSachDaDocAsync(maKhachHang);

            return View(danhSachDaDoc);
        }
    }
}
