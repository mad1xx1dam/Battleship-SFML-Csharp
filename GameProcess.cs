using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace Battleship
{
    internal class GameProcess
    {
        private static Font? textFont;
        private float length;
        //список для хранения границ отрисованных клеток игрока
        FloatRect[,] playerCellBounds = new FloatRect[10, 10];
        //список для хранения границ отрисованных клеток игрока
        FloatRect[,] botCellBounds = new FloatRect[10, 10];

        GameMode gameMode = GameMode.Easy;

        Player player;
        Player bot;
        Direction direction = Direction.Horizontal;

        static int[] shipSizes = new int[] { 4, 3, 3, 2, 2, 2, 1, 1, 1, 1 };
        int nextSize;

        Text advice;

        public Direction Direction{ get { return direction;} set { direction = value; } }
        public FloatRect[,] PlayerCellBounds { get => playerCellBounds; set => playerCellBounds = value; }
        public FloatRect[,] BotCellBounds { get => botCellBounds; set => botCellBounds = value; }
        internal Player Player { get => player; set => player = value; }
        internal Player Bot { get => bot; set => bot = value; }

        public GameProcess (Vector2f playerPosition, float cellLength, Font font)
        {
            length = cellLength;
            textFont = font;
            player = new Player(playerPosition.X, playerPosition.Y, cellLength, "Игрок");
            bot = new Player(playerPosition.X * 3.8f, playerPosition.Y, cellLength, "Компьютер");
            for (int y = 0; y < 10; y++)
                for (int x = 0; x < 10; x++)
                {
                    playerCellBounds[y, x] = player.PlayGround[y, x].CellSprite.GetGlobalBounds();
                    botCellBounds[y, x] = bot.PlayGround[y, x].CellSprite.GetGlobalBounds();
                }
            nextSize = 4;
            advice = TextSpriteCreator.TextCreate($"Расположите {nextSize}-палубный корабль", textFont,
                    (uint)(length / 1.5f), Text.Styles.Bold, (Player.PlayGround[0, 0].Position.X + Player.PlayGround[0, 9].Position.X) / 2, Player.PlayGround[9, 9].Position.Y + length);
        }

        private int prevX = 0; // переменная для хранения предыдущей координаты x
        private int prevY = 0; // переменная для хранения предыдущей координаты y

        int shipsAdded = 0;

        public bool ShipsSettingMouseHandler(Vector2i mousePos)
        {
            if (shipsAdded < 10)
            {
                advice.DisplayedString = $"Расположите {nextSize}-палубный корабль";
                bool wasMouseButtonPressed = false;
                bool shouldBreak = false;
                ResetCellColor(prevY, prevX); // сбрасываем цвет предыдущей клетки
                for (int y = 0; y < 10; y++)
                {
                    for (int x = 0; x < 10; x++)
                    {
                        if (playerCellBounds[y, x].Contains(mousePos.X, mousePos.Y))
                        {
                            if (player.IsPositionAndDirectionAvailable(new Vector2i(y, x), direction, nextSize))
                            {
                                if (direction == Direction.Horizontal)
                                {
                                    for (int i = 0; i < nextSize; i++)
                                        player.PlayGround[y, x + i].CellSprite.Color = Color.Green;
                                    if (Mouse.IsButtonPressed(Mouse.Button.Left) && !wasMouseButtonPressed)
                                    {
                                        wasMouseButtonPressed = true;
                                        Vector2i[] newCoordinates = new Vector2i[nextSize];
                                        for (int i = 0; i < nextSize; i++)
                                            newCoordinates[i] = new Vector2i(y, x + i);
                                        player.AddShip(newCoordinates);
                                        shipsAdded += 1;
                                        if (shipsAdded != 10) nextSize = shipSizes[shipsAdded]; // Установка следующего размера корабля после добавления
                                    }
                                    else wasMouseButtonPressed = false;
                                }
                                else
                                {
                                    for (int i = 0; i < nextSize; i++)
                                        player.PlayGround[y + i, x].CellSprite.Color = Color.Green;
                                    if (Mouse.IsButtonPressed(Mouse.Button.Left))
                                    {
                                        Vector2i[] newCoordinates = new Vector2i[nextSize];
                                        for (int i = 0; i < nextSize; i++)
                                            newCoordinates[i] = new Vector2i(y + i, x);
                                        player.AddShip(newCoordinates);
                                        shipsAdded += 1;
                                        if (shipsAdded != 10) nextSize = shipSizes[shipsAdded];
                                    }
                                }
                            }
                            else
                            {
                                if (direction == Direction.Horizontal) // Окрашиваем клетки в случае недоступности для горизонтального положения
                                {
                                    for (int i = 0; i < nextSize; i++)
                                        if (x + i <= 9)
                                            player.PlayGround[y, x + i].CellSprite.Color = Color.Red;
                                }
                                else // Окрашиваем клетки в случае недоступности для вертикального положения
                                {
                                    for (int i = 0; i < nextSize; i++)
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
            else 
            {
                ResetCellColor(prevY, prevX);
                advice.DisplayedString = " ";
                return true;     
            }
            return false;
        }

        bool playerMove = true;

        public void MoveCalculating()
        {

        }

        public void ResetCellColor(int y, int x)
        {
            if (direction == Direction.Horizontal)
                for (int i = 0; i < nextSize; i++)
                {
                    if (x + i <= 9)
                        player.PlayGround[y, x + i].CellSprite.Color = new Color(255, 255, 255, 255);
                }
            else
                for (int i = 0; i < nextSize; i++)
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
            window.Draw(advice);
        }

        public void ResetPlayGrounds()
        {
            player.ResetPlayGround();
            bot.ResetPlayGround();
            shipsAdded = 0;
            nextSize = shipSizes[shipsAdded];
        }

        public void BotGenerateShips ()
        {
            bot.GenerateShips();
        }

        public void PlayerGenerateShips()
        {
            advice.DisplayedString = " ";
            player.GenerateShips();   
        }
    }
}
