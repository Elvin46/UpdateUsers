using Fiorella.DataAccessLayer;
using Fiorella.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Fiorella.ViewComponents
{
    public class HeaderViewComponent : ViewComponent
    {
        private readonly AppDbContext _dbContext;
        public HeaderViewComponent(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var count = 0;
            double totalAmount = 0;
            var basket = Request.Cookies["basket"];
            if (!string.IsNullOrEmpty(basket))
            {
                var products = JsonConvert.DeserializeObject<List<BasketViewModel>>(basket);
                foreach (var product in products)
                {
                    totalAmount += product.TotalPrice;
                }
                count = products.Count;
            }
            ViewBag.BasketCount = count;
            ViewBag.BasketAmount = totalAmount;
            var bio = await _dbContext.Bios.SingleOrDefaultAsync();

            return View(bio);
        }
    }
}
