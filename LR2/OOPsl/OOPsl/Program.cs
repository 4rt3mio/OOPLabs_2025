//using OOPsl;
//Console.BackgroundColor = ConsoleColor.Black;
//string filePath = "D:\\фигня/qt.txt";
//var editor = new YourEditorClass();
//editor.Run(filePath);
using OOPsl.MenuFunctions;
using OOPsl;
using OOPsl.DocumentFunctions.Managers;

UserManager userManager = new UserManager();
DocumentAccessManager accessManager = new DocumentAccessManager();
DocumentManager documentManager = new DocumentManager(accessManager);
IMenu consoleMenu = new ConsoleMenu(userManager, documentManager);
ApplicationMenu appMenu = new ApplicationMenu(consoleMenu);
appMenu.Run();