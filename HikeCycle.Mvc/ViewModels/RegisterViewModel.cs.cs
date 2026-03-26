namespace HikeCycle.Mvc.ViewModels;

    public class RegisterViewModel
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
        public string? FullName { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
    }