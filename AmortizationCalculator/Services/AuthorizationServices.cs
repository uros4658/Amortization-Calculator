using AmortizationCalc.Interfaces;
using AmortizationCalc.Models;
using Dapper;
using MySql.Data.MySqlClient;
using BCrypt;

namespace AuthorizationServices
{
    public class UserService : IUserService
    {
        private readonly IConfiguration _config;

        public UserService(IConfiguration config)
        {
            _config = config;
        }

        public async Task<User> Register(UserDto request)
        {
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var user = new User
            {
                Username = request.Username,
                PasswordHash = passwordHash
            };

            var connection = new MySqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.OpenAsync();
            await connection.ExecuteAsync("INSERT INTO user (username, passwordHash) VALUES (@Username, @PasswordHash)", user);

            return user;
        }

        public async Task<User> Login(UserDto request)
        {
            var connection = new MySqlConnection(_config.GetConnectionString("DefaultConnection"));
            var user = await connection.QueryFirstOrDefaultAsync<User>("SELECT * FROM user WHERE username = @Username", new { request.Username });

            
            return user;    
        }

        public async Task<IEnumerable<User>> GetAllUsers()
        {
            using var connection = new MySqlConnection(_config.GetConnectionString("DefaultConnection"));
            return await connection.QueryAsync<User>("SELECT * FROM user");
        }

        public async Task<int> DeleteAllUsers()
        {
            using var connection = new MySqlConnection(_config.GetConnectionString("DefaultConnection"));
            return await connection.ExecuteAsync("DELETE FROM user");
        }

    }
}
