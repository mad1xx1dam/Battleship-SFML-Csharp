using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace Battleship
{
    internal class GameProcess
    {
        static Font textFont = new Font("Fonts/TNR.ttf");
        static int[] shipSizes = new int[] { 4, 3, 3, 2, 2, 2, 1, 1, 1, 1 };
        RenderWindow window;
        const int gridSize = 10;
        Random random = new Random();
   
        private float length;
        FloatRect[,] playerCellBounds = new FloatRect[gridSize, gridSize];
        FloatRect[,] botCellBounds = new FloatRect[gridSize, gridSize];
        Player player;
        Player bot;

        GameMode gameMode = GameMode.Easy;
        Direction direction = Direction.Horizontal;
        int shipSize;

        Text advice;
        Vector2f preGameAdvicePosition;
        Vector2f gameAdvicePosition;
        Vector2f botAdvicePosition;
        Text playerShipsInfo;
        Text botShipsInfo;

        public Direction Direction { get { return direction; } set { direction = value; } }
        public FloatRect[,] PlayerCellBounds { get => playerCellBounds; set => playerCellBounds = value; }
        public FloatRect[,] BotCellBounds { get => botCellBounds; set => botCellBounds = value; }
        internal Player Player { get => player; set => player = value; }
        internal Player Bot { get => bot; set => bot = value; }

        public GameProcess(Vector2f playerPosition, float cellLength, RenderWindow renderWindow)
        {
            window = renderWindow;
            length = cellLength;
            player = new Player(playerPosition.X, playerPosition.Y, cellLength, "Игрок");
            bot = new Player(playerPosition.X * 3.8f, playerPosition.Y, cellLength, "Компьютер");
            shipSize = shipSizes.First();
            InitPlayersBounds();
            InitText();
        }
        private void InitPlayersBounds()
        {
            for (int y = 0; y < gridSize; y++)
                for (int x = 0; x < gridSize; x++)
                {
                    playerCellBounds[y, x] = player.PlayGround[y, x].CellSprite.GetGlobalBounds();
                    botCellBounds[y, x] = bot.PlayGround[y, x].CellSprite.GetGlobalBounds();
                }
        }
        private void InitText()
        {
            preGameAdvicePosition = new Vector2f((Player.PlayGround[0, 0].Position.X + Player.PlayGround[0, 9].Position.X - length) / 2, Player.PlayGround[9, 9].Position.Y + length);
            gameAdvicePosition = new Vector2f(window.Size.X / 2, window.Size.Y / 2);
            advice = TextSpriteCreator.TextCreate($"Расположите {shipSize}-палубный корабль", textFont,
                    (uint)(length / 1.5f), Text.Styles.Bold, preGameAdvicePosition.X, preGameAdvicePosition.Y);

            botAdvicePosition = new Vector2f((Bot.PlayGround[0, 0].Position.X + Bot.PlayGround[0, 9].Position.X - length) / 2, Bot.PlayGround[9, 9].Position.Y + length);
            botShipsInfo = TextSpriteCreator.TextCreate("", textFont,
                    (uint)(length / 1.5f), Text.Styles.Regular, botAdvicePosition.X, botAdvicePosition.Y);
            playerShipsInfo = TextSpriteCreator.TextCreate("", textFont,
                    (uint)(length / 1.5f), Text.Styles.Regular, preGameAdvicePosition.X, preGameAdvicePosition.Y);
        }
        private int prevX = 0; 
        private int prevY = 0; 
        int shipsAddedOnPlayerPlayground = 0;
        bool wasMouseButtonPressed;
        public bool ShipsSettingMouseHandler(Vector2i mousePos)
        {
            if (shipsAddedOnPlayerPlayground < 10)
            {
                advice.DisplayedString = $"Расположите {shipSize}-палубный корабль";
                wasMouseButtonPressed = false;
                bool shouldBreak = false;
                ResetCellColor(prevY, prevX); 
                for (int y = 0; y < gridSize; y++)
                {
                    for (int x = 0; x < gridSize; x++)
                    {
                        if (playerCellBounds[y, x].Contains(mousePos.X, mousePos.Y))
                        {
                            if (player.IsPositionAndDirectionAvailable(new Vector2i(y, x), direction, shipSize))
                            {
                                if (direction == Direction.Horizontal)
                                {
                                    for (int i = 0; i < shipSize; i++)
                                        player.PlayGround[y, x + i].CellSprite.Color = Color.Green;
                                    if (Mouse.IsButtonPressed(Mouse.Button.Left) && !wasMouseButtonPressed)
                                    {
                                        ResetCellColor(y, x);
                                        wasMouseButtonPressed = true;
                                        Vector2i[] newCoordinates = new Vector2i[shipSize];
                                        for (int i = 0; i < shipSize; i++)
                                            newCoordinates[i] = new Vector2i(y, x + i);
                                        player.AddShip(newCoordinates);
                                        shipsAddedOnPlayerPlayground += 1;
                                        if (shipsAddedOnPlayerPlayground != 10) shipSize = shipSizes[shipsAddedOnPlayerPlayground]; 
                                    }
                                    else wasMouseButtonPressed = false;
                                }
                                else
                                {
                                    for (int i = 0; i < shipSize; i++)
                                        player.PlayGround[y + i, x].CellSprite.Color = Color.Green;
                                    if (Mouse.IsButtonPressed(Mouse.Button.Left) && !wasMouseButtonPressed)
                                    {
                                        ResetCellColor(y, x);
                                        wasMouseButtonPressed = true;
                                        Vector2i[] newCoordinates = new Vector2i[shipSize];
                                        for (int i = 0; i < shipSize; i++)
                                            newCoordinates[i] = new Vector2i(y + i, x);
                                        player.AddShip(newCoordinates);
                                        shipsAddedOnPlayerPlayground += 1;
                                        if (shipsAddedOnPlayerPlayground != 10) shipSize = shipSizes[shipsAddedOnPlayerPlayground];
                                    }
                                }
                            }
                            else
                            {
                                if (direction == Direction.Horizontal) 
                                {
                                    for (int i = 0; i < shipSize; i++)
                                        if (x + i <= gridSize - 1)
                                            player.PlayGround[y, x + i].CellSprite.Color = Color.Red;
                                }
                                else 
                                {
                                    for (int i = 0; i < shipSize; i++)
                                        if (y + i <= gridSize - 1)
                                            player.PlayGround[y + i, x].CellSprite.Color = Color.Red;
                                }
                            }
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
        int playerShipsLeft = 10;
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
                    Thread.Sleep(900);
                    ProcessBotTurn();
                }
            }
            else
            {
                bot.PlayGround[prevY, prevX].CellSprite.Color = new Color(255, 255, 255, 255);
                if (playerShipsLeft > 0) TextSpriteCreator.ResetText(ref advice, gameAdvicePosition, "ВЫ ПОБЕДИЛИ!");
                else
                {
                    TextSpriteCreator.ResetText(ref advice, gameAdvicePosition, "ПОБЕДИЛ\nКОМПЬЮТЕР :(");
                    for (int i = 0; i < gridSize; i++)
                        for (int j = 0; j < gridSize; j++)
                            if (bot.PlayGround[i, j].CellType == CellType.Ship)
                                bot.PlayGround[i, j].ChangeType(bot.PlayGround[i, j].CellType);
                }
            }
        }
        private void ProcessPlayerTurn(Vector2i mousePos)
        {
            wasMouseButtonPressed = false;
            bot.PlayGround[prevY, prevX].CellSprite.Color = new Color(255, 255, 255, 255);
            for (int y = 0; y < gridSize; y++)
            {
                for (int x = 0; x < gridSize; x++)
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
                                bot.PlayGround[prevY, prevX].CellSprite.Color = new Color(255, 255, 255, 255);
                            }
                        }
                        prevX = x;
                        prevY = y;
                    }
                }
            }
        }
        Vector2i previousDetect = new Vector2i(-1, -1);
        Vector2i firstDetect = new Vector2i(-1, -1);
        Direction directionToFind = Direction.None;
        private void ProcessBotTurn()
        {
            int y = 0;
            int x = 0;
            if (gameMode == GameMode.Easy)
            {
                EasyModeCalculate(ref y, ref x);
            }
            else
            {
                HardModeCalculate(ref y, ref x);
            }
            if (player.PlayGround[y, x].CellType == CellType.Ship)
            {
                previousDetect.X = y; 
                previousDetect.Y = x;
                player.PlayGround[y, x].ChangeType(CellType.ShipBroken);
                int index = player.ShipPositions.FindIndex(arr => arr.Any(coord => coord.X == y && coord.Y == x));
                var newArray = player.ShipPositions[index].Where(coord => !(coord.X == y && coord.Y == x)).ToArray();
                player.ShipPositions[index] = newArray;
                if (newArray.Length == 0) 
                {
                    previousDetect.X = -1; 
                    directionToFind = Direction.None;
                    foreach (Vector2i vector in player.ShipAroundPositions[index])
                        player.PlayGround[vector.X, vector.Y].ChangeType(CellType.Miss);
                    playerShipsLeft -= 1;
                    TextSpriteCreator.ResetText(ref playerShipsInfo, preGameAdvicePosition, $"Осталось кораблей: {playerShipsLeft}");
                }
            }
            else
            {
                playerMove = true;
                player.PlayGround[y, x].ChangeType(CellType.Miss);
                TextSpriteCreator.ResetText(ref advice, gameAdvicePosition, "ВАШ ХОД");
            }
        }
        private void EasyModeCalculate(ref int y, ref int x)
        {
            y = random.Next(0, gridSize);
            x = random.Next(0, gridSize);
            while (player.PlayGround[y, x].CellType == CellType.Miss || player.PlayGround[y, x].CellType == CellType.ShipBroken)
            {
                y = random.Next(0, gridSize);
                x = random.Next(0, gridSize);
            }
        }
        int offset;
        private void HardModeCalculate(ref int y, ref int x)
        {
            if (previousDetect.X == -1)
            {
                EasyModeCalculate(ref y, ref x);
            }
            else
            {
                if (directionToFind == Direction.None)
                {
                    offset = random.Next(0, 2) * 2 - 1; 
                    firstDetect = previousDetect;
                    if (random.Next(0, 2) == 0)
                    {
                        directionToFind = Direction.Vertical; 
                    }
                    else
                    {
                        directionToFind = Direction.Horizontal; 
                    }
                }
                switch (directionToFind)
                {
                    case Direction.Vertical:
                        if (IsCellAvailable(previousDetect.X - offset, previousDetect.Y))
                        {
                            y = previousDetect.X - offset;
                            x = previousDetect.Y; 
                        }
                        else if (IsCellAvailable(firstDetect.X + offset, firstDetect.Y))
                        {
                            y = firstDetect.X + offset;
                            x = firstDetect.Y;
                            offset *= -1; 
                        }
                        else
                        {
                            directionToFind = Direction.Horizontal;
                            offset = random.Next(0, 2) * 2 - 1; 
                            goto case Direction.Horizontal;
                        }
                        break;

                    case Direction.Horizontal:
                        if (IsCellAvailable(previousDetect.X, previousDetect.Y - offset))
                        {
                            y = previousDetect.X;
                            x = previousDetect.Y - offset; 
                        }
                        else if (IsCellAvailable(firstDetect.X, firstDetect.Y + offset))
                        {
                            y = firstDetect.X;
                            x = firstDetect.Y + offset;
                            offset *= -1; 
                        }
                        else
                        {
                            offset = random.Next(0, 2) * 2 - 1;
                            directionToFind = Direction.Vertical;
                            goto case Direction.Vertical; 
                        }
                        break;
                }
            }
        }
        private bool IsCellAvailable(int y, int x)
        {
            return y >= 0 && y <= gridSize - 1 && x >= 0 && x <= gridSize - 1 && player.PlayGround[y, x].CellType != CellType.Miss 
                && player.PlayGround[y, x].CellType != CellType.ShipBroken;
        }
        public void ResetCellColor(int y, int x)
        {
            if (direction == Direction.Horizontal)
                for (int i = 0; i < shipSize; i++)
                {
                    if (x + i <= 9)
                        player.PlayGround[y, x + i].CellSprite.Color = new Color(255, 255, 255, 255);
                }
            else
                for (int i = 0; i < shipSize; i++)
                {
                    if (y + i <= 9)
                        player.PlayGround[y + i, x].CellSprite.Color = new Color(255, 255, 255, 255);
                }
        }
        public void DirectionChange()
        {
            direction = direction == Direction.Horizontal ? Direction.Vertical : Direction.Horizontal;
        }
        public void Draw()
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
            shipsAddedOnPlayerPlayground = 0;
            shipSize = shipSizes[shipsAddedOnPlayerPlayground];
            previousDetect.X = -1;
            playerShipsLeft = 10;
            botShipsLeft = 10;
            TextSpriteCreator.ResetText(ref advice, preGameAdvicePosition, $"Расположите {shipSize}-палубный корабль");
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
        public void ChangeGameMode(GameMode gameMode)
        {
            this.gameMode = gameMode;
        }
    }
}