using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WEBAPIGMINGENIEROSHTTPS.Models;
using System.Net.Mail;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using WEBAPIGMINGENIEROSHTTPS.Custom;



// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WEBAPIGMINGENIEROSHTTPS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    

    public class toDoController : ControllerBase

    {
        private readonly DatadecomprasgmContext db;
        private readonly Utilidades util;
        private readonly AccesoController accesoController;

        public toDoController(DatadecomprasgmContext ctx, Utilidades utilidades, AccesoController accesoController)
        {
            db = ctx;
            util = utilidades;
            this.accesoController = accesoController;
        }

        [HttpPost]
        [Route("RegistrarseTodoList/{nombre}/{correo}/{clave}")]
        public async Task<IActionResult> Registrarse(string nombre, string correo, string clave)
        {
            // Verificar si el correo ya está registrado en UsuariosTodoList
            var usuarioExistente = await db.UsuariosTodoList
                .FirstOrDefaultAsync(u => u.Correo == correo);

            if (usuarioExistente != null)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { isSuccess = false, message = "El correo electrónico ya está registrado" });
            }

            var codigoVerificacion = new Random().Next(100000, 999999).ToString();

            var envioExitoso = await accesoController.EnviarCodigoVerificacion(correo, codigoVerificacion);
            if (!envioExitoso)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { isSuccess = false, message = "Error al enviar el correo de verificación" });
            }

            var modeloUsuario = new UsuariosTodoList
            {
                Nombre = nombre,
                Correo = correo,
                Clave = util.encriptarSHA256(clave),
                CodigoVerificacion = codigoVerificacion,
                Verificado = false
            };

            await db.UsuariosTodoList.AddAsync(modeloUsuario);
            await db.SaveChangesAsync();

            if (modeloUsuario.IdUsuario != 0)
                return StatusCode(StatusCodes.Status200OK, new { isSuccess = true });
            else
                return StatusCode(StatusCodes.Status200OK, new { isSuccess = false });
        }




        [HttpPost]
        [Route("VerificarCorreo")]
        public async Task<IActionResult> VerificarCorreo([FromQuery] string correo, [FromQuery] string codigo)
        {
            var usuario = await db.UsuariosTodoList.FirstOrDefaultAsync(u => u.Correo == correo && u.CodigoVerificacion == codigo);

            if (usuario == null)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { isSuccess = false, message = "Código de verificación incorrecto" });
            }

            usuario.Verificado = true;
            usuario.CodigoVerificacion = null; // Opcional: eliminar el código de verificación una vez verificado
            await db.SaveChangesAsync();

            return StatusCode(StatusCodes.Status200OK, new { isSuccess = true });
        }

        [HttpPost]
        [Route("Login/{correo}/{clave}")]
        public async Task<IActionResult> Login(string correo, string clave)
        {
            var claveEncriptada = util.encriptarSHA256(clave);

            // Buscar el usuario por correo y clave encriptada
            var UsuarioEncontrado = await db.UsuariosTodoList
                .Where(u => u.Correo == correo && u.Clave == claveEncriptada)
                .FirstOrDefaultAsync();

            if (UsuarioEncontrado == null)
            {
                // El correo o la contraseña son incorrectos
                return StatusCode(StatusCodes.Status200OK, new { isSuccess = false, token = "", message = "Correo o contraseña incorrectos" });
            }
            else if (!UsuarioEncontrado.Verificado)
            {
                // El usuario no está verificado
                return StatusCode(StatusCodes.Status200OK, new { isSuccess = false, token = "", message = "El correo electrónico no ha sido verificado" });
            }
            else
            {
                // Generar JWT token
                var token = util.generarJWT(UsuarioEncontrado);

                // Devolver el token junto con el ID del usuario
                return StatusCode(StatusCodes.Status200OK, new
                {
                    isSuccess = true,
                    token = token,
                    userId = UsuarioEncontrado.IdUsuario
                });
            }
        }





        [HttpPost("Obtener_Las_Tareas/{IdUsuario}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public List<toDoList> GetTareas(int IdUsuario)
        {
            // Crear un parámetro para el IdUsuario
            var usuarioidParam = new SqlParameter("@Id", IdUsuario);

            // Ejecutar el procedimiento almacenado y obtener las tareas
            var tareas = db.ToDoList.FromSqlRaw("EXEC TareasDeUnUsuario @Id", usuarioidParam).ToList();

            return tareas;
        }



        [HttpPost("Agregar_Tarea/{tarea}/{completado}/{IdUsuario}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

        public IActionResult AgregarTarea(string tarea, bool completado,int IdUsuario)
        {
            try
            {
                var tareaParam = new SqlParameter("@Tarea", tarea);
                var completadoParam = new SqlParameter("@Completado", completado);
                var usuarioParam = new SqlParameter("@UsuarioId",IdUsuario);

                db.Database.ExecuteSqlRaw("EXEC InsertarTarea @Tarea, @Completado, @UsuarioId", tareaParam, completadoParam,usuarioParam);

                return StatusCode(StatusCodes.Status201Created);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }



        [HttpPut("ActualizarEstadoTarea/{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

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
