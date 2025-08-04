using Shared.Domain.Attributes;

namespace Shared.Domain.Entities;

[Table("currency")]
public class Currency
{
    [Column("id")]
    public int Id { get; set; }

    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Column("rate")]
    public decimal Rate { get; set; }

    public virtual ICollection<UserFavorite> UserFavorites { get; set; } = [];
}