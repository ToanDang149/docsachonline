using doantn.Models;
using doantn.Services;
using Microsoft.AspNetCore.Mvc;

namespace doantn.Controllers
{
    public class BookMarkCuonController : Controller
    {
        private readonly BookMarkCuonService _bookmarkService;
        public BookMarkCuonController(BookMarkCuonService bookmarkService)
        {
            _bookmarkService = bookmarkService;
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] BookMarkCuon model)
        {
            if (string.IsNullOrEmpty(model.MaKhachHang))
                return Json(new { success = false });

            var result = await _bookmarkService.AddBookmark(model);
            return Json(new { success = result });
        }

        [HttpGet]
        public async Task<IActionResult> Get(string maKhachHang, string maSach)
        {
            var data = await _bookmarkService.GetBookmarks(maKhachHang, maSach);
            return Json(data);
        }
        [HttpDelete]
        public async Task<IActionResult> Delete(string maKhachHang, string maSach, int soDong)
        {
            var success = await _bookmarkService.XoaBookmark(maKhachHang, maSach, soDong);
            return Json(new { success });
        }
        [HttpGet]
        public async Task<IActionResult> GetLast(string maKhachHang, string maSach)
        {
            var list = await _bookmarkService.GetBookmarks(maKhachHang, maSach);
            var last = list.LastOrDefault();
            return Json(last);
        }
    }
}
