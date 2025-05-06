using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace FlappyBug.States;

public class PlayingState : AbstractState
{
    private List<Vector2> _ramstickPosities = new();
    private readonly Texture2D _bugpixel, _motherboardBackground, _ramstick;
    private Vector2 _achtergrondPositie, _achtergrond2Positie, _bugpixelPositie;
    private const int _bewegingsSnelheid = 5;
    private float _verticaleSnelheid = 0f;  // Snelheid in de verticale richting
    private const float _zwaartekracht = 0.45f;  // Zwaartekracht kracht
    private const float _springHoogte = -8f;  // De kracht van het springen
    private bool _isSpatiebarIngedrukt = false;
    private TimeSpan _momentLaatsteRamstickAangemaakt;
    private TimeSpan _tijdVerstreken = TimeSpan.Zero; //lokale timer die alleen telt wanneer game niet gepauzeerd is
    private int _score = 0;
    private bool _scoreUpdated = false;
    private bool _isDood = false;
    private bool _isGeluidseffectActief = false;
    private TimeSpan _doodTimer = TimeSpan.Zero; // timer voor de valanimatie
    private bool _isSongPlaying = false;
    private readonly TimeSpan _doodAnimatieDuur = TimeSpan.FromSeconds(1); // duur van de valanimatie

    public PlayingState(FlappyBugGame spel) : base(spel)
    {
        _ramstick = spel.Textures["RamstickTogether"];
        _bugpixel = spel.Textures["BugPixel"];
        _motherboardBackground = spel.Textures["MotherboardBackground"];
        _achtergrond2Positie = new Vector2(_motherboardBackground.Width, 0);
        _bugpixelPositie = new Vector2(spel.Graphics.PreferredBackBufferWidth / 4, spel.Graphics.PreferredBackBufferHeight / 2 - _bugpixel.Height);
        _achtergrondPositie = Vector2.Zero;
    }

    public override void Draw(GameTime gameTime)
    {
        spel.GraphicsDevice.Clear(Color.Black);
        spel.SpriteBatch.Draw(_motherboardBackground, _achtergrond2Positie, Color.White);
        spel.SpriteBatch.Draw(_motherboardBackground, _achtergrondPositie, Color.White);


        foreach (var ramstickPositie in _ramstickPosities)
        {
            spel.SpriteBatch.Draw(_ramstick, ramstickPositie, Color.White);
        }
        spel.SpriteBatch.Draw(_bugpixel, _bugpixelPositie, Color.White);
        spel.SpriteBatch.DrawString(spel.Font, _score.ToString(), new Vector2(spel.Graphics.PreferredBackBufferWidth / 2, 50), Color.White);
    }

    public override void Update(GameTime gameTime)
    {
        if (_isDood) //sterf animatie
        {
            if(!_isGeluidseffectActief){
                spel.ErrorSound.Play();
                _isGeluidseffectActief = true;
            }
            _verticaleSnelheid += _zwaartekracht; // versnel de val door zwaartekracht
            _bugpixelPositie.Y += _verticaleSnelheid;

            // tel de tijd voor de valanimatie
            _doodTimer += gameTime.ElapsedGameTime;

            if (_doodTimer >= _doodAnimatieDuur)
            {
                spel.ChangeState(new GameoverState(spel));
            }

            return; // stop verdere updates als de speler dood is
        }

        if (Keyboard.GetState().IsKeyDown(Keys.Escape))
        {
            spel.ChangeState(new PausedState(spel, this)); // pauzeer
        }
        _tijdVerstreken += gameTime.ElapsedGameTime;


        if(!_isSongPlaying)
        {
            MediaPlayer.Play(spel.Songs["PlayingSong"]);
            _isSongPlaying = true;
        }
        IntroduceerZwaarteKracht(); // zwaartekracht en springen
        BeweegAchtergrond();
        BuizenController(gameTime);
    }

    public void BeweegAchtergrond()
    {
        _achtergrondPositie.X -= _bewegingsSnelheid;

        _achtergrond2Positie.X -= _bewegingsSnelheid;

        if (_achtergrondPositie.X < -_motherboardBackground.Width)
        {
            _achtergrondPositie.X = _motherboardBackground.Width;
        }

        if (_achtergrond2Positie.X < -_motherboardBackground.Width)
        {
            _achtergrond2Positie.X = _motherboardBackground.Width;
        }
    }
    private void BuizenController(GameTime gt)
    {

        MaakBuizen(gt);
        VerplaatsBuizen();
        VerwerkBotsing();
        VerwerkScore();
        _ramstickPosities.RemoveAll(b => b.X < -_ramstick.Width);
    }

    private void MaakBuizen(GameTime gt)
    {
        if ((_tijdVerstreken - _momentLaatsteRamstickAangemaakt).TotalSeconds > 1.5)
        {
            Random rnd = new();
            Vector2 nieuweRamstickPositie = new Vector2(
                spel.Graphics.PreferredBackBufferWidth,
                (-_ramstick.Height / 2) + (spel.Graphics.PreferredBackBufferHeight / 2) + rnd.Next(-spel.Graphics.PreferredBackBufferHeight / 4, spel.Graphics.PreferredBackBufferHeight / 4) //ramstick komt eerst midden op het scherm en verplaatst zich dan naar boven of onder
            );
            _ramstickPosities.Add(nieuweRamstickPositie);
            _momentLaatsteRamstickAangemaakt = _tijdVerstreken;
        }
    }
    private void VerplaatsBuizen()
    {
        for (int i = 0; i < _ramstickPosities.Count; i++)
        {
            _ramstickPosities[i] = _ramstickPosities[i] with { X = _ramstickPosities[i].X - _bewegingsSnelheid };
        }
    }
    private void VerwerkBotsing()
    {
        Rectangle bugRechthoek = new(
            (int)_bugpixelPositie.X, (int)_bugpixelPositie.Y,
            _bugpixel.Width, _bugpixel.Height
        );

        for (int i = 0; i < _ramstickPosities.Count; i++)
        {
            Rectangle ramstickRechthoekBovenkant = new(
                (int)_ramstickPosities[i].X, (int)_ramstickPosities[i].Y,
                _ramstick.Width, _ramstick.Height / 2 - 90
            );
            Rectangle ramstickRechthoekOnderkant = new(
                (int)_ramstickPosities[i].X, (int)_ramstickPosities[i].Y + _ramstick.Height / 2 + 90,
                _ramstick.Width, _ramstick.Height
            );
            if (bugRechthoek.Intersects(ramstickRechthoekBovenkant) || bugRechthoek.Intersects(ramstickRechthoekOnderkant))
            {
                if (_score > spel.HighScore)
                {
                    spel.HighScore = _score;
                }

                MediaPlayer.Stop();
                _isDood = true;
            }
        }
    }
    private void VerwerkScore()
    {
        if (_ramstickPosities.Count > 0)
        {
            if (_bugpixelPositie.X > _ramstickPosities[0].X + _ramstick.Width && !_scoreUpdated)
            {
                _score += 1;
                _scoreUpdated = true; // voorkomen dat score meerdere keren wordt verhoogd
            }

            // reset wanneer de bugpixel weer in de buurt van de ramstick komt
            if (_bugpixelPositie.X <= _ramstickPosities[0].X + _ramstick.Width)
            {
                _scoreUpdated = false;
            }
        }
    }

    public void IntroduceerZwaarteKracht()
    {
        // controleer of we springen als de spatiebalk ingedrukt is
        if (Keyboard.GetState().IsKeyDown(Keys.Space) && !_isSpatiebarIngedrukt && (_bugpixelPositie.Y > _bugpixel.Height))
        {
            // als de bugpixel op de grond is, begin met springen
            if (_bugpixelPositie.Y >= spel.Graphics.PreferredBackBufferHeight - _bugpixel.Height)
            {
                _verticaleSnelheid = _springHoogte; // Zet de verticale snelheid naar de negatieve waarde van jumpStrength
            }
            else
            {
                // als we niet op de grond zijn, springen we in de lucht en voegen we 'springkracht' toe 
                _verticaleSnelheid = _springHoogte; // blijf springen zolang de toets ingedrukt wordt
            }
            _isSpatiebarIngedrukt = true;
        }

        if (Keyboard.GetState().IsKeyUp(Keys.Space) && _isSpatiebarIngedrukt)
        {
            _isSpatiebarIngedrukt = false;
        }

        // pas de zwaartekracht toe, dit gebeurt alleen als we niet op de grond zijn
        if (_bugpixelPositie.Y < spel.Graphics.PreferredBackBufferHeight - _bugpixel.Height)
        {
            _verticaleSnelheid += _zwaartekracht;
        }

        // verplaats de bugpixel op basis van de verticale snelheid
        _bugpixelPositie.Y += _verticaleSnelheid;

        // zorg ervoor dat de bugpixel niet door de onderkant van het scherm valt
        if (_bugpixelPositie.Y > spel.Graphics.PreferredBackBufferHeight - _bugpixel.Height)
        {
            _bugpixelPositie.Y = spel.Graphics.PreferredBackBufferHeight - _bugpixel.Height;
            _verticaleSnelheid = 0f;  // stop met vallen als de bugpixel de onderkant bereikt
        }

    }
}