using FluentMigrator;

namespace CribblyBackend.Migrations
{
    [Migration(20201225105700)]
    public class AddTeamName : Migration
    {
        public override void Down()
        {
            Delete.Column("Name").FromTable("Teams");
        }

        public override void Up()
        {
            Alter.Table("Teams")
                .AddColumn("Name").AsString(255).NotNullable().Unique();
        }
    }
}