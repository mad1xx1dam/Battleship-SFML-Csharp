using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System.ComponentModel.Design;
using System.Linq;

namespace Battleship
{
    internal class GameProcess
    {
        Random random = new Random();
        RenderWindow window;
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
        Vector2f preGameAdvicePosition;
        Vector2f gameAdvicePosition;
        Vector2f botAdvicePosition;

        Text playerShipsInfo;
        Text botShipsInfo;

        public Direction Direction{ get { return direction;} set { direction = value; } }
        public FloatRect[,] PlayerCellBounds { get => playerCellBounds; set => playerCellBounds = value; }
        public FloatRect[,] BotCellBounds { get => botCellBounds; set => botCellBounds = value; }
        internal Player Player { get => player; set => player = value; }
        internal Player Bot { get => bot; set => bot = value; }

        public GameProcess (Vector2f playerPosition, float cellLength, Font font, RenderWindow window)
        {
            this.window = window;
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
            preGameAdvicePosition = new Vector2f((Player.PlayGround[0, 0].Position.X + Player.PlayGround[0, 9].Position.X - length) / 2, Player.PlayGround[9, 9].Position.Y + length);
            gameAdvicePosition = new Vector2f(window.Size.X / 2, window.Size.Y / 2);
            advice = TextSpriteCreator.TextCreate($"Расположите {nextSize}-палубный корабль", textFont,
                    (uint)(length / 1.5f), Text.Styles.Bold, preGameAdvicePosition.X, preGameAdvicePosition.Y);

            botAdvicePosition = new Vector2f((Bot.PlayGround[0, 0].Position.X + Bot.PlayGround[0, 9].Position.X - length) / 2, Bot.PlayGround[9, 9].Position.Y + length);
            botShipsInfo = TextSpriteCreator.TextCreate("", textFont,
                    (uint)(length / 1.5f), Text.Styles.Regular, botAdvicePosition.X, botAdvicePosition.Y);
            playerShipsInfo = TextSpriteCreator.TextCreate("", textFont,
                    (uint)(length / 1.5f), Text.Styles.Regular, preGameAdvicePosition.X, preGameAdvicePosition.Y);
        }

        private int prevX = 0; // переменная для хранения предыдущей координаты x
        private int prevY = 0; // переменная для хранения предыдущей координаты y

        int shipsAdded = 0;
        bool wasMouseButtonPressed;

        public bool ShipsSettingMouseHandler(Vector2i mousePos)
        {
            if (shipsAdded < 10)
            {
                advice.DisplayedString = $"Расположите {nextSize}-палубный корабль";
                wasMouseButtonPressed = false;
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
                                        ResetCellColor(y, x);
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
                                    if (Mouse.IsButtonPressed(Mouse.Button.Left) && !wasMouseButtonPressed)
                                    {
                                        ResetCellColor(y, x);
                                        wasMouseButtonPressed = true;
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
                    }
                    if (shouldBreak) break;
                }
            }
            else 
            {
                ResetCellColor(prevY, prevX);
                bot.GenerateShips();
                ResetTextForGame();
                return true;     
            }
            return false;
        }

        public bool playerMove = true;
        int playerShipsLeft = 0;
        int botShipsLeft = 10;

        public void MoveCalculating(Vector2i mousePos)
        {
            if (playerShipsLeft > 0 && botShipsLeft > 0)
            {
                if (playerMove)
                {
                    ProcessPlayerTurn(mousePos);
                }
                else
                {
                    ProcessBotTurn();
                }
            }
            else
            {
                if (playerShipsLeft > 0 ) TextSpriteCreator.ResetText(ref advice, gameAdvicePosition, "ВЫ ПОБЕДИЛИ!");
                else TextSpriteCreator.ResetText(ref advice, gameAdvicePosition, "ПОБЕДИЛ\nКОМПЬЮТЕР :(");
            }
        }

        private void ProcessPlayerTurn(Vector2i mousePos)
        {
            wasMouseButtonPressed = false;
            bot.PlayGround[prevY, prevX].CellSprite.Color = new Color(255, 255, 255, 255);
            for (int y = 0; y < 10; y++)
            {
                for (int x = 0; x < 10; x++)
                {
                    if (botCellBounds[y, x].Contains(mousePos.X, mousePos.Y))
                    {
                        if (bot.PlayGround[y, x].CellType != CellType.Miss && bot.PlayGround[y, x].CellType != CellType.ShipBroken)
                        {
                            bot.PlayGround[y, x].CellSprite.Color = Color.Green;
                            if (Mouse.IsButtonPressed(Mouse.Button.Left) && !wasMouseButtonPressed)
                            {
                                if (bot.PlayGround[y, x].CellType == CellType.Ship)
                                {
                                    bot.PlayGround[y, x].ChangeType(CellType.ShipBroken);
                                    //ищется индекс того массива, который содержит эту координату, и затем заменяется на массив размера меньше на 1
                                    int index = bot.ShipPositions.FindIndex(arr => arr.Any(coord => coord.X == y && coord.Y == x));
                                    var newArray = bot.ShipPositions[index].Where(coord => coord.X != y || coord.Y != x).ToArray();
                                    bot.ShipPositions[index] = newArray;
                                    if (newArray.Length == 0)
                                    {
                                        foreach (Vector2i vector in bot.ShipAroundPositions[index])
                                            bot.PlayGround[vector.X, vector.Y].ChangeType(CellType.Miss);
                                        botShipsLeft -= 1;
                                        TextSpriteCreator.ResetText(ref botShipsInfo, botAdvicePosition, $"Осталось кораблей: {botShipsLeft}");
                                    }
                                }
                                else
                                {
                                    bot.PlayGround[y, x].ChangeType(CellType.Miss);
                                    playerMove = false;
                                    TextSpriteCreator.ResetText(ref advice, gameAdvicePosition, "ХОД КОМПЬЮТЕРА");
                                }
                            }
                        }
                        prevX = x;
                        prevY = y;
                    }
                }
            }
        }

        private void ProcessBotTurn()
        {
            bot.PlayGround[prevY, prevX].CellSprite.Color = new Color(255, 255, 255, 255);
            int y = random.Next(0, 10);
            int x = random.Next(0, 10);
            while (player.PlayGround[y, x].CellType == CellType.Miss || player.PlayGround[y, x].CellType == CellType.ShipBroken)
            {
                y = random.Next(0, 10);
                x = random.Next(0, 10);
            }
            if (player.PlayGround[y, x].CellType == CellType.Ship)
            {
                player.PlayGround[y, x].ChangeType(CellType.ShipBroken);
                int index = player.ShipPositions.FindIndex(arr => arr.Any(coord => coord.X == y && coord.Y == x));
                var newArray = player.ShipPositions[index].Where(coord => coord.X != y || coord.Y != x).ToArray();
                player.ShipPositions[index] = newArray;
                if (newArray.Length == 0)
                {
                    foreach (Vector2i vector in player.ShipAroundPositions[index])
                        player.PlayGround[vector.X, vector.Y].ChangeType(CellType.Miss);
                    playerShipsLeft -= 1;
                    TextSpriteCreator.ResetText(ref playerShipsInfo, preGameAdvicePosition, $"Осталось кораблей: {playerShipsLeft}");
                }
            }
            else
            {
                player.PlayGround[y, x].ChangeType(CellType.Miss);
                playerMove = true;
                TextSpriteCreator.ResetText(ref advice, gameAdvicePosition, "ВАШ ХОД");
            }
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

        public void Draw ()
        {
            player.Draw(window);
            bot.Draw(window);
            window.Draw(advice);
            window.Draw(playerShipsInfo);
            window.Draw(botShipsInfo);
        }

        public void ResetPlayGrounds()
        {
            player.ResetPlayGround();
            bot.ResetPlayGround();
            shipsAdded = 0;
            nextSize = shipSizes[shipsAdded];
            playerShipsLeft = 10;
            botShipsLeft = 10;
            TextSpriteCreator.ResetText(ref advice, preGameAdvicePosition, $"Расположите {nextSize}-палубный корабль");
            botShipsInfo.DisplayedString = "";
            playerShipsInfo.DisplayedString = "";
        }

        public void PlayerGenerateShips()
        {
            bot.GenerateShips();
            player.GenerateShips();
            ResetTextForGame();
        }

        private void ResetTextForGame()
        {
            TextSpriteCreator.ResetText(ref advice, gameAdvicePosition, "ВАШ ХОД");
            TextSpriteCreator.ResetText(ref playerShipsInfo, preGameAdvicePosition, $"Осталось кораблей: {playerShipsLeft}");
            TextSpriteCreator.ResetText(ref botShipsInfo, botAdvicePosition, $"Осталось кораблей: {botShipsLeft}");
        }
    }
}
