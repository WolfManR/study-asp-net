using FluentMigrator.Builders.Create.Table;

namespace MetricsManagement.Manager.Data.Dapper.Migrations;

public static class MigrationRules
{
    public static void InjectMetricTableSchema(this ICreateTableWithColumnSyntax tableSchema)
    {
        tableSchema.WithColumn("Id").AsInt64().PrimaryKey().Identity()
            .WithColumn("AgentId").AsInt32()
            .WithColumn("Value").AsInt32()
            .WithColumn("Time").AsInt64();
    }
}