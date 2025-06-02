using Microsoft.AspNetCore.Mvc;

namespace doantn.Controllers
{
    public class TimKiemController : Controller
    {
        private readonly SachService _sachService;
        public TimKiemController(SachService sachService)
        {
            _sachService = sachService;
        }
        [HttpGet]
        public async Task<IActionResult> Index(string search)
        {
            ViewBag.Search = search;
            var ketQua = await _sachService.TimKiemSachAsync(search);
            return View(ketQua);
        }
    }
}
