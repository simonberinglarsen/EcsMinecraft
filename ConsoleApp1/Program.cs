namespace ConsoleApp1
{
    internal class Program
    {
        internal static void Main(string[] args)
        {
            using (var win = Game.Instance)
            {
                win.Run();
            }
        }
    }
}