using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace game_0;

public abstract class Player
{
    /// The position of the sprite's center on screen.
    public Vector2 Position { get; set; }

    /// When true the player is drawn as their dead variant.
    public bool IsDead { get; set; }

    public abstract void Update(GameTime gameTime);

    public abstract Choice? MakeChoice();

    public abstract void Draw(GameTime gameTime, SpriteBatch spriteBatch);

    // public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    // {
    //     SpriteEffects spriteEffects = (flipped) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
    //     spriteBatch.Draw(
    //         texture,
    //         position,
    //         null,
    //         Color.White,
    //         .33f,
    //         new Vector2(64, 64),
    //         .5f,
    //         spriteEffects,
    //         0);
    // }
}