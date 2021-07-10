using System.Collections.Generic;
using System.Threading.Tasks;
using CribblyBackend.Core.Teams.Models;

namespace CribblyBackend.Core.Teams.Repositories
{
    public interface ITeamRepository
    {
        Task<Team> GetById(int Id);
        Task<List<Team>> Get();
        void Update(Team Team);
        Task<int> Create(Team Team);
        void Delete(Team Team);
    }
}