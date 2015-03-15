using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Drawing;

namespace Breakout
{
    class Keyboard : Binding
    {
        // booleans for left/right movement
        public bool _goingLeft { get; private set; }
        public bool _goingRight { get; private set; }

        // initialize to default
        public Keyboard()
            : base((int)System.Windows.Forms.Keys.Left, (int)System.Windows.Forms.Keys.Right, 
            (int)System.Windows.Forms.Keys.B, (int)System.Windows.Forms.Keys.Space, 
            (int)System.Windows.Forms.Keys.R)
        {
            _goingLeft = false;
            _goingRight = false;
        }

        //*********************************************************************
        //Method: public void Movement(bool left, bool right)
        //Purpose: Allows the player to hold down the key instead of pressing it repeatedly.
        //Parameters: key being the key that is pressed expressed as an int, Game being the game itself, two bools
        //            inversion for the fitfh level and keydown if the key is held down.
        //Returns: None.
        //*********************************************************************
        public void Movement(int key, Game _game, bool inversion, bool keydown)
        {
            if (keydown)
            {
                if (_game._state == Game.GameState.Initial || _game._state == Game.GameState.Running)
                {
                    if (key == _left)
                    {
                        if (inversion)
                        {
                            _goingLeft = false;
                            _goingRight = true;
                        }

                        _goingLeft = true;
                    }
                    if (key == _right)
                    {
                        if (inversion)
                        {
                            _goingLeft = true;
                            _goingRight = false;
                        }

                        _goingRight = true;
                    }
                }

                if (_game._state == Game.GameState.Over || _game._state == Game.GameState.Paused)
                {
                    _goingRight = false;
                    _goingLeft = false;
                }
            }
            else
            {
                if (key == _right)
                {
                    if (inversion)
                    {
                        _goingLeft = false;
                        _goingRight = true;
                    }
                    else
                    {
                        _goingRight = false;
                    }
                }
                else if (key == _left)
                {
                    if (inversion)
                    {
                        _goingLeft = true;
                        _goingRight = false;
                    }
                    else
                        _goingLeft = false;              
                }
            }
        }

        //*********************************************************************
        //Method: public bool StartGame(int key, Stopwatch sw, Ball ball, Player player, Game _game, List<Brick> _lBricks)
        //Purpose: Allows the player to start the game.
        //Parameters: key being the key pressed to start, sw being the timer for the game, the other are objects from the game itself.
        //Returns: None.
        //*********************************************************************
        public bool StartGame(int key, Stopwatch sw, Ball ball, Player player, Game _game, List<Brick> _lBricks)
        {
            if (key == _startGame)
            {
                // run the game if its at the start
                if (_game._state == Game.GameState.Initial)
                {
                    _game.ChangeState(Game.GameState.Running);
                    sw.Start();
                }
                else if (_game._state == Game.GameState.Start)
                    _game.ChangeState(Game.GameState.Initial);             
                // start a new game if the player gets a 'game over'
                else if (_game._state == Game.GameState.Over)
                {
                    _game = new Game();
                    sw.Reset();
                    // place paddle, and ball
                    ball = new Ball(new PointF(), System.Drawing.Color.Red, 18);
                    player = new Player(new PointF(350, 475), System.Drawing.Color.Green);
                    // set the ball at starting position
                    ball.StartingBall(player);
                    _lBricks.Clear();
                }
                return true;
            }

            return false;
        }

        //*********************************************************************
        //Method: public void PauseGame(int key, Stopwatch sw, Game _game)
        //Purpose: Allows the player to pause the game.
        //Parameters: key being the key that is pressed expressed as an int, Game being the game itself
        //            sw being the timer for the game.
        //Returns: None.
        //*********************************************************************
        public void PauseGame(int key, Stopwatch sw, Game _game)
        {
            if (key == _pause)
            {
                // pause the game if its running, or run the game if the game is paused
                if (_game._state == Game.GameState.Running)
                {
                    _game.ChangeState(Game.GameState.Paused);
                    sw.Stop();
                }
                else if (_game._state == Game.GameState.Paused)
                {
                    _game.ChangeState(Game.GameState.Running);
                    sw.Start();
                }
            }
        }

        //*********************************************************************
        //Method: public int RebindReq(int key, Game _game, Stopwatch sw)
        //Purpose: Allows the player to send a request to rebind keyboard controls
        //Parameters: key being the key that is pressed expressed as an int, Game being the game itself
        //            sw being the timer for the game.
        //Returns: Returns 0 so it zeroes out the rebinding stages.
        //*********************************************************************
        public int RebindReq(int key, Game _game, Stopwatch sw)
        {
            if (key == _rebind && _game._state != Game.GameState.Remap && _game._state != Game.GameState.Paused)
            {
                sw.Stop();
                _oldState = _game._state;
                _game.ChangeState(Game.GameState.Remap);
            }

            // reset the remapping stages
            return 0;
        }

        //*********************************************************************
        //Method: public int RemapProcess(int key, Game _game, ref int stage)
        //Purpose: Displays prompts in the main thread and show the number of stages for control bindings
        //Parameters: key being the key that is pressed expressed as an int, Game being the game itself
        //            stage being the new stage that is returned.
        //Returns: int being the new key and the stage being the next stage
        //*********************************************************************
        public int RemapProcess(int key, Game _game, ref int stage)
        {
            if (key != _rebind)
            {
                stage++;

                return key;
            }

            return 0;
        }
    }
}
