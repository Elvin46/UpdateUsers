using Fiorella.DataAccessLayer;
using Fiorella.Models;
using Fiorella.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fiorella.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _dbContext;

        public HomeController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            var sliderImages = _dbContext.SliderImages.ToList();
            var slider = _dbContext.Sliders.SingleOrDefault();
            var categories = _dbContext.Categories.ToList();
            var products = _dbContext.Products.Include(x => x.Category).ToList();
            var about = _dbContext.Abouts.SingleOrDefault();
            var aboutServices = _dbContext.AboutServices.ToList();
            var expertSection = _dbContext.ExpertsSections.SingleOrDefault();
            var experts = _dbContext.Experts.ToList();
            var subscribe = _dbContext.Subscribes.SingleOrDefault();
            var blogSection = _dbContext.BlogSections.SingleOrDefault();
            var blogs = _dbContext.Blogs.ToList();
            var says = _dbContext.Says.ToList();
            var instagrams = _dbContext.Instagrams.ToList();
            return View(new HomeViewModel
            {
                SliderImages = sliderImages,
                Slider = slider,
                Products = products,
                Categories = categories,
                About = about,
                AboutServices = aboutServices,
                ExpertsSection = expertSection,
                Experts = experts,
                Subscribe = subscribe,
                Blogs = blogs,
                BlogSection = blogSection,
                Says = says,
                Instagrams = instagrams
            });
        }

        public async Task<IActionResult> Basket()
        {
            var basket = Request.Cookies["basket"];
            if (string.IsNullOrEmpty(basket))
                return Content("Empty");
            var basketViewModels = JsonConvert.DeserializeObject<List<BasketViewModel>>(basket);
            var newBasket = new List<BasketViewModel>();
            double totalAmount = 0;
            foreach (var basketViewModel in basketViewModels)
            {
                var product = await _dbContext.Products.FindAsync(basketViewModel.Id);
                if (product==null)
                {
                    continue;
                }
                newBasket.Add(new BasketViewModel
                {
                    Id = product.Id,
                    Name = product.Name,
                    Image = product.Image,
                    Price = product.Price,
                    Count = basketViewModel.Count,
                    TotalPrice = product.Price * basketViewModel.Count
                });
                totalAmount += basketViewModel.TotalPrice;
                ViewBag.TotalAmount = totalAmount;
            }
            basket = JsonConvert.SerializeObject(newBasket);
            Response.Cookies.Append("basket", basket);
            return View(newBasket);
        }
        public async Task<IActionResult> AddToBasket(int? id)
        {
            if (id==null)
            {
                return BadRequest();
            }
            var product = await _dbContext.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            List<BasketViewModel> basketViewModels;
            var existBasket = Request.Cookies["basket"];
            if (string.IsNullOrEmpty(existBasket))
                basketViewModels = new List<BasketViewModel>();
            else
                basketViewModels = JsonConvert.DeserializeObject<List<BasketViewModel>>(existBasket);

            var existBasketViewModel = basketViewModels.FirstOrDefault(x => x.Id == id);
            if (existBasketViewModel == null)
            {
                existBasketViewModel = new BasketViewModel
                {
                    Id = product.Id,

                };
                basketViewModels.Add(existBasketViewModel);
            }
            else
            {
                existBasketViewModel.Count++;
            }
            existBasketViewModel.Name=product.Name;
            existBasketViewModel.Image=product.Image;
            existBasketViewModel.Price=product.Price;
            existBasketViewModel.TotalPrice = product.Price * existBasketViewModel.Count;


            var basket = JsonConvert.SerializeObject(basketViewModels);
            Response.Cookies.Append("basket", basket);
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> CountDown(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            var product = await _dbContext.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            var existBasket = Request.Cookies["basket"];
            if (string.IsNullOrEmpty(existBasket))
                return BadRequest();
            var basketViewModels = JsonConvert.DeserializeObject<List<BasketViewModel>>(existBasket);
            var existBasketViewModel = basketViewModels.FirstOrDefault(x => x.Id == id);
            if (existBasketViewModel.Count != 1)
            {
                existBasketViewModel.Count--;
            }
            if (existBasketViewModel.Count <= 0)
            {
                basketViewModels.Remove(existBasketViewModel);
            }
            var newBasket = new List<BasketViewModel>();
            foreach (var basketViewModel in basketViewModels)
            {
                newBasket.Add(new BasketViewModel
                {
                    Id = basketViewModel.Id,
                    Name = basketViewModel.Name,
                    Image = basketViewModel.Image,
                    Price = basketViewModel.Price,
                    Count = basketViewModel.Count,
                    TotalPrice = basketViewModel.Price * basketViewModel.Count
                });

            }
            existBasket = JsonConvert.SerializeObject(newBasket);
            Response.Cookies.Append("basket", existBasket);
            return RedirectToAction(nameof(Basket));
        }
        public async Task<IActionResult> CountUp(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            var product = await _dbContext.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            var existBasket = Request.Cookies["basket"];
            if (string.IsNullOrEmpty(existBasket))
                return BadRequest();
            var basketViewModels = JsonConvert.DeserializeObject<List<BasketViewModel>>(existBasket);
            var existBasketViewModel = basketViewModels.FirstOrDefault(x => x.Id == id);
            existBasketViewModel.Count++;
            var newBasket = new List<BasketViewModel>();
            foreach (var basketViewModel in basketViewModels)
            {
                newBasket.Add(new BasketViewModel
                {
                    Id = basketViewModel.Id,
                    Name = basketViewModel.Name,
                    Image = basketViewModel.Image,
                    Price = basketViewModel.Price,
                    Count = basketViewModel.Count,
                    TotalPrice = basketViewModel.Price * basketViewModel.Count
                });

            }
            existBasket = JsonConvert.SerializeObject(newBasket);
            Response.Cookies.Append("basket", existBasket);
            return RedirectToAction(nameof(Basket));
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            var product = await _dbContext.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            var existBasket = Request.Cookies["basket"];
            if (string.IsNullOrEmpty(existBasket))
                return BadRequest();
            var basketViewModels = JsonConvert.DeserializeObject<List<BasketViewModel>>(existBasket);
            var existBasketViewModel = basketViewModels.FirstOrDefault(x => x.Id == id);
            basketViewModels.Remove(existBasketViewModel);
            var newBasket = new List<BasketViewModel>();
            foreach (var basketViewModel in basketViewModels)
            {
                newBasket.Add(new BasketViewModel
                {
                    Id = basketViewModel.Id,
                    Name = basketViewModel.Name,
                    Image = basketViewModel.Image,
                    Price = basketViewModel.Price,
                    Count = basketViewModel.Count,
                    TotalPrice = basketViewModel.Price * basketViewModel.Count
                });

            }
            existBasket = JsonConvert.SerializeObject(newBasket);
            Response.Cookies.Append("basket", existBasket);
            return RedirectToAction(nameof(Basket));
        }
    }
}
