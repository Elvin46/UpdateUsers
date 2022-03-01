using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fiorella.Models
{
    public class SliderImage
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [NotMapped]
        public IFormFile Photo { get; set; }
        [NotMapped]
        public IFormFile[] Photos { get; set; }

    }
}
