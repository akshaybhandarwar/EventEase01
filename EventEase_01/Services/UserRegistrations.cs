using EventEase_01.Models;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNetCore.Mvc;

namespace EventEase_01.Services
{

    public class UserRegistrations
    {
        private readonly EventEase01Context _context;
        private readonly IConfiguration _config;
        private readonly AESEncryption _encryptionService;

        public UserRegistrations(EventEase01Context context, IConfiguration config, AESEncryption encryptionService)
        {
            _context = context;
            _config = config;
            _encryptionService = encryptionService;
        }

        public async Task<bool> RegisterUserAsync(RegistrationModel model)
        {

            string PassKey = _config["PasswordKey"];
            string PasswordSalt = null;
            var PasswordHash = _encryptionService.Encrypt(model.Password, out PasswordSalt, PassKey);
            User u1 = new User
            {
                UserName = model.UserName,
                UserEmail = model.Email,
                PasswordHash = PasswordHash,
                PasswordSalt = PasswordSalt,
            };
            await _context.Users.AddAsync(u1);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
