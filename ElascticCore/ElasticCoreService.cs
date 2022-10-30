using Nest;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElascticCore
{
    public class ElasticCoreService<T> : IDisposable, IElasticCoreService<T> where T : BaseModel
    {
        //Elastic üzerinde Indexden bağımız Document atmaya yarar. Yoksa Index yaratır.
        public void CheckExistsAndInsertLog(T logMode, string indexName)
        {
            using (ElasticClientProvider provider = new ElasticClientProvider())
            {
                ElasticClient elasticClient = provider.ElasticClient;
                if (!elasticClient.Indices.Exists(indexName).Exists)
                {
                    var newIndexName = indexName + DateTime.Now.Ticks;
                    var indexSettings = new IndexSettings();
                    // Herbir sunucunun, güvenlik amaçlı bir de yedeği olacaktır. Biri çöker ise, yedeyi devreye girecektir.
                    indexSettings.NumberOfReplicas = 1;
                    //3 sharding ve 1 replika ile toplamda 6 sunucu bulunmaktadır.
                    //Bir diğer dikkat edilecek konu da, her bir Shard’ın Replicasının başka bir Node’da bulunması gerektiğidir.
                    //Bütün yumurtaları aynı sepete koymanın anlamı yoktur :)
                    indexSettings.NumberOfShards = 3;

                    var response = elasticClient.Indices
                        .Create(newIndexName, index =>
                            index.Map<T>(m => m.AutoMap())
                        .InitializeUsing(new IndexState() { Settings = indexSettings })
                        .Aliases(a => a.Alias(indexName)));
                }
                IndexResponse responseIndex = elasticClient.Index<T>(logMode, idx => idx.Index(indexName));
            }
        }

        public IReadOnlyCollection<T> SearchLog(int rowCount)
        {
            using (ElasticClientProvider provider = new ElasticClientProvider())
            {
                ElasticClient elasticClient = provider.ElasticClient;
                string indexName = ConfigurationManager.AppSettings["ElasticIndexName"];
                var response = elasticClient
                    .Search<T>(s => s.Size(rowCount)
                                            .Sort(ss => ss.Descending(d => d.PostDate))
                                            .Index(indexName)
                );
                return response.Documents;
            }
        }

        private bool _disposedValue;

        ~ElasticCoreService() => Dispose(false);

        // Public implementation of Dispose pattern callable by consumers.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                _disposedValue = true;
            }
        }
    }
}
