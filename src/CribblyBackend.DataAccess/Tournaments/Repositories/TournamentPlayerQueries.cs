namespace CribblyBackend.DataAccess.Tournaments.Repositories
{
    public static class TournamentPlayerQueries
    {
        public static string CreateTournamentPlayerAssociation =
                @"INSERT INTO TournamentPlayerAssociation (TournamentId, PlayerId)
                VALUES (@TournamentId, @PlayerId)";

        public static string DeleteTournamentPlayerAssociation =
                @"DELETE FROM TournamentPlayerAssociation
                WHERE TournamentId = @TournamentId
                AND PlayerId = @PlayerId";
    }
}