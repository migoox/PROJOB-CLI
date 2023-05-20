using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameRentalClient
{
    public class CommandFamilyBuilder
    {
        private string _description = "";
        private string _name = "";
        private string _manual = "";

        public CommandFamilyBuilder WithName(string name)
        {
            _name = name;
            return this;
        }

        public CommandFamilyBuilder WithDescription(string description)
        {
            _description = description;
            return this;
        }

        public CommandFamilyBuilder WithManual(string manual)
        {
            _manual = manual;
            return this;
        }

        public CommandFamily Build()
        {
            if (_name == "")
                throw new Exception("CommandFamily name can't be empty.");
            return new CommandFamily(_name, _description, _manual);
        }
    }
}
