using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace game_0;

public class DynamicPlayer : Player
{
    private KeyboardState _keyboardState;

    private Texture2D _frontTexture;

    private Texture2D _backTexture;

    private Texture2D _deadTexture;

    /// When true the player is drawn from behind, facing the computer player.
    public bool FacingAway { get; set; }

    public void LoadContent(ContentManager content)
    {
        _frontTexture = content.Load<Texture2D>("HumanPlayer");
        _backTexture = content.Load<Texture2D>("HumanPlayerBack");
        _deadTexture = content.Load<Texture2D>("HumanPlayerDead");
    }

    public override void Update(GameTime gameTime)
    {
        _keyboardState = Keyboard.GetState();

        // Apply keyboard movement
        // if (_keyboardState.IsKeyDown(Keys.Up) || _keyboardState.IsKeyDown(Keys.W)) position += new Vector2(0, -1);
        // if (_keyboardState.IsKeyDown(Keys.Down) || _keyboardState.IsKeyDown(Keys.S)) position += new Vector2(0, 1);
        // if (_keyboardState.IsKeyDown(Keys.Left) || _keyboardState.IsKeyDown(Keys.A))
        // {
        //     position += new Vector2(-1, 0);
        //     flipped = true;
        // }
        // if (_keyboardState.IsKeyDown(Keys.Right) || _keyboardState.IsKeyDown(Keys.D))
        // {
        //     position += new Vector2(1, 0);
        //     flipped = false;
        // }
    }

    public override Choice? MakeChoice()
    {
        if (_keyboardState.IsKeyDown(Keys.D1)) return (Choice)0;
        else if (_keyboardState.IsKeyDown(Keys.D2)) return (Choice)1;
        else if (_keyboardState.IsKeyDown(Keys.D3)) return (Choice)2;
        return null;
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        // There is no back-facing dead sprite, so dying turns the player around.
        Texture2D texture = IsDead ? _deadTexture : (FacingAway ? _backTexture : _frontTexture);
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