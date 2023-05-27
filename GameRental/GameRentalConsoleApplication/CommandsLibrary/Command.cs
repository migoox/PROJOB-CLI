using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace GameRentalClient
{
    public class Command : IXmlSerializable
    {
        private Func<List<object>, Command, int> _function;
        private Func<List<object>, Command, int> _initFunc;
        private Func<string[], List<object>> _parser;
        private bool _hasParser;
        private bool _hasInitFunc;

        private Action<Command, StreamWriter> _plainTextSerializer;
        private Action<Command, StreamReader> _plainTextDeserializer;

        private Action<Command, XmlWriter> _xmlSerializer;
        private Action<Command, XmlReader> _xmlDeserializer;

        private Action<Command, object> _undoAction;
        private Action<Command, object> _redoAction;

        private string[] _notParsedArgs;

        private List<object> _args;

        // text that can be displayed in the future e.g. in the queue print
        public string Info { get; set; }

        // data that can be used in serialization/deserialization
        public List<object> ContextData { get; set; } = new List<object>();

        // data that is used by redo/undo function
        public object Snapshot { get; set; }

        public string Input { get; set; }
        public bool Cancel { get; set; }
        public string Name { get; }

        // if Queueable=true or Historyable=true and Init Function is used, serialization/deserialization functions are expected
        public bool Queueable { get; }

        // if true, it is expected that Undo Action is set (Redo is optional)
        public bool Historyable { get; }

        public string CommandFamily { get; }

        public string[] NotParsedArgs => _notParsedArgs;

        public List<object> Args => _args;

        public Command()
        {
            this.Name = "";
            this._function = (objects, thisCmd) =>
            {
                throw new NotImplementedException();
            };
            this._hasParser = false;
            this._parser = objects =>
            {
                throw new NotImplementedException();
            };
            this._initFunc = (objects, thisCmd) =>
            {
                throw new NotImplementedException();
            };
            this._hasInitFunc = false;
            this.Queueable = false;
            this.CommandFamily = "";
            this.Info = "";
            this.Cancel = false;
            this.Input = "";
            this._notParsedArgs = new string[] { };
            this._args = new List<object>();
        }

        public Command(string name, 
            Func<List<object>, Command, int> function,
            bool hasParser,
            Func<string[], List<object>> parser,
            bool hasInitFunc,
            Func<List<object>, Command, int> initFunc,
            string commandFamily,
            bool queueable,
            bool historyable,
            Action<Command, object> undoAction,
            Action<Command, object> redoAction,
            Action<Command, StreamWriter> plainTextSerializer,
            Action<Command, StreamReader> plainTextDeserializer,
            Action<Command, XmlWriter> xmlSerializer,
            Action<Command, XmlReader> xmlDeserializer)
        {
            this.Name = name;
            this._function = function;
            this._hasParser = hasParser;
            this._parser = parser;
            this._initFunc = initFunc;
            this._hasInitFunc = hasInitFunc;
            this.Queueable = queueable;
            this.Historyable = historyable;
            this.CommandFamily = commandFamily;
            this.Info = "";
            this.Cancel = false;
            this.Input = "";
            this._notParsedArgs = new string[] { };
            this._args = new List<object>();

            this._undoAction = undoAction;
            this._redoAction = redoAction;
            this._plainTextDeserializer = plainTextDeserializer;
            this._plainTextSerializer = plainTextSerializer;
            this._xmlSerializer = xmlSerializer;
            this._xmlDeserializer = xmlDeserializer;
        }

        public void ParseArgs(string[] args, string input)
        {
            Input = input;
            _notParsedArgs = args;

            // Parse arguments
            _args = new List<object>();
            if (_hasParser)
            {
                try
                {
                    _args = _parser(NotParsedArgs);
                }
                catch (CommandException ex)
                {
                    throw ex;
                }
            }
            else
            {
                foreach (string arg in NotParsedArgs)
                    Args.Add(arg);
            }
        }

        public void Activate()
        {
            // Init function
            if (!_hasInitFunc) return;
            try
            {
                _initFunc(Args, this);
            }
            catch (CommandException ex)
            {
                throw ex;
            }
        }

        public void Execute()
        {
            // Call with parsed arguments
            try
            {
                _function(Args, this);
            }
            catch (CommandException ex)
            {
                throw ex;
            }
        }

        public void Undo()
        {     
            try
            { 
                _undoAction(this, Snapshot);
            }
            catch (CommandException ex)
            {
                throw ex;
            }
        }

        public void Redo()
        {
            try
            {
                _redoAction(this, Snapshot);
            }
            catch (CommandException ex)
            {
                throw ex;
            }
        }

        public override string ToString()
        {
            return Name + " " + string.Join(" ", NotParsedArgs);
        }

        public XmlSchema? GetSchema()
        {
            return null;
        }

        public void ReadPlainText(StreamReader reader)
        {
            _plainTextDeserializer(this, reader);
        }

        public void WritePlainText(StreamWriter writer)
        {
            _plainTextSerializer(this, writer);
        }

        public void ReadXml(XmlReader reader)
        {
            _xmlDeserializer(this, reader);
        }

        public void WriteXml(XmlWriter writer)
        {
            _xmlSerializer(this, writer);
        }
    }
}
