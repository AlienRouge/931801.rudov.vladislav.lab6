using System.Threading.Tasks;
using ForumIpsum.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ForumIpsum.Controllers
{
    public class MockupsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MockupsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> AllForums()
        {
            return View(await _context.ForumCategories.ToListAsync());
        }

        public IActionResult SingleForum()
        {
            return View();
        }

        public IActionResult SingleTopic()
        {
            return View();
        }
    }
}