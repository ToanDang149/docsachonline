using doantn.Services;
using doantn.ViewModel;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace doantn.Controllers
{
    public class BookMarkController : Controller
    {
        private readonly IBookMarkService _bookmarkService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public BookMarkController(IBookMarkService bookmarkService, IHttpContextAccessor httpContextAccessor)
        {
            _bookmarkService = bookmarkService;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        public async Task<IActionResult> GetBookmarks(string maSach)
        {
            var userId = HttpContext.Session.GetString("MaKhachHang");
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var bookmarks = await _bookmarkService.GetBookmarks(userId, maSach);
            return Json(bookmarks);
        }

        [HttpPost]
        public async Task<IActionResult> AddBookmark([FromBody] AddBookmarkRequest model)
        {
            var userId = HttpContext.Session.GetString("MaKhachHang");
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            await _bookmarkService.AddBookmark(userId, model.MaSach, model.TenChuong, model.SoTrang);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteBookmark([FromBody] DeleteBookmarkRequest model)
        {
            var userId = HttpContext.Session.GetString("MaKhachHang");
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            await _bookmarkService.DeleteBookmark(userId, model.MaSach, model.SoTrang);
            return Ok();
        }
    }
}
