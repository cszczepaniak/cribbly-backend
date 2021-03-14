using FluentMigrator;

namespace CribblyBackend.Migrations
{
    [Migration(20210112083704)]
    public class AddTournamentFlags : Migration
    {
        public override void Down()
        {
            Delete.Column("IsOpenForRegistration").FromTable("Tournaments");
            Delete.Column("IsActive").FromTable("Tournaments");
        }

        public override void Up()
        {
            Alter.Table("Tournaments")
                .AddColumn("IsOpenForRegistration").AsBoolean().NotNullable()
                .AddColumn("IsActive").AsBoolean().NotNullable();
        }
    }
}
