﻿using GameRental;
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
using System.Collections;

namespace GameRentalClient
{
    public sealed class Application
    {
        private static readonly Application instance = new Application();

        private Dictionary<string, CommandBuilder> _commands = new Dictionary<string, CommandBuilder>();
        private Dictionary<string, CommandFamily> _commandFamilies = new Dictionary<string, CommandFamily>();
        private List<Command> _commandQueue = new List<Command>();


        private List<Command> _commandHistory = new List<Command>();
        private int _commandHistoryIndex = -1;

        public bool CommandQueueActive { get; set; } = false;
        public bool ShowCommandQueueOnAddMessage { get; set; } = true;
        public string CommandQeueuOnAddMessage { get; set; } = "Command has been queued.";

        private string _helpMessage;
        private bool _exit;

        public List<Command> CommandQueue
        {
            get => _commandQueue;
        }

        public List<Command> CommandHistory
        {
            get => _commandHistory;
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

        public IEnumerable<Command> History()
        {
            for (int i = 0; i <= _commandHistoryIndex; i++)
            {
                yield return _commandHistory[i];
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

        public void AddToHistory(Command cmd)
        {
            _commandHistory.RemoveRange(_commandHistoryIndex + 1, _commandHistory.Count - _commandHistoryIndex - 1);

            _commandHistory.Add(cmd);
            _commandHistoryIndex = _commandHistory.Count - 1;
        }

        public void ClearHistory()
        {
            _commandHistory.Clear();
            _commandHistoryIndex = -1;
        }

        public void UndoCommand()
        {
            if (_commandHistoryIndex == -1)
            {
                DisplayWarning("There is nothing to undo.");
                return;
            }

            try
            {
                _commandHistory[_commandHistoryIndex].Undo();
            }
            catch (CommandException exc)
            {
                HandleException(exc, _commandHistory[_commandHistoryIndex]);
            }
            _commandHistoryIndex--;
        }

        public void RedoCommand()
        {
            if (_commandHistoryIndex + 1 == _commandHistory.Count)
            {
                DisplayWarning("There is nothing to redo.");
                return;
            }
            _commandHistoryIndex++;

            try
            {
                _commandHistory[_commandHistoryIndex].Redo();
            }
            catch (CommandException exc)
            {
                HandleException(exc, _commandHistory[_commandHistoryIndex]);
            }
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

                    if (cmd.Queueable && CommandQueueActive)
                    {
                        if (ShowCommandQueueOnAddMessage)
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine(CommandQeueuOnAddMessage);
                            Console.ResetColor();
                        }
                        _commandQueue.Add(cmd);
                    }
                    else
                    {
                        try
                        {
                            cmd.Execute();

                            if (cmd.Historyable)
                            {
                                AddToHistory(cmd);
                            }
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
            Console.Write("[WARNING] ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(warning);
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

        public enum SerializationType
        {
            Queue,
            History
        }

        public void SerializeToXml(string filePath, SerializationType type)
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

                if (type == SerializationType.Queue)
                {
                    foreach (var cmd in _commandQueue)
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
                }
                else
                {
                    for (int i = 0; i <= _commandHistoryIndex; ++i)
                    {
                        var cmd = _commandHistory[i];

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

        public void DeserializeFromXml(string filePath, SerializationType type)
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

            if (type == SerializationType.Queue)
            {
                this._commandQueue.AddRange(cmds);
            }
            else
            {
                this.ClearHistory();

                foreach (var cmd in cmds)
                {
                    try
                    {
                        cmd.Execute();
                        if (cmd.Historyable)
                        {
                            AddToHistory(cmd);
                        }
                    }
                    catch (CommandException exc)
                    {
                        HandleException(exc, cmd);
                    }
                }
            }
        }

        public void SerializeToPlaintext(string filePath, SerializationType type)
        {
            List<Command> reference;

            if (type == SerializationType.Queue)
                reference = _commandQueue;
            else
                reference = _commandHistory;

            using (StreamWriter writer = new StreamWriter(filePath))
            {
                if (type == SerializationType.Queue)
                {
                    foreach (var cmd in reference)
                    {
                        writer.WriteLine(cmd.Name + " " + string.Join(" ", cmd.NotParsedArgs));
                        cmd.WritePlainText(writer);
                    }
                }
                else
                {
                    for (int i = 0; i <= _commandHistoryIndex; ++i)
                    {
                        var cmd = _commandHistory[i];
                        writer.WriteLine(cmd.Name + " " + string.Join(" ", cmd.NotParsedArgs));
                        cmd.WritePlainText(writer);
                    }
                }
            }
        }

        public void DeserializeFromPlaintext(string filePath, SerializationType type)
        {
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

                    if (type == SerializationType.Queue)
                    {
                        this.AddToQueue(cmd);
                    }
                    else
                    {
                        try
                        {
                            cmd.Execute();

                            if (cmd.Historyable)
                            {
                                AddToHistory(cmd);
                            }
                        }
                        catch (CommandException exc)
                        {
                            HandleException(exc, cmd);
                        }
                    }
                }
            }
        }
    }
}
