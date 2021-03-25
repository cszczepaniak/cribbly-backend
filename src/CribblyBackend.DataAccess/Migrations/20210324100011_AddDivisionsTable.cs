using FluentMigrator;

namespace CribblyBackend.Migrations
{
    [Migration(20210324100011)]
    public class AddDivisionsTable : Migration
    {
        public override void Down()
        {
            Delete.Table("Teams");
        }

        public override void Up()
        {
            Create.Table("Divisions")
                .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                .WithColumn("Name").AsString();
        }
    }
}