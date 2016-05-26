using Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Globalization;

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

            return startOfYear.AddDays(7 * (week - 1));
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

        // <summary>
        // Get the name of a static or instance property from a property access lambda.
        // </summary>
        // <typeparam name="T">Type of the property</typeparam>
        // <param name="propertyLambda">lambda expression of the form: '() => Class.Property' or '() => object.Property'</param>
        // <returns>The name of the property</returns>
        public static string GetPropertyName<T>(Expression<Func<T>> propertyLambda)
        {
            var me = propertyLambda.Body as MemberExpression;

            if (me == null)
            {
                throw new ArgumentException("You must pass a lambda of the form: '() => Class.Property' or '() => object.Property'");
            }

            return me.Member.Name;
        }

        public static bool TryParse(string strValue, out double value)
        {
            value = 0;
            if (string.IsNullOrWhiteSpace(strValue))
                return false;

            if (strValue.IndexOf(',') != -1)
                strValue = strValue.Replace(',', '.');
            return double.TryParse(strValue, NumberStyles.Any, CultureInfo.InvariantCulture, out value);
        }

        public static DateTime DateTimeWithoutMs
        {
            get
            {
                var date = DateTime.Now;
                return new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second);
            }
        }
    }
}
