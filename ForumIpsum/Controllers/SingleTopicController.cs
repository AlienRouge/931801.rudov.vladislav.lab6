using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using ForumIpsum.Data;
using ForumIpsum.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ForumIpsum.Controllers
{
    public class SingleTopicController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> UserManager;

        public SingleTopicController(ApplicationDbContext context, UserManager<IdentityUser> manager)
        {
            _context = context;
            UserManager = manager;
        }

        // GET
        public async Task<IActionResult> Index()
        {
            var topics = _context.Topics;
            return View(await topics.ToListAsync());
        }

        public IActionResult Create()
        {
            return View(new TopicModel());
        }

        [HttpPost]
        public async Task<IActionResult> Create(TopicViewModel model)
        {
            if (!ModelState.IsValid) return View();

            CultureInfo culture = new CultureInfo("en-US");
            TopicModel topic = new TopicModel();
            topic.Title = model.Title;
            topic.Text = model.Text;
            topic.Author = UserManager.GetUserName(User);
            topic.CreateDate = DateTime.Now.ToString("dddd, dd MMMM yyyy HH:mm:ss", culture);

            if (model.Image1 != null)
                topic.Image1 = AddImageInDatabase(model.Image1);

            if (model.Image2 != null)
                topic.Image2 = AddImageInDatabase(model.Image2);

            if (model.Image3 != null)
                topic.Image3 = AddImageInDatabase(model.Image3);

            await _context.Topics.AddAsync(topic);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private byte[] AddImageInDatabase(IFormFile file)
        {
            using var binaryReader = new BinaryReader(file.OpenReadStream());
            var imageData = binaryReader.ReadBytes((int) file.Length);

            return imageData;
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var topic = await _context.Topics.FindAsync(id);
            if (topic == null) return NotFound();

            return View(topic);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int? id, TopicModel model)
        {
            if (id == null) return NotFound();

            var topic = await _context.Topics.FindAsync(id);
            if (topic == null) return NotFound();
            if (!ModelState.IsValid) return View(model);

            CultureInfo culture = new CultureInfo("en-US");
            topic.Title = model.Title;
            topic.Text = model.Text;
            topic.EditDate = DateTime.Now.ToString("dddd, dd MMMM yyyy HH:mm:ss", culture);
            _context.Entry(topic).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var topic = await _context.Topics.FindAsync(id);
            if (topic == null) return NotFound();

            _context.Topics.Remove(topic);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> DeleteImg(int? id, int? imgIndex)
        {
            if (id == null) return NotFound();
            var topic = await _context.Topics.FindAsync(id);
            if (topic == null) return NotFound();

            switch (imgIndex)
            {
                case 1:
                    topic.Image1 = null;
                    break;
                case 2:
                    topic.Image2 = null;
                    break;
                default:
                    topic.Image3 = null;
                    break;
            }

            _context.Entry(topic).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Attach(int? id)
        {
            if (id == null) return NotFound();

            var topic = await _context.Topics.FindAsync(id);
            if (topic == null) return NotFound();

            return View(topic);
        }

        [HttpPost]
        public async Task<IActionResult> Attach(TopicViewModel model, int? id)
        {
            if (id == null) return NotFound();

            var topic = await _context.Topics.FindAsync(id);
            if (topic == null) return NotFound();

            if (model.Image1 != null)
                topic.Image1 = AddImageInDatabase(model.Image1);

            if (model.Image2 != null)
                topic.Image2 = AddImageInDatabase(model.Image2);

            if (model.Image3 != null)
                topic.Image3 = AddImageInDatabase(model.Image3);

            _context.Entry(topic).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}