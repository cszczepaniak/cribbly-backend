using System.Threading.Tasks;
using System.Collections.Generic;
using CribblyBackend.DataAccess.Models;
using CribblyBackend.DataAccess.Repositories;

namespace CribblyBackend.Services
{
    public interface ITeamService
    {
        Task<Team> GetById(int Id);
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
            foreach (Team team in teams)
            {

            }
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
    }
}