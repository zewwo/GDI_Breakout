using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Breakout
{
    class Game
    {
        // current lives
        public int _lives { get; private set; }
        // current score
        public int _score { get; private set; }
        // current level
        public int _level { get; private set; }
        // current state of the game
        public GameState _state { get; private set; }

        // enums for the current state of the game
        public enum GameState
        {
            Start,
            Initial, 
            Running,
            Over,
            Paused,
            Remap,
        }

        public Game()
        {
            _state = GameState.Start;
            _lives = 10;                            // good amount of lives to keep because of the last level
            _score = 0; 
            _level = 1;
        }

        //*********************************************************************
        //Method:  public void AddScore(int score)
        //Purpose: Adds score to the current game.
        //Parameters: int being the score incrementing number.
        //Returns: None.
        //*********************************************************************
        public void AddScore(int score)
        {
            _score += score;
        }

        //*********************************************************************
        //Method: public void LoseLife()
        //Purpose: Fires when the ball(s) go past the paddle. It will make the player lose a life and restart the current game.
        //Parameters: None.
        //Returns: None.
        //*********************************************************************
        public void LoseLife()
        {
            _lives--;
            _state = GameState.Initial;
        }

        //*********************************************************************
        //Method: public void NextLevel()
        //Purpose: Fires when all of the bricks are gone and the player is ready to proceed to the next level.
        //Parameters: None.
        //Returns: None.
        //*********************************************************************
        public void NextLevel()
        {
            _level++;
            _state = GameState.Initial;
        }

        //*********************************************************************
        //Method: public void ChangeState(GameState newState)
        //Purpose: Simply to change the automatic property state.
        //Parameters: GameState being the new state for the game.
        //Returns: None.
        //*********************************************************************
        public void ChangeState(GameState newState)
        {
            _state = newState;
        }
    }
}
