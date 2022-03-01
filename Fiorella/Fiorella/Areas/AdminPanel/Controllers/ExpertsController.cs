using Fiorella.Areas.AdminPanel.Data;
using Fiorella.DataAccessLayer;
using Fiorella.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Fiorella.Areas.AdminPanel.Controllers
{
    [Area("AdminPanel")]
    public class ExpertsController : Controller
    {
        private readonly AppDbContext _dbContext;
        public ExpertsController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IActionResult> Index()
        {
            var experts = await _dbContext.Experts.ToListAsync();
            return View(experts);
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id==null)
            {
                return BadRequest();
            }
            Expert existExpert = await _dbContext.Experts.FirstOrDefaultAsync(x => x.Id == id);
            if (existExpert == null) 
                return Json(new { status = 404 });

            var path = Path.Combine(Constants.ImageFolderPath, existExpert.Image);
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }

            _dbContext.Experts.Remove(existExpert);
            _dbContext.SaveChanges();
            return Json(new { status = 200 });
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            var expert = await _dbContext.Experts.FindAsync(id);
            if (expert == null)
            {
                return NotFound();
            }
            return View(expert);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Expert Expert)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            if (Expert == null)
            {
                return BadRequest();
            }
            var isExistExpert = await _dbContext.Experts.AnyAsync(x => x.Fullname.ToLower() == Expert.Fullname.ToLower() && x.Field.ToLower() == Expert.Field.ToLower());
            if (isExistExpert)
            {
                ModelState.AddModelError("ExistTitle", "This expert already exist");
                return View();
            }
            if (!Expert.Photo.IsImage())
            {
                ModelState.AddModelError("Photo", "It must be photo(jpg,png)");
                return View();
            }
            if (!Expert.Photo.IsAllowedSize(1))
            {
                ModelState.AddModelError("Photo", "Size of photo has to be under 1 mb");
                return View();
            }
            var fileName = await Expert.Photo.GenerateFile(Constants.ImageFolderPath);
            Expert.Image = fileName;
            await _dbContext.Experts.AddAsync(Expert);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Update(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            Expert isExist = _dbContext.Experts.FirstOrDefault(x => x.Id == id);
            if (isExist == null) 
                return NotFound();
            return View(isExist);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id,Expert Expert)
        {
            if (id==null)
            {
                return BadRequest();
            }
            if (id != Expert.Id)
            {
                return BadRequest();
            }
            var existExpert = await _dbContext.Experts.FindAsync(id);
            if (existExpert == null)
            {
                return NotFound();
            }
            if (!ModelState.IsValid)
            {
                return View(existExpert);
            }
            var isExist = await _dbContext.Experts.AnyAsync(x => x.Fullname.ToLower() == Expert.Fullname.ToLower() && x.Id != Expert.Id);
            if (isExist)
            {
                ModelState.AddModelError("Fullname", "This name is already exist");
                return View(existExpert);
            }
            if (Expert.Photo != null)
            {
                if (!Expert.Photo.IsImage())
                {
                    ModelState.AddModelError("Photo", "It must be photo(jpg,png)");
                    return View(existExpert);
                }
                if (!Expert.Photo.IsAllowedSize(1))
                {
                    ModelState.AddModelError("Photo", "Size of photo has to be under 1 mb");
                    return View(existExpert);
                }
                var path = Path.Combine(Constants.ImageFolderPath, existExpert.Image);
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }
                var fileName = await Expert.Photo.GenerateFile(Constants.ImageFolderPath);
                existExpert.Image = fileName;
            }
            
            existExpert.Fullname = Expert.Fullname;
            existExpert.Field = Expert.Field;
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
