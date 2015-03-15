using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using System.Drawing;

namespace Breakout
{
    class Controller : Binding
    {   
        // button locks
        public bool _butLock = false;
        public bool _pauseLock = false;
        public bool _startLock = false;

        private GamePadState gps;

        public Controller()
            : base((int)Microsoft.Xna.Framework.Input.Buttons.DPadLeft, (int)Microsoft.Xna.Framework.Input.Buttons.DPadRight,
            (int)Microsoft.Xna.Framework.Input.Buttons.Start, (int)Microsoft.Xna.Framework.Input.Buttons.A, 
            (int)Microsoft.Xna.Framework.Input.Buttons.Back) { }

        //*********************************************************************
        //Method: public void ButtonPress(GamePadState gps, ref int bind, ref int stage)
        //Purpose: Really long function that checks for button presses for rebinding.
        //Parameters: GamePadState being the controller state, two ints representing the binding number that is returned and the current binding stage.
        //Returns: Returns the binding number for the Button Pressed and the new stage when a button is pressed.
        //*********************************************************************
        public void ButtonPress(ref int bind, ref int stage)
        {
            // check if the controller pressed a button
            if (gps.IsButtonDown(Buttons.A) && !_butLock)
            {
                _butLock = true;
                bind = (int)Buttons.A;
                stage++;
            }
            if (gps.IsButtonDown(Buttons.B) && !_butLock)
            {
                _butLock = true;
                bind = (int)Buttons.B;
                stage++;
            }
            if (gps.IsButtonDown(Buttons.DPadDown) && !_butLock)
            {
                _butLock = true;
                bind = (int)Buttons.DPadDown;
                stage++;
            }
            if (gps.IsButtonDown(Buttons.DPadLeft) && !_butLock)
            {
                _butLock = true;
                bind = (int)Buttons.DPadLeft;
                stage++;
            }
            if (gps.IsButtonDown(Buttons.DPadRight) && !_butLock)
            {
                _butLock = true;
                bind = (int)Buttons.DPadRight;
                stage++;
            }
            if (gps.IsButtonDown(Buttons.DPadUp) && !_butLock)
            {
                _butLock = true;
                bind = (int)Buttons.DPadUp;
                stage++;
            }
            if (gps.IsButtonDown(Buttons.LeftShoulder) && !_butLock)
            {
                _butLock = true;
                bind = (int)Buttons.LeftShoulder;
                stage++;
            }
            if (gps.IsButtonDown(Buttons.LeftTrigger) && !_butLock)
            {
                _butLock = true;
                bind = (int)Buttons.LeftTrigger;
                stage++;
            }
            if (gps.IsButtonDown(Buttons.RightShoulder) && !_butLock)
            {
                _butLock = true;
                bind = (int)Buttons.RightShoulder;
                stage++;
            }
            if (gps.IsButtonDown(Buttons.RightTrigger) && !_butLock)
            {
                _butLock = true;
                bind = (int)Buttons.RightTrigger;
                stage++;
            }
            if (gps.IsButtonDown(Buttons.Start) && !_butLock)
            {
                _butLock = true;
                bind = (int)Buttons.Start;
                stage++;
            }
            if (gps.IsButtonDown(Buttons.X) && !_butLock)
            {
                _butLock = true;
                bind = (int)Buttons.X;
                stage++;
            }
            if (gps.IsButtonDown(Buttons.Y) && !_butLock)
            {
                _butLock = true;
                bind = (int)Buttons.Y;
                stage++;
            }

            if (gps.IsButtonUp(Buttons.A) && gps.IsButtonUp(Buttons.B) && gps.IsButtonUp(Buttons.DPadDown) && gps.IsButtonUp(Buttons.DPadLeft) && gps.IsButtonUp(Buttons.DPadRight) &&
                gps.IsButtonUp(Buttons.DPadUp) && gps.IsButtonUp(Buttons.LeftShoulder) && gps.IsButtonUp(Buttons.LeftTrigger) && gps.IsButtonUp(Buttons.RightShoulder) && gps.IsButtonUp(Buttons.RightTrigger) &&
                gps.IsButtonUp(Buttons.Start) && gps.IsButtonUp(Buttons.X) && gps.IsButtonUp(Buttons.Y))
                _butLock = false;
        }

        //*********************************************************************
        //Method: public void Movement(Player player, Game _game, bool inversion, int width, Ball _ball)
        //Purpose: Move the player paddle left or right.
        //Parameters: Player being the paddle, Game being the current game, bool for the inversion on or off, width 
        //            being the form width, and ball being the player ball
        //Returns: Nothing.
        //*********************************************************************
        public void Movement(Player player, Game _game, bool inversion, int width, Ball _ball)
        {
            if (_game._state == Game.GameState.Initial || _game._state == Game.GameState.Running)
            {
                // going left
                if (gps.IsButtonDown((Microsoft.Xna.Framework.Input.Buttons)_left))
                {
                    if (inversion)
                        player.Move(false, true, width, _ball, _game);
                    else
                        player.Move(true, false, width, _ball, _game);

                }// going right
                else if (gps.IsButtonDown((Microsoft.Xna.Framework.Input.Buttons)_right))
                {
                    if (inversion)
                        player.Move(true, false, width, _ball, _game);
                    else
                        player.Move(false, true, width, _ball, _game);
                }
            }
        }

        //*********************************************************************
        //Method: public bool StartGame(Stopwatch sw, Ball ball, Player player, Game _game, List<Brick> _lBricks)
        //Purpose: Starts the game
        //Parameters: Stopwatch is the current time for the current game. Game being the current game and other are the objects
        //            in the game.
        //Returns: Bool representing if a game is started.
        //*********************************************************************
        public bool StartGame(Stopwatch sw, Ball ball, Player player, Game _game, List<Brick> _lBricks)
        {
            if (gps.IsButtonDown((Microsoft.Xna.Framework.Input.Buttons)_startGame) && !_startLock)
            {
                // run the game if its at the start
                if (_game._state == Game.GameState.Initial)
                {
                    _game.ChangeState(Game.GameState.Running);
                    sw.Start();
                }
                else if (_game._state == Game.GameState.Start)
                {
                    _game.ChangeState(Game.GameState.Initial);
                }
                // start a new game if the player gets a 'game over'
                else if (_game._state == Game.GameState.Over)
                {
                    sw.Reset();
                    _game = new Game();
                    // place paddle, and ball
                    ball = new Ball(new PointF(), System.Drawing.Color.Red, 18);
                    player = new Player(new PointF(350, 475), System.Drawing.Color.Green);
                    // set the ball at starting position
                    ball.StartingBall(player);
                    _lBricks.Clear();
                }

                _startLock = true;
                return true;
            }
            else if (gps.IsButtonUp((Microsoft.Xna.Framework.Input.Buttons)_startGame))
                _startLock = false;

            return false;
        }

        //*********************************************************************
        //Method: public void PauseGame( Stopwatch sw, Game _game)
        //Purpose: Pauses the game.
        //Parameters: Stopwatch being the current timer for the current game. Game being the game
        //Returns: Nothing.
        //*********************************************************************
        public void PauseGame( Stopwatch sw, Game _game)
        {
            // if the start button is pressed, lock the button (debouncing)
            if (gps.IsButtonDown((Microsoft.Xna.Framework.Input.Buttons)_pause) && !_pauseLock)
            {
                // pause the game if its running
                if (_game._state == Game.GameState.Running)
                {
                    _game.ChangeState(Game.GameState.Paused);
                    sw.Stop();
                }
                else if (_game._state == Game.GameState.Paused)
                {
                    sw.Start();
                    _game.ChangeState(Game.GameState.Running);
                }

                _pauseLock = true;
            }
            else if (gps.IsButtonUp((Microsoft.Xna.Framework.Input.Buttons)_pause))
                _pauseLock = false;
        }

        //*********************************************************************
        //Method: public int RebindReq(Game _game, Stopwatch sw)
        //Purpose: Sends out a rebinding request to the main form.
        //Parameters: Stopwatch being the current timer for the current game. Game being the game
        //Returns: Returns 0 to reset the binding stage.
        //*********************************************************************
        public int RebindReq(Game _game, Stopwatch sw)
        {
            if (gps.IsButtonDown((Microsoft.Xna.Framework.Input.Buttons)_rebind) && _game._state != Game.GameState.Remap && _game._state != Game.GameState.Paused)
            {
                sw.Stop();
                _oldState = _game._state;
                _game.ChangeState(Game.GameState.Remap);
            }

            // reset the remapping stages
            return 0;
        }

        //*********************************************************************
        //Method: public void ObtainControllerState(GamePadState gamepad)
        //Purpose: Obtain the gamepad state.
        //Parameters: GamePadState being the controller polled states when a controller is connected.
        //Returns: Nothing.
        //*********************************************************************
        public void ObtainControllerState(GamePadState gamepad)
        {
            gps = gamepad;
        }
    }
}
