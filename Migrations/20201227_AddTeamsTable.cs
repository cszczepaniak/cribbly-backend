using FluentMigrator;

namespace CribblyBackend.Migrations
{
    [Migration(202012271812)]
    public class AddTeamsTable : Migration
    {
        public override void Down()
        {
            Delete.Table("Players");
            Delete.Table("Teams");
        }

        public override void Up()
        {
            Create.Table("Teams")
                .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                .WithColumn("Name").AsString(255).NotNullable().Unique()
                .WithColumn("Division").AsString(255)
                .WithColumn("Wins").AsInt32()
                .WithColumn("Losses").AsInt32()
                .WithColumn("TotalScore").AsInt32()
                .WithColumn("Ranking").AsInt32()
                .WithColumn("Seed").AsInt32()
                .WithColumn("InTournament").AsBoolean();
            Create.Table("Players")
                .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                .WithColumn("Email").AsString(255).NotNullable().Unique()
                .WithColumn("Name").AsString(255).Nullable()
                .WithColumn("Role").AsString(255).Nullable()
                .WithColumn("TeamId").AsInt32().ForeignKey("Teams", "Id").Nullable();
        }
    }
}