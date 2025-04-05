using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using OOPsl.DocumentFunctions;
using OOPsl.DocumentFunctions.Commands;
using OOPsl.DocumentFunctions.Storage;

namespace OOPsl
{
    public struct Position
    {
        public int Row { get; set; }
        public int Col { get; set; }
        public Position(int row, int col) { Row = row; Col = col; }
    }

    public class TextEditor
    {
        private Document document;
        private string text;
        private string customClipboard = "";
        private int cursorIndex;
        private int? selectionAnchor = null;

        private Stopwatch inputTimer = new Stopwatch();
        private readonly TimeSpan inputDelay = TimeSpan.FromMilliseconds(100);

        public TextEditor(Document document)
        {
            this.document = document;
            text = File.Exists(document.FileName) ? File.ReadAllText(document.FileName) : "";
            cursorIndex = text.Length;
            Console.TreatControlCAsInput = true;
            inputTimer.Start();
        }

        private void UpdateScreen()
        {
            Console.Clear();
            string[] lines = text.Split('\n');
            bool hasSelection = selectionAnchor.HasValue && selectionAnchor.Value != cursorIndex;
            int selStart = 0, selEnd = 0;
            if (hasSelection)
            {
                selStart = Math.Min(selectionAnchor.Value, cursorIndex);
                selEnd = Math.Max(selectionAnchor.Value, cursorIndex);
            }

            int globalIndex = 0;
            for (int i = 0; i < lines.Length; i++)
            {
                for (int j = 0; j < lines[i].Length; j++)
                {
                    if (hasSelection && globalIndex >= selStart && globalIndex < selEnd)
                    {
                        Console.BackgroundColor = ConsoleColor.DarkCyan;
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    else
                    {
                        Console.ResetColor();
                    }
                    Console.Write(lines[i][j]);
                    globalIndex++;
                }
                Console.ResetColor();
                Console.WriteLine();
                globalIndex++;
            }
            Console.ResetColor();
            while (Console.KeyAvailable)
            {
                Console.ReadKey(true);
            }
            var (row, col) = GetCursorCoordinates(cursorIndex);
            Console.SetCursorPosition(col, row);
        }

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

        private int MoveCursorUp(int currentIndex)
        {
            string[] lines = text.Split('\n');
            var (curRow, curCol) = GetCursorCoordinates(currentIndex);
            if (curRow == 0) return currentIndex;
            int newRow = curRow - 1;
            int newCol = Math.Min(curCol, lines[newRow].Length);
            int newIndex = 0;
            for (int i = 0; i < newRow; i++)
                newIndex += lines[i].Length + 1;
            newIndex += newCol;
            return newIndex;
        }

        private int MoveCursorDown(int currentIndex)
        {
            string[] lines = text.Split('\n');
            var (curRow, curCol) = GetCursorCoordinates(currentIndex);
            if (curRow >= lines.Length - 1) return currentIndex;
            int newRow = curRow + 1;
            int newCol = Math.Min(curCol, lines[newRow].Length);
            int newIndex = 0;
            for (int i = 0; i < newRow; i++)
                newIndex += lines[i].Length + 1;
            newIndex += newCol;
            return newIndex;
        }

        private bool HasSelection() => selectionAnchor.HasValue && selectionAnchor.Value != cursorIndex;

        private string CurrentSelection
        {
            get
            {
                if (!selectionAnchor.HasValue || selectionAnchor.Value == cursorIndex)
                    return "";
                int start = Math.Min(selectionAnchor.Value, cursorIndex);
                int end = Math.Max(selectionAnchor.Value, cursorIndex);
                return text.Substring(start, end - start);
            }
        }

        public void EditText()
        {
            UpdateScreen();
            while (true)
            {
                if (inputTimer.Elapsed < inputDelay)
                    continue;
                inputTimer.Restart();

                UpdateScreen();
                ConsoleKeyInfo key = Console.ReadKey(true);

                if (key.Modifiers.HasFlag(ConsoleModifiers.Shift))
                {
                    if (!selectionAnchor.HasValue)
                        selectionAnchor = cursorIndex;
                }
                else if (key.Key == ConsoleKey.LeftArrow || key.Key == ConsoleKey.RightArrow ||
                         key.Key == ConsoleKey.UpArrow || key.Key == ConsoleKey.DownArrow ||
                         key.Key == ConsoleKey.Home || key.Key == ConsoleKey.End)
                {
                    selectionAnchor = null;
                }

                if (key.Modifiers.HasFlag(ConsoleModifiers.Control) && key.Key == ConsoleKey.A)
                {
                    selectionAnchor = 0;
                    cursorIndex = text.Length;
                    continue;
                }

                else if (key.Modifiers.HasFlag(ConsoleModifiers.Control) && key.Key == ConsoleKey.C)
                {
                    string sel = CurrentSelection;
                    customClipboard = string.IsNullOrEmpty(sel) ? text : sel;
                    Console.Beep();
                    continue;
                }
                else if (key.Modifiers.HasFlag(ConsoleModifiers.Control) && key.Key == ConsoleKey.V)
                {
                    if (!string.IsNullOrEmpty(customClipboard))
                    {
                        if (HasSelection())
                        {
                            int start = Math.Min(selectionAnchor.Value, cursorIndex);
                            var delCmd = new DeleteCommand(start, Math.Abs(cursorIndex - selectionAnchor.Value), text);
                            document.CommandManager.ExecuteCommand(delCmd);
                            text = delCmd.UpdatedText;
                            cursorIndex = start;
                            selectionAnchor = null;
                        }
                        var insCmd = new InsertCommand(cursorIndex, customClipboard, text);
                        document.CommandManager.ExecuteCommand(insCmd);
                        text = insCmd.UpdatedText;
                        cursorIndex += customClipboard.Length;
                    }
                    continue;
                }
                else if (key.Key == ConsoleKey.Escape)
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
                    cursorIndex = MoveCursorUp(cursorIndex);
                }
                else if (key.Key == ConsoleKey.DownArrow)
                {
                    cursorIndex = MoveCursorDown(cursorIndex);
                }
                else if (key.Key == ConsoleKey.Home)
                {
                    if (key.Modifiers.HasFlag(ConsoleModifiers.Control) || key.Modifiers.HasFlag(ConsoleModifiers.Shift))
                        cursorIndex = 0;
                    else
                    {
                        string[] arr = text.Split('\n');
                        var (curRow, _) = GetCursorCoordinates(cursorIndex);
                        int lineStart = 0;
                        for (int i = 0; i < curRow; i++)
                            lineStart += arr[i].Length + 1;
                        cursorIndex = lineStart;
                    }
                }
                else if (key.Key == ConsoleKey.End)
                {
                    if (key.Modifiers.HasFlag(ConsoleModifiers.Control) || key.Modifiers.HasFlag(ConsoleModifiers.Shift))
                        cursorIndex = text.Length;
                    else
                    {
                        string[] arr = text.Split('\n');
                        var (curRow, _) = GetCursorCoordinates(cursorIndex);
                        int lineStart = 0;
                        for (int i = 0; i < curRow; i++)
                            lineStart += arr[i].Length + 1;
                        cursorIndex = lineStart + arr[curRow].Length;
                    }
                }
                else if (key.Key == ConsoleKey.Backspace)
                {
                    if (HasSelection())
                    {
                        int start = Math.Min(selectionAnchor.Value, cursorIndex);
                        var delCmd = new DeleteCommand(start, Math.Abs(cursorIndex - selectionAnchor.Value), text);
                        document.CommandManager.ExecuteCommand(delCmd);
                        text = delCmd.UpdatedText;
                        cursorIndex = start;
                        selectionAnchor = null;
                    }
                    else if (cursorIndex > 0)
                    {
                        var delCmd = new DeleteCommand(cursorIndex - 1, 1, text);
                        document.CommandManager.ExecuteCommand(delCmd);
                        text = delCmd.UpdatedText;
                        cursorIndex = Math.Max(cursorIndex - 1, 0);
                    }
                }
                else if (key.Key == ConsoleKey.Enter)
                {
                    var insCmd = new InsertCommand(cursorIndex, "\n", text);
                    document.CommandManager.ExecuteCommand(insCmd);
                    text = insCmd.UpdatedText;
                    cursorIndex++;
                }
                else if (key.KeyChar != '\0')
                {
                    if (HasSelection())
                    {
                        int start = Math.Min(selectionAnchor.Value, cursorIndex);
                        var delCmd = new DeleteCommand(start, Math.Abs(cursorIndex - selectionAnchor.Value), text);
                        document.CommandManager.ExecuteCommand(delCmd);
                        text = delCmd.UpdatedText;
                        cursorIndex = start;
                        selectionAnchor = null;
                    }
                    var insCmd = new InsertCommand(cursorIndex, key.KeyChar.ToString(), text);
                    document.CommandManager.ExecuteCommand(insCmd);
                    text = insCmd.UpdatedText;
                    cursorIndex++;
                }
            }
        }

        public void Run()
        {
            Console.TreatControlCAsInput = true;
            EditText();
            Console.TreatControlCAsInput = false;
            document.Content = text;

            while (Console.KeyAvailable)
                Console.ReadKey(true);

            Console.Clear();
            Console.WriteLine("Сохранить файл:");
            Console.WriteLine("1. Локально");
            Console.WriteLine("2. В облако (Google Drive)");
            Console.Write("Выберите опцию (1 или 2): ");
            var key = Console.ReadKey();
            Console.WriteLine();
            IStorageStrategy storageStrategy = null;
            if (key.KeyChar == '1')
                storageStrategy = new LocalFileStorage();
            else if (key.KeyChar == '2')
                storageStrategy = new CloudStorage();
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