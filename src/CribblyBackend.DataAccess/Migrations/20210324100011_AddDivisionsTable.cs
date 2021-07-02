using FluentMigrator;

namespace CribblyBackend.Migrations
{
    [Migration(20210324100011)]
    public class AddDivisionsTable : Migration
    {
        public override void Down()
        {
            Delete.Column("Division").FromTable("Teams");
            Delete.Table("Divisions");
            Alter.Table("Teams")
                .AddColumn("Division").AsInt32();
        }

        public override void Up()
        {
            Create.Table("Divisions")
                .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                .WithColumn("Name").AsString();
            Delete.Column("Division").FromTable("Teams");
            Alter.Table("Teams")
                .AddColumn("Division").AsInt32().ForeignKey("Divisions", "Id").Nullable();
        }
    }
}