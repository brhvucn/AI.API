namespace AI.ChatGPTIntegration.Extensions
{
    public static class ColoredConsole
    {
        /// <summary>
        /// Skriver tekst til konsollen med specificeret forgrundfarve og sætter farven tilbage bagefter
        /// </summary>
        /// <param name="text">Teksten der skal skrives</param>
        /// <param name="color">Farven teksten skal skrives med</param>
        public static void WriteLine(string text, ConsoleColor color)
        {
            ConsoleColor originalColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ForegroundColor = originalColor;
        }

        /// <summary>
        /// Skriver tekst til konsollen med specificeret forgrundfarve og sætter farven tilbage bagefter
        /// </summary>
        /// <param name="text">Teksten der skal skrives</param>
        /// <param name="color">Farven teksten skal skrives med</param>
        public static void Write(string text, ConsoleColor color)
        {
            ConsoleColor originalColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.Write(text);
            Console.ForegroundColor = originalColor;
        }

        /// <summary>
        /// Skriver formateret tekst til konsollen med specificeret forgrundfarve
        /// </summary>
        /// <param name="format">Format string</param>
        /// <param name="color">Farven teksten skal skrives med</param>
        /// <param name="args">Argumenter til formatering</param>
        public static void WriteLine(string format, ConsoleColor color, params object[] args)
        {
            ConsoleColor originalColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(format, args);
            Console.ForegroundColor = originalColor;
        }
    }
}
