using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace GameRentalClient
{
    public class CommandException : Exception
    {
        private bool _usage = true;
        private bool _error = true;
        public bool Usage => _usage;
        public bool Error => _error;

        public CommandException WithErrorFlag(bool flag)
        {
            _error = flag;
            return this;
        }
        public CommandException WithUsageFlag(bool flag)
        {
            _usage = flag;
            return this;
        }

        public CommandException() : base("")
        {
        }

        public CommandException(string message) : base(message)
        {
        }
        public CommandException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    public class CommandInvalidArgumentsException : CommandException
    {
        public CommandInvalidArgumentsException() :
            base("Invalid input arguments.")
        {
        }
        public CommandInvalidArgumentsException(string message) : 
            base("Invalid input arguments: " + message)
        {
        }
    }

    public class CommandTooManyArgumentsException : CommandException
    {
        public CommandTooManyArgumentsException() :
            base("Too many input arguments.")
        {
        }
        public CommandTooManyArgumentsException(string message) :
            base("Too many input arguments: " + message)
        {
        }
    }

    public class CommandNotEnoughInputArgumentsException : CommandException
    {
        public CommandNotEnoughInputArgumentsException() :
            base("Not enough input arguments.")
        {
        }
        public CommandNotEnoughInputArgumentsException(string message) :
            base("Not enough input arguments: " + message)
        {
        }
    }

}
