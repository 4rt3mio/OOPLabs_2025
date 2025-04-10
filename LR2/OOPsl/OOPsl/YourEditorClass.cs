using System;
using System.IO;

namespace OOPsl
{
    class YourEditorClass
    {
        private string text;

        // Печатает весь текст (однократно) при запуске редактора.
        private void PrintText()
        {
            Console.Clear();
            Console.Write(text);
        }

        // Определяет координаты курсора (номер строки и столбца) по линейному индексу в тексте.
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

        // Обновляет вывод с указанной строки до конца текста.
        private void UpdateFromRow(int startRow)
        {
            string[] lines = text.Split('\n');
            int currentRow = startRow;
            for (int i = startRow; i < lines.Length; i++)
            {
                Console.SetCursorPosition(0, currentRow);
                string line = lines[i];
                // Если строка короче ширины окна, дописываем пробелами, чтобы стереть остатки предыдущего вывода.
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
            // Если количество строк уменьшилось, очищаем оставшиеся строки на экране.
            for (int r = currentRow; r < Console.WindowHeight; r++)
            {
                Console.SetCursorPosition(0, r);
                Console.Write(new string(' ', Console.WindowWidth));
            }
        }

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
                else if (key.Key == ConsoleKey.UpArrow)
                {
                    var (curRow, curCol) = GetCursorCoordinates(cursorIndex);
                    if (curRow > 0)
                    {
                        string[] lines = text.Split('\n');
                        int targetRow = curRow - 1;
                        int newCol = Math.Min(curCol, lines[targetRow].Length);
                        int newIndex = 0;
                        for (int i = 0; i < targetRow; i++)
                        {
                            newIndex += lines[i].Length + 1;
                        }
                        newIndex += newCol;
                        cursorIndex = newIndex;
                    }
                }
                else if (key.Key == ConsoleKey.DownArrow)
                {
                    var (curRow, curCol) = GetCursorCoordinates(cursorIndex);
                    string[] lines = text.Split('\n');
                    if (curRow < lines.Length - 1)
                    {
                        int targetRow = curRow + 1;
                        int newCol = Math.Min(curCol, lines[targetRow].Length);
                        int newIndex = 0;
                        for (int i = 0; i < targetRow; i++)
                        {
                            newIndex += lines[i].Length + 1;
                        }
                        newIndex += newCol;
                        cursorIndex = newIndex;
                    }
                }
                else if (key.Key == ConsoleKey.Backspace)
                {
                    if (cursorIndex > 0)
                    {
                        // Определяем строку, в которой происходит удаление.
                        var (currentRow, _) = GetCursorCoordinates(cursorIndex);
                        bool isNewLine = text[cursorIndex - 1] == '\n';
                        text = text.Remove(cursorIndex - 1, 1);
                        cursorIndex--;
                        // Если удалён перенос строки, то изменяется разбиение на строки.
                        int updateRow = isNewLine ? Math.Max(currentRow - 1, 0) : currentRow;
                        UpdateFromRow(updateRow);
                    }
                }
                else if (key.Key == ConsoleKey.Delete)
                {
                    if (cursorIndex < text.Length)
                    {
                        var (currentRow, _) = GetCursorCoordinates(cursorIndex);
                        bool isNewLine = text[cursorIndex] == '\n';
                        text = text.Remove(cursorIndex, 1);
                        UpdateFromRow(currentRow);
                    }
                }
                else if (key.Key == ConsoleKey.Enter)
                {
                    var (currentRow, _) = GetCursorCoordinates(cursorIndex);
                    text = text.Insert(cursorIndex, "\n");
                    cursorIndex++;
                    UpdateFromRow(currentRow);
                }
                else
                {
                    if (key.KeyChar != '\0')
                    {
                        var (currentRow, _) = GetCursorCoordinates(cursorIndex);
                        text = text.Insert(cursorIndex, key.KeyChar.ToString());
                        cursorIndex++;
                        UpdateFromRow(currentRow);
                    }
                }
            }
        }

        public void Run(string filePath)
        {
            text = File.Exists(filePath) ? File.ReadAllText(filePath) : "";
            EditText();
            File.WriteAllText(filePath, text);
        }
    }
}