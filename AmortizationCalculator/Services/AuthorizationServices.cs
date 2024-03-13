using AmortizationCalc.Interfaces;
using AmortizationCalc.Models;
using Dapper;
using MySql.Data.MySqlClient;
using BCrypt;
using System.IdentityModel.Tokens.Jwt;

namespace AuthorizationServices
{
    public class UserService : IUserService
    {
        private readonly IConfiguration _config;
        private MySqlConnection _connection;

        public UserService(IConfiguration config)
        {
            _config = config;
            _connection = new MySqlConnection(_config.GetConnectionString("DefaultConnection"));
        }

        public async Task<User> Register(UserDto request)
        {
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var user = new User
            {
                Username = request.Username,
                PasswordHash = passwordHash
            };

            await _connection.ExecuteAsync("INSERT INTO user (username, passwordHash) VALUES (@Username, @PasswordHash)", user);

            return user;
        }

        public async Task<User> Login(UserDto request)
        {
            var user = await _connection.QueryFirstOrDefaultAsync<User>("SELECT * FROM user WHERE username = @Username", new { request.Username });
            return user;    
        }

        public async Task<IEnumerable<User>> GetAllUsers()
        {
            return await _connection.QueryAsync<User>("SELECT * FROM user");
        }

        public async Task<int> DeleteAllUsers()
        {
            return await _connection.ExecuteAsync("DELETE FROM user");
        }

    }
}
