using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace Battleship
{
    internal class GameProcess
    {
        //список для хранения границ отрисованных клеток игрока
        FloatRect[,] playerCellBounds = new FloatRect[10, 10];
        //список для хранения границ отрисованных клеток игрока
        FloatRect[,] botCellBounds = new FloatRect[10, 10];

        Player player;
        Player bot;
        Direction direction = Direction.Horizontal;

        public Direction Direction{ get { return direction;} set { direction = value; } }
        public FloatRect[,] PlayerCellBounds { get => playerCellBounds; set => playerCellBounds = value; }
        public FloatRect[,] BotCellBounds { get => botCellBounds; set => botCellBounds = value; }
        internal Player Player { get => player; set => player = value; }
        internal Player Bot { get => bot; set => bot = value; }

        public GameProcess (Vector2f playerPosition, float cellLength)
        {
            player = new Player(playerPosition.X, playerPosition.Y, cellLength, "Игрок");
            bot = new Player(playerPosition.X * 3.8f, playerPosition.Y, cellLength, "Компьютер");
            for (int y = 0; y < 10; y++)
                for (int x = 0; x < 10; x++)
                {
                    playerCellBounds[y, x] = player.PlayGround[y, x].CellSprite.GetGlobalBounds();
                    botCellBounds[y, x] = bot.PlayGround[y, x].CellSprite.GetGlobalBounds();
                }
        }

        private int prevX = 0; // переменная для хранения предыдущей координаты x
        private int prevY = 0; // переменная для хранения предыдущей координаты y

        public void ShipsSettingMouseHandler(Vector2i mousePos)
        {
            bool shouldBreak = false;
            if (prevX != -1 && prevY != -1) // проверяем, была ли уже выбрана предыдущая клетка
            {
                ResetCellColor(prevY, prevX); // сбрасываем цвет предыдущей клетки
            }
            for (int y = 0; y < 10; y++)
            {
                for (int x = 0; x < 10; x++)
                {
                    if (playerCellBounds[y, x].Contains(mousePos.X, mousePos.Y))
                    {
                        if (player.IsPositionAndDirectionAvailable(new Vector2i(y, x), direction, 3))
                        {
                            if (direction == Direction.Horizontal)
                            {
                                for (int i = 0; i < 3; i++)
                                    player.PlayGround[y, x + i].CellSprite.Color = Color.Green;
                                if (Mouse.IsButtonPressed(Mouse.Button.Left))
                                {
                                    Vector2i[] newCoordinates = new Vector2i[3];
                                    for (int i = 0; i < 3; i++)
                                        newCoordinates[i] = new Vector2i(y, x + i);
                                    player.AddShip(newCoordinates);
                                }
                            }
                            else
                            {
                                for (int i = 0; i < 3; i++)
                                    player.PlayGround[y + i, x].CellSprite.Color = Color.Green;
                                if (Mouse.IsButtonPressed(Mouse.Button.Left))
                                {
                                    Vector2i[] newCoordinates = new Vector2i[3];
                                    for (int i = 0; i < 3; i++)
                                        newCoordinates[i] = new Vector2i(y + i, x);
                                    player.AddShip(newCoordinates);
                                }
                            }
                        }
                        else
                        {
                            if (direction == Direction.Horizontal) // Окрашиваем клетки в случае недоступности для горизонтального положения
                            {
                                for (int i = 0; i < 3; i++)
                                    if (x + i <= 9)
                                        player.PlayGround[y, x + i].CellSprite.Color = Color.Red;
                            }
                            else // Окрашиваем клетки в случае недоступности для вертикального положения
                            {
                                for (int i = 0; i < 3; i++)
                                    if (y + i <= 9)
                                        player.PlayGround[y + i, x].CellSprite.Color = Color.Red;
                            }
                        }
                        // обновляем предыдущие координаты
                        prevX = x;
                        prevY = y;
                        shouldBreak = true;
                        break;
                    }
                    else if (prevX != -1 && prevY != -1)
                    {
                        ResetCellColor(y, x);
                    }
                }
                if (shouldBreak) break;
            }
        }

        public void ResetCellColor(int y, int x)
        {
            if (direction == Direction.Horizontal)
                for (int i = 0; i < 3; i++)
                {
                    if (x + i <= 9)
                        player.PlayGround[y, x + i].CellSprite.Color = new Color(255, 255, 255, 255);
                }
            else
                for (int i = 0; i < 3; i++)
                {
                    if (y + i <= 9)
                        player.PlayGround[y + i, x].CellSprite.Color = new Color(255, 255, 255, 255);
                }
        }

        public void DirectionChange()
        {
            if (direction == Direction.Horizontal)
                direction = Direction.Vertical;
            else
                direction = Direction.Horizontal;
        }

        public void Draw (RenderWindow window)
        {
            player.Draw(window);
            bot.Draw(window);
        }

        public void ResetPlayGrounds()
        {
            player.ResetPlayGround();
            bot.ResetPlayGround();
        }

        public void BotGenerateShips ()
        {
            bot.GenerateShips();
        }

        public void PlayerGenerateShips()
        {
            player.GenerateShips();
        }
    }
}
