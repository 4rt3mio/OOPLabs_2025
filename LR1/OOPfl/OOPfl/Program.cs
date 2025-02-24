using System;

class Program
{
    static char[,] canvas;
    static int width = Console.WindowWidth; // Ширина терминала
    static int height = Console.WindowHeight - 3; // Высота холста

    static void InitCanvas()
    {
        canvas = new char[height, width];
        for (int i = 0; i < height; i++)
            for (int j = 0; j < width; j++)
                canvas[i, j] = '.'; // Фон
    }

    static void DrawCanvas()
    {
        Console.Clear();
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
                Console.Write(canvas[i, j]);
            Console.WriteLine();
        }
    }

    static void DrawRectangle(int x, int y, int w, int h, char fillChar)
    {
        for (int i = 0; i < h; i++)
        {
            for (int j = 0; j < w; j++)
            {
                int drawX = x + j;
                int drawY = y + i;
                if (drawX < width && drawY < height)
                    canvas[drawY, drawX] = fillChar;
            }
        }
    }

    static void MoveRectangle(int oldX, int oldY, int newX, int newY, int w, int h, char fillChar)
    {
        DrawRectangle(oldX, oldY, w, h, '.'); // Очищаем старое место
        DrawRectangle(newX, newY, w, h, fillChar); // Рисуем в новом месте
        DrawCanvas();
    }

    static void Main()
    {
        Console.CursorVisible = false;
        InitCanvas();

        int x = 5, y = 3, w = 8, h = 4; // Начальные координаты
        DrawRectangle(x, y, w, h, '#');
        DrawCanvas();

        Console.WriteLine("\nНажмите ПРОБЕЛ для перемещения фигуры...");

        while (true)
        {
            var key = Console.ReadKey(true).Key;
            if (key == ConsoleKey.Spacebar)
            {
                int newX = 15, newY = 5; // Новые координаты
                MoveRectangle(x, y, newX, newY, w, h, '#');
                x = newX;
                y = newY;
            }
            else if  (key == ConsoleKey.B)
            {
                int newX = 0, newY = 0; 
                MoveRectangle(x, y, newX, newY, w, h, 'Z');
                x = newX;
                y = newY;
            }
            else if (key == ConsoleKey.Escape)
            {
                break;
            }
        }
    }
}