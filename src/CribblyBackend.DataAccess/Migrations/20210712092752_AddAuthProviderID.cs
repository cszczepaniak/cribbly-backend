using FluentMigrator;

namespace CribblyBackend.Migrations
{
    [Migration(20210712092752)]
    public class AddAuthProviderID : Migration
    {
        public override void Down()
        {
            Delete.Index("auth_provider_id");
            Delete.Column("AuthProviderId").FromTable("Players");
        }

        public override void Up()
        {
            Alter.Table("Players").AddColumn("AuthProviderId").AsString(32).Unique().Indexed("auth_provider_id");
        }
    }
}
