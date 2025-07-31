using Shared.Domain.Attributes;

namespace Shared.Domain.Entities;

[Table("users")]
public class User
{
    [Column("id")]
    public int Id { get; set; }
    
    [Column("name", maxLength: 100)]
    public string Name { get; set; } = string.Empty;
    
    [Column("password", maxLength: 255)]
    public string Password { get; set; } = string.Empty;
    
    // Navigation properties for favorites
    public virtual ICollection<UserFavorite> UserFavorites { get; set; } = new List<UserFavorite>();
} 