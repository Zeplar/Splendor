namespace Splendor
{
    public static class CONSOLE
    {
        public static void WriteLine(string s)
        {
            System.Console.WriteLine(s);
        }
        public static void Write(string s)
        {
            System.Console.Write(s);
        }
        public static void Overwrite(string s)
        {
            System.Console.CursorLeft = 0;
            System.Console.Write("                                 ");
            System.Console.CursorLeft = 0;
            System.Console.Write(s);
        }
        public static void Overwrite(int lineNumber, string s)
        {
            int x = System.Console.CursorTop;
            int y = System.Console.CursorLeft;
            System.Console.CursorTop = lineNumber;
            Overwrite(s);
            System.Console.SetCursorPosition(y, x);
        }

    }
}
