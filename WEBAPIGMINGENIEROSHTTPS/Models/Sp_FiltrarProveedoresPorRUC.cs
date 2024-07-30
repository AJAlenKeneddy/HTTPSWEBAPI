using System.ComponentModel.DataAnnotations;

namespace WEBAPIGMINGENIEROSHTTPS.Models
{
    public class Sp_FiltrarProveedoresPorRUC
    {
        [Key]
        public long Ruc { get; set; }
        [Required]
        public string? Nombre { get; set; }
        
        public string? Ubigeo { get; set; }
        [Required]
        public string? Departamento { get; set; }
        [Required]
        public string? Provincia { get; set; }
        [Required]
        public string? Distrito { get; set; }
        [Required]
        public string? Direccion { get; set; }
       
      
    }
}
