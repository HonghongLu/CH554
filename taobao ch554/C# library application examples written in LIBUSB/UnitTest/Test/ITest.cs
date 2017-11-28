using System.IO;

namespace UnitTest.Test
{
    public interface ITest
    {
        #region PUBLIC METHODS

        void Run(Stream output);

        #endregion
    }
}