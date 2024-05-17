namespace EventEase_01.Services
{
    public class OTPService
    {
        public string GenerateOTP()
        {
            Random random = new Random();
            return random.Next(1000, 9999).ToString("D4");
        }
    }

}
