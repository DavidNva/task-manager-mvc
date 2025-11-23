using System.ComponentModel.DataAnnotations;

namespace TaskManagerMVC.Models
{
    public class PasoCrearDTO
    {
        [Required]
        public string Description { get; set; }
        public bool Realizado { get; set; }
    }
}
