using FluentMigrator;

namespace CribblyBackend.Migrations
{
    [Migration(20210314083905)]
    public class AddDivisions : Migration
    {
        public override void Down()
        {
            Delete.Column("DivisionId").FromTable("Teams");
            Delete.Table("Divisions");
        }

        public override void Up()
        {
            Create.Table("Divisions")
                .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                .WithColumn("Name").AsString().NotNullable();
            Alter.Table("Teams")
                .AddColumn("DivisionId").AsInt32().ForeignKey("Divisions", "Id").Nullable();
        }
    }
}
