using System.Threading.Tasks;

namespace CribblyBackend.Core.Tournaments.Repositories
{
    public interface ITournamentPlayerRepository
    {
        Task CreateAsync(int tournamentId, int playerId);
        Task DeleteAsync(int tournamentId, int playerId);
    }
}