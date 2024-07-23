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
    public class MantenimientoComprobantes : ControllerBase
    {
        private readonly DatadecomprasgmContext db;
        private readonly Consultas _consultasController;
        public MantenimientoComprobantes(DatadecomprasgmContext ctx, Consultas consultasController)
        {
            db = ctx;
            _consultasController = consultasController;
            
        }
        
        // POST: api/Comprobantes/Crear_Comprobante
        [HttpPost("Crear_Comprobante")]
        [Authorize]
        public IActionResult PostComprobante([FromBody] Comprobante comprobante)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                db.Comprobantes.Add(comprobante);
                db.SaveChanges();

                // Aquí utilizamos el método del controlador Consultas para obtener el comprobante creado
                var nuevoComprobante = _consultasController.GetComprobantePorId(comprobante.Numerodocumento!) as ObjectResult;
                if (nuevoComprobante == null || nuevoComprobante.Value == null)
                {
                    return NotFound();
                }

                return CreatedAtAction(nameof(GetComprobantePorId), new { id = comprobante.Idcomprobante }, comprobante);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }


        // PUT: api/Comprobantes/5
        [HttpPut("Editar_Comprobante/{id}")]
        [Authorize]
        public IActionResult PutComprobante(int id, [FromBody] Comprobante comprobante)
        {
            if (id != comprobante.Idcomprobante)
            {
                return BadRequest();
            }

            db.Entry(comprobante).State = EntityState.Modified;

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

        // DELETE: api/Comprobantes/5
        [HttpDelete("Eliminar_Comprobante/{id}")]
        [Authorize]
        public IActionResult DeleteComprobante(int id)
        {
            var comprobante = db.Comprobantes.Find(id);
            if (comprobante == null)
            {
                return NotFound();
            }

            db.Comprobantes.Remove(comprobante);
            db.SaveChanges();
            return NoContent();
        }
        private IActionResult GetComprobantePorId(string id)
        {
            var comprobante = db.Comprobantes.FirstOrDefault(c => c.Numerodocumento == id);
            if (comprobante == null)
            {
                return NotFound();
            }
            return Ok(comprobante);
        }
    }
}
