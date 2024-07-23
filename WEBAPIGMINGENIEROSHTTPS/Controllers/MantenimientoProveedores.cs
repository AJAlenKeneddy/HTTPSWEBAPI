using AppWebApiGMINGENIEROS.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AppWebApiGMINGENIEROS.Controllers
{
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

    [ApiController]
    public class MantenimientoProveedores : ControllerBase
    {
        private readonly DatadecomprasgmContext db;

        public MantenimientoProveedores(DatadecomprasgmContext ctx)
        {
            db = ctx;
            
        }


        // POST: api/Proveedor/Crear_Proveedor
        [HttpPost("Crear_Proveedor")]
        [Authorize]
        public IActionResult PostProveedor([FromBody] Proveedore proveedor)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                db.Proveedores.Add(proveedor);
                db.SaveChanges();

                // Aquí utilizamos el método del controlador Consultas para obtener el comprobante creado
                var nuevoComprobante = GetProveedorPorId(proveedor.Ruc) as ObjectResult;
                if (nuevoComprobante == null || nuevoComprobante.Value == null)
                {
                    return NotFound();
                }

                return CreatedAtAction(nameof(GetProveedorPorId), new { id = proveedor.Ruc }, proveedor );
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        // PUT: api/Proveedor/5
        [HttpPut("Editar_Proveedor/{ruc}")]
        [Authorize]
        public IActionResult PutComprobante(long ruc, [FromBody] Proveedore proveedor)
        {
            if (ruc != proveedor.Ruc)
            {
                return BadRequest();
            }

            db.Entry(proveedor).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // DELETE: api/Proveedor/5
        [HttpDelete("Eliminar_Proveedor/{ruc}")]
        [Authorize]
        public IActionResult DeleteProveedor(long ruc)
        {
            var proveedor = db.Proveedores.Find(ruc);
            if (proveedor == null)
            {
                return NotFound();
            }

            db.Proveedores.Remove(proveedor);
            db.SaveChanges();
            return NoContent();
        }
        private IActionResult GetProveedorPorId(long id)
        {
            var proveedor = db.Proveedores.FirstOrDefault(c => c.Ruc == id);
            if (proveedor == null)
            {
                return NotFound();
            }
            return Ok(proveedor);
        }

    }
}
