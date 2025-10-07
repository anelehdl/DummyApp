using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dashboard.Controllers
{
    [Authorize(Roles = "admin,staff")] // Both admin and staff can access reports
    public class ReportsController : Controller
    {
        private readonly ILogger<ReportsController> _logger;

        public ReportsController(ILogger<ReportsController> logger)
        {
            _logger = logger;
        }

        // GET: /Reports/Index
        public IActionResult Index()
        {
            ViewBag.UserRole = User.IsInRole("admin") ? "Admin" : "Staff";
            return View();
        }
    }
}