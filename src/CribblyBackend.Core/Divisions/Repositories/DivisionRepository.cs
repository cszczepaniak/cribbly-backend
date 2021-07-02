using System.Threading.Tasks;
using CribblyBackend.Core.Divisions.Models;

namespace CribblyBackend.Core.Divisions.Repositories
{
    public interface IDivisionRepository
    {
        void GetById(int id);
        void Update(Division diivision);
        Task<Division> Create(Division division);
        void Delete(Division division);
    }
}