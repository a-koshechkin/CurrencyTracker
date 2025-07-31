using FluentMigrator;

namespace MigrationService.Tool.Migrations;

[Migration(004)]
public class SeedInitialData : Migration
{
    public override void Up()
    {
        Insert.IntoTable("currency").Row(new
        {
            name = "USD",
            rate = 81.83m
        });

        Insert.IntoTable("currency").Row(new
        {
            name = "EUR",
            rate = 94.95m
        });

        Insert.IntoTable("currency").Row(new
        {
            name = "GBP",
            rate = 109.07m
        });

        Insert.IntoTable("currency").Row(new
        {
            name = "JPY",
            rate = 0.55m
        });

        Insert.IntoTable("currency").Row(new
        {
            name = "CAD",
            rate = 59.42m
        });

        Insert.IntoTable("currency").Row(new
        {
            name = "CHF",
            rate = 101.75m
        });

        Insert.IntoTable("currency").Row(new
        {
            name = "AUD",
            rate = 53.28m
        });
    }

    public override void Down()
    {
        Delete.FromTable("currency").AllRows();
    }
} 