namespace Svg
{
    using System.Diagnostics;

    public abstract class SvgLogger
    {
        private static SvgLogger instanceInner;

        public static SvgLogger Instance
        {
            get
            {
                if (SvgLogger.instanceInner == null)
                {
                    return DebugLogger.Instance;
                }

                return SvgLogger.instanceInner;
            }

            set
            {
                SvgLogger.instanceInner = value;
            }
        }

        public abstract void LogVerbose(string text, StackTrace stackTrace = null);

        public abstract void LogInfo(string text, StackTrace stackTrace = null);

        public abstract void LogWarning(string text, StackTrace stackTrace = null);

        public abstract void LogError(string text, StackTrace stackTrace = null);
    }

    internal sealed class DebugLogger : SvgLogger
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
            Trace.TraceInformation(text);
        }

        public override void LogInfo(string text, StackTrace stackTrace)
        {
            Trace.TraceInformation(text);
        }

        public override void LogWarning(string text, StackTrace stackTrace)
        {
            Trace.TraceWarning(text);
        }

        public override void LogError(string text, StackTrace stackTrace)
        {
            Trace.TraceError(text);
        }
    }
}
