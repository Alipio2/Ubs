using System;
using System.Collections.Generic;
using System.Text;
using Ubs.Domain.Context.ValueObjects;

namespace Ubs.Domain.Context.Entities
{
    public class Ubss
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public string Address { get; private set; }
        public string City { get; private set; }
        public string Phone { get; private set; }
        public GeoCode GeoLocation { get; private set; }
        public Scores Score { get; private set; }

               
        public Ubss(int id, string name, string address, string city, string phone, GeoCode geoLocation, Scores score)
        {
            Id = id;
            Name = name;
            Address = address;
            City = city;
            Phone = phone;
            GeoLocation = geoLocation;
            Score = score;
        }
    }
}
