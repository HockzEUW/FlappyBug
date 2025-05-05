using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace FlappyBug.States;

public class PlayingState : AbstractState
{
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

    private TimeSpan tijdVerstreken = TimeSpan.Zero; //lokale timer die alleen telt wanneer game niet gepauzeerd is

    private int score = 0;
    private bool _scoreUpdated = false;

    private bool _isGepauzeerd = false;

    public PlayingState(Game1 spel) : base(spel)
    {
        uitersteYPositieWaarde = spel.Graphics.PreferredBackBufferHeight / 4;
        _buis = spel.Textures["BuisTogether"];
        _bugpixel = spel.Textures["BugPixel"];
        _motherboardBackground = spel.Textures["MotherboardBackground"];
    }

    public override void Draw(GameTime gameTime)
    {
        spel.GraphicsDevice.Clear(Color.Black);
        spel.SpriteBatch.Draw(_motherboardBackground, achtergrond2Positie, Color.White);
        spel.SpriteBatch.Draw(_motherboardBackground, achtergrondPositie, Color.White);
        spel.SpriteBatch.Draw(_bugpixel, bugpixelPositie, Color.White);

        foreach (var buisPositie in _buisPosities)
        {
            spel.SpriteBatch.Draw(_buis, buisPositie, Color.White);
        }
        spel.SpriteBatch.DrawString(spel.Font, score.ToString(), Vector2.Zero, Color.White);
    }

    public override void Update(GameTime gameTime)
    {
        if (Keyboard.GetState().IsKeyDown(Keys.Escape))
        {
            spel.ChangeState(new PausedState(spel, this)); // pauzeer
        }
        tijdVerstreken += gameTime.ElapsedGameTime;

        if (bugpixelPositie == Vector2.Zero)
        {
            bugpixelPositie = new Vector2(spel.Graphics.PreferredBackBufferWidth / 4, spel.Graphics.PreferredBackBufferHeight / 2 - _bugpixel.Height);
        }

        achtergrondPositie.X -= bewegingsSnelheid;

        achtergrond2Positie.X -= bewegingsSnelheid;

        if (achtergrondPositie.X < -_motherboardBackground.Width)
        {
            achtergrondPositie.X = _motherboardBackground.Width;
        }

        if (achtergrond2Positie.X < -_motherboardBackground.Width)
        {
            achtergrond2Positie.X = _motherboardBackground.Width;
        }

        // Controleer of we springen als de spatiebalk ingedrukt is
        if (Keyboard.GetState().IsKeyDown(Keys.Space) && !isSpatiebarIngedrukt && (bugpixelPositie.Y > _bugpixel.Height))
        {
            // Als de bugpixel op de grond is, begin met springen
            if (bugpixelPositie.Y >= spel.Graphics.PreferredBackBufferHeight - _bugpixel.Height)
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

        if (Keyboard.GetState().IsKeyUp(Keys.Space) && isSpatiebarIngedrukt)
        {
            isSpatiebarIngedrukt = false;
        }

        // Pas de zwaartekracht toe, dit gebeurt alleen als we niet op de grond zijn
        if (bugpixelPositie.Y < spel.Graphics.PreferredBackBufferHeight - _bugpixel.Height)
        {
            verticalVelocity += gravity;
        }

        // Verplaats de bugpixel op basis van de verticale snelheid
        bugpixelPositie.Y += verticalVelocity;

        // Zorg ervoor dat de bugpixel niet door de onderkant van het scherm valt
        if (bugpixelPositie.Y > spel.Graphics.PreferredBackBufferHeight - _bugpixel.Height)
        {
            bugpixelPositie.Y = spel.Graphics.PreferredBackBufferHeight - _bugpixel.Height;
            verticalVelocity = 0f;  // Stop met vallen als de bugpixel de onderkant bereikt
        }

        BuizenController(gameTime);

    }
    

    private void BuizenController(GameTime gt)
    {
        if ((tijdVerstreken - _momentLaatsteBuisAangemaakt).TotalSeconds > 1.5)
        {
            MaakBuizen(gt);
        }
        VerplaatsBuizen();
        VerwerkBotsing();
        VerwerkScore();
        _buisPosities.RemoveAll(b => b.X < -_buis.Width);
    }

    private void MaakBuizen(GameTime gt)
    {
        Random rnd = new();
        Vector2 nieuweBuisPositie = new Vector2(
            spel.Graphics.PreferredBackBufferWidth,
            (-_buis.Height / 2) + (spel.Graphics.PreferredBackBufferHeight / 2) + rnd.Next(-uitersteYPositieWaarde, uitersteYPositieWaarde) //buis komt eerst midden op het scherm en verplaatst zich dan naar boven of onder
        );
        _buisPosities.Add(nieuweBuisPositie);
        _momentLaatsteBuisAangemaakt = tijdVerstreken;
    }
    private void VerplaatsBuizen()
    {
        for (int i = 0; i < _buisPosities.Count; i++)
        {
            _buisPosities[i] = _buisPosities[i] with { X = _buisPosities[i].X - bewegingsSnelheid };
        }
    }
    private void VerwerkBotsing()
    {

        Rectangle bugRechthoek = new(
            (int)bugpixelPositie.X, (int)bugpixelPositie.Y,
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
                if (score > spel.HighScore)
                {
                    spel.HighScore = score;
                }
                spel.ChangeState(new GameoverState(spel));
            }
        }
    }
    private void VerwerkScore()
    {
        if (_buisPosities.Count > 0)
        {
            if (bugpixelPositie.X > _buisPosities[0].X + _buis.Width && !_scoreUpdated)
            {
                score += 1;
                _scoreUpdated = true; // voorkomen dat score meerdere keren wordt verhoogd
            }

            // reset wanneer de bugpixel weer in de buurt van de buis komt
            if (bugpixelPositie.X <= _buisPosities[0].X + _buis.Width)
            {
                _scoreUpdated = false;
            }
        }
    }
}