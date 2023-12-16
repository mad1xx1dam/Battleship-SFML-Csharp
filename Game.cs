using Battleship;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

class Game
{
    private static Font font = new Font("Fonts/TNR.ttf");
    RenderWindow window;

    //список для хранения границ отрисованных кнопок
    List<FloatRect> buttonBounds;
    //списки кнопок/спрайтов
    Sprite[] menuSprites;
    Sprite[] settingsSprites;
    List<Sprite> gameSprites;

    Vector2i mousePos;

    Texture backgroundTexture = new Texture("Images/background.jpg");
    Texture shipTexture = new Texture("Images/shipStart.png");
    Texture startTexture = new Texture("Images/play.png");
    Texture settingsTexture = new Texture("Images/settings.png");
    Texture exitTexture = new Texture("Images/exit.png");
    Texture easyTexture = new Texture("Images/easy.png");
    Texture hardTexture = new Texture("Images/hard.png");
    Texture okTexture = new Texture("Images/ok.png");
    Texture returnMenuTexture = new Texture("Images/returnMenu.png");
    Texture randomShipsTexture = new Texture("Images/randomShips.png");

    private Text textMain;
    private Text textAdvice;

    Sprite background;
    Sprite ship;
    Sprite start;
    Sprite settings;
    Sprite exit;
    Sprite easy;
    Sprite hard;
    Sprite ok;
    Sprite returnMenu;
    Sprite randomShips;

    Player player;
    Player bot;

    GameState gameState = GameState.Start;
    GameMode gameMode = GameMode.Easy;

    private float temp = -1f;

    public Game(uint width, uint height, string title, Styles style)
    {
        //настройки окна
        this.window = new RenderWindow(new VideoMode(width, height), title, style);
        window.SetVerticalSyncEnabled(true);

        //главная надпись при запуске приложения
        textMain = TextSpriteCreator.TextCreate("Морской бой".ToUpper(), font, 80, Text.Styles.Bold, window.Size.X / 2.0f, window.Size.Y / 7.5f);
        //надпись-подсказка при запуске приложения
        textAdvice = TextSpriteCreator.TextCreate("Нажмите Enter, чтобы продолжить...".ToUpper(), font, 25, Text.Styles.Regular, window.Size.X / 2.0f, window.Size.Y / 3.5f);
        //спрайт для отрисовки картинки при запуске приложения
        ship = TextSpriteCreator.SpriteCreate(shipTexture, window.Size.X / 2, window.Size.Y / 1.45f);
        //задний фон
        background = TextSpriteCreator.SpriteCreate(backgroundTexture, backgroundTexture.Size.X / 2, backgroundTexture.Size.Y / 2);
        //кнопки меню
        start = TextSpriteCreator.SpriteCreate(startTexture, window.Size.X / 2, window.Size.Y / 4f);
        settings = TextSpriteCreator.SpriteCreate(settingsTexture, window.Size.X / 2, window.Size.Y / 2f);
        exit = TextSpriteCreator.SpriteCreate(exitTexture, window.Size.X / 2, window.Size.Y / 4f * 3);
        //кнопки сложности
        easy = TextSpriteCreator.SpriteCreate(easyTexture, window.Size.X / 2, window.Size.Y / 7 * 3);
        hard = TextSpriteCreator.SpriteCreate(hardTexture, window.Size.X / 2, window.Size.Y / 7 * 4);
        ok = TextSpriteCreator.SpriteCreate(okTexture, window.Size.X / 2 + hardTexture.Size.X / 1.5f, window.Size.Y / 7 * 3);
        //кнопка возврата в меню
        returnMenu = TextSpriteCreator.SpriteCreate(returnMenuTexture, window.Size.X - returnMenuTexture.Size.X / 2, window.Size.Y - returnMenuTexture.Size.Y / 2);
        //"игроки"
        player = new Player(window.Size.X / 6, window.Size.Y / 3, 30, "Игрок");
        bot = new Player(window.Size.X / 1.5f, window.Size.Y / 3, 30, "Компьютер");
        //кнопка генерации рандомной растановки кораблей
        randomShips = TextSpriteCreator.SpriteCreate(randomShipsTexture, window.Size.X / 5, window.Size.Y / 5);
        //для отслеживания позиций тех кнопок, которые никак не будут перемещаться
        buttonBounds = new List<FloatRect>();
        menuSprites = new Sprite[]  { start, settings, exit };
        settingsSprites = new Sprite[] { easy, hard, ok, returnMenu };
        gameSprites = new List<Sprite> { returnMenu, randomShips };

        foreach (Sprite sprite in menuSprites)
            buttonBounds.Add(sprite.GetGlobalBounds());
    }

    public void StartDraw()
    {
        temp += 0.007f;
        float alphaValue = (MathF.Sin(temp) + 1.0f) * 127.5f; // Используем синусоиду для плавного изменения прозрачности
        textAdvice.FillColor = new Color(0, 0, 0, (byte)alphaValue);  // Устанавливаем цвет текста с измененной прозрачностью

        window.Draw(textMain);
        window.Draw(textAdvice);// Отрисовываем текст с установленным цветом
        window.Draw(ship);

        if (Keyboard.IsKeyPressed(Keyboard.Key.Enter)) gameState = GameState.Menu;
    }

    public void MenuDraw()
    {
        foreach (Sprite sprite in menuSprites)
            window.Draw(sprite);
    }
    public void GameDraw()
    {
      

        foreach (Sprite sprite in gameSprites)
            window.Draw(sprite);

        player.Draw(window);
        bot.Draw(window);
    }
    public void SettingsDraw()
    {
        Text info = TextSpriteCreator.TextCreate("Выберите уровень сложности:".ToUpper(), font, 36, Text.Styles.Bold, window.Size.X / 2.0f, window.Size.Y / 3.5f);
        window.Draw(info);

        foreach (Sprite sprite in settingsSprites)
            window.Draw(sprite);
    }

    public void Run()
    {
        while (window.IsOpen)
        {
            window.Clear();
            window.Draw(background);
            window.DispatchEvents();

            if (Keyboard.IsKeyPressed(Keyboard.Key.Escape))
                window.Close();
            mousePos = Mouse.GetPosition(window);
            HandleMouseInput();
            switch (gameState)
            {
                case GameState.Start:
                    StartDraw();
                    break;
                case GameState.Menu:
                    MenuDraw();
                    break;
                case GameState.Game:
                    GameDraw();
                    break;
                case GameState.Settings:
                    SettingsDraw();
                    break;
            }
            window.Display();
        }
    } 
    //переменная добавлена, так как при нажатии лкм, обработчик успевает сработать несколько раз,
    //а этого нам точно не надо, так в таком случае, например, успевают отрисоваться другие вариации полей
    bool isMouseClicked = false;
    //вызывается каждый кадр
    public void HandleMouseInput()
    {
        if (Mouse.IsButtonPressed(Mouse.Button.Left))
        {
            if (!isMouseClicked) // проверка наличия предыдущего нажатия
            {
                isMouseClicked = true; // установка флага нажатия
                foreach (var button in buttonBounds)
                {
                    if (button.Contains(mousePos.X, mousePos.Y))
                    {
                        HandleButtonClick(button);
                        return;
                    }
                }
            }
        }
        else
        {
            isMouseClicked = false; // сброс флага нажатия
        }
    }

    private void HandleButtonClick(FloatRect button)
    {
        if (button == start.GetGlobalBounds())
        {
            bot.GenerateShips();
            foreach (Vector2i[] vector in player.shipPositions)
            {
                foreach (Vector2i vector2 in vector) Console.Write(vector2);
                Console.WriteLine();
            }
            gameState = GameState.Game;
            buttonBounds.Clear();
            foreach (Sprite sprite in gameSprites)
                buttonBounds.Add(sprite.GetGlobalBounds());
        }
        else if (button == settings.GetGlobalBounds())
        {
            gameState = GameState.Settings;
            buttonBounds.Clear();
            foreach (Sprite sprite in settingsSprites)
                buttonBounds.Add(sprite.GetGlobalBounds());
        }
        else if (button == exit.GetGlobalBounds())
        {
            window.Close();
        }
        else if (button == hard.GetGlobalBounds())
        {
            gameMode = GameMode.Hard;
            ok.Position = new Vector2f(hard.Position.X + hard.Texture.Size.X / 1.5f, hard.Position.Y);
        }
        else if (button == easy.GetGlobalBounds())
        {
            gameMode = GameMode.Easy;
            ok.Position = new Vector2f(easy.Position.X + easy.Texture.Size.X / 1.5f, easy.Position.Y);
        }
        else if (button == returnMenu.GetGlobalBounds())
        {
            //очистка игровых полей игрока и компьютера
            bot.ResetPlayGround();
            player.ResetPlayGround();
            gameState = GameState.Menu;
            buttonBounds.Clear();
            foreach (Sprite sprite in menuSprites)
                buttonBounds.Add(sprite.GetGlobalBounds());
        }
        else if (button == randomShips.GetGlobalBounds())
        {
            player.ResetPlayGround();
            player.GenerateShips();
        }
    }
}