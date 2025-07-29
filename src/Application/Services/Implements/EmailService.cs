using Resend;
using Tienda_UCN_api.Src.Application.Services.Interfaces;

namespace Tienda_UCN_api.Src.Application.Services.Implements
{
    /// <summary>
    /// Servicio para enviar correos electrónicos de verificación.
    /// </summary>
    public class EmailService : IEmailService
    {
        private readonly IResend _resend;

        public EmailService(IResend resend)
        {
            _resend = resend;
        }

        /// <summary>
        /// Envía un código de verificación al correo electrónico del usuario.
        /// </summary>
        /// <param name="email">El correo electrónico del usuario.</param>
        /// <param name="code">El código de verificación a enviar.</param>
        public async Task SendVerificationCodeEmailAsync(string email, string code)
        {
            var htmlBody = $@"
            <!DOCTYPE html>
            <html lang=""es"">
            <head>
                <meta charset=""UTF-8"">
                <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                <title>Código de Verificación - Tienda UCN</title>
                <style>
                    body {{
                        font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
                        line-height: 1.6;
                        color: #333;
                        max-width: 600px;
                        margin: 0 auto;
                        padding: 20px;
                        background-color: #f8f9fa;
                    }}
                    .container {{
                        background-color: #ffffff;
                        border-radius: 10px;
                        box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
                        overflow: hidden;
                    }}
                    .header {{
                        background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
                        color: white;
                        padding: 30px 20px;
                        text-align: center;
                    }}
                    .header h1 {{
                        margin: 0;
                        font-size: 28px;
                        font-weight: 300;
                    }}
                    .content {{
                        padding: 40px 30px;
                        text-align: center;
                    }}
                    .welcome {{
                        font-size: 18px;
                        color: #555;
                        margin-bottom: 25px;
                    }}
                    .code-container {{
                        background: linear-gradient(135deg, #f093fb 0%, #f5576c 100%);
                        border-radius: 10px;
                        padding: 25px;
                        margin: 30px 0;
                        box-shadow: 0 4px 15px rgba(240, 147, 251, 0.3);
                    }}
                    .code {{
                        font-size: 36px;
                        font-weight: bold;
                        color: white;
                        letter-spacing: 8px;
                        font-family: 'Courier New', monospace;
                        text-shadow: 2px 2px 4px rgba(0, 0, 0, 0.3);
                    }}
                    .code-label {{
                        font-size: 14px;
                        color: rgba(255, 255, 255, 0.9);
                        margin-bottom: 10px;
                        text-transform: uppercase;
                        letter-spacing: 1px;
                    }}
                    .instructions {{
                        background-color: #e3f2fd;
                        border-left: 4px solid #2196f3;
                        padding: 20px;
                        margin: 25px 0;
                        border-radius: 0 8px 8px 0;
                    }}
                    .instructions h3 {{
                        margin-top: 0;
                        color: #1976d2;
                        font-size: 16px;
                    }}
                    .instructions ol {{
                        margin: 10px 0;
                        padding-left: 20px;
                        text-align: left;
                    }}
                    .instructions li {{
                        margin: 8px 0;
                        color: #555;
                    }}
                    .warning {{
                        background-color: #fff3cd;
                        border: 1px solid #ffeaa7;
                        border-radius: 8px;
                        padding: 15px;
                        margin: 20px 0;
                        color: #856404;
                    }}
                    .warning strong {{
                        color: #d63031;
                    }}
                    .footer {{
                        background-color: #f8f9fa;
                        padding: 25px;
                        text-align: center;
                        color: #6c757d;
                        font-size: 14px;
                        border-top: 1px solid #e9ecef;
                    }}
                    .footer a {{
                        color: #667eea;
                        text-decoration: none;
                    }}
                    .security-note {{
                        font-size: 12px;
                        color: #6c757d;
                        margin-top: 15px;
                        font-style: italic;
                    }}
                    @media only screen and (max-width: 600px) {{
                        body {{
                            padding: 10px;
                        }}
                        .content {{
                            padding: 30px 20px;
                        }}
                        .code {{
                            font-size: 28px;
                            letter-spacing: 4px;
                        }}
                    }}
                </style>
            </head>
            <body>
                <div class=""container"">
                    <!-- Header -->
                    <div class=""header"">
                        <h1>Tienda UCN</h1>
                        <p style=""margin: 10px 0 0 0; opacity: 0.9;"">Universidad Católica del Norte</p>
                    </div>

                    <!-- Main Content -->
                    <div class=""content"">
                        <div class=""welcome"">
                            ¡Hola! 👋<br>
                            Nos alegra que <strong>Tienda UCN</strong> sea parte de tu experiencia universitaria.
                        </div>

                        <!-- Verification Code -->
                        <div class=""code-container"">
                            <div class=""code-label"">Tu código de verificación es:</div>
                            <div class=""code"">{code}</div>
                        </div>

                        <!-- Instructions -->
                        <div class=""instructions"">
                            <h3>Instrucciones:</h3>
                            <ol>
                                <li>Copia el código de verificación de arriba</li>
                                <li>Regresa a la aplicación de Tienda UCN</li>
                                <li>Pega el código en el campo correspondiente</li>
                                <li>Haz click en ""Verificar"" para completar tu registro</li>
                            </ol>
                        </div>

                        <!-- Warning -->
                        <div class=""warning"">
                            <strong>Importante:</strong> Este código expirará pronto por motivos de seguridad.
                            Si no lo usas a tiempo, deberás solicitar uno nuevo.
                        </div>

                        <div class=""security-note"">
                            Por tu seguridad, nunca compartas este código con nadie.<br>
                            El equipo de Tienda UCN jamás te pedirá este código por teléfono o email.
                        </div>
                    </div>

                    <!-- Footer -->
                    <div class=""footer"">
                        <p>
                            <strong>Tienda UCN</strong> - Tu tienda universitaria de confianza<br>
                            Universidad Católica del Norte
                        </p>
                        <div style=""margin-top: 15px; font-size: 12px; color: #999;"">
                            Si no solicitaste este código, puedes ignorar este email de forma segura.
                        </div>
                    </div>
                </div>
            </body>
            </html>";

            var message = new EmailMessage
            {
                To = email,
                Subject = "Código de Verificación - Tienda UCN",
                From = "Tienda - UCN <onboarding@resend.dev>",
                HtmlBody = htmlBody
            };
            await _resend.EmailSendAsync(message);
        }

        /// <summary>
        /// Envía un correo electrónico de bienvenida al usuario.
        /// </summary>
        /// <param name="email">El correo electrónico del usuario.</param>
        public async Task SendWelcomeEmailAsync(string email)
        {
            var htmlBody = $@"
            <!DOCTYPE html>
            <html lang=""es"">
            <head>
                <meta charset=""UTF-8"">
                <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                <title>¡Bienvenido a Tienda UCN!</title>
                <style>
                    body {{
                        font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
                        line-height: 1.6;
                        color: #333;
                        max-width: 600px;
                        margin: 0 auto;
                        padding: 20px;
                        background-color: #f8f9fa;
                    }}
                    .container {{
                        background-color: #ffffff;
                        border-radius: 15px;
                        box-shadow: 0 8px 20px rgba(0, 0, 0, 0.1);
                        overflow: hidden;
                    }}
                    .header {{
                        background: linear-gradient(135deg, #28a745 0%, #20c997 100%);
                        color: white;
                        padding: 40px 30px;
                        text-align: center;
                    }}
                    .header h1 {{
                        margin: 0;
                        font-size: 32px;
                        font-weight: 300;
                        text-shadow: 2px 2px 4px rgba(0, 0, 0, 0.2);
                    }}
                    .welcome-icon {{
                        font-size: 48px;
                        margin-bottom: 15px;
                        display: block;
                    }}
                    .content {{
                        padding: 40px 30px;
                        text-align: center;
                    }}
                    .main-message {{
                        font-size: 20px;
                        color: #28a745;
                        margin-bottom: 25px;
                        font-weight: 600;
                    }}
                    .description {{
                        font-size: 16px;
                        color: #6c757d;
                        margin-bottom: 30px;
                        line-height: 1.7;
                    }}
                    .features-container {{
                        background: linear-gradient(135deg, #e8f5e8 0%, #f0fdf4 100%);
                        border-radius: 12px;
                        padding: 30px;
                        margin: 30px 0;
                        text-align: left;
                    }}
                    .features-title {{
                        text-align: center;
                        color: #28a745;
                        font-size: 18px;
                        font-weight: 600;
                        margin-bottom: 20px;
                    }}
                    .feature-item {{
                        display: flex;
                        align-items: center;
                        margin: 15px 0;
                        padding: 10px 0;
                    }}
                    .feature-icon {{
                        font-size: 24px;
                        margin-right: 15px;
                        width: 30px;
                        text-align: center;
                    }}
                    .feature-text {{
                        color: #495057;
                        font-size: 15px;
                    }}
                    .cta-container {{
                        background: linear-gradient(135deg, #007bff 0%, #0056b3 100%);
                        border-radius: 12px;
                        padding: 25px;
                        margin: 30px 0;
                        text-align: center;
                    }}
                    .cta-title {{
                        color: white;
                        font-size: 18px;
                        font-weight: 600;
                        margin-bottom: 15px;
                    }}
                    .cta-button {{
                        display: inline-block;
                        background-color: #ffffff;
                        color: #007bff;
                        padding: 15px 30px;
                        text-decoration: none;
                        border-radius: 8px;
                        font-weight: 600;
                        font-size: 16px;
                        box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
                        transition: all 0.3s ease;
                    }}
                    .cta-button:hover {{
                        background-color: #f8f9fa;
                        transform: translateY(-2px);
                        box-shadow: 0 6px 12px rgba(0, 0, 0, 0.15);
                    }}
                    .tips-container {{
                        background-color: #fff3cd;
                        border: 1px solid #ffeaa7;
                        border-radius: 10px;
                        padding: 20px;
                        margin: 25px 0;
                    }}
                    .tips-title {{
                        color: #856404;
                        font-size: 16px;
                        font-weight: 600;
                        margin-bottom: 15px;
                        text-align: center;
                    }}
                    .tip-item {{
                        color: #856404;
                        margin: 8px 0;
                        padding-left: 20px;
                        position: relative;
                    }}
                    .tip-item::before {{
                        content: '💡';
                        position: absolute;
                        left: 0;
                        top: 0;
                    }}
                    .footer {{
                        background-color: #f8f9fa;
                        padding: 30px;
                        text-align: center;
                        color: #6c757d;
                        font-size: 14px;
                        border-top: 1px solid #e9ecef;
                    }}
                    .footer a {{
                        color: #28a745;
                        text-decoration: none;
                        font-weight: 500;
                    }}
                    .social-links {{
                        margin-top: 20px;
                    }}
                    .social-links a {{
                        display: inline-block;
                        margin: 0 10px;
                        font-size: 24px;
                        text-decoration: none;
                    }}
                    @media only screen and (max-width: 600px) {{
                        body {{
                            padding: 10px;
                        }}
                        .content {{
                            padding: 30px 20px;
                        }}
                        .header {{
                            padding: 30px 20px;
                        }}
                        .header h1 {{
                            font-size: 28px;
                        }}
                        .welcome-icon {{
                            font-size: 40px;
                        }}
                    }}
                </style>
            </head>
            <body>
                <div class=""container"">
                    <!-- Header -->
                    <div class=""header"">
                        <span class=""welcome-icon"">🎉</span>
                        <h1>¡Bienvenido a Tienda UCN!</h1>
                        <p style=""margin: 10px 0 0 0; opacity: 0.9; font-size: 16px;"">Universidad Católica del Norte</p>
                    </div>

                    <!-- Main Content -->
                    <div class=""content"">
                        <div class=""main-message"">
                            ¡Tu cuenta ha sido verificada exitosamente! 🎓
                        </div>

                        <div class=""description"">
                            Nos emociona tenerte como parte de la <strong>comunidad UCN</strong>.
                            Tienda UCN es tu espacio digital para descubrir productos únicos.
                        </div>

                        <!-- Features -->
                        <div class=""features-container"">
                            <div class=""features-title"">¿Qué puedes hacer ahora?</div>

                            <div class=""feature-item"">
                                <span class=""feature-icon"">🛍️</span>
                                <span class=""feature-text""><strong>Explorar productos</strong> - Descubre artículos únicos y ofertas especiales</span>
                            </div>

                            <div class=""feature-item"">
                                <span class=""feature-icon"">🎯</span>
                                <span class=""feature-text""><strong>Ofertas exclusivas</strong> - Accede a descuentos únicos</span>
                            </div>

                            <div class=""feature-item"">
                                <span class=""feature-icon"">📱</span>
                                <span class=""feature-text""><strong>Compra fácil</strong> - Proceso de compra simple y seguro</span>
                            </div>
                        </div>

                        <!-- Call to Action -->
                        <div class=""cta-container"">
                            <div class=""cta-title"">¡Comienza a explorar ahora!</div>
                        </div>

                        <!-- Tips -->
                        <div class=""tips-container"">
                            <div class=""tips-title"">💡 Consejos para aprovechar al máximo tu experiencia</div>
                            <div class=""tip-item"">Completa tu perfil</div>
                            <div class=""tip-item"">Sigue nuestras redes sociales para no perderte las ofertas flash</div>
                            <div class=""tip-item"">Invita a tus amigos</div>
                        </div>
                    </div>

                    <!-- Footer -->
                    <div class=""footer"">
                        <p>
                            <strong>Tienda UCN</strong> - Tu marketplace de confianza<br>
                            Universidad Católica del Norte
                        </p>

                        <p style=""margin-top: 20px;"">
                            ¿Necesitas ayuda? Contáctanos en <a href=""mailto:jorge.rivera01@ce.ucn.cl"">jorge.rivera01@ce.ucn.cl</a><br>
                            O visita nuestro <a href=""#"">Centro de Ayuda</a>
                        </p>

                        <div style=""margin-top: 20px; font-size: 12px; color: #999;"">
                            Has recibido este email porque te registraste en Tienda UCN.<br>
                            Si tienes alguna pregunta, no dudes en contactarnos.
                        </div>
                    </div>
                </div>
            </body>
            </html>";

            var message = new EmailMessage
            {
                To = email,
                Subject = "Bienvenido a Tienda UCN",
                From = "Tienda - UCN <onboarding@resend.dev>",
                HtmlBody = htmlBody
            };

            await _resend.EmailSendAsync(message);
        }
    }
}
