using FluentMigrator;

namespace CribblyBackend.Migrations
{
    [Migration(20201219100700)]
    public class AddPlayerTable : Migration
    {
        public override void Down()
        {
            Delete.Table("Teams");
            Delete.Table("Players");
        }

        public override void Up()
        {
            Create.Table("Teams")
                .WithColumn("Id").AsInt32().PrimaryKey();
            Create.Table("Players")
                .WithColumn("Id").AsInt32().PrimaryKey()
                .WithColumn("Email").AsString(255).NotNullable().Unique()
                .WithColumn("Name").AsString(255)
                .WithColumn("Role").AsString(255)
                .WithColumn("TeamId").AsInt32().ForeignKey("Teams", "Id");
        }
    }
}