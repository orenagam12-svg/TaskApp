using Microsoft.AspNetCore.Mvc;

namespace TaskApp.Controllers;

// מחזיר את דף ה-HTML הראשי
public class HomeController : Controller
{
    [Route("/")]
    public IActionResult Index() => PhysicalFile(
        Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "index.html"),
        "text/html");
}
