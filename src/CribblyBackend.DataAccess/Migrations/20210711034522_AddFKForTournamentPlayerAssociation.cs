using FluentMigrator;

namespace CribblyBackend.Migrations
{
    [Migration(20210711034522)]
    public class AddFKForTournamentPlayerAssociation : Migration
    {
        public override void Down()
        {
            Alter
                .Column("TournamentId")
                .OnTable("TournamentPlayerAssociation")
                .AsInt32()
                .NotNullable();
            Alter
                .Column("PlayerId")
                .OnTable("TournamentPlayerAssociation")
                .AsInt32()
                .NotNullable();
        }

        public override void Up()
        {
            Alter
                .Column("TournamentId")
                .OnTable("TournamentPlayerAssociation")
                .AsInt32()
                .NotNullable()
                .ForeignKey("Tournaments", "Id");
            Alter
                .Column("PlayerId")
                .OnTable("TournamentPlayerAssociation")
                .AsInt32()
                .NotNullable()
                .ForeignKey("Players", "Id");
        }
    }
}
