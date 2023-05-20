using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameRentalClient
{
    public class CommandFamily
    {
        private Dictionary<string, CommandBuilder> _commands = new Dictionary<string, CommandBuilder>();

        private string _manual;
        private string _description;
        private string _name;

        public CommandFamily(string name, string description, string manual)
        {
            _name = name;
            _description = description;
            _manual = manual;
        }

        public Dictionary<string, CommandBuilder> Commands
        {
            get => _commands;
        }

        public string Description
        {
            get => _description;
        }
        public string Name
        {
            get => _name;
        }
        public string Manual
        {
            get => _manual;
        }
    }
}
