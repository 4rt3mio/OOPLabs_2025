namespace OOPsl.MenuFunctions
{
    public class ApplicationMenu
    {
        private IMenu menu;

        public ApplicationMenu(IMenu menu)
        {
            this.menu = menu;
        }

        public void Run()
        {
            while (true)
            {
                menu.Display();
            }
        }
    }
}
