using FluentMigrator;

namespace CribblyBackend.Migrations
{
    [Migration(20210102072900)]
    public class AddGameTable : Migration
    {
        public override void Down()
        {
            Delete.Table("Scores");
            Delete.Table("Games");
        }

        public override void Up()
        {
            Create.Table("Games")
                .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                .WithColumn("GameRound").AsInt32().NotNullable()
                .WithColumn("ScoreDifference").AsInt32().Nullable()
                .WithColumn("WinnerId").AsInt32().ForeignKey("Teams", "Id").Nullable();
            Create.Table("Scores")
                .WithColumn("GameId").AsInt32().ForeignKey("Games", "Id").PrimaryKey().NotNullable()
                .WithColumn("TeamId").AsInt32().ForeignKey("Teams", "Id").PrimaryKey().NotNullable()
                .WithColumn("GameScore").AsInt32().Nullable();
        }
        
    }
}