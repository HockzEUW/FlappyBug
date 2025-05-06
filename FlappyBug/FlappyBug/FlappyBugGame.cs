using System.Collections.Generic;
using FlappyBug.States;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace FlappyBug
{
    public class FlappyBugGame : Game
    {
        public GraphicsDeviceManager Graphics { get; private set; }
        public SpriteBatch SpriteBatch { get; private set; }
        public SpriteFont Font { get; private set; }
        public SoundEffect ErrorSound { get; private set; }
        public Dictionary<string, Texture2D> Textures { get; set; }
        public Dictionary<string, Song> Songs { get; set; }
        public int HighScore { get; set; } = 0;
        private AbstractState _currentState;

        public FlappyBugGame()
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
            ErrorSound = Content.Load<SoundEffect>("Errorsound");
            Songs = new() {
                { "GameoverSong", Content.Load<Song>("GameoverSong") },
                { "PlayingSong", Content.Load<Song>("PlayingSong") },
                { "StartSong", Content.Load<Song>("StartSong") },
            };
            Textures = new() {
                { "StartBackground", Content.Load<Texture2D>("StartBackground") },
                { "BugPixel", Content.Load<Texture2D>("BugPixel") },
                { "MotherboardBackground", Content.Load<Texture2D>("MotherboardBackground") },
                { "RamstickTogether", Content.Load<Texture2D>("RamstickTogether") },
                { "BugDead", Content.Load<Texture2D>("BugDead") }
            };
        }

        protected override void Update(GameTime gameTime)
        {
            _currentState.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            SpriteBatch.Begin();
            _currentState.Draw(gameTime);
            SpriteBatch.End();
        }
    }
}
