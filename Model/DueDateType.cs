namespace Replay.Model;

public enum DueDateType
{
    ASAP,
    TWO_WEEKS_PRIOR,
    PINSTANCE_DUEDATE,
    THREE_WEEKS_AFTER,
    THREE_MONTHS_AFTER,
    SIX_MONTHS_AFTER,
    CUSTOM
}

public class DueDateTypeHelper
{
    public static string GetStringTranslation(DueDateType toTranslate)
    {
        switch (toTranslate)
        {
            case DueDateType.ASAP:
                return "ASAP";
            case DueDateType.TWO_WEEKS_PRIOR:
                return "2 Wochen vor Vorgangsende";
            case DueDateType.PINSTANCE_DUEDATE:
                return "Zu Vorgangsende";
            case DueDateType.THREE_WEEKS_AFTER:
                return "3 Wochen nach Vorgangsende";
            case DueDateType.THREE_MONTHS_AFTER:
                return "3 Monate nach Vorgangsende";
            case DueDateType.SIX_MONTHS_AFTER:
                return "6 Monate nach Vorgangsende";
            case DueDateType.CUSTOM:
                return "Selbst festlegen";
            default:
                return "NOT IMPLEMENTED";
        }
    }

    public static int DueDateTypToDays(DueDateType toDays)
    {
        switch (toDays)
        {
            case DueDateType.TWO_WEEKS_PRIOR:
                return -14;
            case DueDateType.PINSTANCE_DUEDATE:
                return 0;
            case DueDateType.THREE_WEEKS_AFTER:
                return 21;
            case DueDateType.THREE_MONTHS_AFTER:
                return 84;
            case DueDateType.SIX_MONTHS_AFTER:
                return 168;
            
            default:
                throw new ArgumentException(toDays + " cant be converted to Days!");
        }
    }
    
    public static List<DueDateType> AllDueDayTypes()
    {
        List<DueDateType> ret = new List<DueDateType>()
        {
            DueDateType.ASAP,
            DueDateType.CUSTOM,
            DueDateType.TWO_WEEKS_PRIOR,
            DueDateType.PINSTANCE_DUEDATE,
            DueDateType.THREE_WEEKS_AFTER,
            DueDateType.THREE_MONTHS_AFTER,
            DueDateType.SIX_MONTHS_AFTER

        };
        return ret;
    }
}