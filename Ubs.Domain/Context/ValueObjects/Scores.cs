using System;
using System.Collections.Generic;
using System.Text;

namespace Ubs.Domain.Context.ValueObjects
{
    public  class Scores
    {
        public int Size { get; private set; }
        public int AdaptationForSeniors { get; private set; }
        public int MedicalEquipment { get; private set; }
        public int Medicine { get; private set; }

        
        public Scores(int size, int adaptationForSeniors, int medicalEquipment, int medicine)
        {
            Size = size;
            AdaptationForSeniors = adaptationForSeniors;
            MedicalEquipment = medicalEquipment;
            Medicine = medicine;
        }

        
    }
}
