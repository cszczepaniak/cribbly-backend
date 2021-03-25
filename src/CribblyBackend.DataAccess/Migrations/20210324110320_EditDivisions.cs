using FluentMigrator;

namespace CribblyBackend.Migrations
{
    [Migration(20210324110320)]
    public class EditDivisions : Migration
    {
        public override void Down()
        {
            Delete.Column("Division").FromTable("Teams");
            Alter.Table("Teams")
                .AddColumn("Division").AsInt32();
        }

        public override void Up()
        {

            Delete.Column("Division").FromTable("Teams");
            Alter.Table("Teams")
                .AddColumn("Division").AsInt32().ForeignKey("Divisions", "Id").Nullable();
        }
    }
}