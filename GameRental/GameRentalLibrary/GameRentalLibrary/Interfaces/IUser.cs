using GameRental.Rep0;
using GameRental;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameRental.Collections;

namespace GameRental
{
    public interface IUser : IDatabaseEntity
    {
        public int Id { get; set; }
        public string Nickname { get; set; }
        public SyncList<IGame> OwnedGames { get; set; }
    }
}
