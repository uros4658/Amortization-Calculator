using AmortizationCalc.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AmortizationCalc.Interfaces
{
    public interface IUserService
    {
        Task<User> Register(UserDto request);
        Task<User> Login(UserDto request);
        Task<IEnumerable<User>> GetAllUsers();
        Task<int> DeleteAllUsers();
    }
}
