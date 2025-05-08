using Microsoft.Xna.Framework;

namespace FlappyBug.States;

public abstract class AbstractState(FlappyBugGame game)
{
    protected FlappyBugGame spel = game;
    public abstract void Update(GameTime gameTime);
    public abstract void Draw(GameTime gameTime);
}