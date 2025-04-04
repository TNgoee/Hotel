public class AccountEditDto
{
    public required string UserName { get; set; }
    public required string PasswordHash { get; set; }
    public required string RoleId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}