using Microsoft.AspNetCore.Mvc;
using doantn.Services;
using doantn.ViewModel;
using doantn.Attributes;

namespace doantn.Controllers
{
    [AuthorizeCap("c4")]
    public class QuanLyController : Controller
    {
        private readonly QuanLyService _quanLyService;
        private readonly QuanLyLoaiService _quanLyLoaiService;
        private readonly ILoaiService _loaiService;

        public QuanLyController(QuanLyService quanLyService, QuanLyLoaiService quanLyLoaiService, ILoaiService loaiService)
        {
            _quanLyService = quanLyService;
            _quanLyLoaiService = quanLyLoaiService;
            _loaiService = loaiService;
        }

        public async Task<IActionResult> Sach()
        {
            var danhSachSach = await _quanLyService.GetDanhSachSachQuanLy();
            return View(danhSachSach);
        }
        [HttpGet]
        public async Task<IActionResult> GetLoaiSach()
        {
            var loaiList = await _loaiService.GetLoai();
            return Json(loaiList);
        }
        [HttpPost]
        public async Task<IActionResult> ThemSach([FromForm] SachThemMoiViewModel model)
        {        
            try
            {
                var success = await _quanLyService.ThemSach(model);
                if (success)
                    return Json(new { isSuccess = true, message = "Thêm thành công" });
                else
                    return Json(new { isSuccess = false, message = "Thêm sách thất bại (Service trả false)" });
            }
            catch (Exception ex)
            {
                return Json(new { isSuccess = false, message = "Lỗi server: " + ex.Message });
            }
        }
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var sach = await _quanLyService.GetThongTinSach(id);
            if (sach == null)
                return NotFound();

            return Json(sach);
        }
        [HttpPost]
        public async Task<IActionResult> CapNhatSach(string id, [FromForm] SachThemMoiViewModel model)
        {
            try
            {
                var success = await _quanLyService.CapNhatSach(model, id);
                return Json(new { isSuccess = success, message = success ? "Cập nhật thành công." : "Cập nhật thất bại." });
            }
            catch (Exception ex)
            {
                return Json(new { isSuccess = false, message = "Lỗi server: " + ex.Message });
            }
        }
        [HttpGet]
        public async Task<IActionResult> KiemTraTenSachTrung(string tenSach, string? maSach = null)
        {
            var isTrung = await _quanLyService.KiemTraTenSachTrung(tenSach, maSach);
            return Json(new { isTrung });
        }
        [HttpPost]
        public async Task<IActionResult> XoaSach(string id)
        {
            var success = await _quanLyService.XoaSachAsync(id);
            return Json(new { isSuccess = success, message = success ? "Đã xóa sách." : "Xóa thất bại." });
        }
        [HttpGet]
        public async Task<IActionResult> GetDanhSachCap()
        {
            var list = await _quanLyService.GetAllCap();
            return Json(list);
        }
    }
}
