namespace SharpTox
{
    using System.Diagnostics;

    public abstract class SharpToxLogger
    {
        private static SharpToxLogger instanceInner;

        public static SharpToxLogger Instance
        {
            get
            {
                if (SharpToxLogger.instanceInner == null)
                {
                    return DebugLogger.Instance;
                }

                return SharpToxLogger.instanceInner;
            }

            set
            {
                SharpToxLogger.instanceInner = value;
            }
        }

        public abstract void LogVerbose(string text, StackTrace stackTrace = null);

        public abstract void LogInfo(string text, StackTrace stackTrace = null);

        public abstract void LogWarning(string text, StackTrace stackTrace = null);

        public abstract void LogError(string text, StackTrace stackTrace = null);
    }

    internal sealed class DebugLogger : SharpToxLogger
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
