using System.Threading.Tasks;
using CribblyBackend.DataAccess.Models;
using CribblyBackend.DataAccess.Repositories;

namespace CribblyBackend.Services
{
    public interface ITeamService
    {
        Task<Team> GetById(int Id);
        void Update(Team Team);
        Task Create(Team Team);
        void Delete(Team Team);
    }
    public class TeamService : ITeamService
    {
        private readonly ITeamRepository _teamRepository;

        public TeamService(ITeamRepository teamRepository)
        {
            _teamRepository = teamRepository;
        }

        public async Task Create(Team team)
        {
            await _teamRepository.Create(team);
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