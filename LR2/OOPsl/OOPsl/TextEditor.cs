using OOPsl.DocumentFunctions;
using OOPsl.DocumentFunctions.Storage;

namespace OOPsl
{
    public class TextEditor
    {
        private Document document;
        private string text;

        // Конструктор принимает документ (он содержит имя файла и содержимое)
        public TextEditor(Document document)
        {
            this.document = document;
            // Если файл существует, читаем его, иначе начинаем с пустого текста
            text = File.Exists(document.FileName) ? File.ReadAllText(document.FileName) : "";
        }

        // Выводит весь текст на экран
        private void PrintText()
        {
            Console.Clear();
            Console.Write(text);
        }

        // Определяет координаты курсора (номер строки и столбца) по линейному индексу в тексте
        private (int row, int col) GetCursorCoordinates(int index)
        {
            int row = 0, col = 0;
            for (int i = 0; i < index && i < text.Length; i++)
            {
                if (text[i] == '\n')
                {
                    row++;
                    col = 0;
                }
                else
                {
                    col++;
                }
            }
            return (row, col);
        }

        // Обновляет вывод с указанной строки до конца текста
        private void UpdateFromRow(int startRow)
        {
            string[] lines = text.Split('\n');
            int currentRow = startRow;
            for (int i = startRow; i < lines.Length; i++)
            {
                Console.SetCursorPosition(0, currentRow);
                string line = lines[i];
                if (line.Length < Console.WindowWidth)
                {
                    line = line + new string(' ', Console.WindowWidth - line.Length);
                }
                else if (line.Length > Console.WindowWidth)
                {
                    line = line.Substring(0, Console.WindowWidth);
                }
                Console.Write(line);
                currentRow++;
            }
            // Очищаем оставшиеся строки
            for (int r = currentRow; r < Console.WindowHeight; r++)
            {
                Console.SetCursorPosition(0, r);
                Console.Write(new string(' ', Console.WindowWidth));
            }
        }

        // Основной метод редактирования текста
        public void EditText()
        {
            PrintText();
            int cursorIndex = text.Length;

            while (true)
            {
                var (row, col) = GetCursorCoordinates(cursorIndex);
                Console.SetCursorPosition(col, row);
                ConsoleKeyInfo key = Console.ReadKey(true);

                if (key.Key == ConsoleKey.Escape)
                {
                    break;
                }
                else if (key.Key == ConsoleKey.LeftArrow)
                {
                    if (cursorIndex > 0)
                        cursorIndex--;
                }
                else if (key.Key == ConsoleKey.RightArrow)
                {
                    if (cursorIndex < text.Length)
                        cursorIndex++;
                }
                else if (key.Key == ConsoleKey.Backspace)
                {
                    if (cursorIndex > 0)
                    {
                        var (currentRow, _) = GetCursorCoordinates(cursorIndex);
                        bool isNewLine = text[cursorIndex - 1] == '\n';
                        text = text.Remove(cursorIndex - 1, 1);
                        cursorIndex--;
                        int updateRow = isNewLine ? Math.Max(currentRow - 1, 0) : currentRow;
                        UpdateFromRow(updateRow);
                    }
                }
                else if (key.Key == ConsoleKey.Enter)
                {
                    var (currentRow, _) = GetCursorCoordinates(cursorIndex);
                    text = text.Insert(cursorIndex, "\n");
                    cursorIndex++;
                    UpdateFromRow(currentRow);
                }
                else if (key.KeyChar != '\0')
                {
                    var (currentRow, _) = GetCursorCoordinates(cursorIndex);
                    text = text.Insert(cursorIndex, key.KeyChar.ToString());
                    cursorIndex++;
                    UpdateFromRow(currentRow);
                }
            }
        }

        // Запускает редактор, затем спрашивает, куда сохранить файл, и вызывает соответствующую стратегию
        public void Run()
        {
            EditText();
            // Обновляем содержимое документа
            document.Content = text;

            Console.Clear();
            Console.WriteLine("Сохранить файл:");
            Console.WriteLine("1. Локально");
            Console.WriteLine("2. В облако (Google Drive)");
            Console.Write("Выберите опцию: ");
            string choice = Console.ReadLine();

            IStorageStrategy storageStrategy = null;
            if (choice == "1")
            {
                storageStrategy = new LocalFileStorage();
            }
            else if (choice == "2")
            {
                storageStrategy = new CloudStorage();
            }
            else
            {
                Console.WriteLine("Неверный выбор. Сохранение отменено.");
                Console.WriteLine("Нажмите любую клавишу для возврата...");
                Console.ReadKey();
                return;
            }

            storageStrategy.Save(document);
            Console.WriteLine("Файл сохранён. Нажмите любую клавишу для возврата...");
            Console.ReadKey();
        }
    }
}
