using FluentMigrator;

namespace CribblyBackend.Migrations
{
    [Migration(20201231042500)]
    public class AddGameTable : Migration
    {
        public override void Down()
        {
            Delete.Table("Games");
        }

        public override void Up()
        {
            Create.Table("Games")
                .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                .WithColumn("Round").AsInt32().NotNullable()
                .WithColumn("Type").AsString(255).NotNullable()
                .WithColumn("WinnerId").AsInt32().ForeignKey("Teams", "Id").Nullable()
                .WithColumn("ScoreDifference").AsInt32();

        }
    }
}