using System.Threading.Tasks;
using System.Collections.Generic;
using CribblyBackend.DataAccess.Models;
using CribblyBackend.DataAccess.Repositories;

namespace CribblyBackend.Services
{
    public interface IDivisionService
    {
        void GetById(int Id);
        void Update(Division division);
        Task<Division> Create(Division division);
        void Delete(Division division);
    }
    public class DivisionService : IDivisionService
    {
        private readonly IDivisionRepository _divisionRepository;

        public DivisionService(IDivisionRepository divisionRepository)
        {
            _divisionRepository = divisionRepository;
        }

        public void GetById(int id)
        {
            throw new System.NotImplementedException();
        }
        public async Task<Division> Create(Division division)
        {
            return await _divisionRepository.Create(division);
        }
        public void Update(Division division)
        {
            throw new System.NotImplementedException();
        }
        public void Delete(Division division)
        {
            throw new System.NotImplementedException();
        }
    }
}