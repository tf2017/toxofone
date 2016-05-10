namespace Svg
{
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

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

        public override void LogVerbose(string text, string fileName, string member, int line)
        {
            Debug.WriteLine(text);
        }

        public override void LogInfo(string text, string fileName, string member, int line)
        {
            Debug.WriteLine(text);
        }

        public override void LogWarning(string text, string fileName, string member, int line)
        {
            Debug.WriteLine(text);
        }

        public override void LogError(string text, string fileName, string member, int line)
        {
            Debug.WriteLine(text);
        }
    }
}
