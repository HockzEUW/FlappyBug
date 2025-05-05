using FlappyBug;
using FlappyBug.States;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

public class PausedState : AbstractState
{

    private PlayingState _previousState;
    public PausedState(Game1 spel, PlayingState playingState) : base(spel)
    {
        _previousState = playingState;
    }

    public override void Draw(GameTime gameTime)
    {
        spel.SpriteBatch.Draw(spel.Textures["StartBackground"], Vector2.Zero, Color.White);
        spel.SpriteBatch.DrawString(spel.Font, "PAUSED", new Vector2(spel.Graphics.PreferredBackBufferWidth / 3, spel.Graphics.PreferredBackBufferHeight / 2), Color.DarkOrange);
        spel.SpriteBatch.DrawString(spel.Font, "PRESS ENTER BUTTON TO CONTINUE", new Vector2(spel.Graphics.PreferredBackBufferWidth / 3, (spel.Graphics.PreferredBackBufferHeight / 2) + 50), Color.DarkOrange);
    }

    public override void Update(GameTime gameTime)
    {
        if (Keyboard.GetState().IsKeyDown(Keys.Enter))
        {
            spel.ChangeState(_previousState);
        }
    }
}