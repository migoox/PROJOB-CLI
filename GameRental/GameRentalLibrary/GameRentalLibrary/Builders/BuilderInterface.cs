using GameRental.Rep0;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using GameRental.Rep8;
using GameRental;
using GameRental.Adapters;

namespace GameRental.Builders
{
    public interface IDatabaseBuilder
    {
        public IDatabaseEntity BuildRep0();
        public IStackRepresentation BuildRep8();

        public IDatabaseEntity BuildRep8AndAdapt();
        public AbstractBuilder With(string name, object obj);
        public string ToString();

    }
}
