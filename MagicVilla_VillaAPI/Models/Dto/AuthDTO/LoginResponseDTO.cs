using MagicVilla_VillaAPI.Models.Auth;

namespace MagicVilla_VillaAPI.Models.Dto.AuthDTO
{
    public class LoginResponseDTO
    {
        public LocalUser User { get; set; }
        public string Token { get; set; }

    }
}
