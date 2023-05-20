using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameRental;

namespace GameRental.Collections
{
    public class SyncList<T> where T : IDatabaseEntity
    {
        private List<T> _list = new List<T>();

        public SyncList()
        {

        }

        public SyncList(List<T> list)
        {
            _list = list;
        }

        public List<T> GetSynced()
        {
            AbstractDatabaseEntity.SyncRefsWithDatabase(_list);
            return _list;
        }
    }
}
