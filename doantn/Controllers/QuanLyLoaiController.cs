using doantn.Attributes;
using doantn.Services;
using Microsoft.AspNetCore.Mvc;

namespace doantn.Controllers
{
    [AuthorizeCap("c4")]
    public class QuanLyLoaiController : Controller
    {
        private readonly QuanLyLoaiService _quanLyLoaiService;

        public QuanLyLoaiController(QuanLyLoaiService quanLyLoaiService)
        {
            _quanLyLoaiService = quanLyLoaiService; 
        }

        public async Task<IActionResult> LoaiSach()
        {
            var danhSachLoai = await _quanLyLoaiService.GetAllLoaiSach();
            return View(danhSachLoai);
        }

        [HttpPost]
        public async Task<IActionResult> ThemLoaiSach(string tenLoai)
        {
            try
            {
                await _quanLyLoaiService.ThemLoaiSach(tenLoai);
                return Json(new { isSuccess = true });
            }
            catch (Exception ex)
            {
                return Json(new { isSuccess = false, message = ex.Message });
            }
        }
        [HttpPost]
        public async Task<IActionResult> XoaLoaiSach(string maLoai)
        {
            try
            {
                await _quanLyLoaiService.XoaLoaiSach(maLoai);
                return Json(new { isSuccess = true });
            }
            catch (Exception ex)
            {
                return Json(new { isSuccess = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CapNhatLoai(string maLoai, string tenLoai)
        {
            try
            {
                await _quanLyLoaiService.CapNhatLoai(maLoai, tenLoai);
                return Json(new { isSuccess = true });
            }
            catch (Exception ex)
            {
                return Json(new { isSuccess = false, message = ex.Message });
            }
        }

    }
}
