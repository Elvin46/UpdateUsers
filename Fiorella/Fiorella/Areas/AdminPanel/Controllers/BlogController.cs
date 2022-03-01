using Fiorella.DataAccessLayer;
using Fiorella.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Fiorella.Areas.AdminPanel.Controllers
{
    [Area("AdminPanel")]
    public class BlogController : Controller
    {
        private readonly AppDbContext _dbContext;

        public BlogController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<IActionResult> Index()
        {
            var blogs = await _dbContext.Blogs.ToListAsync();
            return View(blogs);
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id==null)
            {
                return BadRequest();
            }
            var blog = await _dbContext.Blogs.FindAsync(id);
            if (blog==null)
            {
                return NotFound();
            }
            return View(blog);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Blog Blog)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            var isExistBlog = await _dbContext.Blogs.AnyAsync(x => x.Title.ToLower() == Blog.Title.ToLower());
            if (isExistBlog)
            {
                ModelState.AddModelError("ExistTitle", "Blog exist with this title");
                return View();
            }
            await _dbContext.Blogs.AddAsync(Blog);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
