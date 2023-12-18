using SFML.Graphics;
using SFML.System;

namespace Battleship
{
    internal class Player
    {
        static Font font = new Font("Fonts/TNR.ttf");
        static Random random = new Random();
        static string[] letters = new string[] { "А", "Б", "В", "Г", "Д", "Е", "Ж", "З", "И", "К" };
        static int[] shipSizes = new int[] { 4, 3, 3, 2, 2, 2, 1, 1, 1, 1 };
        const int gridSize = 10;

        List<Vector2i[]> shipPositions = new List<Vector2i[]>();
        List<Vector2i[]> shipAroundPositions = new List<Vector2i[]>();
        Cell[] upLine = new Cell[gridSize];
        Text[] upLineText = new Text[gridSize];
        Cell[] leftLine = new Cell[gridSize];
        Text[] leftLineText = new Text[gridSize];
        Cell[,] playGround = new Cell[gridSize, gridSize];

        Text name;

        public Cell[,] PlayGround { get => playGround; set => playGround = value; }
        public List<Vector2i[]> ShipPositions { get => shipPositions; set => shipPositions = value; }
        public List<Vector2i[]> ShipAroundPositions { get => shipAroundPositions; set => shipAroundPositions = value; }

        public Player(float x, float y, float length, string name)
        {
            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    playGround[i, j] = new Cell(x + j * length, y + i * length, length, CellType.Water);
                }
            }
            for (int k = 0; k < gridSize; k++)
            {
                upLine[k] = new Cell(x + k * length, y - length, length, CellType.DigitOrLetter);
                upLineText[k] = TextSpriteCreator.TextCreate(letters[k], font, (uint)(length / 2), Text.Styles.Bold, x + k * length, y - length);

                leftLine[k] = new Cell(x - length, y + k * length, length, CellType.DigitOrLetter);
                leftLineText[k] = TextSpriteCreator.TextCreate((k + 1).ToString(), font, (uint)(length / 2), Text.Styles.Bold, x - length, y + k * length);
            }
            this.name = TextSpriteCreator.TextCreate(name, font, (uint)(length / 1.5f), Text.Styles.Italic, (playGround[0, 9].Position.X + playGround[0, 0].Position.X) / 2, y - length * 2);
        }
        public void Draw(RenderWindow window)
        {
            foreach (Cell cell in playGround) cell.Draw(window);
            foreach (Cell cell in upLine) cell.Draw(window);
            foreach (Cell cell in leftLine) cell.Draw(window);
            foreach (Text text in upLineText) window.Draw(text);
            foreach (Text text in leftLineText) window.Draw(text);
            window.Draw(name);
        }
        public void AddShip(Vector2i[] coordinates)
        {
            shipPositions.Add(coordinates);
            foreach (Vector2i position in coordinates)
                playGround[position.X, position.Y].ChangeType(CellType.Ship);
            if (shipPositions.Count == 10) CalculateCellsAroundBrokenShip();
        }
        public void GenerateShips()
        {
            ResetPlayGround();
            foreach (int size in shipSizes)
            {
                shipPositions.Add(GenerateCoordinates(size));
            }
            CalculateCellsAroundBrokenShip();
        }
        private Vector2i[] GenerateCoordinates(int count)
        {
            Vector2i[] newCoordinates = new Vector2i[count];
            Random random = new Random();
            Direction direction = (Direction)random.Next(0, 2);
            int y = random.Next(0, gridSize);
            int x = random.Next(0, gridSize);
            bool coordinatesGenerated = false;

            while (!coordinatesGenerated)
            {
                y = direction == Direction.Vertical ? random.Next(0, gridSize - count + 1) : random.Next(0, gridSize);
                x = direction == Direction.Vertical ? random.Next(0, gridSize) : random.Next(0, gridSize - count + 1);
                if (IsPositionAndDirectionAvailable(new Vector2i(y, x), direction, count))
                {
                    for (int i = 0; i < count; i++)
                    {
                        int currentY = direction == Direction.Vertical ? y + i : y;
                        int currentX = direction == Direction.Vertical ? x : x + i;
                        playGround[currentY, currentX].ChangeType(CellType.Ship, name.DisplayedString != "Компьютер");
                        Vector2i toAdd = new Vector2i(currentY, currentX);
                        newCoordinates[i] = toAdd;
                    }
                    coordinatesGenerated = true;
                }
            }
            return newCoordinates;
        }
        public bool IsPositionAndDirectionAvailable(Vector2i position, Direction direction, int count)
        {
            int y = position.X;
            int x = position.Y;

            if (direction == Direction.Vertical)
            {
                if (y + count > gridSize) return false; 
                int[] xToCheck = new int[3] { x - 1, x, x + 1 };

                int yPrevious = y - 1 >= 0 ? y - 1 : 0;      
                int yNext = y + count <= gridSize - 1 ? y + count : gridSize - 1;  

                foreach (int xCheck in xToCheck)
                    if (xCheck >= 0 && xCheck <= gridSize - 1)
                        for (int i = yPrevious; i <= yNext; i++)
                        {
                            if (playGround[i, xCheck].CellType == CellType.Ship) return false;
                        }
            }
            else 
            {
                if (x + count > gridSize) return false; 
                int[] yToCheck = new int[3] { y - 1, y, y + 1 };

                int xPrevious = x - 1 >= 0 ? x - 1 : 0;      
                int xNext = x + count <= gridSize - 1 ? x + count : gridSize - 1;  

                foreach (int yCheck in yToCheck)
                    if (yCheck >= 0 && yCheck <= gridSize - 1)
                        for (int i = xPrevious; i <= xNext; i++)
                        {
                            if (playGround[yCheck, i].CellType == CellType.Ship) return false;
                        }
            }
            return true; 
        }
        public void CalculateCellsAroundBrokenShip()
        {
            List<Vector2i> surroundingCoords = new List<Vector2i>();
            for (int i = 0; i < shipPositions.Count(); i++)
            {
                surroundingCoords = new List<Vector2i>();
                int shipLength = shipPositions[i].Length;
                if (shipLength == 1 || (shipLength > 1 && shipPositions[i][0].Y == shipPositions[i][1].Y))
                {
                    int constX = shipPositions[i][0].Y;
                    int firstY = shipPositions[i].Select(pos => pos.X).Min() - 1 >= 0 ? shipPositions[i].Select(pos => pos.X).Min() - 1 : 0;
                    int lastY = shipPositions[i].Select(pos => pos.X).Max() + 1 <= gridSize - 1 ? shipPositions[i].Select(pos => pos.X).Max() + 1 : gridSize - 1;

                    for (int y = firstY; y <= lastY; y++)
                    {
                        for (int offsetX = -1; offsetX <= 1; offsetX++)
                        {
                            int newX = constX + offsetX;
                            Vector2i coordCheck = new Vector2i(y, newX);
                            if (newX >= 0 && newX <= gridSize - 1 && !shipPositions[i].Contains(coordCheck)) 
                                surroundingCoords.Add(coordCheck);
                        }
                    }
                }
                else
                {
                    int constY = shipPositions[i][0].X;
                    int firstX = shipPositions[i].Select(pos => pos.Y).Min() - 1 >= 0 ? shipPositions[i].Select(pos => pos.Y).Min() - 1 : 0;
                    int lastX = shipPositions[i].Select(pos => pos.Y).Max() + 1 <= gridSize - 1 ? shipPositions[i].Select(pos => pos.Y).Max() + 1 : gridSize - 1;

                    for (int x = firstX; x <= lastX; x++)
                    {
                        for (int offsetY = -1; offsetY <= 1; offsetY++)
                        {
                            int newY = constY + offsetY;
                            Vector2i coordCheck = new Vector2i(newY, x);
                            if (newY >= 0 && newY <= gridSize - 1 && !shipPositions[i].Contains(coordCheck)) 
                                surroundingCoords.Add(coordCheck);
                        }
                    }
                }
                shipAroundPositions.Add(surroundingCoords.ToArray());
            }
        }
        public void ResetPlayGround()
        {
            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    if (playGround[i, j].CellType != CellType.Water)
                        playGround[i, j].ChangeType(CellType.Water);
                }
            }
            shipPositions.Clear();
            shipAroundPositions.Clear();
        }
    }
}