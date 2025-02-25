namespace OOPfl
{
    public class Application
    {
        private Canvas canvas { set; get; }
        private InputManager inputManager;

        public Application()
        {
            InitializeCanvas();
            FileManager fileManager = new FileManager(canvas);
            CommandManager commandManager = new CommandManager();
            inputManager = new InputManager(fileManager, commandManager, canvas);
        }

        private void InitializeCanvas()
        {
            int width = GetValidatedIntInput("Введите ширину канвы: ", min: 5, max: Console.WindowWidth - 2);
            int height = GetValidatedIntInput("Введите высоту канвы: ", min: 5, max: Console.WindowHeight - 3);
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
                Console.Write("\nВведите команду: ");
                string input = Console.ReadLine()?.Trim();

                if (string.IsNullOrEmpty(input))
                {
                    Console.WriteLine("Ошибка: Команда не может быть пустой!");
                    continue;
                }

                if (input.ToLower() == "/q")
                {
                    Console.WriteLine("Выход из приложения...");
                    break;
                }

                inputManager.ProcessInput(input);

                Console.Clear();
                canvas.Redraw();
            }
        }
    }
}