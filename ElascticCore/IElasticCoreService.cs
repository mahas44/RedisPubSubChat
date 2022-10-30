using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElascticCore
{
    public interface IElasticCoreService<T> where T : BaseModel
    {
        public IReadOnlyCollection<T> SearchLog(int rowCount);
        public void CheckExistsAndInsertLog(T logMode, string indexName);
    }
}
