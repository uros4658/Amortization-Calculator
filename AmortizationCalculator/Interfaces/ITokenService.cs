using AmortizationCalc.Models;

namespace AmortizationCalc.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(User user);
        RefreshToken GenerateRefreshToken();
        void SetCookie(HttpResponse response, string token);
        void SetRefreshToken(User user, RefreshToken newRefreshToken);
    }
}
