using EllipticCurve.Utils;
using System.ComponentModel.DataAnnotations;

namespace WEBAPIGMINGENIEROSHTTPS.Models
{
    public class toDoList
    {
        [Key]
        public int TareaId { get; set; }
        [Required]
        public int IdUsuario { get;set; }
        [Required]
        public string? tarea { get; set; }
        [Required]
        public Boolean completado { get; set; }

    }
}
