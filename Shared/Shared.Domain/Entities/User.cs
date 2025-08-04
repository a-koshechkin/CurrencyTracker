using Shared.Domain.Attributes;

namespace Shared.Domain.Entities;

[Table("user")]
public class User
{
    [Column("id")]
    public int Id { get; set; }
    
    [Column("name")]
    public string Name { get; set; } = string.Empty;
    
    [Column("password")]
    public string Password { get; set; } = string.Empty;
} 