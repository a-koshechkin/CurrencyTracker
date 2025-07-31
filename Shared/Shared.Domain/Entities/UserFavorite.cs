using Shared.Domain.Attributes;

namespace Shared.Domain.Entities;

[Table("user_favorites")]
public class UserFavorite
{
    [Column("id")]
    public int Id { get; set; }
    
    [Column("user_id")]
    public int UserId { get; set; }
    
    [Column("currency_id")]
    public int CurrencyId { get; set; }
    
    // Navigation properties
    public virtual User User { get; set; } = null!;
    public virtual Currency Currency { get; set; } = null!;
} 