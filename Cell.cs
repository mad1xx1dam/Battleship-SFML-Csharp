using SFML.Graphics;
using SFML.System;

namespace Battleship
{
    internal class Cell
    {
        Sprite cellSprite = new Sprite();
        float cellLength;
        public Vector2f Position { get; set; }
        public CellType CellType { get; set; }

        public Sprite CellSprite
        {
            get { return cellSprite; }
            set { cellSprite = value; }
        }

        public Cell(float x, float y, float length, CellType type)
        {
            cellLength = length;
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
                        TextSpriteCreator.ResetSprite(ref cellSprite, new Texture("Images/water.jpg"), cellLength);
                        break;
                    case CellType.DigitOrLetter:
                        TextSpriteCreator.ResetSprite(ref cellSprite, new Texture("Images/letter.jpg"), cellLength);
                        break;
                    case CellType.Ship:
                        TextSpriteCreator.ResetSprite(ref cellSprite, new Texture("Images/ship.jpg"), cellLength);
                        break;
                    case CellType.ShipBroken:
                        TextSpriteCreator.ResetSprite(ref cellSprite, new Texture("Images/shipBroken.jpg"), cellLength);
                        break;
                    case CellType.Miss:
                        TextSpriteCreator.ResetSprite(ref cellSprite, new Texture("Images/miss.jpg"), cellLength);
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
