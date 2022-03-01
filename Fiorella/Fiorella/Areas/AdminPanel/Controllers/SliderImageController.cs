using Fiorella.Areas.AdminPanel.Data;
using Fiorella.DataAccessLayer;
using Fiorella.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Fiorella.Areas.AdminPanel.Controllers
{
    [Area("AdminPanel")]
    public class SliderImageController : Controller
    {
        private readonly AppDbContext _dbContext;
        public SliderImageController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<IActionResult> Index()
        {
            var sliderImages = await _dbContext.SliderImages.ToListAsync();
            return View(sliderImages);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SliderImage sliderImage)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            if (sliderImage == null)
            {
                return BadRequest();
            }
            var imagesCount = await _dbContext.SliderImages.CountAsync();
            if (sliderImage.Photos.Count()+imagesCount > 5)
            {
                ModelState.AddModelError("Photos", $"{5 - imagesCount} images can be uploaded");
                return View();
            }

            foreach (var photo in sliderImage.Photos)
            {
                if (!photo.IsImage())
                {
                    ModelState.AddModelError("Photo", "It must be photo(jpg,png)");
                    return View();
                }
                if (!photo.IsAllowedSize(1))
                {
                    ModelState.AddModelError("Photo", "Size of photo has to be under 1 mb");
                    return View();
                }
                var fileName = await photo.GenerateFile(Constants.ImageFolderPath);
                var newSliderImage = new SliderImage { Name = fileName };
                await _dbContext.SliderImages.AddAsync(newSliderImage);
                await _dbContext.SaveChangesAsync();
            }
            
            return RedirectToAction(nameof(Index));
        }
    }
}
