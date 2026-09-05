using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace game_0;

public class RockPaperScissors : Game
{
    private GraphicsDeviceManager _graphics;

    private SpriteBatch _spriteBatch;

    private KeyboardState _keyboardState;

    private KeyboardState _priorKeyboardState;

    private SpriteFont _bangers;

    private State _gameState;

    private DynamicPlayer _humanPlayer;

    private DeterministicPlayer _computerPlayer;

    private Choice _humanPlayerChoice;

    private Choice _computerPlayerChoice;

    private Result _result;

    public RockPaperScissors()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        _gameState = State.Initial;
        _humanPlayer = new();
        _computerPlayer = new();
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _bangers = Content.Load<SpriteFont>("bangers");
    }

    protected override void Update(GameTime gameTime)
    {
        _keyboardState = Keyboard.GetState();
        if (_keyboardState.IsKeyDown(Keys.Escape)) Exit();

        _humanPlayer.Update(gameTime);
        _computerPlayer.Update(gameTime);

        // TODO: Add your update logic here
        switch (_gameState)
        {
            case State.Initial:

                if (_keyboardState.IsKeyDown(Keys.C) && _priorKeyboardState.IsKeyUp(Keys.C)) _gameState = State.Choosing;
                break;
            case State.Choosing:
                Choice? playerChoice = _humanPlayer.MakeChoice();
                if (playerChoice != null)
                {
                    _humanPlayerChoice = (Choice)playerChoice;
                    _computerPlayerChoice = (Choice)_computerPlayer.MakeChoice();
                    _gameState = State.Results;
                }
                break;
            case State.Results:
                if (_keyboardState.IsKeyDown(Keys.C) && _priorKeyboardState.IsKeyUp(Keys.C)) _gameState = State.Scoring;
                break;
            case State.Scoring:
                _result = Score(_humanPlayerChoice, _computerPlayerChoice);
                if (_keyboardState.IsKeyDown(Keys.C) && _priorKeyboardState.IsKeyUp(Keys.C)) _gameState = State.Ending;
                break;
            case State.Ending:
                if (_keyboardState.IsKeyDown(Keys.Enter)) _gameState = State.Choosing;
                break;
        }

        _priorKeyboardState = _keyboardState;
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _spriteBatch.Begin();

        // TODO: Add your update logic here
        switch (_gameState)
        {
            case State.Initial:
                _spriteBatch.DrawString(_bangers, "EXTREME Rock Paper Scissors - Press C to continue", new Vector2(2, 2), Color.Gold);
                break;
            case State.Choosing:
                _spriteBatch.DrawString(_bangers, "Choose 1 for Rock, 2 for Paper, 3 for Scissors", new Vector2(2, 2), Color.Gold);
                break;
            case State.Results:
                _spriteBatch.DrawString(_bangers, $"You chose {_humanPlayerChoice}, computer chose {_computerPlayerChoice}.", new Vector2(2, 2), Color.Gold);
                break;
            case State.Scoring:
                _spriteBatch.DrawString(_bangers, $"Result: ${_result}", new Vector2(2, 2), Color.Gold);
                break;
            case State.Ending:
                _spriteBatch.DrawString(_bangers, $"Play again? - Press ENTER", new Vector2(2, 2), Color.Gold);
                break;
            default:
                break;
        }

        _spriteBatch.End();

        base.Draw(gameTime);
    }

    private Result Score(Choice firstPlayerChoice, Choice secondPlayerChoice)
    {
        switch (firstPlayerChoice)
        {
            case Choice.Rock:
                if (secondPlayerChoice == Choice.Rock) return Result.Tie;
                else if (secondPlayerChoice == Choice.Scissors) return Result.FirstPlayerWins;
                else return Result.SecondPlayerWins; // Paper
            case Choice.Paper:
                if (secondPlayerChoice == Choice.Paper) return Result.Tie;
                else if (secondPlayerChoice == Choice.Rock) return Result.FirstPlayerWins;
                else return Result.SecondPlayerWins; // Scissors
            default: // Scissors
                if (secondPlayerChoice == Choice.Scissors) return Result.Tie;
                else if (secondPlayerChoice == Choice.Paper) return Result.FirstPlayerWins;
                else return Result.SecondPlayerWins; // Rock
        }
    }
}
