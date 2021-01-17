using FluentMigrator;

namespace CribblyBackend.Migrations
{
    [Migration(20210111052959)]
    public class PersistTournaments : Migration
    {
        public override void Down()
        {
            Delete.Table("Tournaments");
        }

        public override void Up()
        {
            Create.Table("Tournaments")
                .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                .WithColumn("Date").AsDateTime().NotNullable();
        }
    }
}
