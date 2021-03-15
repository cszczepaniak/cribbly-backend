using System.Collections.Generic;
using System.Threading.Tasks;
using CribblyBackend.DataAccess.Models;
using CribblyBackend.DataAccess.Repositories;

namespace CribblyBackend.Services
{
    public interface IDivisionService
    {
        Task<Division> GetById(int id);
        Task<int> Create(string name, IEnumerable<int> teamIds);
    }
    public class DivisionService : IDivisionService
    {
        private readonly IDivisionRepository _divisionRepository;

        public DivisionService(IDivisionRepository divisionRepository)
        {
            _divisionRepository = divisionRepository;
        }

        public async Task<int> Create(string name, IEnumerable<int> teamIds)
        {
            return await _divisionRepository.Create(name, teamIds);
        }

        public async Task<Division> GetById(int id)
        {
            return await _divisionRepository.GetById(id);
        }
    }
}