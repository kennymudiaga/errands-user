namespace Errands.Users.Domain.Config
{
    public class UserPolicy
    {
        public bool EnableLockout { get; set; }
        public int MaxPasswordFailCount { get; set; }
        public int PasswordLockoutMinutes { get; set; }
        public int TokenExpiryMinutes { get; set; }
        public int SignupExpiryMinutes { get; set; }
        public int SessionTimeout { get; set; }
    }
}
