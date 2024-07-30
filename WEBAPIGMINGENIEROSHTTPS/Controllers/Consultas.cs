using WEBAPIGMINGENIEROSHTTPS.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WEBAPIGMINGENIEROSHTTPS.Controllers
{
    [Route("api/[controller]")]
    
    [ApiController]
    public class Consultas : ControllerBase
    {
        private readonly DatadecomprasgmContext db;
        public Consultas(DatadecomprasgmContext ctx)
        {
            db = ctx;
        }


        [HttpGet("Obtener_Todos_Los_Comprobantes")]
        
        public IActionResult GetComprobantes()
        {
            // Aquí puedes usar el token recibido en el parámetro 'token'
            var comprobantes = db.Comprobantes.ToList();

            var result = comprobantes.Select(c => new
            {
                idcomprobante = c.Idcomprobante,
                fecha = c.Fecha.ToString("yyyy-MM-dd"),
                numerodocumento = c.Numerodocumento,
                ruc = c.Ruc,
                razonsocial = c.Razonsocial,
                concepto = c.Concepto,
                moneda = c.Moneda,
                importe = c.Importe,
                tipodocumento = c.Tipodocumento,
                emitidorecibido = c.Emitidorecibido
            }).ToList();

            return Ok(result);
        }


        

        [HttpGet("Obtener_Todos_Los_Proveedores")]

        public List<Proveedore> GetProveedores()
        {
            return db.Proveedores.ToList();
        }

        [HttpGet("Filtrar_Un_Comprobante/{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult GetComprobantePorId(string id)
        {
            var comprobante = db.Comprobantes.FirstOrDefault(c => c.Numerodocumento == id);
            if (comprobante == null)
            {
                return NotFound(); // Devolver 404 si no se encuentra el comprobante
            }
            return Ok(comprobante); // Devolver el comprobante encontrado
        }


        [HttpGet("Filtrar_Comprobantes_Año_Mes")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult GetComprobantesFiltro([FromQuery] int? anio, [FromQuery] int? mes)
        {
            var comprobantes = db.Sp_FiltrarComprobantes
                .FromSqlRaw<sp_FiltrarComprobantes>("sp_FiltrarComprobantes {0}, {1}", anio!, mes!)
                .ToList();

            var result = comprobantes.Select(c => new
            {
                idcomprobante = c.Idcomprobante,
                fecha = c.Fecha.ToString("yyyy-MM-dd"),
                numerodocumento = c.Numerodocumento,
                ruc = c.Ruc,
                razonsocial = c.Razonsocial,
                concepto = c.Concepto,
                moneda = c.Moneda,
                importe = c.Importe,
                tipodocumento = c.Tipodocumento,
                emitidorecibido = c.Emitidorecibido
            }).ToList();

            return Ok(result);
        }


        [HttpGet("Filtrar_Proveedor_RUC")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult GetProveedoresPorRUC([FromQuery] string ruc)
        {
            var proveedores = db.Sp_FiltrarProveedoresPorRUCs
                .FromSqlRaw<Sp_FiltrarProveedoresPorRUC>("sp_FiltrarProveedoresPorRUC {0}", ruc)
                .ToList();

            return Ok(proveedores);
        }


        
    }





}

