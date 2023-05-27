using GameRental;
using GameRental.Adapters;
using GameRental.Rep0;
using GameRental.Rep8;
using System.Text.RegularExpressions;
using System.Collections;
using System.Reflection.Metadata;
using GameRental.Extensions;
using System;
using System.Globalization;
using System.Net.NetworkInformation;
using System.Xml;
using GameRental.Builders;
using System.Xml.Serialization;

namespace GameRentalClient
{
    class Program
    {
        static IEnumerable<IDatabaseEntity> Filter(List<object> args)
        {
            ICollection entries = (ICollection)args[0];
            bool[] filter = new bool[entries.Count];
            int i = 0;

            for (i = 0; i < entries.Count; ++i)
            {
                filter[i] = true;
            }

            for (i = 1; i < args.Count; ++i)
            {
                object[] arg = (object[])args[i];

                string fieldName = (string)arg[0];

                Func<IComparable, IComparable, bool> condition =
                    (Func<IComparable, IComparable, bool>)arg[1];

                IComparable right = (IComparable)arg[2];

                try
                {
                    int current = 0;
                    foreach (IDatabaseEntity entry in entries)
                    {
                        if (filter[current])
                        {
                            if (!condition((IComparable)entry.GetField(fieldName), right))
                                filter[current] = false;
                        }
                        ++current;
                    }
                }
                catch (Exception ex)
                {
                    throw new CommandInvalidArgumentsException(ex.Message);
                }
            }

            i = 0;
            foreach (IDatabaseEntity entry in entries)
            {
                if (filter[i])
                    yield return entry;

                ++i;
            }
        }

        static (string[], object) GetPartsAddEditInterface(string tableName, string input)
        {
            string[] parts = Regex.Split(input, @"(=)");

            if (parts.Length != 3)
                throw new Exception("Incorrect syntax");

            if (parts[2].Length < 1)
                throw new Exception("Incorrect syntax");

            object val;

            if (parts[2][0] == '[')
            {
                val = Parser.ParseIntArray(parts[2]);
            }
            else
            {
                val = Parser.ComparableTypeParser(parts[2]);
            }

            Type type = val.GetType();
            Type desiredType = AbstractBuilder.GetFieldType(tableName, parts[0]);
            if (desiredType != type)
                throw new Exception($"Incompatible fields, \"{desiredType}\" was expected, while \"{type}\" was received");

            return (parts, val);
        }

        static Dictionary<string, object?>? RunAddEditInterface(string tableName, Command thisCmd)
        {
            Dictionary<string, object?> dictionary =
                AbstractBuilder.GetAvailableFieldTypes(tableName).ToDictionary(h => h, h => (object)null);

            Console.WriteLine($"\nAvailable fields: {string.Join(", ", AbstractBuilder.GetAvailableFieldTypes(tableName).ToArray())}\n");
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("> ");
                Console.ResetColor();
                string input = Console.ReadLine();

                if (string.IsNullOrEmpty(input)) continue;

                // add to input
                thisCmd.Input += "\n" + input;

                if (input == "EXIT")
                {
                    thisCmd.Cancel = true;
                    return null;
                }

                if (input == "DONE") break;


                // Try to parse and add
                try
                {
                    (string[] parts, object val) = GetPartsAddEditInterface(tableName, input);

                    if (!dictionary.ContainsKey(parts[0]))
                        throw new Exception("Field is not available");

                    dictionary[parts[0]] = val;
                }
                catch (Exception ex)
                {
                    Application.Instance.DisplayError(ex.Message);
                }
            }

            return dictionary;
        }

        static int Main(string[] args)
        {
            Dictionary<string, Func<IComparable, IComparable, bool>> delegates = new Dictionary<string, Func<IComparable, IComparable, bool>>();
            delegates.Add("=", (a, b) => a.Equals(b));
            delegates.Add("<", (a, b) => a.CompareTo(b) < 0);
            delegates.Add(">", (a, b) => a.CompareTo(b) > 0);
            delegates.Add("<=", (a, b) => a.CompareTo(b) <= 0);
            delegates.Add(">=", (a, b) => a.CompareTo(b) >= 0);

            Database.Instance.FillWithRep0();

            Application.Instance.AddCommandBuilder(new CommandBuilder()
                .WithName("help")
                .WithDescription("lists all available commands with their description and usage")
                .WithCall((args, thisCmd) =>
                {
                    Application.Instance.DisplayHelp();
                    Console.WriteLine("\nType \"man <name_of_the_command>\" to receive detailed info.\n");
                    return 0;
                }));

            Application.Instance.AddCommandBuilder(new CommandBuilder()
                .WithName("clear")
                .WithDescription("clears console output")
                .WithCall((args, thisCmd) =>
                {
                    Console.Clear();
                    return 0;
                }));

            Application.Instance.AddCommandBuilder(new CommandBuilder()
                .WithName("exit")
                .WithDescription("gracefully finish execution of your application")
                .WithCall((args, thisCmd) =>
                {
                    if (args.Count > 0)
                        throw new CommandTooManyArgumentsException();
                    Application.Instance.Exit();
                    return 0;
                }));

            Application.Instance.AddCommandBuilder(new CommandBuilder()
                .WithName("man")
                .WithDescription("displays manual of the command given as an argument")
                .WithUsage("man <name_of_the_command>, man <name_of_the_family> <name_of_the_command>")
                .WithCall((args, thisCmd) =>
                {
                    if (args.Count == 0)
                        throw new CommandNotEnoughInputArgumentsException().WithErrorFlag(false);
                    if (args.Count == 1)
                    {
                        try
                        {
                            Application.Instance.DisplayManual((string)args[0]);
                        }
                        catch (Exception exc)
                        {
                            throw new CommandException(exc.Message).WithUsageFlag(false);
                        }
                    }
                    else if (args.Count == 2)
                    {
                        try
                        {
                            Application.Instance.DisplayManual((string)args[1], (string)args[0]);
                        }
                        catch (Exception exc)
                        {
                            throw new CommandException(exc.Message).WithUsageFlag(false);
                        }
                    }
                    else
                    {
                        throw new CommandTooManyArgumentsException();
                    }
                   

                    return 0;
                }));

            Application.Instance.AddCommandBuilder(new CommandBuilder()
                .WithName("list")
                .WithDescription("prints all objects of a particular type")
                .WithManual("The format of the command should be as follows:\r\n\r\n    list <name_of_the_class>\r\n\r\nThe command should print to the console all of the objects of this class where printing an object\r\nmeans listing all of its fields.\r\n\r\nExample usages:\r\n    list game\r\n    list animal")
                .WithUsage("list <name_of_the_class>")
                .WithParser(args =>
                {
                    if (args.Length < 1)
                        throw new CommandNotEnoughInputArgumentsException().WithErrorFlag(false);
                    if (args.Length > 1)
                        throw new CommandTooManyArgumentsException();

                    string tableName = args[0];
                    try
                    {
                        var table =
                            Database.Instance.GetTable(tableName).Values;
                        return new List<object>() { table };
                    }
                    catch (Exception ex)
                    {
                        throw new CommandInvalidArgumentsException(ex.Message);
                    }
                })
                .WithCall((args, thisCmd) =>
                {
                    var table = (ICollection)args.First();
                    foreach (var item in table)
                    {
                        Console.WriteLine(item);
                    }

                    return 0;
                }));

            CommandBuilder findBuilder = new CommandBuilder()
                .WithName("find")
                .WithDescription("prints objects matching certain conditions")
                .WithManual("Find works similarly to 'list', but you can also add optional parameters for filtering results. Only\r\nresults that fulfil all of the requirements are to be printed.\r\n\r\nThe format of the command should be as follows:\r\n\r\n    find <name_of_the_class> [<requirement>...]\r\n\r\nwhere requirements (space separated list of requirements) specify acceptable values of atomic non\r\nreference fields. They follow format:\r\n\r\n    <name_of_field>=|<|><value>\r\n\r\nWhere \"=|<|>\" means any strong comparison operator. For numerical fields natural comparison should\r\nbe used. Strings should use a lexicographic order. For other types only \"=\" is allowed. If a value\r\nwere to contain spaces it should be placed inside quotation marks.\r\n\r\nExample usage:\r\n    find game name=\"Elden Ring\"")
                .WithUsage("find <name_of_the_class> [<requirement>...]")
                .WithParser(args =>
                {
                    // Check arguments count
                    if (args.Length < 1)
                        throw new CommandNotEnoughInputArgumentsException().WithErrorFlag(false);

                    // Create list of parsed arguments
                    List<object> objects = new List<object>();

                    // Get correct table
                    string tableName = args[0];
                    ICollection entries; 
                    try
                    {
                        entries = Database.Instance.GetTable(tableName).Values;
                    }
                    catch (Exception ex)
                    {
                        throw new CommandInvalidArgumentsException(ex.Message);
                    }

                    objects.Add(entries);

                    for (int i = 1; i < args.Length; ++i)
                    {
                        // Get split argument to objects
                        string[] parts = Regex.Split(args[i], @"(<=|<|>=|>|=)");
                        if (parts.Length != 3)
                            throw new CommandInvalidArgumentsException(
                                "Required format: <name_of_field>(=|<|>|<=|>=)<value>");

                        object[] argParsed = new object[3];

                        // String left value which represents entry in the table
                        argParsed[0] = parts[0];

                        // Func<IComparable, IComparable, bool>
                        argParsed[1] = delegates[parts[1]];

                        // IComparable right value
                        argParsed[2] = Parser.ComparableTypeParser(parts[2]);

                        // Add argument
                        objects.Add(argParsed);
                    }

                    return objects;
                })
                .WithCall((args, thisCmd) =>
                {
                    // Print Entries
                    foreach (IDatabaseEntity entry in Filter(args))
                    {
                        Console.WriteLine(entry);
                    }

                    return 0;
                });
            Application.Instance.AddCommandBuilder(findBuilder);

            var addCommandBuilder = new CommandBuilder()
                .WithQueueable(true)
                .WithName("add")
                .WithDescription("adds a new object of a particular type")
                .WithManual(
                    "\r\n    add <name_of_the_class> base|secondary\r\n\r\nwhere base|secondary defines the representation in which the object should be created. After\r\nreceiving the first line the program waits for further instructions from the user\r\nThe format for each line is as follows:\r\n\r\n\t<name_of_field>=<value>\r\n\r\nA line like that means that the value of the field <name_of_field> for the newly created object\r\nshould be equal to <value>. The user can enter however many lines they want in such a format (even\r\nrepeating the fields that they have already defined - in this case the previous value is overridden)\r\ndescribing the object until using one of the following commands: DONE or EXIT\r\n\r\nAfter receiving the DONE command the creation process should finish and the program should add a new\r\nobject described by the user to the collection. After receiving the EXIT command the creation\r\nprocess should also finish but no new object is created and nothing is added to the collection. The\r\ndata provided by the user is also discarded.\r\n\r\nExample usages:\r\n    add book base\r\n    [Available fields: 'title, year, pageCount']\r\n    title=\"The Right Stuff\"\r\n    year=1993\r\n    name=abc\r\n    [Some sensible error message]\r\n    DONE\r\n    [Book created]\r\n\r\nadd book secondary\r\n    [Available fields: \"title, year, pageCount\"]\r\n    title=\"The Right Stuff\"\r\n    EXIT\r\n    [Book creation abandoned]\r\n")
                .WithUsage("add <name_of_the_class> base|secondary")
                .WithParser(args =>
                {
                    if (args.Length == 0)
                        throw new CommandNotEnoughInputArgumentsException().WithErrorFlag(false);
                    if (args.Length < 2)
                        throw new CommandNotEnoughInputArgumentsException();
                    if (args.Length > 2)
                        throw new CommandTooManyArgumentsException();

                    List<object> objects = new List<object>();

                    string tableName = args[0];

                    // Add table name
                    objects.Add(tableName);

                    // Add Builder
                    try
                    {
                        var builder = AbstractBuilder.GetBuilderByType(tableName);
                        objects.Add(builder);
                    }
                    catch (Exception ex)
                    {
                        throw new CommandInvalidArgumentsException(ex.Message);
                    }

                    // Add base/secondary argument
                    string[] argsArray = args.ToArray();
                    if (Regex.IsMatch(argsArray[1], @"(base)|(secondary)"))
                    {
                        objects.Add(argsArray[1]);
                    }
                    else
                    {
                        throw new CommandInvalidArgumentsException(
                            "Second argument shold be \"base\" or \"secondary\"");
                    }

                    return objects;
                })
                .WithPlaintextSerializer((thisCmd, writer) =>
                {
                    var dictionary = (Dictionary<string, object?>)thisCmd.ContextData[0];

                    foreach (var elem in dictionary)
                    {
                        if (elem.Value == null) continue;
                        writer.WriteLine(elem.Key + "=" + Parser.FormatObject(elem.Value));
                    }

                    writer.WriteLine("DONE");
                })
                .WithXmlSerializer((thisCmd, xmlWriter) =>
                {
                    var dictionary = (Dictionary<string, object?>)thisCmd.ContextData[0];

                    foreach (var elem in dictionary)
                    {
                        if (elem.Value == null) continue;

                        xmlWriter.WriteStartElement(CultureInfo.CurrentCulture.TextInfo.ToTitleCase(elem.Key));
                        xmlWriter.WriteValue(Parser.FormatObject(elem.Value));
                        xmlWriter.WriteEndElement();
                    }
                })
                .WithPlaintextDeserializer((thisCmd, reader) =>
                {
                    var tableName = (string)thisCmd.Args[0];
                    Dictionary<string, object?> dictionary =
                        AbstractBuilder.GetAvailableFieldTypes(tableName).ToDictionary(h => h, h => (object)null);

                    string? input = reader.ReadLine();

                    while (input != null && input != "DONE")
                    {
                        try
                        {
                            (string[] parts, object val) = GetPartsAddEditInterface(tableName, input);

                            if (!dictionary.ContainsKey(parts[0]))
                                throw new Exception("Field is not available");

                            dictionary[parts[0]] = val;
                        }
                        catch (Exception ex)
                        {
                            throw new CommandException(ex.Message);
                        }

                        input = reader.ReadLine();
                    }

                    foreach (var pair in dictionary)
                    {
                        thisCmd.Info += pair.Key + "=" + (pair.Value == null ? "null" : pair.Value) + "\n";
                    }

                    thisCmd.ContextData.Add(dictionary);

                    thisCmd.Args.Add(dictionary);
                })
                .WithXmlDeserializer((thisCmd, xmlReader) =>
                {
                    var tableName = (string)thisCmd.Args[0];
                    Dictionary<string, object?> dictionary =
                        AbstractBuilder.GetAvailableFieldTypes(tableName).ToDictionary(h => h, h => (object)null);

                    string currentElement = "";
                    while (xmlReader.Read())
                    {

                        if (xmlReader.NodeType == XmlNodeType.Element)
                        {
                            currentElement = xmlReader.Name.ToLower();
                        }
                        else if (xmlReader.NodeType == XmlNodeType.Text)
                        {
                            string input = currentElement + "=" + xmlReader.Value;
                            try
                            {
                                (string[] parts, object val) = GetPartsAddEditInterface(tableName, input);

                                if (!dictionary.ContainsKey(parts[0]))
                                    throw new Exception("Field is not available");

                                dictionary[parts[0]] = val;
                            }
                            catch (Exception ex)
                            {
                                throw new CommandException(ex.Message);
                            }
                        }
                        else if (xmlReader.NodeType == XmlNodeType.EndElement)
                        {
                            if (xmlReader.Name == "Context")
                                break;
                        }
                    }

                    foreach (var pair in dictionary)
                    {
                        thisCmd.Info += pair.Key + "=" + (pair.Value == null ? "null" : pair.Value) + "\n";
                    }

                    thisCmd.ContextData.Add(dictionary);

                    thisCmd.Args.Add(dictionary);
                })
                .WithInit((args, thisCmd) =>
                {
                    var tableName = (string)args[0];
                    var builder = (IDatabaseBuilder)args[1];
                    var type = (string)args[2];

                    var dictionary = RunAddEditInterface(tableName, thisCmd);

                    if (dictionary == null)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        Console.WriteLine($"\nAborted: no changes queued\n");
                        Console.ResetColor();
                        thisCmd.Cancel = true;
                        return 0;
                    }

                    thisCmd.ContextData.Add(dictionary);
                    args.Add(dictionary);

                    Console.WriteLine("");

                    foreach (var pair in dictionary)
                    {
                        Console.WriteLine(pair.Key + "=" + (pair.Value == null ? "null" : pair.Value));

                        thisCmd.Info += pair.Key + "=" + (pair.Value == null ? "null" : pair.Value) + "\n";

                        if (pair.Value == null) continue;


                        builder.With(pair.Key, pair.Value);
                    }

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"\nSuccess: {tableName} changes queued\n");
                    Console.ResetColor();

                    return 0;
                })
                .WithCall((args, thisCmd) =>
                {
                    var tableName = (string)args[0];
                    var builder = (IDatabaseBuilder)args[1];
                    var type = (string)args[2];
                    var dictionary = (Dictionary<string, object?>)args[3];

                    foreach (var pair in dictionary)
                    {
                        if (pair.Value == null) continue;

                        builder.With(pair.Key, pair.Value);
                    }

                    if (type == "base")
                    {
                        Database.Instance.Add(tableName, builder.BuildRep0());
                    }
                    else
                    {
                        Database.Instance.Add(tableName, builder.BuildRep8AndAdapt());
                    }

                    return 0;
                });
            Application.Instance.AddCommandBuilder(addCommandBuilder);

            Application.Instance.AddCommandBuilder(new CommandBuilder()
                .WithQueueable(true)
                .WithName("edit")
                .WithUsage("edit <name_of_the_class> [<requirement> ...].")
                .WithManual("This command allows editing a given record if requirement conditions (which work the\r\nsame as in the find command) specify one record uniquely. Editing works the same as\r\nadding a new element\r\n\t<name_of_field>=<value>\r\nreplace the field's old value with a new one until DONE or EXIT is provided. When \r\nEXIT is chosen, it does not modify any value.")
                .WithDescription("edits values of the given record")
                .WithParser(args =>
                {
                    if (args.Length == 0)
                        throw new CommandNotEnoughInputArgumentsException().WithErrorFlag(false);

                    List<object> objects = new List<object>();

                    string tableName = args[0];

                    // Add table name
                    objects.Add(tableName);

                    objects.Add(findBuilder.Parser(args));
                    return objects;
                })
                .WithPlaintextSerializer((thisCmd, writer) =>
                {
                    addCommandBuilder.PlainTextSerializer(thisCmd, writer);
                })
                .WithXmlSerializer((thisCmd, xmlWriter) =>
                {
                    addCommandBuilder.XmlSerializer(thisCmd, xmlWriter);
                })
                .WithPlaintextDeserializer((thisCmd, reader) =>
                {
                    addCommandBuilder.PlainTextDeserializer(thisCmd, reader);
                })
                .WithXmlDeserializer((thisCmd, xmlReader) =>
                {
                    addCommandBuilder.XmlDesereializer(thisCmd, xmlReader);
                })
                .WithInit((args, thisCmd) =>
                {
                    var tableName = (string)args[0];
                    var dictionary = RunAddEditInterface(tableName, thisCmd);
                    if (dictionary == null)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        Console.WriteLine($"\nAborted: no changes queued\n");
                        Console.ResetColor();
                        thisCmd.Cancel = true;
                        return 0;
                    }

                    args.Add(dictionary);
                    thisCmd.ContextData.Add(dictionary);
                    Console.WriteLine("");

                    foreach (var pair in dictionary)
                    {
                        if (pair.Value == null) continue;

                        Console.WriteLine(pair.Key + "=" + (pair.Value == null ? "null" : pair.Value));

                        thisCmd.Info += pair.Key + "=" + (pair.Value == null ? "null" : pair.Value) + "\n";
                    }

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"\nSuccess: {tableName} changes queued\n");
                    Console.ResetColor();

                    return 0;
                })
                .WithCall((args, thisCmd) =>
                { 
                    List<object> findArgs = (List<object>)args[1];

                    Dictionary<string, object?> dictionary = (Dictionary<string, object?>)args[args.Count - 1];

                    foreach (var entry in Filter(findArgs))
                    {
                        foreach (var pair in dictionary)
                        {
                            if (pair.Value == null) continue;

                            entry.SetField(pair.Key, pair.Value);
                        }
                    }

                    return 0;
                }));
                
            Application.Instance.AddCommandBuilder(new CommandBuilder()
                .WithQueueable(true)
                .WithName("delete")
                .WithUsage("delete <name_of_the_class> [<requirement> ...].")
                .WithManual(
                    "delete <name_of_the_class> [<requirement> …] - removes given record from collections.\n\nThis command allows deleting a given record if requirement conditions (which work the\nsame as in the find and edit command) specify one record uniquely.")
                .WithDescription("allows deleting a given record if requirement conditions specify one record uniquely")
                .WithParser(args =>
                {
                    if (args.Length == 0)
                        throw new CommandNotEnoughInputArgumentsException().WithErrorFlag(false);

                    List<object> objects = new List<object>();

                    string tableName = args[0];

                    // Add table name
                    objects.Add(tableName);

                    objects.Add(findBuilder.Parser(args));
                    return objects;
                })
                .WithCall((args, thisCmd) =>
                {
                    List<object> findArgs = (List<object>)args[1];

                    IEnumerable<IDatabaseEntity>? filtered = Filter(findArgs);

                    if (filtered == null || filtered.Count() != 1)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        Console.WriteLine("More than one entry found, no deletion applied.");
                        Console.ResetColor();
                        return 0;
                    }

                    var elem = filtered.First();
                    elem.Delete();
                    Database.Instance.GetTable(elem.TypeName).Remove(elem.Id);

                    return 0;
                }));

            /* QUEUE FAMILY */

            Application.Instance.AddCommandFamily(new CommandFamilyBuilder()
                .WithName("queue")
                .WithDescription("allows command queue management")
                .Build());

            Application.Instance.AddCommandBuilder(new CommandBuilder()
                .WithFamily("queue")
                .WithName("print")
                .WithManual("This command should print each stored in queue commands its name and all command\r\nparameters in human-readable form.")
                .WithDescription("prints all commands currently stored in the queue")
                .WithCall((args, thisCmd) =>
                {
                    int index = 1;
                    Console.WriteLine("\nQueue state:\n");
                    foreach (var cmd in Application.Instance.CommandQueue)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write($"{index}.");
                        Console.WriteLine($" {cmd}");
                        Console.ResetColor();
                        Console.WriteLine($"{cmd.Info}");
                        Console.ResetColor();
                        ++index;
                    }
                    return 0;
                }));

            Application.Instance.AddCommandBuilder(new CommandBuilder()
                .WithFamily("queue")
                .WithName("dismiss")
                .WithManual("This command clears all commands which are currently stored in the queue.")
                .WithDescription("clears all commands which are currently stored in the queue")
                .WithCall((args, thisCmd) =>
                {
                    Application.Instance.ClearCommandQueue();
                    return 0;
                }));

            Application.Instance.AddCommandBuilder(new CommandBuilder()
                .WithFamily("queue")
                .WithName("commit")
                .WithDescription("execute all commands from the queue")
                .WithManual("This command executes all commands stored in the queue in order of their \r\naddition. After that queue should be cleared and proper collection modified.")
                .WithCall((args, thisCmd) =>
                {
                    Application.Instance.ExecuteCommandQueue();
                    return 0;
                }));

            Application.Instance.AddCommandBuilder(new CommandBuilder()
                .WithFamily("queue")
                .WithName("export")
                .WithDescription("exports all commands currently stored in the queue to the specified file")
                .WithUsage("queue export {filename}, queue export {filename} [format]")
                .WithManual("\tqueue export {filename} [format]\n\nThis command saves all commands from the queue to the file. There are supported \r\ntwo formats \"XML\" (default) and \"plaintext\". The structure of XML should contain \r\nonly necessary fields. The plain text format should be the same as it is in the\r\ncommand line – that means that pasting the content of the file to the console \r\nshould add stored commands.")
                .WithParser(args =>
                {
                    if (args.Length == 0)
                        throw new CommandNotEnoughInputArgumentsException().WithErrorFlag(false);

                    if (args.Length == 1)
                        args = new string[] { args[0], "xml" };

                    if (args.Length < 2)
                        throw new CommandNotEnoughInputArgumentsException();
                    if (args.Length > 2)
                        throw new CommandTooManyArgumentsException();

                    try
                    {
                        args[0] = (string)Parser.ComparableTypeParser(args[0]);
                    }
                    catch (Exception exc)
                    {
                        throw new CommandInvalidArgumentsException($"Parser failed, filename \"{args[0]}\" is invalid");
                    }

                    args[1] = args[1].ToLower();

                    if (args[1] != "xml" && args[1] != "plaintext")
                        throw new CommandInvalidArgumentsException("format should be \"xml\" or \"plaintext\"");

                    return new List<object>() { args[0], args[1] };

                })
                .WithCall((args, thisCmd) =>
                {
                    string filename = (string)args[0];
                    string format = (string)args[1];

                    if (format == "xml")
                    {
                        Application.Instance.SerializeQueueToXml(filename);
                    }
                    else // plaintext
                    {
                        Application.Instance.SerializeQueueToPlaintext(filename);
                    }

                    return 0;
                }));

            Application.Instance.AddCommandBuilder(new CommandBuilder()
                .WithFamily("queue")
                .WithName("import")
                .WithDescription("imports all commands currently stored in the specified file into command queue")
                .WithUsage("queue import {filename}")
                .WithManual("\tqueue import {filename}\n\nThis command imports commands from the specified file to the command queue. \r\nThe file format should be either XML. The file should contain only the necessary fields.")
                .WithParser(args =>
                {
                    if (args.Length == 0)
                        throw new CommandNotEnoughInputArgumentsException().WithErrorFlag(false);

                    if (args.Length == 1)
                        args = new string[] { args[0], "xml" };

                    if (args.Length < 2)
                        throw new CommandNotEnoughInputArgumentsException();
                    if (args.Length > 2)
                        throw new CommandTooManyArgumentsException();

                    try
                    {
                        args[0] = (string)Parser.ComparableTypeParser(args[0]);
                    }
                    catch (Exception exc)
                    {
                        throw new CommandInvalidArgumentsException($"Parser failed, filename \"{args[0]}\" is invalid");
                    }

                    args[1] = args[1].ToLower();

                    if (args[1] != "xml" && args[1] != "plaintext")
                        throw new CommandInvalidArgumentsException("format should be \"xml\" or \"plaintext\"");

                    return new List<object>() { args[0], args[1] };

                })
                .WithCall((args, thisCmd) =>
                {
                    string filename = (string)args[0];
                    string format = (string)args[1];

                    if (format == "xml")
                    {
                        Application.Instance.DeserializeQueueFromXml(filename);
                    }
                    else // plaintext
                    {
                        Application.Instance.DeserializeQueueFromPlaintext(filename);
                    }

                    return 0;
                }));

            Application.Instance.Run();
            return 0;
        }
    }
}