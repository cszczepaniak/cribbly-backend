using System.Threading.Tasks;
using CribblyBackend.Core.Divisions.Models;
using CribblyBackend.Core.Teams.Models;

namespace CribblyBackend.Core.Divisions.Repositories
{
    public interface IDivisionRepository
    {
        Task<Division> GetByIdAsync(int id);
        Task UpdateAsync(Division diivision);
        Task<Division> CreateAsync(Division division);
        Task<Division> AddTeamAsync(int id, Team team);
        Task DeleteAsync(Division division);
    }
}