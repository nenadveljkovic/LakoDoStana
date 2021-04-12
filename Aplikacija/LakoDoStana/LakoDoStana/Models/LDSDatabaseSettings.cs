using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LakoDoStana.Models
{
    public class LDSDatabaseSettings:ILDSDatabaseSettings
    {
        public string LDSCollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }
    public interface ILDSDatabaseSettings
    {
        string LDSCollectionName { get; set; }
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
    }
}
