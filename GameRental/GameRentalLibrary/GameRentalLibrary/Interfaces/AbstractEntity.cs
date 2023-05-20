using GameRental.Builders;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameRental
{
    public abstract class AbstractDatabaseEntity
    {
        public int Id { get; set; }

        protected bool _isDeleted = false;
        public bool IsDeleted
        {
            get => _isDeleted;
        }

        public string TypeName { get; init; }

        protected Dictionary<string, Func<object>> Getters { get; init; }

        protected Dictionary<string, Action<object>> Setters { get; init; }

        public static void SyncRefsWithDatabase(IList list)
        {
            for (int i = list.Count - 1; i >= 0; --i)
            {
                AbstractDatabaseEntity elem = (AbstractDatabaseEntity)list[i];

                if (elem.IsDeleted) 
                    list.RemoveAt(i);
            }
        }

        public static AbstractDatabaseEntity? SyncRefsWithDatabase(ref AbstractDatabaseEntity? entity)
        {
            if (entity == null) return null;

            if (entity.IsDeleted)
                entity = null;

            return entity;
        }

        public object GetField(string name)
        {
            if (!Getters.ContainsKey(name))
                throw new Exception($"Field \"{name}\" doesn't exist");

            return Getters[name]();
        }

        public void SetField(string name, object arg)
        {
            if (!Setters.ContainsKey(name))
                throw new Exception($"Field \"{name}\" doesn't exist");

            Setters[name](arg);
        }

        public void Delete()
        {
            _isDeleted = true;
        }
    }
}
