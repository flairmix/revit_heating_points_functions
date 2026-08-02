namespace Fill_ADSK_Parameters
{
    public class PositionRule
    {
        public PositionRule(string group, string pattern, int basePosition)
        {
            Group = group;
            Pattern = pattern;
            BasePosition = basePosition;
        }

        public string Group { get; private set; }
        public string Pattern { get; private set; }
        public int BasePosition { get; private set; }
    }
}
