using Microsoft.Xna.Framework;

namespace FlappyBug.States;

public abstract class AbstractState(Game1 spel)
{
    protected Game1 spel = spel;
    public abstract void Update(GameTime gameTime);
    public abstract void Draw(GameTime gameTime);
}