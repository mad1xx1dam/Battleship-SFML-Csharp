using SFML.Graphics;
using SFML.System;

namespace Battleship
{
    internal class Cell
    {
        public Vector2f Position { get; set; }
        public CellType CellType { get; set; }
        Sprite cellSprite = new Sprite();

        public Cell(float x, float y, float length, CellType type)
        {
            Position = new Vector2f(x, y);
            ChangeType(type); // Вызываем метод ChangeType с корректным значением cellType
          
            cellSprite = TextSpriteCreator.SpriteCreate(cellSprite.Texture, x, y);
            cellSprite.Scale = new Vector2f(length / cellSprite.Texture.Size.X, length / cellSprite.Texture.Size.Y);
        }

        public void ChangeType(CellType newCellType, bool changeTexture = true)
        {
            CellType = newCellType; // Установка нового типа клетки
            if (changeTexture)
            {
                switch (newCellType)
                {
                    case CellType.Water:
                        cellSprite.Texture = new Texture("Images/water.jpg");
                        break;
                    case CellType.DigitOrLetter:
                        cellSprite.Texture = new Texture("Images/letter.jpg");
                        break;
                    case CellType.Ship:
                        cellSprite.Texture = new Texture("Images/ship.jpg"); // Примерный путь к изображению корабля
                        break;
                }
            }
        }

        public void Draw(RenderWindow window)
        {
            window.Draw(cellSprite);
        }

    }
}
