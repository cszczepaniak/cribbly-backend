using FluentMigrator;

namespace CribblyBackend.Migrations
{
    [Migration(20210629091542)]
    public class AddPlayerTournamentAssociation : Migration
    {
        public override void Down()
        {
            Delete.Table("TournamentPlayerAssociation");
        }

        public override void Up()
        {
            Create.Table("TournamentPlayerAssociation")
                .WithColumn("TournamentID").AsInt32().NotNullable()
                .WithColumn("PlayerID").AsInt32().NotNullable();
            Create.PrimaryKey().OnTable("TournamentPlayerAssociation").Columns(new[] { "TournamentID", "PlayerID" });
        }
    }
}
