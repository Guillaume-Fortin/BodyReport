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

        public static DateTime YearWeekToPlanningDateTime(int year, int week)
        {
            DateTime startOfYear = new DateTime(year, 1, 1);
            int daysToRetreiveLasteMondayDay = DayOfWeek.Monday - startOfYear.DayOfWeek;
            //If daysToRetreiveLasteMondayDay == 0, it's first day new planningCalendar
            if (daysToRetreiveLasteMondayDay < 0) //Difference in day (Tuesday to Saturday)
            {
                startOfYear = startOfYear.AddDays(7 + daysToRetreiveLasteMondayDay);
            }
            else if (daysToRetreiveLasteMondayDay == 1) //Sunday
            {
                startOfYear = startOfYear.AddDays(1);
            }

            return startOfYear.AddDays(7 *(week -1));
        }

        public static TEnum IntToEnum<TEnum>(int value)
        {
            Type type = typeof(TEnum);
            if (Enum.IsDefined(type, value))
            {
                return (TEnum)((object)value);
            }
            else
                return (TEnum)(Enum.GetValues(type).GetValue(0));
        }
    }
}
