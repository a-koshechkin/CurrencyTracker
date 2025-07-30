using FluentMigrator;

namespace MigrationService.Tool.Migrations;

[Migration(002)]
public class CreateCurrenciesTable : Migration
{
    public override void Up()
    {
        Create.Table("currency")
            .WithColumn("id").AsInt32().PrimaryKey().Identity()
            .WithColumn("name").AsString(100).NotNullable()
            .WithColumn("rate").AsDecimal(18, 6).NotNullable();
    }

    public override void Down()
    {
        Delete.Table("currency");
    }
}