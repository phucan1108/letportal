namespace LetPortal.Core.Persistences
{
    public class ComparisonProperty
    {
        public string Name { get; set; }

        public string SourceValue { get; set; }

        public string TargetValue { get; set; }

        public ComparedState ComparedState { get; set; }
    }

    public enum ComparedState
    {
        New,
        Unchanged,
        Changed,
        Deleted
    }
}
