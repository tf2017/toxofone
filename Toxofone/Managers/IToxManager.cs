namespace Toxofone.Managers
{
    using SharpTox.Av;
    using SharpTox.Core;

    public interface IToxManager
    {
        void InitManager(Tox tox, ToxAv toxAv);
    }
}
