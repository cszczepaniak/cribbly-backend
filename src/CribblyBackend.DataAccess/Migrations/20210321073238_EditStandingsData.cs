using FluentMigrator;

namespace CribblyBackend.Migrations
{
    [Migration(20210321073238)]
    public class EditStandingsData : Migration
    {
        public override void Down()
        {
            Alter.Table("Teams")
                .AddColumn("Division").AsString()
                .AddColumn("Wins").AsInt32()
                .AddColumn("Losses").AsInt32()
                .AddColumn("TotalScore").AsInt32()
                .AddColumn("Seed").AsInt32()
                .AddColumn("InTournament").AsBoolean();
        }

        public override void Up()
        {
            Delete.Column("Division").FromTable("Teams");
            Delete.Column("Wins").FromTable("Teams");
            Delete.Column("Losses").FromTable("Teams");
            Delete.Column("TotalScore").FromTable("Teams");
            Delete.Column("Seed").FromTable("Teams");
            Delete.Column("InTournament").FromTable("Teams");
            Alter.Table("Teams")
                .AddColumn("Division").AsInt32();
        }
    }
}