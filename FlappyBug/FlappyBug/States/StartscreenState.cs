using FlappyBug;
using FlappyBug.States;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

public class StartscreenState(Game1 spel) : AbstractState(spel)
{
    private float rotationAngle;
    private Vector2 spriteOrigin;

    public override void Draw(GameTime gameTime)
    {
        spel.GraphicsDevice.Clear(Color.Black);
        spel.SpriteBatch.Draw(spel.Textures["StartBackground"], Vector2.Zero, Color.White);
        spel.SpriteBatch.Draw(spel.Textures["BugPixel"], new Vector2(spel.Graphics.PreferredBackBufferWidth / 3 * 2, spel.Graphics.PreferredBackBufferHeight / 6), null, Color.White, rotationAngle, spriteOrigin, 1.0f, SpriteEffects.None, 0f); ;
        spel.SpriteBatch.DrawString(spel.Font, spel.HighScore.ToString(), new Vector2(792, 510), Color.White);
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