using ApiFluentResults.Domain.Errors;
using FluentResults;

namespace ApiFluentResult.Services
{
    public class UserService
    {
        private readonly List<User> _users;

        public UserService(List<User> users)
        {
            _users = users;
        }

        public async Task<Result<User>> Get(int id)
        {
            var user = _users.FirstOrDefault(u => u.Id == id);
            if (user is null)
            {
                return Result.Fail<User>(new NotFoundError("User not found"));
            }
            return Result.Ok(user);
        }

        public async Task<Result<List<User>>> GetAll()
        {
            return Result.Ok(_users);
        }

        public async Task<Result<User>> Create(User user)
        {
            var userExists = _users.FirstOrDefault(u => u.Email == user.Email);
            if (userExists is not null)
            {
                return Result.Fail<User>(new UserEmailAlreadyExistsError(user.Email));
            }
            _users.Add(user);
            return Result.Ok(user);
        }
    }
}
