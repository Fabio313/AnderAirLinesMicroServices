using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using APIAirportDapper.Repository;
using Models;

namespace APIAirportDapper.Service
{
    public class AirportService
    {
        private IAirportRepository _airportRepository;

        public AirportService()
        {
            _airportRepository = new AirportRepository();
        }

        public bool Add(Airport airport)
        {
            return _airportRepository.Add(airport);
        }

        public List<Airport> GetAll()
        {
            return _airportRepository.GetAll();
        }

        public Airport GetbyCode(string code)
        {
            return _airportRepository.GetByCode(code);
        }
    }
}
