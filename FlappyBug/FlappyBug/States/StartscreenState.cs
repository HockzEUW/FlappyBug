using FlappyBug;
using FlappyBug.States;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

public class StartscreenState(FlappyBugGame spel) : AbstractState(spel)
{
    private float _draaihoek;
    private Vector2 _spriteBegin;
    private bool _isMuziekActief = false;

    public override void Draw(GameTime gameTime)
    {
        spel.GraphicsDevice.Clear(Color.Black);
        spel.SpriteBatch.Draw(spel.Textures["StartBackground"], Vector2.Zero, Color.White);
        spel.SpriteBatch.Draw(spel.Textures["BugPixel"], new Vector2(spel.Graphics.PreferredBackBufferWidth / 3 * 2, spel.Graphics.PreferredBackBufferHeight / 6), null, Color.White, _draaihoek, _spriteBegin, 1.0f, SpriteEffects.None, 0f); ;
        spel.SpriteBatch.DrawString(spel.Font, spel.HighScore.ToString(), new Vector2(792, 510), Color.White);
    }

    public override void Update(GameTime gameTime)
    {
        if(!_isMuziekActief)
        {
            MediaPlayer.Play(spel.Songs["StartSong"]);
            _isMuziekActief = true;
        }

        if (Keyboard.GetState().IsKeyDown(Keys.Enter))
        {
            spel.ChangeState(new PlayingState(spel));
            MediaPlayer.Stop();
        }

        DraaiBugPixel(gameTime);
    }

    private void DraaiBugPixel(GameTime gameTime)
    {
        float verstreken = (float)gameTime.ElapsedGameTime.TotalSeconds;
        float cirkel = MathHelper.Pi * 2;

        _draaihoek += verstreken;
        _draaihoek %= cirkel;
        _spriteBegin.X = spel.Textures["BugPixel"].Width / 2;
        _spriteBegin.Y = spel.Textures["BugPixel"].Height / 2;

    }
}