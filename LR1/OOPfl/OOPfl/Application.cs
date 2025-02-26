namespace OOPfl
{
    public class Application
    {
        private Canvas canvas { set; get; }
        private InputManager inputManager;

        public Application()
        {
            Console.SetWindowSize(Console.LargestWindowWidth, Console.LargestWindowHeight);
            InitializeCanvas();
            FileManager fileManager = new FileManager(canvas);
            CommandManager commandManager = new CommandManager();
            inputManager = new InputManager(fileManager, commandManager, canvas);
        }

        private void InitializeCanvas()
        {
            int maxWidth = Console.WindowWidth - 6;
            int maxHeight = Console.WindowHeight - 3;

            Console.WriteLine($"Максимально доступный размер канвы: {maxWidth}x{maxHeight}");
            Console.WriteLine("Введите размеры канвы:");

            int width = GetValidatedIntInput($"Ширина (5-{maxWidth}): ", 5, maxWidth);
            int height = GetValidatedIntInput($"Высота (5-{maxHeight}): ", 5, maxHeight);
            char backgroundChar = GetValidCharInput("Введите символ фона: ");

            canvas = new Canvas(width, height, backgroundChar);
        }

        private int GetValidatedIntInput(string message, int min, int max)
        {
            int value;
            do
            {
                Console.Write(message);
                string input = Console.ReadLine();

                if (!int.TryParse(input, out value) || value < min || value > max)
                {
                    Console.WriteLine($"Ошибка: Введите число от {min} до {max}.");
                    continue;
                }

                break;
            } while (true);

            return value;
        }

        private char GetValidCharInput(string message)
        {
            Console.Write(message);
            char input = Console.ReadKey().KeyChar;
            Console.WriteLine();
            return input;
        }

        public void Start()
        {
            Console.Clear();
            canvas.Redraw();

            while (true)
            {
                Console.Write("Введите команду: ");
                string input = Console.ReadLine()?.Trim().ToLower();

                if (string.IsNullOrEmpty(input))
                {
                    Console.WriteLine("Ошибка: Команда не может быть пустой!");
                    continue; 
                }

                if (input == "/q")
                {
                    Console.WriteLine("Выход из программы...");
                    break;
                }

                bool success = inputManager.ProcessInput(input);

                if (success)
                {
                    canvas.Redraw();
                }
            }
        }
    }
}