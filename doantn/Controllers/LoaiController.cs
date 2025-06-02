using Microsoft.AspNetCore.Mvc;

namespace doantn.Controllers
{
    public class LoaiController : Controller
    {
        private readonly ILoaiService _loaiService;

        public LoaiController(ILoaiService loaiService)
        {
            _loaiService = loaiService;
        }

        [HttpGet]
        public async Task<IActionResult> GetLoai()
        {
            var loaiList = await _loaiService.GetLoai();
            return Json(loaiList);
        }
    }
}
