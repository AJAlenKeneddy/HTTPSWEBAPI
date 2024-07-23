using System;
using System.Collections.Generic;

namespace AppWebApiGMINGENIEROS.Models;

public partial class Proveedore
{
    public long Ruc { get; set; }

    public string? Nombre { get; set; }

    public string? Ubigeo { get; set; }

    public string? Departamento { get; set; }

    public string? Provincia { get; set; }

    public string? Distrito { get; set; }

    public string? Direccion { get; set; }

    public virtual Ubigeo? UbigeoNavigation { get; set; }
}
