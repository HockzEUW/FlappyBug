using Microsoft.Xna.Framework;

namespace FlappyBug.States;

public abstract class AbstractState {
    protected Game1 spel;
    
    protected AbstractState(Game1 spel) {
        this.spel = spel;
    }

    public abstract void Update(GameTime gameTime);
    public abstract void Draw(GameTime gameTime);
}