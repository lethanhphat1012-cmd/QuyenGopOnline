using Microsoft.AspNetCore.Mvc;

namespace QuyenGopOnline.Controllers
{
    public class DashboardController : Controller
    {
        // GET: /Dashboard/Index
        public IActionResult Index()
        {
            return View();
        }
    }
}