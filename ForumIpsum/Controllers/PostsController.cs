using System.Threading.Tasks;
using ForumIpsum.Data;
using ForumIpsum.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ForumIpsum.Controllers
{
    public class PostsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PostsController(ApplicationDbContext context)
        {
            _context = context;
        }
     
        public async Task<IActionResult> Index()
        {
            var posts = _context.Posts.Include(f => f.Category);
            return View(await posts.ToListAsync());
        }

        public IActionResult Create()
        {
            var categories = new SelectList(_context.ForumCategories, "Id", "Name");
            ViewBag.Categorys = categories;
            return View(new Post());
        }

        [HttpPost]
        public async Task<IActionResult> Create(Post model)
        {
            if (!ModelState.IsValid) return View(model);
            await _context.Posts.AddAsync(model);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            
            var post = await _context.Posts
                .Include(f => f.Category)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (post == null) return NotFound();
            
            return View(post);
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var post = await _context.Posts.FindAsync(id);
            if (post == null) return NotFound();
            
            var categories = new SelectList(_context.ForumCategories, "Id", "Name", post.CategoryId);
            ViewBag.Categorys = categories;
            return View(post);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int? id, Post model)
        {
            if (id == null) return NotFound();

            var post = await _context.Posts.FindAsync(id);
            if (post == null) return NotFound();
            if (!ModelState.IsValid) return View(model);

            post.Title = model.Title;
            post.Text = model.Text;
            post.Category = model.Category;
            post.CategoryId = model.CategoryId;
            _context.Entry(post).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var post = await _context.Posts.FindAsync(id);
            if (post == null) return NotFound();

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}