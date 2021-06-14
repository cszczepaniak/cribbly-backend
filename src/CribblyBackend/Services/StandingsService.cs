using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using CribblyBackend.DataAccess.Models;
using CribblyBackend.DataAccess.Repositories;

namespace CribblyBackend.Services
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
            team.PlayInGames = allGames.Where(g => g.GameRound <= Game.Round.Round3).ToList();
            team.BracketGames = allGames.Where(g => g.GameRound >= Game.Round.TourneyRound1).ToList();

            team.Wins = allGames.Count(g => g.Winner != null && g.Winner.Name == team.Name);
            team.Losses = allGames.Count(g => g.Winner != null && g.Winner.Name != team.Name);

            foreach(Game game in allGames)
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