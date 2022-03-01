namespace Fiorella.ViewModels
{
    public class BasketViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public string Image { get; set; }
        public int Count { get; set; } = 1;
        public double TotalPrice { get; set; }
    }
}
