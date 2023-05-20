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
    public interface IGame : IDatabaseEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Genre { get; set; }
        public string Devices { get; set; }
        public SyncList<IReview> Reviews { get; set; }
        public SyncList<IMod> Mods { get; set; }
        public SyncList<IUser> Authors { get; set; }
    }
}
