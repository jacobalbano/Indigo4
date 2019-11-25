namespace Indigo.Core.Logging
{
    public class LogLineInfo
    {
        public string CallingClass { get; internal set; }
        public string CallingMethod { get; internal set; }

        public string Message { get; internal set; }
        public int Depth { get; internal set; }

        public override string ToString()
        {
            string tabs = Depth > 0 ? new string('\t', Depth) : string.Empty;

            if (CallingClass != null)
                return $"{CallingClass}.{CallingMethod}: {tabs}{Message}";
            else
                return $"{tabs}{Message}";
        }
    }
}