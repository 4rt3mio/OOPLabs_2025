using OOPtl.Application.Services;
using OOPtl.DataAccess.Api;
using OOPtl.Presentation.Commands;
using OOPtl.Presentation;
using OOPtl.DataAccess.Repositories;

var testAdapter = new QuoteApiAdapter();
var quote = await testAdapter.GetRandomQuoteAsync();
//Console.WriteLine($"API Test Result: {quote.Content} - {quote.Author}\n");

var repository = new StudentRepository("students.json");
var service = new StudentService(repository, testAdapter);

var addCommand = new AddStudentCommand(service);
var editCommand = new EditStudentCommand(service);
var viewCommand = new ViewStudentsCommand(service);

var ui = new ConsoleUI(addCommand, editCommand, viewCommand);
await ui.RunAsync();