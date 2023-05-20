using GameRental;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameRental
{
    public interface IReview : IDatabaseEntity
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public int Rating { get; set; }
        public IUser Author { get; set; }
    }
}
