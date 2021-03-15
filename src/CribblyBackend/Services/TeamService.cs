using System.Threading.Tasks;
using CribblyBackend.DataAccess.Models;
using CribblyBackend.DataAccess.Repositories;

namespace CribblyBackend.Services
{
    public interface ITeamService
    {
        Task<Team> GetById(int Id);
        Task<int> Create(Team Team);
        Task AddToDivision(int teamId, int divisionId);
    }
    public class TeamService : ITeamService
    {
        private readonly ITeamRepository _teamRepository;

        public TeamService(ITeamRepository teamRepository)
        {
            _teamRepository = teamRepository;
        }

        public async Task AddToDivision(int teamId, int divisionId)
        {
            await _teamRepository.AddToDivision(teamId, divisionId);
        }

        public async Task<int> Create(Team team)
        {
            return await _teamRepository.Create(team);
        }

        public async Task<Team> GetById(int id)
        {
            return await _teamRepository.GetById(id);
        }
    }
}