namespace Replay.Model;

public enum Status
{
    NOT_STARTED,
    IN_PROGRESS,
    FINISHED
}

public class TranslationStatus 
{
    public static String getTranslation(Status status)
    {
        switch (status)
        {
            case Status.IN_PROGRESS:
                return "In Bearbeitung";
            case Status.FINISHED:
                return "Erledigt";
            case Status.NOT_STARTED:
                return "Unbearbeitet";
            
        }

        return "";
    }
}
