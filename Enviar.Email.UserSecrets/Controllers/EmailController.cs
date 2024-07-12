using Enviar.Email.UserSecrets.Model;
using Microsoft.AspNetCore.Mvc;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Enviar.Email.UserSecrets.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmailController : ControllerBase
    {
        #region Variaveis
        
        private readonly string _usuarioSmtp = "seu-email-cadastrado-no-sendgrid@gmail.com";
        private readonly IConfiguration _config;

        #endregion


        #region Construtores

        public EmailController(IConfiguration config)
        {
            _config = config;
        }
        
        #endregion

       
        #region Controllers
        
        [HttpPost]
        public async Task<IActionResult> SendEmailAsync(EmailRequest emailRequest)
        {
            var enviadoComSucesso = await EnviarEmail(emailRequest.Assunto, emailRequest.Destinatario, emailRequest.Corpo);

            if (enviadoComSucesso)
                return Ok("E-mail enviado com sucesso!");


            return BadRequest("Erro ao enviar e-mail:");

        }
        
        #endregion


        #region Metodos

        protected async Task<bool> EnviarEmail(string assunto, string destinatario, string corpo)
        {
            //config
            var apiKey = _config.GetValue<string>("Providers:Sendgrid:ApiKey");
            var client = new SendGridClient(apiKey);

            //Envolvidos
            var from = new EmailAddress(_usuarioSmtp, "BDS Atendiemnto");
            var subject = assunto;
            var to = new EmailAddress(destinatario);

            //Conteúdo
            var htmlContent = "<strong>" + corpo + "</strong>";

            //envio
            var msg = MailHelper.CreateSingleEmail(from, to, subject, corpo, htmlContent);
            var response = await client.SendEmailAsync(msg);

            //validação retorno
            if (response.IsSuccessStatusCode)
                return true;

            return false;
        }
        
        #endregion

    }
}

