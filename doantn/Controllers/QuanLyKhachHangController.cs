using doantn.Attributes;
using doantn.Models;
using doantn.Services;
using doantn.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace doantn.Controllers
{
    [AuthorizeCap("c4")]
    public class QuanLyKhachHangController : Controller
    {
        private readonly QuanLyKhachHangService _quanLyKhachHangService;

        public QuanLyKhachHangController(QuanLyKhachHangService quanLyKhachHangService)
        {
            _quanLyKhachHangService = quanLyKhachHangService;
        }

        public async Task<IActionResult> KhachHang()
        {
            var danhSach = await _quanLyKhachHangService.GetAllKhachHang();
            return View(danhSach);
        }
        [HttpPost]
        public async Task<IActionResult> ThemNguoiDung(KhachHangThemViewModel model)
        {
            var (isSuccess, message) = await _quanLyKhachHangService.ThemKhachHang(model);
            return Json(new { isSuccess, message });
        }

        [HttpGet]
        public async Task<IActionResult> GetDanhSachCap()
        {
            var list = await _quanLyKhachHangService.GetAllCap();
            return Json(list);
        }
        [HttpGet]
        public async Task<IActionResult> GetThongTinKhachHang(string maKhachHang)
        {
            var kh = await _quanLyKhachHangService.GetThongTinKhachHang(maKhachHang);
            return Json(kh);
        }

        [HttpPost]
        public async Task<IActionResult> CapNhatNguoiDung(KhachHang model)
        {
            var (isSuccess, message) = await _quanLyKhachHangService.CapNhatKhachHang(model);
            return Json(new { isSuccess, message });
        }
        [HttpGet]
        public async Task<IActionResult> KiemTraTaiKhoanTrung(string taiKhoan, string? maKhachHang = null)
        {
            var isTrung = await _quanLyKhachHangService.KiemTraTaiKhoanTrung(taiKhoan, maKhachHang);
            return Json(new { isTrung });
        }
        [HttpPost]
        public async Task<IActionResult> XoaNguoiDung(string maKhachHang)
        {
            var result = await _quanLyKhachHangService.XoaKhachHang(maKhachHang);
            return Json(new { isSuccess = result });
        }
    }
}
