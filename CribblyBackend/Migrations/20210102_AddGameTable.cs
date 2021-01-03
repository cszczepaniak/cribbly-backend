using FluentMigrator;

namespace CribblyBackend.Migrations
{
    [Migration(20210102072800)]
    public class AddGameTable : Migration
    {
        public override void Down()
        {
            Delete.Table("Assignments");
            Delete.Table("Games");
        }

        public override void Up()
        {
            Create.Table("Games")
                .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                .WithColumn("GameRound").AsInt32().NotNullable()
                .WithColumn("ScoreDifference").AsInt32().Nullable()
                .WithColumn("WinnerId").AsInt32().ForeignKey("Teams", "Id").Nullable();
            Create.Table("Assignments")
                .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                .WithColumn("GameId").AsInt32().ForeignKey("Games", "Id").NotNullable()
                .WithColumn("TeamId").AsInt32().ForeignKey("Teams", "Id").NotNullable();
        }
        
    }
}