using FluentMigrator;
using MigrationService.Tool.Constants;

namespace MigrationService.Tool.Migrations;

[Migration(002)]
public class CreateCurrenciesTable : Migration
{
    public override void Up()
    {
        Create.Table(DatabaseConstants.Tables.Currency)
            .WithColumn(DatabaseConstants.Columns.Currency.Id).AsInt32().PrimaryKey().Identity()
            .WithColumn(DatabaseConstants.Columns.Currency.Name).AsString(DatabaseConstants.DataTypes.CurrencyNameMaxLength).NotNullable()
            .WithColumn(DatabaseConstants.Columns.Currency.Rate).AsDecimal(DatabaseConstants.DataTypes.CurrencyRatePrecision, DatabaseConstants.DataTypes.CurrencyRateScale).NotNullable();
    }

    public override void Down()
    {
        Delete.Table(DatabaseConstants.Tables.Currency);
    }
}