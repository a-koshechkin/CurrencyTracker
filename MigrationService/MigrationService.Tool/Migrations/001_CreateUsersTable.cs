using FluentMigrator;

namespace MigrationService.Tool.Migrations;

[Migration(001)]
public class CreateUsersTable : Migration
{
    public override void Up()
    {
        Create.Table("user")
            .WithColumn("id").AsInt32().PrimaryKey().Identity()
            .WithColumn("name").AsString(100).NotNullable()
            .WithColumn("password").AsString(255).NotNullable();
    }

    public override void Down()
    {
        Delete.Table("user");
    }
}