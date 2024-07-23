using AppWebApiGMINGENIEROS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AppWebApiGMINGENIEROS.Controllers
{
    
    public class ComprobantesController : Controller
    {
        private readonly DatadecomprasgmContext bd;
        public ComprobantesController(DatadecomprasgmContext _contexto)
        {
            bd = _contexto;
        }

        // GET: ComprobantesController
        public ActionResult IndexComprobantes(int nro_pag = 0, string ruc = null!, string numeroDocumento = null!)
        {
            // Llamar a la acción BuscarPorRuc para manejar los filtros y la paginación
            return RedirectToAction("BuscarPorRuc", new { nro_pag, ruc, numeroDocumento });
        }





        public ActionResult Details(int id)
        {
            var comprobante = bd.Comprobantes.FirstOrDefault(c => c.Idcomprobante == id);
            if (comprobante == null)
            {
                return NotFound();
            }
            return View(comprobante);
        }


        // GET: ComprobantesController/Create
        [Authorize]
        public ActionResult CreateComprobante()
        {
            Comprobante nuevo = new Comprobante();

            return View(nuevo);
        }

        // POST: ComprobantesController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        
        public ActionResult CreateComprobante(Comprobante obj)
        {
            try
            {

                if (ModelState.IsValid)
                {
                    bd.Comprobantes.Add(obj); // lo graba en la memoria
                    bd.SaveChanges(); // lo graba en la BD
                    TempData["mensaje"] = "Nuevo Comprobante registrado correctamente";
                    return RedirectToAction(nameof(IndexComprobantes));
                }
            }
            catch (Exception ex)
            {
                ViewBag.mensaje = ex.Message;
            }

            return View();
        }
        

        public ActionResult EditComprobantes(int id)
        {
            var comprobante = bd.Comprobantes.FirstOrDefault(c => c.Idcomprobante == id);
            if (comprobante == null)
            {
                return NotFound(); // Otra vista para mostrar un mensaje de "no encontrado"
            }
            return View(comprobante);
        }
        public ActionResult BuscarPorRuc(string ruc = null!, string numeroDocumento = null!, int nro_pag = 0)
        {
            const int cant_filas = 50;

            IQueryable<Comprobante> comprobantesFiltrados = bd.Comprobantes;

            // Aplicar filtros según los parámetros recibidos
            if (!string.IsNullOrEmpty(ruc))
            {
                comprobantesFiltrados = comprobantesFiltrados.Where(c => c.Ruc!.Contains(ruc));
            }

            if (!string.IsNullOrEmpty(numeroDocumento))
            {
                comprobantesFiltrados = comprobantesFiltrados.Where(c => c.Numerodocumento!.Contains(numeroDocumento));
            }

            var n = comprobantesFiltrados.Count(); // Obtener el número total de comprobantes filtrados

            // Calcular el número total de páginas para los comprobantes filtrados
            int cant_paginas2 = (n % cant_filas == 0) ? n / cant_filas : n / cant_filas + 1;
            ViewBag.CANT_PAGINAS = cant_paginas2;

            // Obtener el total de la compra para la página actual de los comprobantes filtrados
            var totalCompra = comprobantesFiltrados
                                .OrderBy(c => c.Idcomprobante) // Asegurarse de ordenar para la paginación
                                .Skip(nro_pag * cant_filas)
                                .Take(cant_filas)
                                .Sum(c => c.Importe);

            // Obtener los comprobantes filtrados para la página actual
            var comprobantesPaginados = comprobantesFiltrados
                                            .OrderBy(c => c.Idcomprobante) // Asegurarse de ordenar para la paginación
                                            .Skip(nro_pag * cant_filas)
                                            .Take(cant_filas)
                                            .ToList();

            ViewBag.TotalPago = totalCompra;

            // Pasar los valores de búsqueda a la vista para mostrarlos si es necesario
            ViewBag.NroRuc = ruc;
            ViewBag.NumeroDocumento = numeroDocumento;

            return View("IndexComprobantes", comprobantesPaginados);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        

        public ActionResult EditComprobantes(int id, Comprobante comprobanteEditado)
        {
            var comprobante = bd.Comprobantes.FirstOrDefault(c => c.Idcomprobante == id);
            if (comprobante == null)
            {
                return NotFound(); // Otra vista para mostrar un mensaje de "no encontrado"
            }

            // Actualiza los campos del comprobante con los valores del comprobanteEditado

            comprobante.Fecha = comprobanteEditado.Fecha;
            comprobante.Numerodocumento = comprobanteEditado.Numerodocumento;
            comprobante.Ruc = comprobanteEditado.Ruc;
            comprobante.Razonsocial = comprobanteEditado.Razonsocial;
            comprobante.Concepto = comprobanteEditado.Concepto;
            comprobante.Moneda = comprobanteEditado.Moneda;
            comprobante.Importe = comprobanteEditado.Importe;
            comprobante.Tipodocumento = comprobanteEditado.Tipodocumento;
            comprobante.Emitidorecibido = comprobanteEditado.Emitidorecibido;
            // ... (continúa con las demás propiedades)

            bd.SaveChanges(); // Guarda los cambios en la base de datos

            return RedirectToAction(nameof(IndexComprobantes));
        }


    }
}
