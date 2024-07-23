using System;
using System.Collections.Generic;

namespace AppWebApiGMINGENIEROS.Models;

public partial class Comprobante
{
    public int Idcomprobante { get; set; }

    public string? Numerodocumento { get; set; }

    public string? Ruc { get; set; }

    public string? Razonsocial { get; set; }

    public string? Concepto { get; set; }

    public string? Moneda { get; set; }

    public decimal? Importe { get; set; }

    public string? Tipodocumento { get; set; }

    public string? Emitidorecibido { get; set; }

    public DateOnly Fecha { get; set; }
}
