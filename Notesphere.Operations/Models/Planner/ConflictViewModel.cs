using Notesphere.Entities.PlannerModels;

namespace Notesphere.Operations.Models.Planner
{
    public class ConflictViewModel
    {
        // List of all conflicts
        public List<Conflict> Conflicts { get; set; }

        // Unresolved conflicts only
        public List<Conflict> UnresolvedConflicts { get; set; }

        // Total conflict count
        public int TotalConflicts { get; set; }
        public int UnresolvedCount { get; set; }

        // Conflicts by severity
        public int MinorConflicts { get; set; }
        public int ModerateConflicts { get; set; }
        public int SevereConflicts { get; set; }

        // Resolution suggestions
        public List<string> Suggestions { get; set; }

        // Date range for conflict report
        public DateTime ReportStartDate { get; set; }
        public DateTime ReportEndDate { get; set; }
        public DateTime GeneratedAt { get; set; }

        // User ID
        public string UserId { get; set; }

        // Show resolved conflicts toggle
        public bool ShowResolved { get; set; }

        // Constructor
        public ConflictViewModel()
        {
            Conflicts = new List<Conflict>();
            UnresolvedConflicts = new List<Conflict>();
            Suggestions = new List<string>();
            GeneratedAt = DateTime.Now;
            ShowResolved = false;
        }

        // Calculate conflict statistics
        public void CalculateStatistics()
        {
            TotalConflicts = Conflicts.Count;
            UnresolvedCount = UnresolvedConflicts.Count;

            MinorConflicts = 0;
            ModerateConflicts = 0;
            SevereConflicts = 0;

            foreach (var conflict in Conflicts)
            {
                switch (conflict.ConflictSeverity)
                {
                    case "Minor":
                        MinorConflicts++;
                        break;
                    case "Moderate":
                        ModerateConflicts++;
                        break;
                    case "Severe":
                        SevereConflicts++;
                        break;
                }
            }
        }

        // Get summary message
        public string GetSummaryMessage()
        {
            if (TotalConflicts == 0)
                return "No scheduling conflicts detected. Your schedule is clear!";

            if (UnresolvedCount == 0)
                return "All conflicts have been resolved.";

            return $"You have {UnresolvedCount} unresolved conflict(s) that need attention.";
        }

        // Check if there are any severe conflicts
        public bool HasSevereConflicts()
        {
            return SevereConflicts > 0;
        }
    }
}
