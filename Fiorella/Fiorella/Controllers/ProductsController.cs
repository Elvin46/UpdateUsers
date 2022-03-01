using Fiorella.DataAccessLayer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Fiorella.Controllers
{
    public class ProductsController : Controller
    {
        private readonly AppDbContext _dbContext;
        private readonly int _productsCount;

        public ProductsController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
            _productsCount = _dbContext.Products.Count();
        }

        public IActionResult Index()
        { 
            var products = _dbContext.Products.Include(x => x.Category).Take(4).ToList();
            return View(products);
        }
        public IActionResult Load(int skip)
        {
            if (skip >= _productsCount)
            {
                return BadRequest();
            }
            var products = _dbContext.Products.Include(x => x.Category).Skip(skip).Take(4).ToList();
            return PartialView("_ProductsPartial", products);
        }
    }
}
