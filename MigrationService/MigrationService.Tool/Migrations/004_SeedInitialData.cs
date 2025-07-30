using FluentMigrator;

namespace MigrationService.Tool.Migrations;

[Migration(004)]
public class SeedInitialData : Migration
{
    public override void Up()
    {
        Insert.IntoTable("currency").Row(new
        {
            name = "US Dollar",
            rate = 81.83m
        });

        Insert.IntoTable("currency").Row(new
        {
            name = "Euro",
            rate = 94.95m
        });

        Insert.IntoTable("currency").Row(new
        {
            name = "British Pound",
            rate = 109.07m
        });

        Insert.IntoTable("currency").Row(new
        {
            name = "Japanese Yen",
            rate = 0.55m
        });

        Insert.IntoTable("currency").Row(new
        {
            name = "Canadian Dollar",
            rate = 59.42m
        });

        Insert.IntoTable("currency").Row(new
        {
            name = "Swiss Franc",
            rate = 101.75m
        });

        Insert.IntoTable("currency").Row(new
        {
            name = "Australian Dollar",
            rate = 53.28m
        });
    }

    public override void Down()
    {
        Delete.FromTable("currency").AllRows();
    }
} 