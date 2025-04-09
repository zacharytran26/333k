using Microsoft.AspNetCore.Mvc;

namespace Tran_Zachary_HW5.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}