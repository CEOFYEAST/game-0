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

    /// Rock, Paper and Scissors art, indexed by Choice.
    private Texture2D[] _choiceTextures;

    /// The 128x128 choice sprites are drawn at half size so a row of them fits between the players.
    private const float ChoiceScale = 0.5f;

    private const string ContinuePrompt = "Press C to continue";

    private const string QuitPrompt = "Press ESC to quit";

    /// How long a choice sprite takes to slide in from the edge of the screen, in seconds.
    private const float SlideDuration = 0.4f;

    /// Seconds since the current state began, which drives the slide-in animations.
    private double _stateTime;

    private State _gameState;

    private DynamicPlayer _humanPlayer;

    private DeterministicPlayer _computerPlayer;

    private Choice _humanPlayerChoice;

    private Choice _computerPlayerChoice;

    private Result _result;

    private int _humanScore;

    private int _computerScore;

    public RockPaperScissors()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        ChangeState(State.Initial);
        _humanPlayer = new();
        _computerPlayer = new();
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _humanPlayer.LoadContent(Content);
        _computerPlayer.LoadContent(Content);
        _bangers = Content.Load<SpriteFont>("bangers");

        // The order here has to match the Choice enum, since Choice indexes this array.
        _choiceTextures = new Texture2D[]
        {
            Content.Load<Texture2D>("Rock"),
            Content.Load<Texture2D>("Paper"),
            Content.Load<Texture2D>("Scissors")
        };
    }

    protected override void Update(GameTime gameTime)
    {
        _stateTime += gameTime.ElapsedGameTime.TotalSeconds;

        _keyboardState = Keyboard.GetState();
        if (_keyboardState.IsKeyDown(Keys.Escape)) Exit();

        _humanPlayer.Update(gameTime);
        _computerPlayer.Update(gameTime);

        // TODO: Add your update logic here
        switch (_gameState)
        {
            case State.Initial:
                if (CheckForInput(Keys.C)) ChangeState(State.Choosing);
                break;

            case State.Choosing:
                Choice? playerChoice = _humanPlayer.MakeChoice();
                if (playerChoice != null)
                {
                    _humanPlayerChoice = (Choice)playerChoice;
                    _computerPlayerChoice = (Choice)_computerPlayer.MakeChoice();
                    _result = Score(_humanPlayerChoice, _computerPlayerChoice);
                    if (_result == Result.FirstPlayerWins) _humanScore++;
                    else if (_result == Result.SecondPlayerWins) _computerScore++;
                    ChangeState(State.Results);
                }
                break;

            case State.Results:
                // A tie settles nothing, so replay the round rather than ending it.
                if (CheckForInput(Keys.C)) ChangeState(_result == Result.Tie ? State.Choosing : State.Ending);
                break;

            case State.Ending:
                if (CheckForInput(Keys.Enter)) ChangeState(State.Choosing);
                break;
        }

        _priorKeyboardState = _keyboardState;
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _spriteBatch.Begin();

        float screenWidth = GraphicsDevice.Viewport.Width;
        float screenHeight = GraphicsDevice.Viewport.Height;

        _spriteBatch.DrawString(_bangers, QuitPrompt, new Vector2(2, 2), Color.Gold);

        // Default placement; the states where the players face off override it below.
        _computerPlayer.Position = new Vector2(screenWidth / 2, screenHeight * 0.2f);
        _humanPlayer.FacingAway = false;
        _humanPlayer.Position = new Vector2(screenWidth / 2, screenHeight * 0.6f);
        _humanPlayer.IsDead = false;
        _computerPlayer.IsDead = false;

        // TODO: Add your update logic here
        switch (_gameState)
        {
            case State.Initial:
                DrawCenteredString($"EXTREME Rock Paper Scissors - {ContinuePrompt}", screenWidth / 2, screenHeight * 0.25f);
                break;
            case State.Choosing:
                _computerPlayer.Draw(gameTime, _spriteBatch);

                // A row of the three choices, each labelled with the key that picks it,
                // rising together from below the bottom edge.
                float rowY = SlideIn(screenHeight + ChoiceHalfHeight, screenHeight * 0.46f);
                for (int choiceIndex = 0; choiceIndex < _choiceTextures.Length; choiceIndex++)
                {
                    float choiceX = screenWidth * (0.3f + 0.2f * choiceIndex);
                    DrawChoice((Choice)choiceIndex, new Vector2(choiceX, rowY));
                    DrawCenteredString($"{choiceIndex + 1}", choiceX, rowY + screenHeight * 0.1f);
                }

                _humanPlayer.FacingAway = true;
                _humanPlayer.Position = new Vector2(screenWidth / 2, screenHeight * 0.8f);
                break;
            case State.Results:
                _humanPlayer.FacingAway = true;
                _humanPlayer.Position = new Vector2(screenWidth / 2, screenHeight * 0.8f);
                _humanPlayer.IsDead = _result == Result.SecondPlayerWins;
                _computerPlayer.IsDead = _result == Result.FirstPlayerWins;

                _computerPlayer.Draw(gameTime, _spriteBatch);

                // Each player's pick flies in from their own edge to just off their right shoulder.
                DrawChoice(_computerPlayerChoice, new Vector2(screenWidth * 0.65f, SlideIn(-ChoiceHalfHeight, _computerPlayer.Position.Y)));
                DrawChoice(_humanPlayerChoice, new Vector2(screenWidth * 0.65f, SlideIn(screenHeight + ChoiceHalfHeight, _humanPlayer.Position.Y)));

                // The outcome and the prompt stack as one block centered between the players.
                float betweenPlayersY = (_computerPlayer.Position.Y + _humanPlayer.Position.Y) / 2;
                DrawCenteredString(ResultText(_result), screenWidth / 2, betweenPlayersY - _bangers.LineSpacing / 2f);
                DrawCenteredString(ContinuePrompt, screenWidth / 2, betweenPlayersY + _bangers.LineSpacing / 2f);
                break;
            case State.Ending:
                // The two stand side by side, each with their score on their outside shoulder.
                _humanPlayer.Position = new Vector2(screenWidth * 0.35f, screenHeight * 0.6f);
                _humanPlayer.IsDead = _result == Result.SecondPlayerWins;
                _computerPlayer.Position = new Vector2(screenWidth * 0.65f, screenHeight * 0.6f);
                _computerPlayer.IsDead = _result == Result.FirstPlayerWins;
                _computerPlayer.Draw(gameTime, _spriteBatch);

                DrawCenteredString($"{_humanScore}", screenWidth * 0.2f, _humanPlayer.Position.Y);
                DrawCenteredString($"{_computerScore}", screenWidth * 0.8f, _computerPlayer.Position.Y);

                DrawCenteredString("Play again? Press ENTER.", screenWidth / 2, screenHeight * 0.25f - _bangers.LineSpacing / 2f);
                break;
            default:
                break;
        }

        _humanPlayer.Draw(gameTime, _spriteBatch);

        _spriteBatch.End();

        base.Draw(gameTime);
    }

    private void ChangeState(State state)
    {
        _gameState = state;
        _stateTime = 0;
    }

    /// Half the drawn height of a choice sprite, used to park it just off an edge of the screen.
    private float ChoiceHalfHeight => _choiceTextures[0].Height * ChoiceScale / 2f;

    /// Eases a choice sprite from an off-screen edge to its resting spot as the state begins.
    private float SlideIn(float fromY, float toY)
    {
        float progress = MathHelper.Clamp((float)(_stateTime / SlideDuration), 0f, 1f);
        return MathHelper.SmoothStep(fromY, toY, progress);
    }

    private void DrawChoice(Choice choice, Vector2 center)
    {
        Texture2D texture = _choiceTextures[(int)choice];
        _spriteBatch.Draw(
            texture,
            center,
            null,
            Color.White,
            0f,
            new Vector2(texture.Width, texture.Height) / 2,
            ChoiceScale,
            SpriteEffects.None,
            0f
        );
    }

    private void DrawCenteredString(string text, float centerX, float centerY)
    {
        Vector2 size = _bangers.MeasureString(text);
        _spriteBatch.DrawString(_bangers, text, new Vector2(centerX, centerY) - size / 2, Color.Gold);
    }

    private string ResultText(Result result)
    {
        switch (result)
        {
            case Result.FirstPlayerWins: return "You win!";
            case Result.SecondPlayerWins: return "You lose!";
            default: return "It's a tie!";
        }
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

    private bool CheckForInput(Keys key)
    {
        return _keyboardState.IsKeyDown(key) && _priorKeyboardState.IsKeyUp(key);
    }
}
