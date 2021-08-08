using System.Collections.Generic;
using System.Threading.Tasks;
using CribblyBackend.Core.Teams.Models;

namespace CribblyBackend.Core.Teams.Repositories
{
    public interface ITeamRepository
    {
        Task<Team> GetByIdAsync(int id);
        Task<List<Team>> GetAllAsync();
        Task UpdateAsync(Team team);
        Task<int> CreateAsync(Team team);
        Task DeleteAsync(int id);
    }
}