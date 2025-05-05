using FlappyBug;
using FlappyBug.States;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

public class GameoverState(FlappyBugGame spel) : AbstractState(spel)
{
    public override void Draw(GameTime gameTime)
    {
        spel.SpriteBatch.Draw(spel.Textures["BugDead"], Vector2.Zero, Color.White);
        spel.SpriteBatch.DrawString(spel.Font, spel.HighScore.ToString(), new Vector2(spel.Graphics.PreferredBackBufferWidth / 2, 380), Color.White); //highscore
    }

    public override void Update(GameTime gameTime)
    {
        if (Keyboard.GetState().IsKeyDown(Keys.R))
        {
            spel.ChangeState(new PlayingState(spel));
        }
    }
}
