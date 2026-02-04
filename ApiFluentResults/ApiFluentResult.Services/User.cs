using ApiFluentResults.Domain.Errors;
using FluentResults;

namespace ApiFluentResult.Services
{
    public class User
    {
        public User() { }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }

        public static Result<User> Create(int id, string name, string email)
        {
            if (id == 0)
                return Result.Fail<User>(new DomainError("Id cannot be 0"));

            if (name.Length < 3)
                return Result.Fail<User>(new MaximumLengthInvalidError(3));

            return new User
            {
                Id = id,
                Name = name,
                Email = email,
            };
        }
    }
}
