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

        Insert.IntoTable("user").Row(new
        {
            name = "john_doe",
            password = "$2a$12$5qIf.dz9JhgK5y.52r6G7OeO41uHgwEki/FAVc8sqqOLmhmu7A8Ge"
        });

        Insert.IntoTable("user").Row(new
        {
            name = "jane_smith",
            password = "$2a$12$zAOVU8lkmAO5e6uBm7rh0Oz3VKRItnBYjsXWrg0Df.0g/l1pMKiE2"
        });

        Insert.IntoTable("user_favorites").Row(new
        {
            user_id = 1,
            currency_id = 1
        });

        Insert.IntoTable("user_favorites").Row(new
        {
            user_id = 1,
            currency_id = 2
        });

        Insert.IntoTable("user_favorites").Row(new
        {
            user_id = 1,
            currency_id = 3
        });

        Insert.IntoTable("user_favorites").Row(new
        {
            user_id = 2,
            currency_id = 2
        });

        Insert.IntoTable("user_favorites").Row(new
        {
            user_id = 2,
            currency_id = 4
        });

        Insert.IntoTable("user_favorites").Row(new
        {
            user_id = 2,
            currency_id = 5
        });
    }

    public override void Down()
    {
        Delete.FromTable("user_favorites").AllRows();
        Delete.FromTable("user").AllRows();
        Delete.FromTable("currency").AllRows();
    }
} 