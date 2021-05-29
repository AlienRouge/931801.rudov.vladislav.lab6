using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ForumIpsum.Data;
using ForumIpsum.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using System.Net.Http.Headers;
using System.IO;
using ForumIpsum.Services;

namespace ForumIpsum.Controllers
{
    public class FilesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly MapService _mimeMappingService;

        public FilesController(ApplicationDbContext context,
            IHostingEnvironment hostingEnvironment,
            MapService mimeMappingService)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _mimeMappingService = mimeMappingService;
        }

        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Files.Include(f => f.Folder);
            return View(await applicationDbContext.ToListAsync());
        }

        public IActionResult Details(Guid? id)
        {
            if (id == null) return NotFound();
            
            var file = _context.Files
                .SingleOrDefault(m => m.Id == id);
            if (file == null) return NotFound();
            
            ViewBag.Folder = file.FolderId;
            return View(file);
        }


        public IActionResult Create(Guid? id)
        {
            ViewBag.Id = id;
            return View(new FileViewModel());
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Guid id, FileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Details", "Folders", new {id = id});
            }

            var fileName = Path.GetFileName(ContentDispositionHeaderValue.Parse(model.FilePath.ContentDisposition)
                .FileName.Trim('"'));
            var fileExt = Path.GetExtension(fileName);

            var file = new Models.File
            {
                Name = model.Name,
                Extension = fileExt,
                FolderId = id,
                Size = model.FilePath.Length
            };
            file.Name ??= fileName;

            var path = Path.Combine(_hostingEnvironment.WebRootPath, "attachments", file.Id.ToString("N") + fileExt);

            using (var fileStream = new FileStream(path, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.Read))
            {
                model.FilePath.CopyTo(fileStream);
            }

            _context.Files.Add(file);
            _context.SaveChanges();
            return this.RedirectToAction("Details", "Folders", new {id = id});
        }


        public IActionResult Edit(Guid? id)
        {
            ViewBag.Id = id;
            return View(new FileViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Guid id, FileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var file = _context.Files.SingleOrDefault(e => e.Id == id);
            file.Name = model.Name;
            _context.SaveChanges();
            return RedirectToAction("Details", "Files", new {id = id});
        }

        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null) return NotFound();
            
            var file = await _context.Files
                .Include(f => f.Folder)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (file == null)
            {
                return NotFound();
            }

            return View(file);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var file = await _context.Files.Include(e => e.Folder).SingleOrDefaultAsync(m => m.Id == id);
            _context.Files.Remove(file);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Folders", new {id = file.FolderId});
        }

        public IActionResult Download(Guid id)
        {
            var file = _context.Files
                .SingleOrDefault(e => e.Id == id);
            if (file == null)
                return NotFound();
            var attachmentPath = Path.Combine(_hostingEnvironment.WebRootPath, "attachments",
                file.Id.ToString("N") + Path.GetExtension(file.Extension));
            return PhysicalFile(attachmentPath, _mimeMappingService.GetContentType(file.Extension), file.Name);
        }
    }
}