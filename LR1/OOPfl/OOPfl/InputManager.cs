using OOPfl;

public class InputManager
{
    private FileManager fileManager;
    private CommandManager commandManager;
    private Canvas canvas;

    public InputManager(FileManager fileManager, CommandManager commandManager, Canvas canvas)
    {
        this.fileManager = fileManager;
        this.commandManager = commandManager;
        this.canvas = canvas;
    }

    public bool ProcessInput(string input)
    {
        if (input.StartsWith("/help"))
        {
            string[] parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            DisplayHelp(parts.Length > 1 ? parts[1] : null);
            return false; 
        }
        else if (input.StartsWith("/ps"))
        {
            bool showHidden = input.Contains("-a");
            canvas.PrintShapes(showHidden);
            return false; 
        }
        else if (input.StartsWith("/add"))
        {
            return AddShape(input);
        }
        else if (input.StartsWith("/remove"))
        {
            return RemoveShape(input);
        }
        else if (input.StartsWith("/move"))
        {
            return MoveShape(input);
        }
        else if (input.StartsWith("/fill"))
        {
            return FillShape(input);
        }
        else if (input.StartsWith("/undo"))
        {
            return Undo(input);
        }
        else if (input.StartsWith("/redo"))
        {
            return Redo(input);
        }
        else if (input.StartsWith("/save"))
        {
            return SaveCanvas(input);
        }
        else if (input.StartsWith("/load"))
        {
            return LoadCanvas(input);
        }
        else
        {
            Console.WriteLine("Ошибка: Неизвестная команда.");
            return false; 
        }
    }

    private void DisplayHelp(string command = null)
    {
        if (string.IsNullOrEmpty(command))
        {
            Console.WriteLine("\nСписок доступных команд:");
            Console.WriteLine("/q - Выйти из программы");
            Console.WriteLine("/help - Показать список команд");
            Console.WriteLine("/help [команда] - Показать справку по конкретной команде");
            Console.WriteLine("/ps - Вывести все данные о фигурах");
            Console.WriteLine("/add [фигура] [x] [y] [символ] [параметры фигуры] - Добавить фигуру");
            Console.WriteLine("/remove [id фигуры] - Удалить фигуру");
            Console.WriteLine("/move [id фигуры] [x] [y] - Переместить фигуру");
            Console.WriteLine("/fill [id фигуры] [символ] - Изменить заливку фигуры");
            Console.WriteLine("/undo - Отменить последнюю команду");
            Console.WriteLine("/redo - Повторить последнюю отмененную команду");
            Console.WriteLine("/save [имя файла] - Сохранить холст в файл");
            Console.WriteLine("/load [имя файла] - Загрузить холст из файла");
            Console.WriteLine("\nВведите '/help [команда]', чтобы узнать больше.");
        }
        else
        {
            switch (command.ToLower())
            {
                case "add":
                    Console.WriteLine("\nИспользование: /add [фигура] [x] [y] [символ] [параметры]");
                    Console.WriteLine("Примеры:");
                    Console.WriteLine("  /add rectangle 5 5 # 10 6  - Прямоугольник 10x6 на (5,5)");
                    Console.WriteLine("  /add triangle 3 3 * 7 4    - Треугольник шириной 7 и высотой 4 на (3,3)");
                    Console.WriteLine("  /add circle 10 10 @ 5      - Круг радиусом 5 на (10,10)");
                    break;
                case "remove":
                    Console.WriteLine("\nИспользование: /remove [id фигуры]");
                    Console.WriteLine("Удаляет фигуру с указанным ID.");
                    break;
                case "move":
                    Console.WriteLine("\nИспользование: /move [id фигуры] [x] [y]");
                    Console.WriteLine("Перемещает фигуру с указанным ID в новые координаты.");
                    break;
                case "fill":
                    Console.WriteLine("\nИспользование: /fill [id фигуры] [символ]");
                    Console.WriteLine("Изменяет заливку фигуры с указанным ID.");
                    break;
                case "undo":
                    Console.WriteLine("\nИспользование: /undo");
                    Console.WriteLine("Отменяет последнюю выполненную команду.");
                    break;
                case "redo":
                    Console.WriteLine("\nИспользование: /redo");
                    Console.WriteLine("Повторяет последнюю отмененную команду.");
                    break;
                case "save":
                    Console.WriteLine("\nИспользование: /save [имя файла]");
                    Console.WriteLine("Сохраняет текущее состояние холста в файл.");
                    break;
                case "load":
                    Console.WriteLine("\nИспользование: /load [имя файла]");
                    Console.WriteLine("Загружает сохраненный холст из файла.");
                    break;
                case "ps":
                    Console.WriteLine("\nИспользование: /ps [-a]");
                    Console.WriteLine("Выводит список всех фигур на канве.");
                    Console.WriteLine("  /ps    - Показывает только видимые фигуры");
                    Console.WriteLine("  /ps -a - Показывает все, включая скрытые");
                    break;
                default:
                    Console.WriteLine($"Команда '/help {command}' не найдена.");
                    break;
            }
        }
        Console.WriteLine();
    }

    private bool AddShape(string input)
    {
        string[] parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        int id = 0;

        if (parts.Length < 6)
        {
            Console.WriteLine("Ошибка: Недостаточно аргументов для добавления фигуры.");
            return false;
        }

        string shapeType = parts[1].ToLower();
        int x, y, width, height, radius;
        char symbol;

        if (!char.TryParse(parts[4], out symbol))
        {
            Console.WriteLine("Ошибка: Некорректный символ для фигуры.");
            return false;
        }

        if (shapeType == "circle")
        {
            if (parts.Length != 6) 
            {
                Console.WriteLine("Ошибка: Некорректное количество аргументов для окружности.");
                return false;
            }

            if (!int.TryParse(parts[5], out radius) || radius <= 0)
            {
                Console.WriteLine("Ошибка: Радиус окружности должен быть положительным числом.");
                return false;
            }

            if (!int.TryParse(parts[2], out x) || x < 0 || !int.TryParse(parts[3], out y) || y < 0)
            {
                Console.WriteLine("Ошибка: Некорректные координаты (они должны быть неотрицательными).");
                return false;
            }

            if (x - radius < 0 || x + radius >= canvas.Width || y - radius < 0 || y + radius >= canvas.Height)
            {
                Console.WriteLine("Ошибка: Окружность не помещается в канву.");
                return false;
            }
            id = canvas.GetShapes().Count;
            Coordinates crd = new Coordinates(x, y);
            Shape shape = new Circle(id, radius, symbol, crd);
            canvas.AddShape(shape);
        }
        else if (shapeType == "rectangle" || shapeType == "triangle")
        {
            if (parts.Length != 7)
            {
                Console.WriteLine("Ошибка: Некорректное количество аргументов для фигуры.");
                return false;
            }

            if (!int.TryParse(parts[5], out width) || width <= 0 || !int.TryParse(parts[6], out height) || height <= 0)
            {
                Console.WriteLine("Ошибка: Ширина и высота фигуры должны быть положительными числами.");
                return false;
            }

            if (!int.TryParse(parts[2], out x) || x < 0 || !int.TryParse(parts[3], out y) || y < 0)
            {
                Console.WriteLine("Ошибка: Некорректные координаты (они должны быть неотрицательными).");
                return false;
            }

            if (x + width >= canvas.Width || y + height >= canvas.Height)
            {
                Console.WriteLine("Ошибка: Фигура не помещается в канву.");
                return false;
            }
            if (shapeType == "rectangle")
            {
                id = canvas.GetShapes().Count;
                Coordinates crd = new Coordinates(x, y);
                Shape shape = new Rectangle(id, width, height, symbol, crd);
                canvas.AddShape(shape);
            }
            else
            {
                id = canvas.GetShapes().Count;
                Coordinates crd = new Coordinates(x, y);
                Shape shape = new Triangle(id, width, height, symbol, crd);
                canvas.AddShape(shape);
            }
        }
        else
        {
            Console.WriteLine("Ошибка: Неизвестный тип фигуры.");
            return false; 
        }

        ShapeCommand sC = new AddShapeCommand(canvas, id);
        commandManager.ExecuteCommand(sC);
        return true;
    }

    private bool RemoveShape(string input)
    {
        string[] parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length != 2)
        {
            Console.WriteLine("Ошибка: Некорректное количество аргументов для удаления фигуры.");
            return false;
        }

        int id;
        if (!int.TryParse(parts[1], out id) || id < 0)
        {
            Console.WriteLine("Ошибка: Некорректный ID фигуры.");
            return false;
        }

        var shape = canvas.GetShapeById(id);
        if (shape == null)
        {
            Console.WriteLine("Ошибка: Фигура с таким ID не найдена.");
            return false;
        }

        if (!shape.IsVisible)
        {
            Console.WriteLine("Ошибка: Фигура уже была удалена или скрыта.");
            return false;
        }

        ShapeCommand rC = new RemoveShapeCommand(canvas, id);
        commandManager.ExecuteCommand(rC);
        return true;
    }

    private bool MoveShape(string input)
    {
        string[] parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length != 4)
        {
            Console.WriteLine("Ошибка: Некорректное количество аргументов для перемещения фигуры.");
            return false;
        }

        int id;
        if (!int.TryParse(parts[1], out id) || id < 0)
        {
            Console.WriteLine("Ошибка: Некорректный ID фигуры.");
            return false;
        }

        var shape = canvas.GetShapeById(id);
        if (shape == null)
        {
            Console.WriteLine("Ошибка: Фигура с таким ID не найдена.");
            return false;
        }

        if (!shape.IsVisible)
        {
            Console.WriteLine("Ошибка: Фигура уже была удалена или скрыта.");
            return false;
        }

        int deltaX, deltaY;
        if (!int.TryParse(parts[2], out deltaX))
        {
            Console.WriteLine("Ошибка: Некорректное значение для delta_x.");
            return false;
        }

        if (!int.TryParse(parts[3], out deltaY))
        {
            Console.WriteLine("Ошибка: Некорректное значение для delta_y.");
            return false;
        }

        int newX = shape.StartPosition.X + deltaX;
        int newY = shape.StartPosition.Y + deltaY;

        if (shape is Circle circle)
        {
            int radius = circle.Radius;

            if (newX - radius < 0 || newX + radius >= canvas.Width || newY - radius < 0 || newY + radius >= canvas.Height)
            {
                Console.WriteLine("Ошибка: Окружность выходит за пределы канвы.");
                return false;
            }
        }
        else
        {
            if (newX < 0 || newX >= canvas.Width || newY < 0 || newY >= canvas.Height)
            {
                Console.WriteLine("Ошибка: Фигура выходит за пределы канвы.");
                return false;
            }
        }
        Coordinates crd = new Coordinates(deltaX, deltaY);
        ShapeCommand mC = new MoveShapeCommand(canvas, id, crd);
        commandManager.ExecuteCommand(mC);
        return true;
    }

    private bool FillShape(string input)
    {
        string[] parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length != 3)
        {
            Console.WriteLine("Ошибка: Некорректное количество аргументов для заливки фигуры.");
            return false;
        }

        int id;
        if (!int.TryParse(parts[1], out id) || id < 0)
        {
            Console.WriteLine("Ошибка: Некорректный ID фигуры.");
            return false;
        }

        var shape = canvas.GetShapeById(id);
        if (shape == null)
        {
            Console.WriteLine("Ошибка: Фигура с таким ID не найдена.");
            return false;
        }

        if (!shape.IsVisible)
        {
            Console.WriteLine("Ошибка: Фигура скрыта или удалена.");
            return false;
        }

        char fillSymbol;
        if (parts[2].Length != 1 || !char.TryParse(parts[2], out fillSymbol))
        {
            Console.WriteLine("Ошибка: Некорректный символ для заливки.");
            return false;
        }

        ShapeCommand fC = new FillBackgroundCommand(canvas, id, fillSymbol, canvas.GetShapeById(id).FillChar);
        commandManager.ExecuteCommand(fC);
        return true;
    }

    private bool Undo(string input)
    {
        string[] parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length != 1)
        {
            Console.WriteLine("Ошибка: Команда '/undo' не требует дополнительных аргументов.");
            return false;
        }

        commandManager.Undo();
        return true;
    }

    private bool Redo(string input)
    {
        string[] parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length != 1)
        {
            Console.WriteLine("Ошибка: Команда '/redo' не требует дополнительных аргументов.");
            return false;
        }

        commandManager.Redo();
        return true;
    }

    private bool SaveCanvas(string input)
    {
        string[] parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length != 2)
        {
            Console.WriteLine("Ошибка: Неверное количество аргументов для команды '/save'. Ожидается путь к файлу.");
            return false;
        }

        string filePath = parts[1];

        if (canvas.GetShapes().Count == 0)
        {
            Console.WriteLine("Ошибка: Нет фигур на канве для сохранения.");
            return false;
        }

        if (string.IsNullOrWhiteSpace(filePath))
        {
            Console.WriteLine("Ошибка: Путь к файлу не может быть пустым.");
            return false;
        }

        fileManager.Save(filePath, canvas, commandManager);
        return true;
    }

    private bool LoadCanvas(string input)
    {
        string[] parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length != 2)
        {
            Console.WriteLine("Ошибка: Неверное количество аргументов для команды '/load'. Ожидается путь к файлу.");
            return false;
        }

        string filePath = parts[1];

        if (string.IsNullOrWhiteSpace(filePath))
        {
            Console.WriteLine("Ошибка: Путь к файлу не может быть пустым.");
            return false;
        }

        if (!File.Exists(filePath))
        {
            Console.WriteLine("Ошибка: Файл не существует.");
            return false;
        }

        fileManager.Load(filePath, canvas, commandManager);
        return true;
    }
}