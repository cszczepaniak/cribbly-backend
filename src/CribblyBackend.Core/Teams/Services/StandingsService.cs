using System.Threading.Tasks;
using System.Linq;
using CribblyBackend.Core.Teams.Models;
using CribblyBackend.Core.Games.Repositories;
using CribblyBackend.Core.Games.Models;

namespace CribblyBackend.Core.Teams.Services
{
    public interface IStandingsService
    {
        Task<Team> Calculate(Team team);
    }

    public class StandingsService : IStandingsService
    {
        private readonly IGameRepository _gameRepository;
        public StandingsService(IGameRepository gameRepository)
        {
            _gameRepository = gameRepository;
        }
        public async Task<Team> Calculate(Team team)
        {
            var allGames = await _gameRepository.GetByTeamId(team.Id);
            team.PlayInGames = allGames.Where(g => g.GameRound <= Round.Round3).ToList();
            team.BracketGames = allGames.Where(g => g.GameRound >= Round.TourneyRound1).ToList();

            team.Wins = allGames.Count(g => g.Winner != null && g.Winner.Id == team.Id);
            team.Losses = allGames.Count(g => g.Winner != null && g.Winner.Id != team.Id);

            foreach (var game in allGames)
            {
                if (game.Winner != null && game.Winner.Name == team.Name)
                {
                    team.TotalScore += 121;
                }
                else if (game.Winner != null)
                {
                    team.TotalScore += (121 - game.ScoreDifference);
                }
            }

            return team;
        }
    }


}