using FlappyBug;
using FlappyBug.States;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

public class GameoverState : AbstractState
{
    public GameoverState(Game1 spel) : base(spel)
    {
    }

    public override void Draw(GameTime gameTime)
    {
        spel.SpriteBatch.Draw(spel.Textures["BugDead"], Vector2.Zero, Color.White);       
    }

    public override void Update(GameTime gameTime)
    {
        if(Keyboard.GetState().IsKeyDown(Keys.S))
        {
            spel.ChangeState(new StartscreenState(spel));
        }
    }
}
