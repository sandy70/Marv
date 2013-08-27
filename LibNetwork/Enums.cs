namespace LibNetwork
{
    public enum BnInputType
    {
        Default,
        User
    }

    public enum EvidenceType
    {
        SoftEvidence,
        StateSelected
    }

    public enum VertexInputType
    {
        SoftEvidence,
        PastYear,
        StateSelected
    }

    public enum VertexType
    {
        Interval,
        None,
        Number
    }

    public static class Groups
    {
        public static readonly string All = "all";
        public static readonly string Default = "0";
    }
}