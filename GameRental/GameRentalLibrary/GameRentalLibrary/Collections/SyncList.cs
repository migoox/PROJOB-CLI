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
        private List<T> _deleted = new List<T>();

        public SyncList()
        {

        }

        public SyncList(List<T> list)
        {
            _list = list;
        }

        public List<T> GetSynced()
        {
            for (int i = _list.Count - 1; i >= 0; --i)
            {
                if (_list[i].IsDeleted)
                {
                    _deleted.Add(_list[i]);
                    _list.RemoveAt(i);
                }
            }

            for (int i = _deleted.Count - 1; i >= 0; --i)
            {
                if (!_deleted[i].IsDeleted)
                {
                    _list.Add(_deleted[i]);
                    _deleted.RemoveAt(i);
                }
            }

            return _list;
        }
    }
}
