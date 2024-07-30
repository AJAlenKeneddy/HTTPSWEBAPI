using System.ComponentModel.DataAnnotations;

namespace WEBAPIGMINGENIEROSHTTPS.Models
{
    public class UsuariosTodoList
    {
        [Key]
        public int IdUsuario { get; set; }
        [Required]
        public String? Nombre { get; set; }
        [Required]
        public String? Correo { get; set; }
        [Required]
        public String? Clave { get; set; }
        
        public String? CodigoVerificacion { get; set; }
        
        public Boolean Verificado {  get; set; }



    }
}
