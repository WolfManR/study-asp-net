using FluentMigrator.Builders.Create.Table;

namespace MetricsManagement.Agent.Data.Dapper.Migrations;

public static class MigrationRules
{
    public static void InjectMetricTableSchema(this ICreateTableWithColumnSyntax tableSchema)
    {
        tableSchema.WithColumn("Id").AsInt64().PrimaryKey().Identity()
            .WithColumn("Value").AsInt32()
            .WithColumn("Time").AsInt64();
    }
}