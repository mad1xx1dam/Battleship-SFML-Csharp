using SFML.Graphics;
using SFML.System;

namespace Battleship
{
    internal class TextSpriteCreator
    {
        public static Text TextCreate(string stringText, Font font, uint size, Text.Styles style, float posX, float posY)
        {
            Text text = new Text(stringText, font, size);
            text.FillColor = Color.Black;
            text.Style = style;
            FloatRect textRect = text.GetLocalBounds(); //как бы устанавливает прямоугольное поле, в рамках которого находится текст
            text.Origin = new Vector2f(textRect.Width / 2.0f, textRect.Height / 2.0f); //текст центруется относительно этого поля
            text.Position = new Vector2f(posX, posY);

            return text;
        }

        public static Sprite SpriteCreate(Texture texture, float posX, float posY)
        {
            Sprite sprite = new Sprite(texture);
            sprite.Position = new Vector2f(posX, posY);
            sprite.Origin = new Vector2f(texture.Size.X / 2.0f, texture.Size.Y / 2.0f);

            return sprite;
        }

        public static void ResetText(ref Text text, Vector2f newPosition, string newTitle)
        {
            text.DisplayedString = newTitle;
            FloatRect textRect = text.GetLocalBounds(); //как бы устанавливает прямоугольное поле, в рамках которого находится текст
            text.Origin = new Vector2f(textRect.Width / 2.0f, textRect.Height / 2.0f); //текст центруется относительно этого поля
            text.Position = newPosition;
        }
    }
}
