using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameRental
{
    public interface IDatabaseEntity
    {
        public int Id { get; set; }
        public bool IsDeleted { get; }
        public string TypeName { get; }
        public object GetField(string name);
        public string ToString();
        public void SetField(string name, object arg);
        public void Delete();
    }
}
