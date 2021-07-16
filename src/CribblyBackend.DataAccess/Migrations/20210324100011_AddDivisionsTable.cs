using FluentMigrator;

namespace CribblyBackend.Migrations
{
    [Migration(20210324100011)]
    public class AddDivisionsTable : Migration
    {
        public override void Down()
        {
            Delete.ForeignKey("FK_Teams_Division_Divisions_Id").OnTable("Teams");
            Delete.Table("Divisions");
        }

        public override void Up()
        {
            Create.Table("Divisions")
                .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                .WithColumn("Name").AsString();
            Alter.Table("Teams")
                .AddColumn("Division").AsInt32().ForeignKey("Divisions", "Id").Nullable();
        }
    }
}