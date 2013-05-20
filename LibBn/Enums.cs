namespace LibBn
{
    public enum BnInputType
    {
        Default,
        User
    }

    public enum VertexInputType
    {
        SoftEvidence,
        PastYear,
        StateSelected
    }

    public enum VertexState
    {
        Details,
        Summary
    }

    public static class Groups
    {
        public static readonly string All = "all";
        public static readonly string Default = "0";
    }
}