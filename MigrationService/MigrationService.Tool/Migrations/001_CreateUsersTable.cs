using FluentMigrator;
using MigrationService.Tool.Constants;

namespace MigrationService.Tool.Migrations;

[Migration(001)]
public class CreateUsersTable : Migration
{
    public override void Up()
    {
        Create.Table(DatabaseConstants.Tables.User)
            .WithColumn(DatabaseConstants.Columns.User.Id).AsInt32().PrimaryKey().Identity()
            .WithColumn(DatabaseConstants.Columns.User.Name).AsString(100).NotNullable()
            .WithColumn(DatabaseConstants.Columns.User.Password).AsString(255).NotNullable();
    }

    public override void Down()
    {
        Delete.Table(DatabaseConstants.Tables.User);
    }
}