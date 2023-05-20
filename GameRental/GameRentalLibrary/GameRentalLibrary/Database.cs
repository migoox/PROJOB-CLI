using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Emit;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using GameRental.Rep0;
using GameRental.Builders;
using GameRental.Rep8;
using GameRental.Adapters;
using GameRental.Collections;


namespace GameRental
{
    interface IEntityInfo
    {
        public IDictionary Table { get; }
        public void Add(IDatabaseEntity entity);
        public void Clear();
    }
    public class Info<T> : IEntityInfo where T : IDatabaseEntity
    {
        protected Dictionary<int, T> _table = new Dictionary<int, T>();
        private int _currentId = 1;
        public IDictionary Table
        {
            get => _table;
        }

        public void Add(IDatabaseEntity entity)
        {
            _table.Add(_currentId, (T)entity);
            _table[_currentId].Id = _currentId++;
        }
        public void Clear()
        {
            Table.Clear();
            _currentId = 1;
        }
    }
    
    public sealed class Database
    {
        private static readonly Database instance = new Database();
        private string _keyNotFoundMsg;

        private Dictionary<string, IEntityInfo> _tables =
            new Dictionary<string, IEntityInfo>();
        public Dictionary<int, IGame> Games
        {
            get => (Dictionary<int, IGame>)_tables["game"].Table;
        }
        public Dictionary<int, IMod> Mods
        {
            get => (Dictionary<int, IMod>)_tables["mod"].Table;
        }
        public Dictionary<int, IReview> Reviews
        {
            get => (Dictionary<int, IReview>)_tables["review"].Table;
        }
        public Dictionary<int, IUser> Users
        {
            get => (Dictionary<int, IUser>)_tables["user"].Table;
        }

        public static Database Instance
        {
            get
            {
                return instance;
            }
        }

        private Database()
        {
            _tables.Add("game", new Info<IGame>());
            _tables.Add("mod", new Info<IMod>());
            _tables.Add("review", new Info<IReview>());
            _tables.Add("user", new Info<IUser>());

            _keyNotFoundMsg = "Table not found exception.\nPossible tables: " + string.Join(", ", _tables.Keys);
        }

        public void Clear()
        {
            foreach (var elem in _tables)
                elem.Value.Clear();
        }
        public void AddGame(IGame game)
        {
            _tables["game"].Add(game);
        }
        public void AddUser(IUser user)
        {
            _tables["user"].Add(user);

        }
        public void AddReview(IReview review)
        {
            _tables["review"].Add(review);

        }
        public void AddMod(IMod mod)
        {
            _tables["mod"].Add(mod);
        }

        public IDictionary GetTable(string tableName)
        {
            if (!_tables.ContainsKey(tableName))
                throw new KeyNotFoundException(_keyNotFoundMsg);
            return _tables[tableName].Table;
        }

        public IDictionary GetTable<T>()
            where T : IDatabaseEntity
        {
            foreach (var table in _tables)
            {
                if (table.Value.Table is (Dictionary<int, T>))
                    return table.Value.Table;
            }

            throw new KeyNotFoundException(_keyNotFoundMsg);
        }

        public void Add(string tableName, IDatabaseEntity entity)
        {
            if (!_tables.ContainsKey(tableName))
                throw new KeyNotFoundException(_keyNotFoundMsg);
            _tables[tableName].Add(entity);
        }

        public SyncList<T> GetByIds<T>(int[] ids)
            where T : IDatabaseEntity
        {
            var dictionary = (Dictionary<int, T>)Database.Instance.GetTable<T>();
            return new SyncList<T>(new List<T>(dictionary
                    .Select(g => g.Value)
                    .Where(g => ids.Contains(g.Id))));
        }

        public int[] GetByRefs<T>(SyncList<T> refs)
            where T : IDatabaseEntity
        {
            var list = refs.GetSynced();
            int[] indices = new int[list.Count];
            for (int i = 0; i < list.Count; ++i)
            {
                indices[i] = list[i].Id;
            }
            return indices;
        }
    }
}
