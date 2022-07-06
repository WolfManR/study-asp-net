using FluentMigrator;

namespace Banks.Accounts.Data.Dapper.Migrations;

[Migration(1)]
public class AccountsMigration : Migration
{
    private const string AccountsTableName = "Accounts";


    public override void Up()
    {
        Create.Table(AccountsTableName)
            .WithColumn("Id").AsInt32().PrimaryKey().Identity()
            .WithColumn("IdentityId").AsString(256).NotNullable()
            .WithColumn("Holder").AsString(256).NotNullable();
    }

    public override void Down()
    {
        Delete.Table(AccountsTableName);
    }
}