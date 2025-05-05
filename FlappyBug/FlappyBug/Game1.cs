using System.Collections.Generic;
using FlappyBug.States;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FlappyBug
{
    public class Game1 : Game
    {
        public GraphicsDeviceManager Graphics { get; private set; }
        public SpriteBatch SpriteBatch { get; private set; }
        public SpriteFont Font { get; private set; }
        public Dictionary<string, Texture2D> Textures { get; set; }
        private AbstractState _currentState;
        public int HighScore { get; set; } = 0;

        public Game1()
        {
            Graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            _currentState = new StartscreenState(this);
        }

        protected override void Initialize()
        {
            Graphics.PreferredBackBufferWidth = 1280;
            Graphics.PreferredBackBufferHeight = 720;
            Graphics.ApplyChanges();

            base.Initialize();
        }

        public void ChangeState(AbstractState newState)
        {
            _currentState = newState;
        }

        protected override void LoadContent()
        {
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            Font = Content.Load<SpriteFont>("FlappyFont");
            Textures = new() {
                { "StartBackground", Content.Load<Texture2D>("StartBackground") },
                { "BugPixel", Content.Load<Texture2D>("BugPixel") },
                { "MotherboardBackground", Content.Load<Texture2D>("MotherboardBackground") },
                { "BuisTogether", Content.Load<Texture2D>("BuisTogether") },
                { "BugDead", Content.Load<Texture2D>("BugDead") }
            };
        }

        protected override void Update(GameTime gameTime)
        {
            _currentState.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            SpriteBatch.Begin();
            _currentState.Draw(gameTime);
            SpriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
