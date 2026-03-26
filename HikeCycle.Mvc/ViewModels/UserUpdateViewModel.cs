public class UserUpdateViewModel {
    public int Id { get; set; }
    public string Email { get; set; } = "";
    public string Role { get; set; } = "user";
    public string? FullName { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
}