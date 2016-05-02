namespace NAudio
{
    using System.Diagnostics;

    public abstract class NAudioLogger
    {
        private static NAudioLogger instanceInner;

        public static NAudioLogger Instance
        {
            get
            {
                if (NAudioLogger.instanceInner == null)
                {
                    return DebugLogger.Instance;
                }

                return NAudioLogger.instanceInner;
            }

            set
            {
                NAudioLogger.instanceInner = value;
            }
        }

        public abstract void LogVerbose(string text, StackTrace stackTrace = null);

        public abstract void LogInfo(string text, StackTrace stackTrace = null);

        public abstract void LogWarning(string text, StackTrace stackTrace = null);

        public abstract void LogError(string text, StackTrace stackTrace = null);
    }

    internal sealed class DebugLogger : NAudioLogger
    {
        private static readonly DebugLogger instanceInner;

        static DebugLogger()
        {
            instanceInner = new DebugLogger();
        }

        public new static DebugLogger Instance
        {
            get { return instanceInner; }
        }

        private DebugLogger()
        {
        }

        public override void LogVerbose(string text, StackTrace stackTrace)
        {
            System.Diagnostics.Debug.WriteLine(text);
        }

        public override void LogInfo(string text, StackTrace stackTrace)
        {
            System.Diagnostics.Debug.WriteLine(text);
        }

        public override void LogWarning(string text, StackTrace stackTrace)
        {
            System.Diagnostics.Debug.WriteLine(text);
        }

        public override void LogError(string text, StackTrace stackTrace)
        {
            System.Diagnostics.Debug.WriteLine(text);
        }
    }
}
