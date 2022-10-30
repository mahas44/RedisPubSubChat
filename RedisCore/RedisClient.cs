using StackExchange.Redis;
using System.Configuration;

namespace RedisCore
{
    public class RedisClient : IDisposable
    {
        public ConnectionMultiplexer connection;
        public string Channel = "Abuzer";
        public string Channel2 = "Hayri";

        public RedisClient()
        {
            string connString = ConfigurationManager.AppSettings["RedisConnectionString"];
            var options = ConfigurationOptions.Parse(connString);
            options.Password = ConfigurationManager.AppSettings["RedisPassword"];
            connection = ConnectionMultiplexer.Connect(options);
        }

        private bool disposedValue;
        ~RedisClient() => Dispose(false);

        // Public implementation of Dispose pattern callable by consumers.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }
                // TODO: free unmanaged resource (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }


        public ISubscriber GetSubscriber()
        {
            return connection.GetSubscriber();
        }

    }
}