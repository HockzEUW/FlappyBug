using FlappyBug;
using FlappyBug.States;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

public class StartscreenState : AbstractState
{
    private Vector2 _startscreenPositie = Vector2.Zero;
    private float rotationAngle;
    private Vector2 spritePosition;
    private Vector2 spriteOrigin;

    public StartscreenState(Game1 spel) : base(spel)
    {

    }

    public override void Draw(GameTime gameTime)
    {
        spel.GraphicsDevice.Clear(Color.Black);
        spel.SpriteBatch.Draw(spel.Textures["BugPixel"], new Vector2(spel.Graphics.PreferredBackBufferWidth / 2, spel.Graphics.PreferredBackBufferHeight / 2), null, Color.White, rotationAngle, spriteOrigin, 1.0f, SpriteEffects.None, 0f); ;
        spel.SpriteBatch.DrawString(spel.Font, "FLAPPY BUG", new Vector2(spel.Graphics.PreferredBackBufferWidth / 3, spel.Graphics.PreferredBackBufferHeight / 2), Color.DarkOrange);
        spel.SpriteBatch.DrawString(spel.Font, "PRESS ENTER BUTTON", new Vector2(spel.Graphics.PreferredBackBufferWidth / 3, (spel.Graphics.PreferredBackBufferHeight / 2) + 50), Color.DarkOrange);
        spel.SpriteBatch.DrawString(spel.Font, "HIGHSCORE " + spel.HighScore.ToString(), new Vector2(spel.Graphics.PreferredBackBufferWidth / 3, (spel.Graphics.PreferredBackBufferHeight / 2) + 100), Color.DarkOrange);
    }

    public override void Update(GameTime gameTime)
    {
        if (Keyboard.GetState().IsKeyDown(Keys.Enter))
        {
            spel.ChangeState(new PlayingState(spel));
        }

        DraaiBugPixel(gameTime);
    }

    private void DraaiBugPixel(GameTime gameTime)
    {
        float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
        rotationAngle += elapsed;
        float circle = MathHelper.Pi * 2;
        rotationAngle %= circle;
        spriteOrigin.X = spel.Textures["BugPixel"].Width / 2;
        spriteOrigin.Y = spel.Textures["BugPixel"].Height / 2;

    }
}