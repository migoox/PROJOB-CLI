using GameRental;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using System.Reflection.PortableExecutable;

namespace GameRentalClient
{
    public sealed class Application
    {
        private static readonly Application instance = new Application();

        private Dictionary<string, CommandBuilder> _commands = new Dictionary<string, CommandBuilder>();
        private Dictionary<string, CommandFamily> _commandFamilies = new Dictionary<string, CommandFamily>();
        private List<Command> _commandQueue = new List<Command>();

        private string _helpMessage;
        private bool _exit;

        public List<Command> CommandQueue
        {
            get => _commandQueue;
        }

        public Dictionary<string, CommandBuilder> Commands
        {
            get => _commands;
        }

        private Application()
        {
            _exit = false;
        }

        public static Application Instance
        {
            get
            {
                return instance;
            }
        }

        public void AddCommandBuilder(CommandBuilder commandBuilder)
        {
            if (commandBuilder.CommandFamily == "")
            {
                _commands[commandBuilder.Name] = commandBuilder;
                return;
            }

            if (!_commandFamilies.ContainsKey(commandBuilder.CommandFamily))
                throw new Exception("Command family doesn't exist.");

            _commandFamilies[commandBuilder.CommandFamily].Commands.Add(commandBuilder.Name, commandBuilder);
        }

        public void AddCommandFamily(CommandFamily commandFamily)
        {
            if (_commandFamilies.ContainsKey(commandFamily.Name)) 
                return;

            _commandFamilies.Add(commandFamily.Name, commandFamily);
        }

        public void ExecuteCommandQueue()
        {
            foreach (Command cmd in _commandQueue)
            {
                cmd.Execute();
            }
            _commandQueue.Clear();
        }

        public void Run()
        {
            Console.WriteLine("Game Rental Client CLI - type \"help\" for help\n\n");


            while (!_exit)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write(">>> ");
                Console.ResetColor();
                string? input = Console.ReadLine();

                if (string.IsNullOrEmpty(input)) continue;

                var cmd = Parse(input);

                if (cmd != null)
                {
                    cmd.Activate();

                    if (cmd.Cancel) continue;

                    if (cmd.Queueable)
                    {
                        _commandQueue.Add(cmd);
                    }
                    else
                    {
                        try
                        {
                            cmd.Execute();
                        }
                        catch (CommandException exc)
                        {
                            HandleException(exc, cmd);
                        }
                    }
                }
            }
        }

        public Command? Parse(string input)
        {
            string[] parts = Regex.Split(input, @" +(?=(?:[^""]*""[^""]*"")*[^""]*$)");

            parts = parts.Where(x => x != "").ToArray();

            if (parts.Length == 0) return null;

            if (_commandFamilies.ContainsKey(parts[0]))
            {
                // Command family
                string family = parts[0];
                if (parts.Length == 1)
                {
                    // DisplayError($"Command family \"{family}\" needs a member");
                    DisplayManual(family);
                    return null;
                }

                string name = parts[1];
                if (!_commandFamilies[family].Commands.ContainsKey(name))
                {
                    DisplayError($"Command family \"{family}\" doesn't have member \"{name}\"");
                    DisplayManual(family);

                    return null;
                }

                string[] args = new string[parts.Length - 2];
                Array.Copy(parts, 2, args, 0, args.Length);

                Command cmd = _commandFamilies[family].Commands[name].Build();

                try
                {
                    cmd.ParseArgs(args, input);
                }
                catch (CommandException exc)
                {
                    HandleException(exc, cmd);
                    return null;
                }

                return cmd;
            }
            else
            {
                // No command family
                string name = parts[0];

                if (!_commands.ContainsKey(name))
                {
                    DisplayError($"Command \"{name}\" not found");
                    return null;
                }
                
                string[] args = new string[parts.Length - 1];
                Array.Copy(parts, 1, args, 0, args.Length);

                Command cmd = _commands[name].Build();

                try
                {
                    cmd.ParseArgs(args, input);
                }
                catch (CommandException exc)
                {
                    HandleException(exc, cmd);
                    return null;
                }

                return cmd;
            }
        }

        public void Exit()
        {
            _exit = true;
        }

        public void DisplayWarning(string warning)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("[WARNING] " + warning);
            Console.ResetColor();
        }

        public void DisplayError(string error)
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.Write("[ERROR] ");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(error);
            Console.ResetColor();
        }

        public void DisplayUsage(string usage)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("\nUSAGE:");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("\t" + usage + "\n");
            Console.ResetColor();
        }

        public void DisplayHelp()
        {
            int index = 1;
            Console.WriteLine($"\nCommands:\n");
            foreach (var item in _commands)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write($"\t{index}. ");
                Console.Write($"{item.Key} ");
                Console.ResetColor();
                Console.WriteLine($"- {item.Value.Description}");

                index++;
            }

            Console.WriteLine($"\nCommand families:\n");
            index = 1;
            foreach (var item in _commandFamilies)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write($"\t{index}. ");
                Console.Write($"{item.Key} ");
                Console.ResetColor();
                Console.WriteLine($"- {item.Value.Description}");

                index++;
            }
        }

        public void DisplayManual(string cmdName, string family = "")
        {
            if (family == "")
            {
                if (_commands.ContainsKey(cmdName))
                {
                    DisplayCommandManual(cmdName);
                }
                else if (_commandFamilies.ContainsKey(cmdName))
                {
                    DisplayFamilyManual(cmdName);
                }
                else
                {
                    throw new Exception($"Command or command family \"{cmdName}\" not found");
                }
            }
            else if (_commandFamilies.ContainsKey(family) && _commandFamilies[family].Commands.ContainsKey(cmdName))
            {
                DisplayFamilyCommandManual(cmdName, family);
            }
            else
            {
                throw new Exception($"Command \"{cmdName}\" not found in family \"{family}\"");
            }
        }

        public void DisplayCommandManual(string cmdName)
        {
            var command = _commands[cmdName];

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"\nManual for \"{cmdName}\"\n");

            Console.Write("DESCRIPTION: ");
            Console.ResetColor();
            Console.WriteLine(command.Description + "\n");

            Console.WriteLine(command.Manual + "\n");
        }

        public void DisplayFamilyManual(string cmdName)
        {
            var family = _commandFamilies[cmdName];

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"\nManual for \"{cmdName}\"\n");

            Console.Write("DESCRIPTION: ");
            Console.ResetColor();
            Console.WriteLine(family.Description + "\n");

            Console.WriteLine(family.Manual);

            int index = 1;
            foreach (var command in family.Commands.Values)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write($"\t{index}. ");
                Console.Write($"{command.Name} ");
                Console.ResetColor();
                Console.WriteLine($"- {command.Description}" + "\n");
                Console.WriteLine(command.Manual + "\n");

                index++;
            }
        }

        public void DisplayFamilyCommandManual(string cmdName, string family)
        {
            var command = _commandFamilies[family].Commands[cmdName];

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"\nManual for \"{family} {cmdName}\"\n");

            Console.Write("DESCRIPTION: ");
            Console.ResetColor();
            Console.WriteLine($"- {command.Description}" + "\n");

            Console.WriteLine(command.Manual + "\n");
        }

        public void HandleException(CommandException exc, Command cmd, string additionalMessage = "")
        {
            if (exc.Error)
                DisplayError(exc.Message);

            if (exc.Usage)
            {
                if (_commands.ContainsKey(cmd.Name))
                {
                    DisplayUsage(_commands[cmd.Name].Usage);
                }
                else if (_commandFamilies.ContainsKey(cmd.CommandFamily))
                {
                    DisplayUsage(_commandFamilies[cmd.CommandFamily].Commands[cmd.Name].Usage);

                }
            }
        }

        public void ClearCommandQueue()
        {
            this._commandQueue.Clear();
        }

        public void AddToQueue(Command command)
        {
            _commandQueue.Add(command);
        }

        public void SerializeQueueToXml(string filePath)
        {
            StringWriter stringWriter = new StringWriter();
            using (XmlTextWriter xmlWriter = new XmlTextWriter(stringWriter))
            {
                // Formatting options for the XML
                xmlWriter.Formatting = Formatting.Indented;
                xmlWriter.Indentation = 4;

                // Write the XML declaration
                xmlWriter.WriteStartDocument();
                xmlWriter.WriteStartElement("Commands");

                foreach (var cmd in Application.Instance.CommandQueue)
                {
                    xmlWriter.WriteStartElement("Command");
                    xmlWriter.WriteAttributeString("Name", $"{cmd.Name}");
                    xmlWriter.WriteStartElement("Arguments");

                    foreach (string arg in cmd.NotParsedArgs)
                    {
                        xmlWriter.WriteStartElement("Argument");
                        xmlWriter.WriteValue(arg);
                        xmlWriter.WriteEndElement();
                    }

                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("Context");
                    cmd.WriteXml(xmlWriter);
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteEndElement();
                }

                xmlWriter.WriteEndElement();
                xmlWriter.WriteEndDocument();

                xmlWriter.Flush();
            }

            // Get the XML content from the StringWriter
            string xmlContent = stringWriter.ToString();
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlContent);
            xmlDoc.Save(filePath);
        }

        public void DeserializeQueueFromXml(string filePath)
        {
            List<Command> cmds = new List<Command>();
            using (XmlReader xmlReader = XmlReader.Create(filePath))
            {
                StringBuilder inputBuilder = new StringBuilder();

                string currentElement = "";
                int cmdIndex = 0;
                while (xmlReader.Read())
                {
                    if (xmlReader.NodeType == XmlNodeType.Element)
                    {
                        currentElement = xmlReader.Name;

                        if (currentElement == "Context")
                        {
                            cmds[cmdIndex].ReadXml(xmlReader);
                        }

                        if (xmlReader.HasAttributes)
                        {
                            while (xmlReader.MoveToNextAttribute())
                            {
                                if (currentElement == "Command")
                                {
                                    inputBuilder.Append(xmlReader.Value).Append(" ");
                                }
                            }
                            xmlReader.MoveToElement();
                        }
                    }
                    else if (xmlReader.NodeType == XmlNodeType.Text)
                    {
                        if (currentElement == "Argument")
                        {
                            inputBuilder.Append(xmlReader.Value).Append(" ");
                        }
                    }
                    else if (xmlReader.NodeType == XmlNodeType.EndElement)
                    {
                        if (xmlReader.Name == "Arguments")
                        {
                            cmds.Add(Parse(inputBuilder.ToString()));

                            inputBuilder.Clear();
                        }

                        if (xmlReader.Name == "Command")
                        {
                            ++cmdIndex;
                        }
                    }
                }
            }

            _commandQueue.AddRange(cmds);
        }

        public void SerializeQueueToPlaintext(string filePath)
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                foreach (var cmd in Application.Instance.CommandQueue)
                {
                    writer.WriteLine(cmd.Name + " " + string.Join(" ", cmd.NotParsedArgs));
                    cmd.WritePlainText(writer);
                }
            }
        }

        public void DeserializeQueueFromPlaintext(string filePath, bool truncateQueue = false)
        {
            if (truncateQueue) _commandQueue.Clear();
            
            using (StreamReader reader = new StreamReader(filePath))
            {
                while (!reader.EndOfStream)
                {
                    string input = reader.ReadLine();
                    if (input == null) break;

                    Command cmd = Parse(input);

                    if (cmd == null)
                        throw new Exception("Parser error");

                    cmd.ReadPlainText(reader);

                    _commandQueue.Add(cmd);
                }
            }
        }
    }
}
