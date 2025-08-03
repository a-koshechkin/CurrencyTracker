using FluentMigrator;

namespace MigrationService.Tool.Migrations;

[Migration(003)]
public class CreateUserfavoritesTable : Migration
{
    public override void Up()
    {
        Create.Table("user_favorites")
            .WithColumn("user_id").AsInt32().NotNullable()
            .WithColumn("currency_id").AsInt32().NotNullable();

        Create.PrimaryKey("pk_user_favorites")
            .OnTable("user_favorites")
            .Columns("user_id", "currency_id");

        Create.ForeignKey("fk_user_favorites_user")
            .FromTable("user_favorites").ForeignColumn("user_id")
            .ToTable("user").PrimaryColumn("id");

        Create.ForeignKey("fk_user_favorites_currency")
            .FromTable("user_favorites").ForeignColumn("currency_id")
            .ToTable("currency").PrimaryColumn("id");
    }

    public override void Down()
    {
        Delete.Table("user_favorites");
    }
}