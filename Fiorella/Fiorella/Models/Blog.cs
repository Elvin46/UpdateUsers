using System;
using System.ComponentModel.DataAnnotations;

namespace Fiorella.Models
{
    public class Blog
    {
        public int Id { get; set; }
        [Required,MaxLength(50)]
        public string Title { get; set; }
        [MaxLength(100)]
        public string Subtitle { get; set; }
        [Required]
        public string Image { get; set; }
        [Required]
        public string Date { get; set; }
    }
}
