# PROJOB-CLI
The purpose of this project is to simulate database behavior and offer a command-line interface (CLI) that enables the use of quasi-SQL commands. The project was developed for the Objective Patterns course at my university.

Some of the Object-oriented patterns used are:
- Singleton,
- Adapter,
- Command,
- Iterators,
- Memento.

In the whole project, reflecion was prohibited.

## Tasks description
The project undertaken during the course involved developing an application to store and manage data related to various objects in two different representation formats. Throughout the project, a focus was placed on modularity, allowing for easy addition of new functionalities without requiring extensive modifications to the existing codebase. 

The tasks assigned during the project can be summarized as follows:

### Task 1: Data Representation and Interfacing
In this task, the main and secondary representation formats for the data were provided. The main format was designed for human readability and ease of use, while the secondary format was used for storage purposes, considering external dependencies or API requirements. 

The goal was to create classes, structs, and interfaces to store the provided data in both formats. Objects were created for both formats, and the data was printed to ensure consistency. Additionally, an adapter was implemented to access the secondary format through the main format, allowing users to interact with the data seamlessly.

### Task 2: Collection and Algorithm Implementation
The objective of this task was to extend the application's functionality by introducing two collections: a doubly linked list and a vector. These collections supported adding and deleting objects of any representation and provided forward and reverse iterators. 

Two algorithms, namely the Find Algorithm and Print Algorithm, were also implemented. The Find Algorithm allowed users to search for objects in a collection based on a given predicate and search direction. It returned the first matching object or a null value. On the other hand, the Print Algorithm printed all objects in the collection that satisfied a given predicate.

### Task 3: Command-Line Interface Integration
In this task, the application was modified to run in a loop, accepting and processing commands from the command-line interface (CLI). The previously created data and collections were maintained, and the application was made compatible with the new functionalities. The implemented commands included "list" and "find." The "list" command printed all objects of a specified type, presenting their non-reference fields in a readable format. The "find" command allowed users to filter objects based on certain conditions and print the matching results. The application could be gracefully terminated using the "exit" command.

Laboratory Part:
The laboratory part expanded on the previous tasks and introduced an "add" command. This command enabled users to add a new object of a specific type interactively. The user provided information about the object's fields, either in the base or secondary representation format. The program guided the user by presenting the available field names and waited for input to assign field values. The object creation process was completed by entering the "DONE" command, which added the object to the collection, or by entering the "EXIT" command to discard the entered data.

### Task 4: Command Queue 
In this part of the project commands mechanism has been expanded with a queue and commits. Instead of executing commands immediately after their call, each command should be added to a command queue. Commands implemented:
- `edit <name_of_the_class> [<requirement> ...]` - adds new entries to the database,
- `queue print` - prints all commands currently stored in the queue. Each command's name and parameters are displayed in a human-readable form,
- `queue export` {filename} [format] - saves all commands from the queue to the specified file. The supported formats are "XML" (default) and "plaintext". The XML format should contain only necessary fields, while the plain text format should be the same as the command line format. Pasting the content of the file to the console should add the stored commands,
- `queue commit` - executes all commands stored in the queue in the order of their addition. After execution, the queue should be cleared, and the appropriate collection modified.

### Task 5: Deserialization and deletion 
The following commands has been added:
- `delete <name_of_the_class> [<requirement> ...]` - allows deleting a given record if the requirement conditions, similar to the find and edit commands, specify one record uniquely,
- `queue dismiss` - clears all commands currently stored in the queue,
- `queue load {filename}` - loads exported commands saved in a given file to the end of the queue. The loaded commands should be in the same order as they were during exporting. Both XML and plain-text file formats are supported.

### Task 6: Commands history
 In this part of the project commands mechanism has been expanded with a history of all executed commands. Additional commands include
- `history` - lists all executed commands.
- `undo` - reverts the changes made by the most recently executed command.
- `redo` - reapplies the changes made by the most recently undone command.

The program maintains a "command history" to allow for multiple undo/redo steps.
