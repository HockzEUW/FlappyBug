using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace FlappyBug
{
    public class Game1 : Game
    {
        public GraphicsDeviceManager Graphics;
        public SpriteBatch SpriteBatch;
        public Dictionary<string, Texture2D> Textures { get; set; }
        private List<Vector2> _buisPosities = new();
        private Texture2D _bugpixel;
        private Texture2D _motherboardBackground;
        private Texture2D _buis;
        private Vector2 achtergrondPositie = Vector2.Zero;        
        private Vector2 achtergrond2Positie = new Vector2(1792, 0);
        private const int bewegingsSnelheid = 5;
        private Vector2 bugpixelPositie = Vector2.Zero;
        private float verticalVelocity = 0f;  // Snelheid in de verticale richting
        private const float gravity = 0.45f;  // Zwaartekracht kracht
        private const float springHoogte = -8f;  // De kracht van het springen
        private bool isSpatiebarIngedrukt = false;
        private TimeSpan _momentLaatsteBuisAangemaakt;
        private readonly int uitersteYPositieWaarde;

        private int score = 0;
        private bool _scoreUpdated = false;

        public SpriteFont Font {get; private set;}

        public Game1()
        {
            Graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            uitersteYPositieWaarde = Graphics.PreferredBackBufferHeight / 4;
        }

        protected override void Initialize()
        {
            Graphics.PreferredBackBufferWidth = 1280;
            Graphics.PreferredBackBufferHeight = 720;
            Graphics.ApplyChanges();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            _bugpixel = Content.Load<Texture2D>("BugPixel");
            _motherboardBackground = Content.Load<Texture2D>("MotherboardBackground");
            _buis = Content.Load<Texture2D>("BuisTogether");
            Font = Content.Load<SpriteFont>("File");
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (bugpixelPositie == Vector2.Zero)
            {
                bugpixelPositie = new Vector2(Graphics.PreferredBackBufferWidth / 4, Graphics.PreferredBackBufferHeight / 2 - _bugpixel.Height);
            }

            achtergrondPositie.X -= bewegingsSnelheid;

            achtergrond2Positie.X -= bewegingsSnelheid;

            if(achtergrondPositie.X < -_motherboardBackground.Width ){
                achtergrondPositie.X = _motherboardBackground.Width;
            }

            if(achtergrond2Positie.X < -_motherboardBackground.Width ){
                achtergrond2Positie.X = _motherboardBackground.Width;
            }
            
            // Controleer of we springen als de spatiebalk ingedrukt is
            if (Keyboard.GetState().IsKeyDown(Keys.Space) && !isSpatiebarIngedrukt && (bugpixelPositie.Y > _bugpixel.Height))
            {
                // Als de bugpixel op de grond is, begin met springen
                if (bugpixelPositie.Y >= Graphics.PreferredBackBufferHeight - _bugpixel.Height)
                {
                    verticalVelocity = springHoogte; // Zet de verticale snelheid naar de negatieve waarde van jumpStrength
                }
                else
                {
                    // Als we niet op de grond zijn, blijven we in de lucht en blijven we 'springkracht' toevoegen zolang de spatie ingedrukt blijft
                    verticalVelocity = springHoogte; // Blijf springen zolang de toets ingedrukt wordt
                }
                isSpatiebarIngedrukt = true;
            }

            if(Keyboard.GetState().IsKeyUp(Keys.Space) && isSpatiebarIngedrukt){
                isSpatiebarIngedrukt = false;
            }

            // Pas de zwaartekracht toe, dit gebeurt alleen als we niet op de grond zijn
            if (bugpixelPositie.Y < Graphics.PreferredBackBufferHeight - _bugpixel.Height)
            {
                verticalVelocity += gravity;
            }

            // Verplaats de bugpixel op basis van de verticale snelheid
            bugpixelPositie.Y += verticalVelocity;

            // Zorg ervoor dat de bugpixel niet door de onderkant van het scherm valt
            if (bugpixelPositie.Y > Graphics.PreferredBackBufferHeight - _bugpixel.Height)
            {
                bugpixelPositie.Y = Graphics.PreferredBackBufferHeight - _bugpixel.Height;
                verticalVelocity = 0f;  // Stop met vallen als de bugpixel de onderkant bereikt
            }

            buizenController(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            SpriteBatch.Begin();
            SpriteBatch.Draw(_motherboardBackground, achtergrond2Positie, Color.White);
            SpriteBatch.Draw(_motherboardBackground, achtergrondPositie, Color.White);
            SpriteBatch.Draw(_bugpixel, bugpixelPositie, Color.White);

            foreach(var buisPositie in _buisPosities){
                SpriteBatch.Draw(_buis, buisPositie, Color.White);
            }
            SpriteBatch.DrawString(Font, score.ToString(), Vector2.Zero, Color.Red);
            SpriteBatch.End();
            base.Draw(gameTime);
        }


        private void buizenController(GameTime gt){
            if((gt.TotalGameTime - _momentLaatsteBuisAangemaakt).TotalSeconds > 1.5){
                maakBuizen(gt);
            }
            verplaatsBuizen();
            verwerkBotsing();
            verwerkScore();
            _buisPosities.RemoveAll(b => b.X < -_buis.Width);
        }

        private void maakBuizen(GameTime gt){
            Random rnd = new();
            Vector2 nieuweBuisPositie = new Vector2(
                Graphics.PreferredBackBufferWidth,
                (-_buis.Height / 2) + (Graphics.PreferredBackBufferHeight / 2) + rnd.Next(-uitersteYPositieWaarde, uitersteYPositieWaarde) //buis komt eerst midden op het scherm en verplaatst zich dan naar boven of onder
            );
            _buisPosities.Add(nieuweBuisPositie);
            _momentLaatsteBuisAangemaakt = gt.TotalGameTime;
        }
        private void verplaatsBuizen(){
            for(int i = 0; i < _buisPosities.Count; i++){
                _buisPosities[i] = _buisPosities[i] with {X = _buisPosities[i].X - bewegingsSnelheid};
            }
        }
        private void verwerkBotsing(){
            Rectangle bugRechthoek = new(
                (int)bugpixelPositie.X, (int) bugpixelPositie.Y,
                _bugpixel.Width, _bugpixel.Height
            );

            for(int i = 0; i < _buisPosities.Count; i++){
                Rectangle buisRechthoekBovenkant = new(
                    (int)_buisPosities[i].X, (int)_buisPosities[i].Y,
                    _buis.Width, _buis.Height / 2 - 90
                );
                Rectangle buisRechthoekOnderkant = new(
                    (int)_buisPosities[i].X, (int)_buisPosities[i].Y + _buis.Height / 2 + 90,
                    _buis.Width, _buis.Height
                ); 
                if(bugRechthoek.Intersects(buisRechthoekBovenkant) || bugRechthoek.Intersects(buisRechthoekOnderkant)){
                    Exit();
                }
            }
        }
        private void verwerkScore(){
            if (_buisPosities.Count > 0)
            {
                if (bugpixelPositie.X > _buisPosities[0].X + _buis.Width && !_scoreUpdated)
                {
                    score += 1; // Increment score by 1
                    _scoreUpdated = true; // Set the flag to true to prevent multiple increments
                }

                // Reset the flag when the bug is no longer past the pipe
                if (bugpixelPositie.X <= _buisPosities[0].X + _buis.Width)
                {
                    _scoreUpdated = false;
                }
            }
        }
    }
}
