using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace game_0;

public class RockPaperScissors : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    private State _gameState;

    private DynamicPlayer _humanPlayer;

    private DeterministicPlayer _computerPlayer;

    private Choice _humanPlayerChoice;

    private Choice _computerPlayerChoice;

    public RockPaperScissors()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here
        _gameState = State.Initial;
        _humanPlayer = new();
        _computerPlayer = new();
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        // TODO: use this.Content to load your game content here
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        // TODO: Add your update logic here
        switch (_gameState)
        {
            case State.Initial:
                _gameState = State.Choosing;
                Console.WriteLine("Choose 1 for Rock, 2 for Paper, 3 for Scissors");
                break;
            case State.Choosing:
                Choice? playerChoice = _humanPlayer.MakeChoice();
                if (playerChoice != null)
                {
                    _humanPlayerChoice = (Choice)playerChoice;
                    _computerPlayerChoice = (Choice)_computerPlayer.MakeChoice();
                    Console.WriteLine($"Human Choice: {_humanPlayerChoice}");
                    Console.WriteLine($"Computer Choice: {_computerPlayerChoice}");
                    _gameState = State.Scoring;
                }
                break;
            case State.Scoring:
                Result result = Score(_humanPlayerChoice, _computerPlayerChoice);
                Console.WriteLine($"Result: {result}");
                _gameState = State.Ending;
                break;
            default:
                break;
        }

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        // TODO: Add your drawing code here

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
