using Fiorella.Models;
using System.Collections.Generic;

namespace Fiorella.ViewModels
{
    public class HomeViewModel
    {
        public List<SliderImage> SliderImages{ get; set; }
        public Slider Slider { get; set; }
        public List<Product> Products { get; set; }
        public List<Category> Categories { get; set; }
        public About About { get; set; }
        public List<AboutServices> AboutServices { get; set; }
        public ExpertsSection ExpertsSection { get; set; }
        public List<Expert> Experts { get; set; }
        public Subscribe Subscribe { get; set; }
        public BlogSection BlogSection { get; set; }
        public List<Blog> Blogs { get; set; }
        public List<Say> Says { get; set; }
        public List<Instagram> Instagrams { get; set; }
    }
}
