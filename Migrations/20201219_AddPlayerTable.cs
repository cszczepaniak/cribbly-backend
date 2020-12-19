using FluentMigrator;

namespace CribblyBackend.Migrations
{
    [Migration(20201219100700)]
    public class AddPlayerTable : Migration
    {
        public override void Down()
        {
            Delete.Table("Players");
            Delete.Table("Teams");
        }

        public override void Up()
        {
            Create.Table("Teams")
                .WithColumn("Id").AsInt32().PrimaryKey().Identity();
            Create.Table("Players")
                .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                .WithColumn("Email").AsString(255).NotNullable().Unique()
                .WithColumn("Name").AsString(255).Nullable()
                .WithColumn("Role").AsString(255).Nullable()
                .WithColumn("TeamId").AsInt32().ForeignKey("Teams", "Id").Nullable();
        }
    }
}