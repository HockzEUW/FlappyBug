using FlappyBug;
using FlappyBug.States;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

public class PausedState(FlappyBugGame spel, PlayingState playingState) : AbstractState(spel)
{
    private Color _stringColor = new(0, 255, 255);
    private PlayingState _previousState = playingState;

    public override void Draw(GameTime gameTime)
    {
        _previousState.Draw(gameTime);
        spel.SpriteBatch.DrawString(spel.Font, "PAUSED", new Vector2(500, 300), _stringColor);
        spel.SpriteBatch.DrawString(spel.Font, "PRESS \'ENTER\' TO CONTINUE", new Vector2(250, 350), _stringColor);
    }

    public override void Update(GameTime gameTime)
    {
        if (Keyboard.GetState().IsKeyDown(Keys.Enter))
        {
            spel.ChangeState(_previousState);
        }
    }
}