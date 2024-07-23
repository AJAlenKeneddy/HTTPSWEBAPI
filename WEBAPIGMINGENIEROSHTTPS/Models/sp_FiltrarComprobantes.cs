using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AppWebApiGMINGENIEROS.Models
{
    public class sp_FiltrarComprobantes
    {
        [Key]
        public int Idcomprobante { get; set; }
        [Required]
        public DateTime Fecha { get; set; }
        [Required]
        public string? Numerodocumento { get; set; }
        [Required]
        public string? Ruc { get; set; }
        [Required]
        public string? Razonsocial { get; set; }
        [Required]
        public string? Concepto { get; set; }
        [Required]
        public string? Moneda { get; set; }
        [Required]
        public decimal? Importe { get; set; }
        [Required]
        public string? Tipodocumento { get; set; }
        [Required]
        public string? Emitidorecibido { get; set; }
    }
}
