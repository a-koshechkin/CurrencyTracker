using FluentMigrator;

namespace MigrationService.Tool.Migrations;

[Migration(003)]
public class CreateUserFavouritesTable : Migration
{
    public override void Up()
    {
        Create.Table("user_favourites")
            .WithColumn("user_id").AsInt32().NotNullable()
            .WithColumn("currency_id").AsInt32().NotNullable();

        Create.PrimaryKey("pk_user_favourites")
            .OnTable("user_favourites")
            .Columns("user_id", "currency_id");

        Create.ForeignKey("fk_user_favourites_user")
            .FromTable("user_favourites").ForeignColumn("user_id")
            .ToTable("user").PrimaryColumn("id");

        Create.ForeignKey("fk_user_favourites_currency")
            .FromTable("user_favourites").ForeignColumn("currency_id")
            .ToTable("currency").PrimaryColumn("id");
    }

    public override void Down()
    {
        Delete.Table("user_favourites");
    }
}