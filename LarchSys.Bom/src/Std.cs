namespace LarchSys {
    internal class Std {
        #region Fluend

        public static ColorWriter Red => new(ConsoleColor.Red);
        public static ColorWriter Geen => new(ConsoleColor.Green);
        public static ColorWriter Blue => new(ConsoleColor.Blue);
        public static ColorWriter Yellow => new(ConsoleColor.Yellow);
        public static ColorWriter Cyan => new(ConsoleColor.Cyan);
        public static ColorWriter Magenta => new(ConsoleColor.Magenta);
        public static ColorWriter Gray => new(ConsoleColor.Gray);
        public static ColorWriter White => new(ConsoleColor.White);
        public static ColorWriter Black => new(ConsoleColor.Black);

        public static ColorWriter DarkRed => new(ConsoleColor.DarkRed);
        public static ColorWriter DarkGeen => new(ConsoleColor.DarkGreen);
        public static ColorWriter DarkBlue => new(ConsoleColor.DarkBlue);
        public static ColorWriter DarkYellow => new(ConsoleColor.DarkYellow);
        public static ColorWriter DarkCyan => new(ConsoleColor.DarkCyan);
        public static ColorWriter DarkMagenta => new(ConsoleColor.DarkMagenta);
        public static ColorWriter DarkGray => new(ConsoleColor.DarkGray);

        #endregion


        #region Exceptions

        public static void WriteLine(Exception e)
        {
            WriteLine(null, e);
        }

        public static void WriteLine(string? message, Exception e)
        {
            using var writer = Std.Red;
            writer.Line($"=========================");
            if (!string.IsNullOrEmpty(message)) {
                writer.Line(message);
            }

            var ex = e;
            do {
                writer.Line("[" + ex.GetType().FullName + "]: " + ex.Message);
                if (ex is not AggregateException) {
                    writer.Line(ex.StackTrace);
                }

                ex = ex.InnerException;
                if (ex != null) {
                    writer.Line("-----------------------");
                }
            } while (ex != null);

            writer.Line($"=========================");
        }

        #endregion


        #region Light

        public static void WriteLine(string? line = null)
        {
            Console.WriteLine(line);
        }

        public static void Write(string? line = null)
        {
            Console.WriteLine(line);
        }

        public static void RedLine(string? line = null)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(line);
            Console.ResetColor();
        }

        public static void GreenLine(string? line = null)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(line);
            Console.ResetColor();
        }

        public static void BlueLine(string? line = null)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(line);
            Console.ResetColor();
        }

        public static void YellowLine(string? line = null)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(line);
            Console.ResetColor();
        }

        public static void CyanLine(string? line = null)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(line);
            Console.ResetColor();
        }

        public static void MagentaLine(string? line = null)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine(line);
            Console.ResetColor();
        }

        public static void GrayLine(string? line = null)
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine(line);
            Console.ResetColor();
        }

        //public static void WhiteLine(string? line = null)
        //{
        //    Console.ForegroundColor = ConsoleColor.White;
        //    Console.WriteLine(line);
        //    Console.ResetColor();
        //}

        public static void BlackLine(string? line = null)
        {
            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine(line);
            Console.ResetColor();
        }

        #endregion


        #region Dark

        public static void DarkRedLine(string? line = null)
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine(line);
            Console.ResetColor();
        }

        public static void DarkGeenLine(string? line = null)
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine(line);
            Console.ResetColor();
        }

        public static void DarkBlueLine(string? line = null)
        {
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine(line);
            Console.ResetColor();
        }

        public static void DarkYellowLine(string? line = null)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine(line);
            Console.ResetColor();
        }

        public static void DarkCyanLine(string? line = null)
        {
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine(line);
            Console.ResetColor();
        }

        public static void DarkMagentaLine(string? line = null)
        {
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.WriteLine(line);
            Console.ResetColor();
        }

        public static void DarkGrayLine(string? line = null)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine(line);
            Console.ResetColor();
        }

        #endregion
    }


    internal class ColorWriter : IDisposable {
        public ColorWriter(ConsoleColor color)
        {
            Console.ForegroundColor = color;
        }

        public void Line(string? line = null)
        {
            Console.WriteLine(line);
        }

        public void Write(string text)
        {
            Console.Write(text);
        }

        public void ResetColor()
        {
            Console.ResetColor();
        }

        public void Dispose()
        {
            ResetColor();
        }
    }
}
