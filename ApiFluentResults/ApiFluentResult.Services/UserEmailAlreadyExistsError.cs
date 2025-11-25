using ApiFluentResults.Domain.Errors;

namespace ApiFluentResult.Services
{
    public class UserEmailAlreadyExistsError : DomainError
    {
        public UserEmailAlreadyExistsError(string email)
            : base($"Correo {email} ya existe", "EMAIL_ALREADY_EXISTS")
        {
            Metadata.Add("email", email);
        }
    }
}
