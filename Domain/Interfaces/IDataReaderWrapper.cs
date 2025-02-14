using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLReplicator.Domain.Interfaces
{
    public interface IDataReaderWrapper
    {
        public List<string> ReadAttributes();
        public List<string> ReadValues();

    }
}
