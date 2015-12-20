using Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Framework
{
    public static class Utils
    {
        public static double TransformLengthToUnitSytem(TUnitType fromUnitType, TUnitType toUnitType, double length)
        {
            if (fromUnitType == toUnitType)
                return length;

            double result = length;
            if (fromUnitType == TUnitType.Imperial && toUnitType == TUnitType.Metric)
            { //Convert Inch to cm (1 inch = 2.54 cm)
                result *= 2.54d;
            }
            else if (fromUnitType == TUnitType.Metric && toUnitType == TUnitType.Imperial)
            { //Convert cm to Inch  (1 cm = 0.393701 inch)
                result *= 0.393701d;
            }
            return result;
        }

        public static double TransformWeightToUnitSytem(TUnitType fromUnitType, TUnitType toUnitType, double weight)
        {
            if (fromUnitType == toUnitType)
                return weight;

            double result = weight;
            if (fromUnitType == TUnitType.Imperial && toUnitType == TUnitType.Metric)
            { //Convert Pound to kg  (1 Pound = 0.453592 kg)
                result *= 0.453592d;
            }
            else if (fromUnitType == TUnitType.Metric && toUnitType == TUnitType.Imperial)
            { //Convert Kg to Pound  (1 kg = 2.20462 Pound)
                result *= 2.20462;
            }
            return result;
        }
    }
}
