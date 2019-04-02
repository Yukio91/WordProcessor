using System;
using System.Data;
using WordProcessor.Utils.Logger;

namespace WordProcessor.Db
{
    internal abstract class DbManagerBase : IDisposable
    {
        protected readonly ILogger Logger;
        protected IDbConnection DbConnection;

        protected DbManagerBase(ILogger logger = null)
        {
            Logger = logger;
        }

        ~DbManagerBase()
        {
            Close();
        }

        public void Dispose()
        {
            Close();
        }

        public void Close()
        {
            DbConnection?.Close();
            DbConnection?.Dispose();
        }
    }
}