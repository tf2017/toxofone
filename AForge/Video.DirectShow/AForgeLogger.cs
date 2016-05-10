namespace AForge.Video.DirectShow
{
    using System.Diagnostics;

    public abstract class AForgeLogger
    {
        private static AForgeLogger instanceInner;

        public static AForgeLogger Instance
        {
            get
            {
                if (AForgeLogger.instanceInner == null)
                {
                    return DebugLogger.Instance;
                }

                return AForgeLogger.instanceInner;
            }

            set
            {
                AForgeLogger.instanceInner = value;
            }
        }

        public abstract void LogVerbose(string text, StackTrace stackTrace = null);

        public abstract void LogInfo(string text, StackTrace stackTrace = null);

        public abstract void LogWarning(string text, StackTrace stackTrace = null);

        public abstract void LogError(string text, StackTrace stackTrace = null);
    }

    internal sealed class DebugLogger : AForgeLogger
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
            Debug.WriteLine(text);
        }

        public override void LogInfo(string text, StackTrace stackTrace)
        {
            Debug.WriteLine(text);
        }

        public override void LogWarning(string text, StackTrace stackTrace)
        {
            Debug.WriteLine(text);
        }

        public override void LogError(string text, StackTrace stackTrace)
        {
            Debug.WriteLine(text);
        }
    }
}
