using SFML.System;

namespace Battleship
{
    internal class GameProcess
    {
        Player player;
        Player bot;

        public GameProcess (Vector2f playerPosition, float cellLength)
        {
            player = new Player(playerPosition.X, playerPosition.Y, cellLength, "Игрок");
            bot = new Player(playerPosition.X * 3.8f, playerPosition.Y, cellLength, "Компьютер");
        }

        public void Move () { }
    }
}
