using FluentMigrator;
using MigrationService.Tool.Constants;

namespace MigrationService.Tool.Migrations;

[Migration(004)]
public class SeedInitialData : Migration
{
    public override void Up()
    {

        Insert.IntoTable(DatabaseConstants.Tables.Currency)
            .Row(new { name = "USD", rate = 81.83m })
            .Row(new { name = "EUR", rate = 94.95m })
            .Row(new { name = "GBP", rate = 109.07m })
            .Row(new { name = "JPY", rate = 0.55m })
            .Row(new { name = "CAD", rate = 59.42m })
            .Row(new { name = "CHF", rate = 101.75m })
            .Row(new { name = "AUD", rate = 53.28m });

        Insert.IntoTable(DatabaseConstants.Tables.User)
            .Row(new { name = "john_doe", password = "$2a$12$5qIf.dz9JhgK5y.52r6G7OeO41uHgwEki/FAVc8sqqOLmhmu7A8Ge" })
            .Row(new { name = "jane_smith", password = "$2a$12$zAOVU8lkmAO5e6uBm7rh0Oz3VKRItnBYjsXWrg0Df.0g/l1pMKiE2" });

        Insert.IntoTable(DatabaseConstants.Tables.UserFavorites)
            .Row(new { user_id = 1, currency_id = 1 })
            .Row(new { user_id = 1, currency_id = 2 })
            .Row(new { user_id = 1, currency_id = 3 })
            .Row(new { user_id = 2, currency_id = 2 })
            .Row(new { user_id = 2, currency_id = 4 })
            .Row(new { user_id = 2, currency_id = 5 });
    }

    public override void Down()
    {
        Delete.FromTable(DatabaseConstants.Tables.UserFavorites).AllRows();
        Delete.FromTable(DatabaseConstants.Tables.User).AllRows();
        Delete.FromTable(DatabaseConstants.Tables.Currency).AllRows();
    }
} 