using SFML.System;
using SFML.Window;

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