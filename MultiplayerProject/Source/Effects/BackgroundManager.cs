﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MultiplayerProject.Source
{
    class BackgroundManager
    {
        // Image used to display the static background
        private Texture2D _mainBackground;
        private Rectangle _rectBackground;

        private ParallaxingBackground _bgLayer1;
        private ParallaxingBackground _bgLayer2;

        public BackgroundManager()
        {
        }

        public void Initalise(ContentManager content)
        {
            _bgLayer1 = new ParallaxingBackground();
            _bgLayer2 = new ParallaxingBackground();
            _rectBackground = new Rectangle(0, 0, Application.WINDOW_WIDTH, Application.WINDOW_HEIGHT);

            _bgLayer1.Initialize(content, "bgLayer1", Application.WINDOW_WIDTH, Application.WINDOW_HEIGHT, -1);
            _bgLayer2.Initialize(content, "bgLayer2", Application.WINDOW_WIDTH, Application.WINDOW_HEIGHT, -2);
            _mainBackground = content.Load<Texture2D>("mainbackground");
        }

        public void Update(GameTime gameTime)
        {
            // Update the parallaxing background
            _bgLayer1.Update(gameTime);
            _bgLayer2.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            //Draw the Main Background Texture
            spriteBatch.Draw(_mainBackground, _rectBackground, Color.White);

            // Draw the moving background
            _bgLayer1.Draw(spriteBatch);
            _bgLayer2.Draw(spriteBatch);
        }
    }
}
