using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameRental.Rep8
{
    public interface IStackRepresentation
    {
        public Tuple<int, Stack<string>> Data { get; set; }
    }
}
