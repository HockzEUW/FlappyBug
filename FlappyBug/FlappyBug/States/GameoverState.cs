using FlappyBug;
using FlappyBug.States;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

public class GameoverState(FlappyBugGame spel, int score) : AbstractState(spel)
{
    private bool _isMuziekActief = false;
    public override void Draw(GameTime gameTime)
    {
        spel.SpriteBatch.Draw(spel.Textures["BugDead"], Vector2.Zero, Color.White);
        spel.SpriteBatch.DrawString(spel.Font, score.ToString(), new Vector2(825, 333), Color.White); //current score
        spel.SpriteBatch.DrawString(spel.Font, spel.HighScore.ToString(), new Vector2(825, 388), Color.White); //highscore
    }

    public override void Update(GameTime gameTime)
    {
        if (!_isMuziekActief)
        {
            MediaPlayer.Play(spel.Songs["GameoverSong"]);
            _isMuziekActief = true;
        }
        if (Keyboard.GetState().IsKeyDown(Keys.R))
        {
            spel.ChangeState(new PlayingState(spel));
        }
    }
}
