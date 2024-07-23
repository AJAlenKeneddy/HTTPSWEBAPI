using System.ComponentModel.DataAnnotations;

namespace AppWebApiGMINGENIEROS.Models
{
    public class toDoList
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string? tarea { get; set; }
        [Required]
        public Boolean completado { get; set; }

    }
}
