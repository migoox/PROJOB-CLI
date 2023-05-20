using GameRental.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameRental
{
    public interface IMod : IDatabaseEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public SyncList<IUser> Authors { get; set; }
        public SyncList<IMod> Compatibility { get; set; }
    }
}
