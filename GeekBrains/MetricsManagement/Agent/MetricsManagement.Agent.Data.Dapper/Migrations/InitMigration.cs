using FluentMigrator;

namespace MetricsManagement.Agent.Data.Dapper.Migrations;

[Migration(1)]
public class InitMigration : Migration
{
    public override void Up()
    {
        Create.Table(MetricsTables.ProcessTimeTotal).InjectMetricTableSchema();
    }

    public override void Down()
    {
        Delete.Table(MetricsTables.ProcessTimeTotal);
    }
}