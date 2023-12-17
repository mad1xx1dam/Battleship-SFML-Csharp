using SFML.Window;
using System.Diagnostics;

namespace Battleship
{
    class Program
    {
        static void Main()
        {
            Game game = new Game(1600, 800, "Морской бой", Styles.None);
            game.Run();
        } 
    }
}