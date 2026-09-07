using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace game_0;

public class DeterministicPlayer : Player
{
    private Texture2D _texture;

    private Texture2D _deadTexture;

    public void LoadContent(ContentManager content)
    {
        _texture = content.Load<Texture2D>("ComputerPlayer");
        _deadTexture = content.Load<Texture2D>("ComputerPlayerDead");
    }

    public override void Update(GameTime gameTime)
    {
        // Apply keyboard movement
        // if (keyboardState.IsKeyDown(Keys.Up) || keyboardState.IsKeyDown(Keys.W)) position += new Vector2(0, -1);
        // if (keyboardState.IsKeyDown(Keys.Down) || keyboardState.IsKeyDown(Keys.S)) position += new Vector2(0, 1);
        // if (keyboardState.IsKeyDown(Keys.Left) || keyboardState.IsKeyDown(Keys.A))
        // {
        //     position += new Vector2(-1, 0);
        //     flipped = true;
        // }
        // if (keyboardState.IsKeyDown(Keys.Right) || keyboardState.IsKeyDown(Keys.D))
        // {
        //     position += new Vector2(1, 0);
        //     flipped = false;
        // }
    }

    public override Choice? MakeChoice()
    {
        MathHelper.Random random = new();
        int choiceIndex = random.Next(3);
        Choice choice = (Choice)choiceIndex;
        return choice;
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        Texture2D texture = IsDead ? _deadTexture : _texture;
        spriteBatch.Draw(
            texture,
            Position,
            null,
            Color.White,
            0f,
            new Vector2(texture.Width, texture.Height) / 2,
            1f,
            SpriteEffects.None,
            0f
        );
    }
}