using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AppWebApiGMINGENIEROS.Models;

using Microsoft.EntityFrameworkCore;
using SendGrid.Helpers.Mail;
using Microsoft.Data.SqlClient;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AppWebApiGMINGENIEROS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

    public class toDoController : ControllerBase

    {
        private readonly DatadecomprasgmContext db;
        public toDoController(DatadecomprasgmContext ctx)
        {
            db = ctx;
        }

        // GET: api/<ToDoList>
        [HttpGet("Obtener_Las_Tareas")]
        public List<toDoList> GetTareas()
        {
            return db.ToDoList.ToList();
        }


        [HttpPost("Agregar_Tarea/{tarea}/{completado}")]
        [Authorize]
        public IActionResult AgregarTarea(string tarea, bool completado)
        {
            try
            {
                var tareaParam = new SqlParameter("@Tarea", tarea);
                var completadoParam = new SqlParameter("@Completado", completado);

                db.Database.ExecuteSqlRaw("EXEC InsertarTarea @Tarea, @Completado", tareaParam, completadoParam);

                return StatusCode(StatusCodes.Status201Created);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }



        [HttpPut("ActualizarEstadoTarea/{id}")]
        [Authorize]
        public IActionResult ActualizarEstadoTarea(int id)
        {
            try
            {
                var idParam = new SqlParameter("@Id", id);
                db.Database.ExecuteSqlRaw("EXEC ActualizarEstadoTarea @Id", idParam);

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        [HttpPut("Actualizar_Tarea/{id}/{nuevaTarea}")]
        [Authorize]
        public IActionResult ActualizarTarea(int id, string nuevaTarea)
        {
            if (string.IsNullOrWhiteSpace(nuevaTarea))
            {
                return BadRequest("El nuevo valor de la tarea no puede estar vacío.");
            }

            try
            {
                var idParam = new SqlParameter("@Id", id);
                var tareaParam = new SqlParameter("@Tarea", nuevaTarea);
                db.Database.ExecuteSqlRaw("EXEC ActualizarTarea @Id, @Tarea", idParam, tareaParam);

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }


    }
}
