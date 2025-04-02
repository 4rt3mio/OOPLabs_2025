//using OOPsl;
//Console.BackgroundColor = ConsoleColor.Black;
//string filePath = "D:\\фигня/qt.txt";
//var editor = new YourEditorClass();
//editor.Run(filePath);
using OOPsl.MenuFunctions;
using OOPsl.DocumentFunctions.Managers;
using OOPsl.UserFunctions;

UserManager userManager = new UserManager(); 
DocumentAccessManager accessManager = new DocumentAccessManager(); 
DocumentManager documentManager = new DocumentManager(accessManager);
accessManager.RestoreUserDocuments(userManager, documentManager);
IMenu userMenu = new ConsoleMenu(userManager, documentManager, accessManager);
ApplicationMenu appMenu = new ApplicationMenu(userMenu);
appMenu.Run();