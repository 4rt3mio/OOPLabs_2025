using OOPfl;

namespace Tester
{
    public class InputManagerTests
    {
        [Fact]
        public void AddShape_WhenShapeDoesNotFitInCanvas_ShouldDisplayErrorMessage()
        {
            var canvas = new Canvas(10, 10, '.');
            var commandManager = new CommandManager();
            var fileManager = new FileManager();
            var inputManager = new InputManager(fileManager, commandManager, canvas);

            var stringWriter = new StringWriter();
            Console.SetOut(stringWriter);

            inputManager.ProcessInput("/add rectangle 0 0 # 15 10");

            var output = stringWriter.ToString();
            Assert.Contains("Ошибка: Фигура не помещается в канву.", output);
        }

        [Fact]
        public void AddCircle_WhenCircleDoesNotFitOnCanvas_ShouldDisplayErrorMessage()
        {
            var canvas = new Canvas(100, 40, '.');
            var commandManager = new CommandManager();
            var fileManager = new FileManager();
            var inputManager = new InputManager(fileManager, commandManager, canvas);

            var stringWriter = new StringWriter();
            Console.SetOut(stringWriter);

            inputManager.ProcessInput("/add circle 3 3 @ 6");

            var output = stringWriter.ToString();
            Assert.Contains("Ошибка: Окружность не помещается в канву.", output);
        }

        [Fact]
        public void AddShape_WhenInsufficientArguments_ShouldDisplayErrorMessage()
        {
            var canvas = new Canvas(10, 10, '.');
            var commandManager = new CommandManager();
            var fileManager = new FileManager();
            var inputManager = new InputManager(fileManager, commandManager, canvas);

            var stringWriter = new StringWriter();
            Console.SetOut(stringWriter);

            inputManager.ProcessInput("/add rectangle 0 0 # 10");

            var output = stringWriter.ToString();
            Assert.Contains("Ошибка: Некорректное количество аргументов для фигуры.", output);
        }

        [Fact]
        public void AddShape_WhenInvalidCoordinates_ShouldDisplayErrorMessage()
        {
            var canvas = new Canvas(10, 10, '.');
            var commandManager = new CommandManager();
            var fileManager = new FileManager();
            var inputManager = new InputManager(fileManager, commandManager, canvas);

            var stringWriter = new StringWriter();
            Console.SetOut(stringWriter);

            inputManager.ProcessInput("/add rectangle -1 -1 # 10 10");

            var output = stringWriter.ToString();
            Assert.Contains("Ошибка: Некорректные координаты (они должны быть неотрицательными).", output);
        }

        [Fact]
        public void AddShape_WhenShapeOutOfBounds_ShouldDisplayErrorMessage()
        {
            var canvas = new Canvas(10, 10, '.');
            var commandManager = new CommandManager();
            var fileManager = new FileManager();
            var inputManager = new InputManager(fileManager, commandManager, canvas);

            var stringWriter = new StringWriter();
            Console.SetOut(stringWriter);

            inputManager.ProcessInput("/add rectangle 5 5 # 10 10");

            var output = stringWriter.ToString();
            Assert.Contains("Ошибка: Фигура не помещается в канву.", output);
        }

        [Fact]
        public void MoveShape_WhenShapeMovesOutOfBounds_ShouldDisplayErrorMessage()
        {
            var canvas = new Canvas(10, 10, '.');
            var commandManager = new CommandManager();
            var fileManager = new FileManager();
            var inputManager = new InputManager(fileManager, commandManager, canvas);

            var stringWriter = new StringWriter();
            Console.SetOut(stringWriter);

            inputManager.ProcessInput("/add rectangle 0 0 # 5 5");
            inputManager.ProcessInput("/move 0 10 10");

            var output = stringWriter.ToString();
            Assert.Contains("Ошибка: Прямоугольник выходит за пределы канвы.", output);
        }

        [Fact]
        public void MoveShape_WhenInvalidCoordinates_ShouldDisplayErrorMessage()
        {
            var canvas = new Canvas(10, 10, '.');
            var commandManager = new CommandManager();
            var fileManager = new FileManager();
            var inputManager = new InputManager(fileManager, commandManager, canvas);

            var stringWriter = new StringWriter();
            Console.SetOut(stringWriter);

            inputManager.ProcessInput("/add rectangle 0 0 # 5 5");
            inputManager.ProcessInput("/move 0 -1 -1");

            var output = stringWriter.ToString();
            Assert.Contains("Ошибка: Прямоугольник выходит за пределы канвы.", output);
        }

        [Fact]
        public void MoveShape_WhenShapeDoesNotExist_ShouldDisplayErrorMessage()
        {
            var canvas = new Canvas(10, 10, '.');
            var commandManager = new CommandManager();
            var fileManager = new FileManager();
            var inputManager = new InputManager(fileManager, commandManager, canvas);

            var stringWriter = new StringWriter();
            Console.SetOut(stringWriter);

            inputManager.ProcessInput("/move 999 10 10");

            var output = stringWriter.ToString();
            Assert.Contains("Ошибка: Фигура с таким ID не найдена.", output);
        }

        [Fact]
        public void SaveCanvas_WhenNoShapes_ShouldDisplayErrorMessage()
        {
            var canvas = new Canvas(10, 10, '.');
            var commandManager = new CommandManager();
            var fileManager = new FileManager();
            var inputManager = new InputManager(fileManager, commandManager, canvas);

            var stringWriter = new StringWriter();
            Console.SetOut(stringWriter);

            inputManager.ProcessInput("/save canvas.txt");

            var output = stringWriter.ToString();
            Assert.Contains("Ошибка: Нет фигур на канве для сохранения.", output);
        }

        [Fact]
        public void LoadCanvas_WhenFileDoesNotExist_ShouldDisplayErrorMessage()
        {
            var canvas = new Canvas(10, 10, '.');
            var commandManager = new CommandManager();
            var fileManager = new FileManager();
            var inputManager = new InputManager(fileManager, commandManager, canvas);

            var stringWriter = new StringWriter();
            Console.SetOut(stringWriter);

            inputManager.ProcessInput("/load non_existing_file.txt");

            var output = stringWriter.ToString();
            Assert.Contains("Ошибка: Файл не существует.", output);
        }

        [Fact]
        public void RemoveShape_ShouldSetIsVisibleToFalse()
        {
            var canvas = new Canvas(20, 20, '.');
            var commandManager = new CommandManager();
            var shape = new Rectangle(1, 5, 5, '#', new Coordinates(2, 2));
            canvas.AddShape(shape);

            var removeCommand = new RemoveShapeCommand(canvas, shape.Id);
            commandManager.ExecuteCommand(removeCommand);

            var removedShape = canvas.GetShapeById(1);
            Assert.False(removedShape.IsVisible);
        }

        [Fact]
        public void UndoRemoveShape_ShouldRestoreIsVisibleToTrue()
        {
            var canvas = new Canvas(20, 20, '.');
            var commandManager = new CommandManager();
            var shape = new Rectangle(1, 5, 5, '#', new Coordinates(2, 2));
            canvas.AddShape(shape);

            var removeCommand = new RemoveShapeCommand(canvas, shape.Id);
            commandManager.ExecuteCommand(removeCommand);

            commandManager.Undo();

            var restoredShape = canvas.GetShapeById(1);
            Assert.True(restoredShape.IsVisible);
        }

        [Fact]
        public void RedoRemoveShape_ShouldRemoveShapeAgain()
        {
            var canvas = new Canvas(20, 20, '.');
            var commandManager = new CommandManager();
            var shape = new Rectangle(1, 5, 5, '#', new Coordinates(2, 2));
            canvas.AddShape(shape);

            var removeCommand = new RemoveShapeCommand(canvas, shape.Id);
            commandManager.ExecuteCommand(removeCommand);

            commandManager.Undo();

            commandManager.Redo();

            var removedShape = canvas.GetShapeById(1);
            Assert.False(removedShape.IsVisible);
        }
    }
}