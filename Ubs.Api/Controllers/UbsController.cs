using Domain.Contracts.Repositories;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ubs.Domain.Context.Entities;

namespace Ubs.Api.Controllers
{
    public class UbsController : Controller
    {
        private readonly IUbssRepository _repository;

        public UbsController(IUbssRepository repository)
        {
            _repository = repository;
        }

        [Route("v1/Ubs")]
        [HttpGet]
        [ResponseCache(Location = ResponseCacheLocation.Client, Duration = 60)]
        public IEnumerable<Ubss> Get()
        {
            return _repository.GetAll();
        }

        [ResponseCache(Location = ResponseCacheLocation.Client, Duration = 10)]
        [Route("v1/Ubs/latitude={lat},longitude={log}")]
        [HttpGet]
        public Ubss Get(decimal lat, decimal log)
        {
            return _repository.GetByCoordinate(lat, log);
        }

        [Route("v1/Ubs/page={page},per_page={per_page}")]
        [HttpGet]
        [ResponseCache(Location = ResponseCacheLocation.Client, Duration = 30)]
        public IEnumerable<Ubss> Get(int page, int per_page)
        {
            var listUbs = _repository.GetAll();
            return listUbs.ToList().Skip(page * per_page).Take(per_page);
        }

        [ResponseCache(Location = ResponseCacheLocation.Client, Duration = 10)]
        [Route("v1/Ubs/query={lat},{log},page={page},per_page={per_page}")]
        [HttpGet]
        public IEnumerable<Ubss> Get(decimal lat, decimal log, int page, int per_page)
        {
            var ubs= _repository.GetByCoordinate(lat, log);
            var listUbs = new List<Ubss>() {ubs};
            return listUbs.Skip((page - 1) * per_page).Take(per_page).ToList();
        }

        

    }
}
