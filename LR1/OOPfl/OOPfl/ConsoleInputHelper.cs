namespace OOPfl
{
    public static class ConsoleInputHelper
    {
        public static int GetValidatedIntInput(string message, int min, int max)
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

        public static char GetValidCharInput(string message)
        {
            Console.Write(message);
            char input = Console.ReadKey().KeyChar;
            Console.WriteLine();
            return input;
        }
    }
}
