using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AppWebApiGMINGENIEROS.Custom;
using AppWebApiGMINGENIEROS.Models;
using AppWebApiGMINGENIEROS.Models.DTOs;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using MimeKit;
using MailKit.Net.Smtp;




namespace AppWebApiGMINGENIEROS.Controllers
{
    [Route("api/[controller]")]

    [ApiController]
    public class AccesoController : ControllerBase
    {
        private readonly DatadecomprasgmContext _datadecomprasgmContext;
        private readonly Utilidades _utilidades;


        public AccesoController(DatadecomprasgmContext datadecomprasgmContext, Utilidades utilidades)
        {
            _datadecomprasgmContext = datadecomprasgmContext;
            _utilidades = utilidades;

        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login(UsuarioDTO objeto)
        {
            var UsuarioEncontrado = await _datadecomprasgmContext.Usuarios
                .Where(u => u.Correo == objeto.Correo && u.Clave == _utilidades.encriptarSHA256(objeto.Clave))
                .FirstOrDefaultAsync();

            if (UsuarioEncontrado == null)
            {
                return StatusCode(StatusCodes.Status200OK, new { isSuccess = false, token = "" });
            }
            else
            {
                // Generar JWT token
                var token = _utilidades.generarJWT(UsuarioEncontrado);

                // Preparar el cuerpo del correo con el token
                
                var emailBody = $@"
                    <html>
                    <body>
                        <h2>Token de Inicio de Sesión</h2>
                        <p>Tu token JWT es:
                <br>
                <strong>{ token}</strong></p>
                    </body>
                    </html>
                ";

                // Configurar el mensaje de correo electrónico
                var emailMessage = new MimeMessage();
                emailMessage.From.Add(new MailboxAddress("Remitente", "alenaguilar24@gmail.com")); // Remitente
                emailMessage.To.Add(new MailboxAddress("Destinatario", objeto.Correo)); // Destinatario
                emailMessage.Subject = "Token de Inicio de Sesión"; // Asunto
                emailMessage.Body = new TextPart("html") { Text = emailBody }; // Cuerpo del mensaje

                try
                {
                    // Configurar cliente SMTP y enviar correo electrónico
                    using (var client = new SmtpClient())
                    {
                        await client.ConnectAsync("smtp.gmail.com", 587, false); // Conectar al servidor SMTP
                        await client.AuthenticateAsync("alenaguilar24@gmail.com", "krvd ajsr ruuf fwgj"); // Autenticar
                        await client.SendAsync(emailMessage); // Enviar correo electrónico
                        await client.DisconnectAsync(true); // Desconectar
                    }

                    return StatusCode(StatusCodes.Status200OK, new { isSuccess = true, message = "Envio exitoso. Verifica tu correo electrónico." });

                }
                catch (Exception ex)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new { isSuccess = false, message = "Envio erroneo. Verifica tus Credenciales."+ ex });
                }
            }
        }
    






[HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("Registrarse")]
        public async Task<IActionResult> Registrarse(UsuarioDTO objeto)
        {
            var modeloUsuario = new Usuario
            {
                Nombre = objeto.Nombre,
                Correo = objeto.Correo,
                Clave = _utilidades.encriptarSHA256(objeto.Clave),

            };
            await _datadecomprasgmContext.Usuarios.AddAsync(modeloUsuario);
            await _datadecomprasgmContext.SaveChangesAsync();

            if (modeloUsuario.IdUsuario != 0)
                return StatusCode(StatusCodes.Status200OK, new { isSuccess = true });
            else
                return StatusCode(StatusCodes.Status200OK, new { isSuccess = false });



        }




    }
}
