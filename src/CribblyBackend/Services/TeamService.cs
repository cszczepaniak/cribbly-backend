using System.Threading.Tasks;
using System.Collections.Generic;
using CribblyBackend.DataAccess.Models;
using CribblyBackend.DataAccess.Repositories;

namespace CribblyBackend.Services
{
    public interface ITeamService
    {
        Task<Team> GetById(int Id);
        Task<IEnumerable<Game>> GetGamesAsync(int teamId);
        Task<List<Team>> Get();
        void Update(Team Team);
        Task<int> Create(Team Team);
        Task Delete(int id);
    }
    public class TeamService : ITeamService
    {
        private readonly ITeamRepository _teamRepository;
        private readonly IStandingsService _standingsService;
        private readonly IGameRepository _gameRepository;

        public TeamService(ITeamRepository teamRepository, IStandingsService standingsService, IGameRepository gameRepository)
        {
            _teamRepository = teamRepository;
            _standingsService = standingsService;
            _gameRepository = gameRepository;
        }
        public async Task<List<Team>> Get()
        {
            return await _teamRepository.Get();
        }
        public async Task<int> Create(Team team)
        {
            return await _teamRepository.Create(team);
        }

        public async Task Delete(int id)
        {
            await _teamRepository.Delete(id);
        }

        public async Task<Team> GetById(int id)
        {
            return await _teamRepository.GetById(id);;
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