using SFML.Graphics;
using SFML.System;
using System;

namespace Battleship
{
    internal class Player
    {
        Random random = new Random();
        //для отслеживания пока что пустых позиций
        public List<Vector2i[]> shipPositions = new List<Vector2i[]>();
        //для отслеживания пока что пустых позиций
        private List<Vector2i> allowedPositions = new List<Vector2i>();
        private static Font font = new Font("Fonts/TNR.ttf");
        //список букв для отрисовки в клетках
        static string[] letters = new string[] { "А", "Б", "В", "Г", "Д", "Е", "Ж", "З", "И", "К" };
        //дополнительные ряды сверху/слева для информации о "координатах"
        Cell[] upLine = new Cell[10];
        Text[] upLineText = new Text[10];
        Cell[] leftLine = new Cell[10];
        Text[] leftLineText = new Text[10];
        //само "поле битвы"
        public Cell[,] playGround = new Cell[10, 10];
        //имя
        Text name;

        public Player(float x, float y, float length, string name)
        {
            // Создание игрового поля
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    playGround[i, j] = new Cell(x + j * length, y + i * length, length, CellType.Water);
                    allowedPositions.Add(new Vector2i(i, j));
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
            this.name = TextSpriteCreator.TextCreate(name, font, 20, Text.Styles.Italic, playGround[0, 4].Position.X, y - length * 2);
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
            foreach (Vector2i[] positionArray in shipPositions)
                foreach (Vector2i position in positionArray)
                    playGround[position.X, position.Y].ChangeType(CellType.Ship);
        }

        private Vector2i[] GenerateCoordinates(int count)
        {
            Vector2i[] newCoordinates = new Vector2i[count];

            bool coordinatesGenerated = false;

            while (!coordinatesGenerated)
            {
                int x = random.Next(0, 10);
                int y = random.Next(0, 10);

                if (x + count <= 10)
                {
                    Vector2i startingPosition = new Vector2i(x, y);
                    if (IsPositionAvailable(startingPosition, count))
                    {
                        for (int i = 0; i < count; i++)
                        {
                            Vector2i toAdd = new Vector2i(x + i, y);
                            newCoordinates[i] = toAdd;
                            allowedPositions.Remove(toAdd);
                        }
                        coordinatesGenerated = true;
                    }
                }
            }

            return newCoordinates;
        }

        private bool IsPositionAvailable(Vector2i position, int count)
        {
            for (int i = 0; i < count; i++)
            {
                if (playGround[position.X + i, position.Y].CellType == CellType.Ship)
                {
                    return false; // позиция занята
                }
            }
            return true; // позиция доступна
        }
    }
}


