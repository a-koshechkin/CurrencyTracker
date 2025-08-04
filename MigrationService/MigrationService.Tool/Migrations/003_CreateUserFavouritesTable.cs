using FluentMigrator;
using MigrationService.Tool.Constants;

namespace MigrationService.Tool.Migrations;

[Migration(003)]
public class CreateUserfavoritesTable : Migration
{
    public override void Up()
    {
        Create.Table(DatabaseConstants.Tables.UserFavorites)
            .WithColumn(DatabaseConstants.Columns.UserFavorites.UserId).AsInt32().NotNullable()
            .WithColumn(DatabaseConstants.Columns.UserFavorites.CurrencyId).AsInt32().NotNullable();

        Create.PrimaryKey("pk_user_favorites")
            .OnTable(DatabaseConstants.Tables.UserFavorites)
            .Columns(DatabaseConstants.Columns.UserFavorites.UserId, DatabaseConstants.Columns.UserFavorites.CurrencyId);

        Create.ForeignKey("fk_user_favorites_user")
            .FromTable(DatabaseConstants.Tables.UserFavorites).ForeignColumn(DatabaseConstants.Columns.UserFavorites.UserId)
            .ToTable(DatabaseConstants.Tables.User).PrimaryColumn(DatabaseConstants.Columns.User.Id);

        Create.ForeignKey("fk_user_favorites_currency")
            .FromTable(DatabaseConstants.Tables.UserFavorites).ForeignColumn(DatabaseConstants.Columns.UserFavorites.CurrencyId)
            .ToTable(DatabaseConstants.Tables.Currency).PrimaryColumn(DatabaseConstants.Columns.Currency.Id);
    }

    public override void Down()
    {
        Delete.Table(DatabaseConstants.Tables.UserFavorites);
    }
}