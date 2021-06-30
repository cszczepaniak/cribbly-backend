using FluentMigrator;

namespace CribblyBackend.Migrations
{
    [Migration(20210629092503)]
    public class RenameTournamentPlayerAssocColumns : Migration
    {
        public override void Down()
        {
            Rename.Column("TournamentId").OnTable("TournamentPlayerAssociation").To("TournamentID");
            Rename.Column("PlayerId").OnTable("TournamentPlayerAssociation").To("PlayerID");
        }

        public override void Up()
        {
            Rename.Column("TournamentID").OnTable("TournamentPlayerAssociation").To("TournamentId");
            Rename.Column("PlayerID").OnTable("TournamentPlayerAssociation").To("PlayerId");
        }
    }
}
