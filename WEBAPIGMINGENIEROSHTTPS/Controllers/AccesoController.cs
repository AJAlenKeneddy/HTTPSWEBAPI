using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WEBAPIGMINGENIEROSHTTPS.Models;
using WEBAPIGMINGENIEROSHTTPS.Custom;
using WEBAPIGMINGENIEROSHTTPS.Models.Services;

namespace WEBAPIGMINGENIEROSHTTPS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccesoController : ControllerBase
    {
        private readonly DatadecomprasgmContext db;
        private readonly Utilidades util;
        private readonly EmailService emailService;

        public AccesoController(DatadecomprasgmContext datadecomprasgmContext, Utilidades utilidades, EmailService emailService)
        {
            db = datadecomprasgmContext;
            util = utilidades;
            this.emailService = emailService;
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromQuery] string correo, [FromQuery] string clave)
        {
            var claveEncriptada = util.encriptarSHA256(clave);
            var UsuarioEncontrado = await db.Usuarios
                .Where(u => u.Correo == correo && u.Clave == claveEncriptada)
                .FirstOrDefaultAsync();

            if (UsuarioEncontrado == null)
            {
                return StatusCode(StatusCodes.Status200OK, new { isSuccess = false, token = "" });
            }
            else
            {
                // Generar JWT token
                var token = util.generarJWT(UsuarioEncontrado);

                // Preparar el cuerpo del correo con el token
                var emailBody = $@"
                    <html>
                    <body>
                        <h2>Token de Inicio de Sesión</h2>
                        <p>Tu token JWT es:
                        <br>
                        <strong>{token}</strong></p>
                    </body>
                    </html>
                ";

                var emailSent = await emailService.SendEmailAsync(
                    "Remitente",
                    "alenaguilar24@gmail.com",
                    "Destinatario",
                    correo,
                    "Token de Inicio de Sesión",
                    emailBody);

                if (emailSent)
                {
                    return StatusCode(StatusCodes.Status200OK, new { isSuccess = true, message = "Envio exitoso. Verifica tu correo electrónico." });
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new { isSuccess = false, message = "Envio erroneo. Verifica tus Credenciales." });
                }
            }
        }

        [HttpPost]
        [Route("Registrarse")]
        public async Task<IActionResult> Registrarse([FromQuery] string nombre, [FromQuery] string correo, [FromQuery] string clave)
        {
            var modeloUsuario = new Usuario
            {
                Nombre = nombre,
                Correo = correo,
                Clave = util.encriptarSHA256(clave),
            };

            await db.Usuarios.AddAsync(modeloUsuario);
            await db.SaveChangesAsync();

            if (modeloUsuario.IdUsuario != 0)
                return StatusCode(StatusCodes.Status200OK, new { isSuccess = true });
            else
                return StatusCode(StatusCodes.Status200OK, new { isSuccess = false });
        }

        // Método interno no expuesto como endpoint
        internal async Task<bool> EnviarCodigoVerificacion(string correo, string codigo)
        {
            var emailBody = $"Tu código de verificación es: {codigo}";
            return await emailService.SendEmailAsync(
                "Tu Nombre",
                "tuemail@example.com",
                "Destinatario",
                correo,
                "Código de verificación",
                emailBody);
        }
    }
}
