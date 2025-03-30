namespace OOPsl
{
    public sealed class EditorSettings
    {
        private static readonly EditorSettings instance = new EditorSettings();

        public string Theme { get; set; }
        public int FontSize { get; set; }

        private EditorSettings() { }

        public static EditorSettings Instance
        {
            get { return instance; }
        }
    }
}
