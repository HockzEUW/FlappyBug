using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace FlappyBug.States;

public class PlayingState : AbstractState
{
    private List<Vector2> _buisPosities = new();
    private readonly Texture2D _bugpixel, _motherboardBackground, _buis;
    private Vector2 _achtergrondPositie, _achtergrond2Positie, _bugpixelPositie;
    private const int _bewegingsSnelheid = 5;
    private float _verticalVelocity = 0f;  // Snelheid in de verticale richting
    private const float _gravity = 0.45f;  // Zwaartekracht kracht
    private const float _springHoogte = -8f;  // De kracht van het springen
    private bool _isSpatiebarIngedrukt = false;
    private TimeSpan _momentLaatsteBuisAangemaakt;
    private readonly int _uitersteYPositieWaarde;
    private TimeSpan _tijdVerstreken = TimeSpan.Zero; //lokale timer die alleen telt wanneer game niet gepauzeerd is
    private int _score = 0;
    private bool _scoreUpdated = false;
    private bool _isDood = false;
    private TimeSpan _doodTimer = TimeSpan.Zero; // timer voor de valanimatie
    private readonly TimeSpan _doodAnimatieDuur = TimeSpan.FromSeconds(1); // duur van de valanimatie

    public PlayingState(FlappyBugGame spel) : base(spel)
    {
        _uitersteYPositieWaarde = spel.Graphics.PreferredBackBufferHeight / 4;
        _buis = spel.Textures["BuisTogether"];
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


        foreach (var buisPositie in _buisPosities)
        {
            spel.SpriteBatch.Draw(_buis, buisPositie, Color.White);
        }
        spel.SpriteBatch.Draw(_bugpixel, _bugpixelPositie, Color.White);
        spel.SpriteBatch.DrawString(spel.Font, _score.ToString(), new Vector2(spel.Graphics.PreferredBackBufferWidth / 2, 50), Color.White);
    }

    public override void Update(GameTime gameTime)
    {
        if (_isDood) //sterf animatie
        {
            _verticalVelocity += _gravity; // versnel de val door zwaartekracht
            _bugpixelPositie.Y += _verticalVelocity;

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
        _buisPosities.RemoveAll(b => b.X < -_buis.Width);
    }

    private void MaakBuizen(GameTime gt)
    {
        if ((_tijdVerstreken - _momentLaatsteBuisAangemaakt).TotalSeconds > 1.5)
        {
            Random rnd = new();
            Vector2 nieuweBuisPositie = new Vector2(
                spel.Graphics.PreferredBackBufferWidth,
                (-_buis.Height / 2) + (spel.Graphics.PreferredBackBufferHeight / 2) + rnd.Next(-_uitersteYPositieWaarde, _uitersteYPositieWaarde) //buis komt eerst midden op het scherm en verplaatst zich dan naar boven of onder
            );
            _buisPosities.Add(nieuweBuisPositie);
            _momentLaatsteBuisAangemaakt = _tijdVerstreken;
        }
    }
    private void VerplaatsBuizen()
    {
        for (int i = 0; i < _buisPosities.Count; i++)
        {
            _buisPosities[i] = _buisPosities[i] with { X = _buisPosities[i].X - _bewegingsSnelheid };
        }
    }
    private void VerwerkBotsing()
    {
        Rectangle bugRechthoek = new(
            (int)_bugpixelPositie.X, (int)_bugpixelPositie.Y,
            _bugpixel.Width, _bugpixel.Height
        );

        for (int i = 0; i < _buisPosities.Count; i++)
        {
            Rectangle buisRechthoekBovenkant = new(
                (int)_buisPosities[i].X, (int)_buisPosities[i].Y,
                _buis.Width, _buis.Height / 2 - 90
            );
            Rectangle buisRechthoekOnderkant = new(
                (int)_buisPosities[i].X, (int)_buisPosities[i].Y + _buis.Height / 2 + 90,
                _buis.Width, _buis.Height
            );
            if (bugRechthoek.Intersects(buisRechthoekBovenkant) || bugRechthoek.Intersects(buisRechthoekOnderkant))
            {
                if (_score > spel.HighScore)
                {
                    spel.HighScore = _score;
                }

                // activeer de dood toestand
                _isDood = true;
                _verticalVelocity = 0f; // reset de verticale snelheid
                _doodTimer = TimeSpan.Zero; // reset de dood timer
            }
        }
    }
    private void VerwerkScore()
    {
        if (_buisPosities.Count > 0)
        {
            if (_bugpixelPositie.X > _buisPosities[0].X + _buis.Width && !_scoreUpdated)
            {
                _score += 1;
                _scoreUpdated = true; // voorkomen dat score meerdere keren wordt verhoogd
            }

            // reset wanneer de bugpixel weer in de buurt van de buis komt
            if (_bugpixelPositie.X <= _buisPosities[0].X + _buis.Width)
            {
                _scoreUpdated = false;
            }
        }
    }

    public void VoerSterfAnimatieUit()
    {

    }
    public void IntroduceerZwaarteKracht()
    {
        // controleer of we springen als de spatiebalk ingedrukt is
        if (Keyboard.GetState().IsKeyDown(Keys.Space) && !_isSpatiebarIngedrukt && (_bugpixelPositie.Y > _bugpixel.Height))
        {
            // als de bugpixel op de grond is, begin met springen
            if (_bugpixelPositie.Y >= spel.Graphics.PreferredBackBufferHeight - _bugpixel.Height)
            {
                _verticalVelocity = _springHoogte; // Zet de verticale snelheid naar de negatieve waarde van jumpStrength
            }
            else
            {
                // als we niet op de grond zijn, blijven we in de lucht en blijven we 'springkracht' toevoegen zolang de spatie ingedrukt blijft
                _verticalVelocity = _springHoogte; // blijf springen zolang de toets ingedrukt wordt
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
            _verticalVelocity += _gravity;
        }

        // verplaats de bugpixel op basis van de verticale snelheid
        _bugpixelPositie.Y += _verticalVelocity;

        // zorg ervoor dat de bugpixel niet door de onderkant van het scherm valt
        if (_bugpixelPositie.Y > spel.Graphics.PreferredBackBufferHeight - _bugpixel.Height)
        {
            _bugpixelPositie.Y = spel.Graphics.PreferredBackBufferHeight - _bugpixel.Height;
            _verticalVelocity = 0f;  // stop met vallen als de bugpixel de onderkant bereikt
        }

    }
}