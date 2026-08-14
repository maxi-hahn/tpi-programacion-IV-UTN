namespace Application.Templates
{
    public static class EmailTemplates
    {
        public static string VerifyAccount(string userName, string verificationLink)
        {
            return $@"
            <h2>¡Bienvenido {userName}!</h2>

            <p>Gracias por registrarte en Gym Management.</p>

            <p>Para activar tu cuenta hace click en el siguiente enlace:</p>

            <a href='{verificationLink}'>
                Verificar cuenta
            </a>

            <br/><br/>

            <p>Este enlace expirará en 24 horas.</p>";
        }

        public static string ResendVerification(string userName, string verificationLink)
        {
            return $@"
            <h2>Hola {userName}</h2>

            <p>Solicitaste un nuevo enlace de verificación.</p>

            <a href='{verificationLink}'>
                Verificar cuenta
            </a>";
        }

        public static string ResetPassword(string userName, string resetLink)
        {
            return $@"
            <h2>Hola {userName}</h2>

            <p>Recibimos una solicitud para restablecer tu contraseña.</p>

            <a href='{resetLink}'>
                Restablecer contraseña
            </a>

            <p>Este enlace expirará en 15 minutos.</p>";
        }
        public static string SubscriptionExpiring(
            string userName,
            DateTime expirationDate,
            int daysLeft)
        {
            return $@"
            <h2>Hola {userName}</h2>

            <p>Tu suscripción vencerá el día
            <strong>{expirationDate:dd/MM/yyyy}</strong>.</p>

            <p>Te quedan <strong>{daysLeft}</strong> días de servicio.</p>

            <p>Renová tu suscripción para seguir utilizando el sistema.</p>";
        }

        public static string SubscriptionExpired(string userName)
        {
            return $@"
            <h2>Hola {userName}</h2>

            <p>Tu suscripción ha expirado.</p>

            <p>Para volver a inscribirte en clases deberás renovar tu plan.</p>";
        }
        public static string UserDeactivated(string userName)
        {
            return $@"
            <h2>Hola {userName}</h2>

            <p>Tu cuenta ha sido <strong>desactivada</strong> por un administrador.</p>

            <p>Si crees que esto es un error, por favor contacta con soporte.</p>

            <p>No podrás acceder al sistema hasta que tu cuenta sea reactivada.</p>";
        }

        public static string UserActivated(string userName)
        {
            return $@"
            <h2>Hola {userName}</h2>

            <p>Tu cuenta ha sido <strong>activada</strong> nuevamente.</p>

            <p>Ya puedes acceder al sistema y disfrutar de nuestros servicios.</p>";
        }
    }
}