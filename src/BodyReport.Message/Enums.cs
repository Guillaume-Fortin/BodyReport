using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.Message
{
    public enum TSexType : int
    {
        MAN=0, WOMAN=1
    }

    public enum TExerciseType
    {

    }

    public enum TUnitType
    {
        Imperial,
        Metric
    }

    public enum TMonthType
    {
        NotSet = 0,
        January = 1,
        February = 2,
        March = 3,
        April = 4,
        May = 5,
        June = 6,
        July = 7,
        August = 8,
        September = 9,
        October = 10,
        November = 11,
        December = 12
    }

    public enum TDataBaseServerType
    {
        SqlServer,
        PostgreSQL
    }

	public enum TEditMode
	{
		Create,
		Edit
	}

	public enum LangType
	{
		en_US,
		fr_FR
	}

    public enum TTraceLevel
    {
        Info,
        Debug,
        Warning,
        Error
    }
}
