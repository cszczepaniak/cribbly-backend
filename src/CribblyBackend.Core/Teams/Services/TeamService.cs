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
        Task<List<Team>> Get();
        void Update(Team Team);
        Task<int> Create(Team Team);
        Task Delete(int id);
    }
    public class TeamService : ITeamService
    {
        private readonly ITeamRepository _teamRepository;

        public TeamService(ITeamRepository teamRepository)
        {
            _teamRepository = teamRepository;
        }
        public async Task<List<Team>> Get()
        {
            return await _teamRepository.GetAllAsync();
        }
        public async Task<int> Create(Team team)
        {
            return await _teamRepository.CreateAsync(team);
        }

        public async Task Delete(int id)
        {
            await _teamRepository.DeleteAsync(id);
        }

        public async Task<Team> GetById(int id)
        {
            return await _teamRepository.GetByIdAsync(id);
        }
        public void Update(Team team)
        {
            throw new System.NotImplementedException();
        }
    }
}