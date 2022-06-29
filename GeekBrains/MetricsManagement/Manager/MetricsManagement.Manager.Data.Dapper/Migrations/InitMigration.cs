using FluentMigrator;

namespace MetricsManagement.Manager.Data.Dapper.Migrations;

[Migration(1)]
public class InitMigration : Migration
{
    public override void Up()
    {
        Create.Table(MetricsTables.Agents)
            .WithColumn("Id").AsInt32().PrimaryKey().Identity()
            .WithColumn("Uri").AsString(2048)
            .WithColumn("IsEnabled").AsBoolean().NotNullable();

        Create.Table(MetricsTables.ProcessTimeTotal)
            .InjectMetricTableSchema();
    }

    public override void Down()
    {
        Delete.Table(MetricsTables.Agents);
        Delete.Table(MetricsTables.ProcessTimeTotal);
    }
}