using System.ComponentModel.DataAnnotations;

public class TaskBlueprintTemp
{
    [Required(ErrorMessage = "Bitte Aufgabename angeben!")]
    [StringLength(25, ErrorMessage = "Der Aufgabename darf maximal 25 Zeichen lang sein")]
    public string TaskName { get; set; } = string.Empty;

    [Range(-1825, 1825, ErrorMessage = "Die Aufgabenfälligkeit darf nur um 5 Jahre vom Prozesszieldatum abweichen (-1825, 1825)")]
    public int DaysRelativeToDueDate { get; set; }
    public string? Instructions { get; set; }
    
    [Required(ErrorMessage = "Bitte Verantwortlichkeit angeben!")]
    public int PermittedRoleId { get; set; }
    
    [Required(ErrorMessage = "Bitte mindestens eine Vertragsart wählen")]
    public string? PermittedContractTypeIds { get; set; }
    
    [Required(ErrorMessage = "Bitte mindestens eine Abteilung wählen")]
    public string? PermittedDepartmentIds{ get; set; }

    [Required(ErrorMessage = "If you get this error you broke the selction")]
    public string DueDateType { get; set; } = string.Empty;
    
}