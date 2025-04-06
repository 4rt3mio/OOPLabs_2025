using System.Runtime.InteropServices;

namespace OOPsl
{
    public sealed class EditorSettings
    {
        private static readonly EditorSettings instance = new EditorSettings();

        // Например, "Dark" или "Light"
        public string Theme { get; set; } = "Dark";

        // Размер шрифта в пунктах (будет использован как высота шрифта в пикселях)
        public int FontSize { get; set; } = 16;

        private EditorSettings() { }

        public static EditorSettings Instance => instance;

        /// <summary>
        /// Применяет настройки шрифта в консоли через WinAPI.
        /// </summary>
        public void ApplyFontSettings()
        {
            IntPtr hConsoleOutput = GetStdHandle(STD_OUTPUT_HANDLE);
            if (hConsoleOutput == IntPtr.Zero)
            {
                Console.WriteLine("Не удалось получить дескриптор консоли.");
                return;
            }

            CONSOLE_FONT_INFO_EX fontInfo = new CONSOLE_FONT_INFO_EX();
            fontInfo.cbSize = (uint)Marshal.SizeOf(fontInfo);
            // Устанавливаем нужный размер шрифта.
            fontInfo.dwFontSize.X = 0; // Ширина можно оставить авто
            fontInfo.dwFontSize.Y = (short)FontSize;
            // Указываем семейство шрифта: 
            // FF_DONTCARE (0) | TMPF_TRUETYPE (0x04) часто работают с Consolas
            fontInfo.FontFamily = 0x30; // Пример: Consolas
            fontInfo.FontWeight = 400;  // Нормальный вес
            fontInfo.FaceName = "Consolas";

            if (!SetCurrentConsoleFontEx(hConsoleOutput, false, ref fontInfo))
            {
                Console.WriteLine("Ошибка при установке шрифта консоли.");
            }
        }

        private const int STD_OUTPUT_HANDLE = -11;

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetCurrentConsoleFontEx(IntPtr consoleOutput, bool maximumWindow, ref CONSOLE_FONT_INFO_EX consoleCurrentFontEx);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetStdHandle(int nStdHandle);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct CONSOLE_FONT_INFO_EX
        {
            public uint cbSize;
            public uint nFont;
            public COORD dwFontSize;
            public int FontFamily;
            public int FontWeight;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string FaceName;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct COORD
        {
            public short X;
            public short Y;
        }
    }
}
