using System.ComponentModel.DataAnnotations;

namespace TaskManagerMVC.Models
{
    public class TaskEditarDTO
    {
        [Required]
        [StringLength(250)]
        public string Title { get; set; }
        public string description { get; set; }
    }
}
