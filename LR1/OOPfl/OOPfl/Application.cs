﻿namespace OOPfl
{
    public class Application
    {
        private Canvas canvas;
        private InputManager inputManager;
        private Drawer drawer;

        public Application()
        {
            Console.SetWindowSize(Console.LargestWindowWidth, Console.LargestWindowHeight);
            InitializeCanvas();
            FileManager fileManager = new FileManager();
            CommandManager commandManager = new CommandManager();
            inputManager = new InputManager(fileManager, commandManager, canvas);
            drawer = new Drawer(canvas);
        }

        private void InitializeCanvas()
        {
            int maxWidth = Console.WindowWidth/2 - 6;
            int maxHeight = Console.WindowHeight - 3;

            Console.WriteLine($"Максимально доступный размер канвы: {maxWidth}x{maxHeight}");
            Console.WriteLine("Введите размеры канвы:");

            int width = ConsoleInputHelper.GetValidatedIntInput($"Ширина (5-{maxWidth}): ", 5, maxWidth);
            int height = ConsoleInputHelper.GetValidatedIntInput($"Высота (5-{maxHeight}): ", 5, maxHeight);
            char backgroundChar = ConsoleInputHelper.GetValidCharInput("Введите символ фона: ");

            canvas = new Canvas(width, height, backgroundChar);
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
                    drawer.Draw();
                }
            }
        }
    }
}