namespace AForge.Video.DirectShow
{
    using System;
    using System.Runtime.CompilerServices;

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

        public abstract void LogVerbose(string text,
            [CallerFilePath] string fileName = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0);

        public abstract void LogInfo(string text,
            [CallerFilePath] string fileName = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0);

        public abstract void LogWarning(string text,
            [CallerFilePath] string fileName = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0);

        public abstract void LogError(string text,
            [CallerFilePath] string fileName = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0);
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

        public override void LogVerbose(string text, string fileName, string member, int line)
        {
            System.Diagnostics.Debug.WriteLine(text);
        }

        public override void LogInfo(string text, string fileName, string member, int line)
        {
            System.Diagnostics.Debug.WriteLine(text);
        }

        public override void LogWarning(string text, string fileName, string member, int line)
        {
            System.Diagnostics.Debug.WriteLine(text);
        }

        public override void LogError(string text, string fileName, string member, int line)
        {
            System.Diagnostics.Debug.WriteLine(text);
        }
    }
}
