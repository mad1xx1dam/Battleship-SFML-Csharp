using SFML.Graphics;
using SFML.System;

namespace Battleship
{
    internal class Cell
    {
        private static Texture waterTexture = new Texture("Images/water.jpg");
        private static Texture letterTexture = new Texture("Images/letter.jpg");
        private static Texture shipTexture = new Texture("Images/ship.jpg");
        private static Texture shipBrokenTexture = new Texture("Images/shipBroken.jpg");
        private static Texture missTexture = new Texture("Images/miss.jpg");

        float cellLength;
        Sprite cellSprite = new Sprite();
        Vector2f position;
        CellType cellType;
        public Vector2f Position {  get { return position; } set { position = value; } }
        public CellType CellType { get { return cellType; } set { cellType = value; } }
        public Sprite CellSprite { get { return cellSprite; } set { cellSprite = value; } }

        public Cell(float x, float y, float length, CellType type)
        {
            cellLength = length;
            Position = new Vector2f(x, y);
            ChangeType(type); 
            cellSprite = TextSpriteCreator.SpriteCreate(cellSprite.Texture, x, y);
            cellSprite.Scale = new Vector2f(length / cellSprite.Texture.Size.X, length / cellSprite.Texture.Size.Y);
        }
        public void ChangeType(CellType newCellType, bool changeTexture = true)
        {
            CellType = newCellType; 
            if (changeTexture)
            {
                switch (newCellType)
                {
                    case CellType.Water:
                        ResetSprite(waterTexture);
                        break;
                    case CellType.DigitOrLetter:
                        ResetSprite(letterTexture);
                        break;
                    case CellType.Ship:
                        ResetSprite(shipTexture);
                        break;
                    case CellType.ShipBroken:
                        ResetSprite(shipBrokenTexture);
                        break;
                    case CellType.Miss:
                        ResetSprite(missTexture);
                        break;
                }
            }
        }
        private void ResetSprite(Texture texture)
        {
            cellSprite.Texture = texture;
            cellSprite.Scale = new Vector2f(cellLength / texture.Size.X, cellLength / texture.Size.Y);
            cellSprite.Origin = new Vector2f(texture.Size.X / 2.0f, texture.Size.Y / 2.0f);
        }
        public void Draw(RenderWindow window)
        {
            window.Draw(cellSprite);
        }
    }
}
