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
        public List<Vector2i[]> shipPositions = new List<Vector2i[]>();
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
            int[] shipSizes = new int[] { 4, 3, 3, 2, 2, 2, 1, 1, 1, 1 };
            foreach (int size in shipSizes)
            {
                shipPositions.Add(GenerateCoordinates(size));
            }           
        }

        public void AddShip(Vector2i[] coordinates)
        {
            shipPositions.Add(coordinates);
            foreach (Vector2i position in coordinates)
                playGround[position.X, position.Y].ChangeType(CellType.Ship);
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
        }
    }
}