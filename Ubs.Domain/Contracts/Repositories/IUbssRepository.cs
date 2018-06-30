using System.Collections.Generic;
using Ubs.Domain.Context.Entities;

namespace Domain.Contracts.Repositories
{
   public interface IUbssRepository
    {
        void Insert(Ubss pObject);

        void InsertList(List<Ubss> pList);

        List<Ubss> GetAll();

        Ubss GetByCoordinate(decimal lat, decimal log);
    }
}
