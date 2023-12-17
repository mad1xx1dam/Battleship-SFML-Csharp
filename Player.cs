using SFML.Graphics;
using SFML.System;

namespace Battleship
{
    internal class Player
    {
        private static Font font = new Font("Fonts/TNR.ttf");
        //список букв для отрисовки в клетках
        static string[] letters = new string[] { "А", "Б", "В", "Г", "Д", "Е", "Ж", "З", "И", "К" };
        Random random = new Random();
        //для отслеживания пока что пустых позиций
        List<Vector2i[]> shipPositions = new List<Vector2i[]>();
        List<Vector2i[]> shipAroundPositions = new List<Vector2i[]>();
        //для отслеживания пока что пустых позиций
        //дополнительные ряды сверху/слева для информации о "координатах"
        Cell[] upLine = new Cell[10];
        Text[] upLineText = new Text[10];
        Cell[] leftLine = new Cell[10];
        Text[] leftLineText = new Text[10];
        //само "поле битвы"
        private Cell[,] playGround = new Cell[10, 10];
        //имя
        Text name;

        public Cell[,] PlayGround { get => playGround; set => playGround = value; }
        public List<Vector2i[]> ShipPositions { get => shipPositions; set => shipPositions = value; }
        public List<Vector2i[]> ShipAroundPositions { get => shipAroundPositions; set => shipAroundPositions = value; }

        public Player(float x, float y, float length, string name)
        {
            // Создание игрового поля
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    playGround[i, j] = new Cell(x + j * length, y + i * length, length, CellType.Water);
                }
            }

            // Создание верхнего ряда букв и заполнение текстом
            for (int k = 0; k < 10; k++)
            {
                upLine[k] = new Cell(x + k * length, y - length, length, CellType.DigitOrLetter);
                upLineText[k] = TextSpriteCreator.TextCreate(letters[k], font, (uint)(length / 2), Text.Styles.Bold, x + k * length, y - length);

                // Создание левого ряда цифр и заполнение текстом
                leftLine[k] = new Cell(x - length, y + k * length, length, CellType.DigitOrLetter);
                leftLineText[k] = TextSpriteCreator.TextCreate((k + 1).ToString(), font, (uint)(length / 2), Text.Styles.Bold, x - length, y + k * length);
            }
            this.name = TextSpriteCreator.TextCreate(name, font, (uint)(length / 1.5f), Text.Styles.Italic, (playGround[0, 9].Position.X + playGround[0, 0].Position.X) / 2, y - length * 2);
        }

        public void Draw(RenderWindow window)
        {
            //вывод клеток
            foreach (Cell cell in playGround) cell.Draw(window);
            foreach (Cell cell in upLine) cell.Draw(window);
            foreach (Cell cell in leftLine) cell.Draw(window);
            //вывод текста
            foreach (Text text in upLineText) window.Draw(text);
            foreach (Text text in leftLineText) window.Draw(text);
            window.Draw(name);
        }

        public void GenerateShips()
        {
            ResetPlayGround();
            int[] shipSizes = new int[] { 4, 3, 3, 2, 2, 2, 1, 1, 1, 1 };
            foreach (int size in shipSizes)
            {
                shipPositions.Add(GenerateCoordinates(size));
            }
            CalculateCellsAroundBrokenShip();
        }

        public void AddShip(Vector2i[] coordinates)
        {
            shipPositions.Add(coordinates);
            foreach (Vector2i position in coordinates)
                playGround[position.X, position.Y].ChangeType(CellType.Ship);
            if (shipPositions.Count == 10) CalculateCellsAroundBrokenShip();
        }

        private Vector2i[] GenerateCoordinates(int count)
        {
            Vector2i[] newCoordinates = new Vector2i[count];

            int y = 0;
            int x = 0;
            Direction direction;

            bool coordinatesGenerated = false;

            while (!coordinatesGenerated)
            {
                direction = (Direction)random.Next(0, 2);
                
                switch (direction)
                {
                    //не имеет смысла рассматривать y, начиная от 10 - count + 1 до конца, так как корабль просто не вместится
                    case Direction.Vertical:
                        y = random.Next(0, 10 - count + 1);
                        x = random.Next(0, 10);
                        break;
                    //не имеет смысла рассматривать x, начиная от 10 - count + 1 до конца, так как корабль просто не вместится
                    case Direction.Horizontal:
                        y = random.Next(0, 10);
                        x = random.Next(0, 10 - count + 1);
                        break;
                }

                if (IsPositionAndDirectionAvailable(new Vector2i(y, x), direction, count))
                {
                    Vector2i currentCell;
                    if (direction == Direction.Vertical) // Вертикальная ориентация
                    {
                        for (int i = 0; i < count; i++)
                        {
                            currentCell = new Vector2i(y + i, x);
                            playGround[currentCell.X, currentCell.Y].ChangeType(CellType.Ship, name.DisplayedString != "Компьютер");
                            Vector2i toAdd = new Vector2i(y + i, x);
                            newCoordinates[i] = toAdd;
                        }   
                    }
                    else // Горизонтальная ориентация
                    {
                        for (int i = 0; i < count; i++)
                        {
                            currentCell = new Vector2i(y, x + i);
                            playGround[currentCell.X, currentCell.Y].ChangeType(CellType.Ship, name.DisplayedString != "Компьютер");
                            Vector2i toAdd = new Vector2i(y, x + i);
                            newCoordinates[i] = toAdd;
                        }
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

            //если направление вертикальное 
            if (direction == Direction.Vertical)
            {
                if (y + count > 10) return false; //просто не вместится в вертикаль
                int[] xToCheck = new int[3] { x - 1, x, x + 1 };
                
                int yPrevious = y - 1 >= 0 ? y - 1 : 0;      //предыдущая точка до корабаля
                int yNext = y + count <= 9 ? y + count : 9;  //следующая за кораблем точка

                foreach (int xCheck in xToCheck)
                    if (xCheck >= 0 && xCheck <= 9)
                        for (int i = yPrevious; i <= yNext; i++)
                        {
                            if (playGround[i, xCheck].CellType == CellType.Ship) return false;
                        }
            }
            else //горизонталь
            {
                if (x + count > 10) return false; //просто не вместится в горизонталь
                int[] yToCheck = new int[3] { y - 1, y, y + 1 };
                
                int xPrevious = x - 1 >= 0 ? x - 1 : 0;      //предыдущая точка до корабаля
                int xNext = x + count <= 9 ? x + count : 9;  //следующая за кораблем точка

                foreach (int yCheck in yToCheck)
                    if (yCheck >= 0 && yCheck <= 9)
                        for (int i = xPrevious; i <= xNext; i++)
                        {
                            if (playGround[yCheck, i].CellType == CellType.Ship) return false;
                        }
            }
            return true; // позиция доступна
        }

        public void ResetPlayGround ()
        {
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    playGround[i, j].ChangeType(CellType.Water); 
                }
            }
            shipPositions.Clear();
            shipAroundPositions.Clear();
        }

        //метод для вычисления координат вокруг корабля на случай, если корабль потонет
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
                    int lastY = shipPositions[i].Select(pos => pos.X).Max() + 1 <= 9 ? shipPositions[i].Select(pos => pos.X).Max() + 1 : 9;

                    for (int y = firstY; y <= lastY; y++)
                    {
                        for (int offsetX = -1; offsetX <= 1; offsetX++)
                        {
                            int newX = constX + offsetX;
                            Vector2i coordCheck = new Vector2i(y, newX);
                            if (newX >= 0 && newX <= 9 && !shipPositions[i].Contains(coordCheck)) // Проверить, что newY находится в пределах 0-9
                                surroundingCoords.Add(coordCheck);
                        }
                    }
                }
                else
                {
                    int constY = shipPositions[i][0].X;
                    int firstX = shipPositions[i].Select(pos => pos.Y).Min() - 1 >= 0 ? shipPositions[i].Select(pos => pos.Y).Min() - 1 : 0;
                    int lastX = shipPositions[i].Select(pos => pos.Y).Max() + 1 <= 9 ? shipPositions[i].Select(pos => pos.Y).Max() + 1 : 9;

                    for (int x = firstX; x <= lastX; x++)
                    {
                        for (int offsetY = -1; offsetY <= 1; offsetY++)
                        {
                            int newY = constY + offsetY;
                            Vector2i coordCheck = new Vector2i(newY, x);
                            if (newY >= 0 && newY <= 9 && !shipPositions[i].Contains(coordCheck)) // Проверить, что newY находится в пределах 0-9
                                surroundingCoords.Add(coordCheck);
                        }
                    }
                }
                shipAroundPositions.Add(surroundingCoords.ToArray());
            }
        }
    }
}