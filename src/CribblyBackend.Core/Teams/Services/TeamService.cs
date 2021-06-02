using System.Collections.Generic;
using System.Threading.Tasks;
using CribblyBackend.Core.Games.Models;
using CribblyBackend.Core.Games.Repositories;
using CribblyBackend.Core.Teams.Models;
using CribblyBackend.Core.Teams.Repositories;

namespace CribblyBackend.Core.Teams.Services
{
    public interface ITeamService
    {
        Task<Team> GetById(int Id);
        Task<IEnumerable<Game>> GetGamesAsync(int teamId);
        Task<List<Team>> Get();
        void Update(Team Team);
        Task<int> Create(Team Team);
        void Delete(Team Team);
    }
    public class TeamService : ITeamService
    {
        private readonly ITeamRepository _teamRepository;
        private readonly IGameRepository _gameRepository;

        public TeamService(ITeamRepository teamRepository, IGameRepository gameRepository)
        {
            _teamRepository = teamRepository;
            _gameRepository = gameRepository;
        }
        public async Task<List<Team>> Get()
        {
            var teams = await _teamRepository.Get();
            //TODO: Calculate wins, losses, 

            return teams;
        }
        public async Task<int> Create(Team team)
        {
            return await _teamRepository.Create(team);
        }

        public void Delete(Team team)
        {
            throw new System.NotImplementedException();
        }

        public async Task<Team> GetById(int id)
        {
            return await _teamRepository.GetById(id);
        }
        public void Update(Team team)
        {
            throw new System.NotImplementedException();
        }

        public async Task<IEnumerable<Game>> GetGamesAsync(int teamId)
        {
            return await _gameRepository.GetByTeamId(teamId);
        }
    }
}