using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace GameRentalClient
{
    public class CommandBuilder
    {
        private string _name;
        private Func<List<object>, Command, int> _function;
        private Func<List<object>, Command, int> _initFunc;
        private Func<string[], List<object>> _parser;

        private Action<Command, StreamWriter> _plainTextSerializer;
        private Action<Command, StreamReader> _plainTextDeserializer;

        private Action<Command, XmlWriter> _xmlSerializer;
        private Action<Command, XmlReader> _xmlDeserializer;

        private bool _hasInitFunc;
        private bool _hasParser;
        private string _usage;
        private string _description;
        private string _manual;
        private string _commandFamily;
        private bool _queueable;

        public string Name => _name;
        public string CommandFamily => _commandFamily;
        public string Usage => _usage;
        public string Description => _description;
        public string Manual => _manual;

        public Func<List<object>, Command, int> InitFunc => _initFunc;
        public Func<List<object>, Command, int> Call => _function;
        public Func<string[], List<object>> Parser => _parser;

        public Action<Command, StreamWriter> PlainTextSerializer => _plainTextSerializer;
        public Action<Command, StreamReader> PlainTextDeserializer => _plainTextDeserializer;
        public Action<Command, XmlWriter> XmlSerializer => _xmlSerializer;
        public Action<Command, XmlReader> XmlDesereializer => _xmlDeserializer;

        public CommandBuilder()
        {
            _name = "";
            _hasInitFunc = false;
            _hasParser = false;
            _function = (objects , thisCmd)=>
            {
                throw new NotImplementedException();
            };
            _parser = objects =>
            {
                throw new NotImplementedException();
            };
            _initFunc = (objects , thisCmd) =>
            {
                throw new NotImplementedException();
            };
            _usage = "";
            _description = "";
            _manual = "";
            _commandFamily = "";

            _plainTextSerializer = (thisCmd, writer) =>
            {
                return;
            };

            _plainTextDeserializer = (thisCmd, reader) =>
            {
                return;
            };

            _xmlSerializer = (thisCmd, reader) =>
            {
                return;
            };

            _xmlDeserializer = (thisCmd, writer) =>
            {
                return;
            };
        }

        public CommandBuilder WithName(string name)
        {
            _name = name;
            return this;
        }

        public CommandBuilder WithCall(Func<List<object>, Command, int> function)
        {
            _function = function;
            return this;
        }

        public CommandBuilder WithParser(Func<string[], List<object>> parser)
        {
            _parser = parser;
            _hasParser = true;
            return this;
        }

        public CommandBuilder WithPlaintextSerializer(Action<Command, StreamWriter> plainTextSerializer)
        {
            _plainTextSerializer = plainTextSerializer;
            return this;
        }

        public CommandBuilder WithPlaintextDeserializer(Action<Command, StreamReader> plainTextDeserializer)
        {
            _plainTextDeserializer = plainTextDeserializer;
            return this;
        }

        public CommandBuilder WithXmlSerializer(Action<Command, XmlWriter> xmlSerializer)
        {
            _xmlSerializer = xmlSerializer;
            return this;
        }

        public CommandBuilder WithXmlDeserializer(Action<Command, XmlReader> xmlDeserializer)
        {
            _xmlDeserializer = xmlDeserializer;
            return this;
        }

        public CommandBuilder WithInit(Func<List<object>, Command, int> initFunc)
        {
            _initFunc = initFunc;
            _hasInitFunc = true;
            return this;
        }

        public CommandBuilder WithQueueable(bool queueable)
        {
            _queueable = queueable;
            return this;
        }

        public CommandBuilder WithUsage(string usage)
        {
            this._usage = usage;
            return this;
        }

        public CommandBuilder WithManual(string manual)
        {
            this._manual = manual;
            return this;
        }
        public CommandBuilder WithDescription(string description)
        {
            this._description = description;
            return this;
        }

        public CommandBuilder WithFamily(string family)
        {
            this._commandFamily = family;
            return this;
        }

        public Command Build()
        {
            if (_name == null)
            {
                throw new InvalidOperationException("Command name is required");
            }

            if (_function == null)
            {
                throw new InvalidOperationException("Command call is required");
            }

            return new Command(_name, _function, _hasParser, _parser, _hasInitFunc, _initFunc,_commandFamily, _queueable,
                _plainTextSerializer, _plainTextDeserializer, _xmlSerializer, _xmlDeserializer);
        }
    }
}
