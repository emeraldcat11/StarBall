using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;

namespace StarBall
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class StarBall : Game
    {
        Random random = new Random();
        Texture2D ballTexture;
        Texture2D starTexture;
        Vector2 ballPosition;
        Vector2 starPosition;
        float ballSpeed;
        int deadZone;
        string gameTitle;
        int score;
        int timeLeft = 30;

        GraphicsDeviceManager _graphics;
        SpriteBatch _spriteBatch;
        RenderTarget2D _renderCanvas;
        Rectangle _renderDestination = Rectangle.Empty;

        bool _isResizing;
        bool initialSizing;

        int _canvasFixedWidth = 960;
        int _canvasFixedHeight = 540;

        public StarBall()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            Window.AllowUserResizing = true;
            Window.ClientSizeChanged += OnClientSizeChanged;
        }

        private void OnClientSizeChanged(object sender, EventArgs e)
        {
            if (!_isResizing && Window.ClientBounds.Width > 0 && Window.ClientBounds.Height > 0)
            {
                _isResizing = true;
                CalcRenderDestination();
                _isResizing = false;
            }
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            ballPosition = new Vector2(_canvasFixedWidth / 2, _canvasFixedHeight / 2);
            ballSpeed = 200f;
            //deadZone = 4096;
            float starX = random.Next(0, _canvasFixedWidth);
            float starY = random.Next(0, _canvasFixedHeight);
            starPosition = new Vector2(starX, starY);
            score = 0;
            gameTitle = "StarBall";
            Window.Title = $"{gameTitle} - Score: {score}";


            base.Initialize();

        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            // Initialization
            _renderCanvas = new RenderTarget2D(GraphicsDevice, _canvasFixedWidth, _canvasFixedHeight);

            // TODO: Use this.Content to load your game content here
            ballTexture = Content.Load<Texture2D>("ball");
            starTexture = Content.Load<Texture2D>("star");

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        private void CalcRenderDestination()
        {
            Point size = GraphicsDevice.Viewport.Bounds.Size;

            float scaleX = (float)size.X / _renderCanvas.Width;
            float scaleY = (float)size.Y / _renderCanvas.Height;
            float scale = Math.Min(scaleX, scaleY);

            _renderDestination.Width = (int)(_renderCanvas.Width * scale);
            _renderDestination.Height = (int)(_renderCanvas.Height * scale);

            _renderDestination.X = (size.X - _renderDestination.Width) / 2;
            _renderDestination.Y = (size.Y - _renderDestination.Height) / 2;
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            timeLeft = 30 - gameTime.TotalGameTime.Seconds;
            if (timeLeft < 0)
                timeLeft = 0;
            Window.Title = $"{gameTitle} - Score: {score} - Time Left: {timeLeft}";
            if (timeLeft <= 0)
            {
                base.Update(gameTime);
                return;
            }


            MouseState mouseState = Mouse.GetState();
            KeyboardState keyboardState = Keyboard.GetState();
            GamePadState gamePadState = default;
            try { gamePadState = GamePad.GetState(PlayerIndex.One); }
            catch (NotImplementedException) { /* ignore gamePadState */ }

            if (keyboardState.IsKeyDown(Keys.Escape) ||
                keyboardState.IsKeyDown(Keys.Back) ||
                gamePadState.Buttons.Back == ButtonState.Pressed)
            {
                try { Exit(); }
                catch (PlatformNotSupportedException) { /* ignore */ }
            }


            // TODO: Add your update logic here
            if (keyboardState.IsKeyDown(Keys.Up))
            {
                ballPosition.Y -= ballSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            if (keyboardState.IsKeyDown(Keys.Down))
            {
                ballPosition.Y += ballSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            if (keyboardState.IsKeyDown(Keys.Left))
            {
                ballPosition.X -= ballSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            if (keyboardState.IsKeyDown(Keys.Right))
            {
                ballPosition.X += ballSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }


            if (ballPosition.X > _canvasFixedWidth - ballTexture.Width / 2)
            {
                ballPosition.X = _canvasFixedWidth - ballTexture.Width / 2;
            }
            else if (ballPosition.X < ballTexture.Width / 2)
            {
                ballPosition.X = ballTexture.Width / 2;
            }

            if (ballPosition.Y > _canvasFixedHeight - ballTexture.Height / 2)
            {
                ballPosition.Y = _canvasFixedHeight - ballTexture.Height / 2;
            }
            else if (ballPosition.Y < ballTexture.Height / 2)
            {
                ballPosition.Y = ballTexture.Height / 2;
            }

            var ballRect = new Rectangle((int)ballPosition.X, (int)ballPosition.Y, ballTexture.Width, ballTexture.Height);
            var starRect = new Rectangle((int)starPosition.X, (int)starPosition.Y, starTexture.Width, starTexture.Height);
            if (ballRect.Intersects(starRect))
            {
                float newStarX = random.Next(0, _canvasFixedWidth);
                float newStarY = random.Next(0, _canvasFixedHeight);
                starPosition = new Vector2(newStarX, newStarY);
                score += 10;
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {

            if (timeLeft <= 0)
            {
                base.Draw(gameTime);
                return;
            }

            // Draw graphics on the fixed-size canvas we created
            GraphicsDevice.SetRenderTarget(_renderCanvas);
            GraphicsDevice.Clear(Color.CornflowerBlue);
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            _spriteBatch.Draw(
                starTexture,
                starPosition,
                null,
                Color.White,
                0f,
                new Vector2(starTexture.Width / 2, starTexture.Height / 2),
                Vector2.One,
                SpriteEffects.None,
                0f
            );
            _spriteBatch.Draw(
                ballTexture,
                ballPosition,
                null,
                Color.White,
                0f,
                new Vector2(ballTexture.Width / 2, ballTexture.Height / 2),
                Vector2.One,
                SpriteEffects.None,
                0f
            );
            _spriteBatch.End();

            // Set graphics target back to the full viewscreen, and add black bars
            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(Color.Black);
            if (!initialSizing)
            {
                initialSizing = true;
                CalcRenderDestination();
            }

            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            // RenderTarget2D inherits from Texture2D so we can draw _renderCanvas just like a texture
            _spriteBatch.Draw(_renderCanvas, _renderDestination, Color.White);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
