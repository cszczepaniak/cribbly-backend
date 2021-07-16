using System.Threading.Tasks;
using CribblyBackend.Core.Divisions.Models;
using CribblyBackend.Core.Teams.Models;

namespace CribblyBackend.Core.Divisions.Repositories
{
    public interface IDivisionRepository
    {
        Task<Division> GetById(int id);
        void Update(Division diivision);
        Task<Division> Create(Division division);
        Task<Division> AddTeam(int id, Team team);
        void Delete(Division division);
    }
}