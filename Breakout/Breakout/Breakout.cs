//*********************************************************************
//Program: Breakout
//Files: Program.cs
// Breakout.cs
// Breakout.Designer.cs
// Breakout.resx
// Game.cs
// GameObjects.cs
// Brick.cs
// Player.cs
// Ball.cs
// Binding.cs
// Keyboard.cs
// Controller.cs
//Description: The game will recreate the game Ricochet in Windows format.
//             The game features only five levels with premade levels and a very difficult boss at the end.
//Version: 1.0
//Date: March 24th 2014 - April 6th 2014
//Author: Kevin Nguyen
//Class: CNT4D
//Instructor: Simon Walker
//*********************************************************************

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Threading;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace Breakout
{
    public partial class Breakout : Form
    {
        public delegate void _delvoidvoid();

        private Graphics _gr;
        private static Random _rng = new Random();

        // game class
        private Game _game;
                     
        // not in a list of game objects because there is only one paddle + ball and a multiple bricks.
        // easier to do them seperately
        // list of the Bricks in the game and ball + player references               
        private List<Brick> _lBricks = new List<Brick>();
        private Player _player;
        private Ball _ball;
        // elapse time for the game
        private Stopwatch _gSW = new Stopwatch();

        //--- KEYMAPPING ---//
        Keyboard _keys = new Keyboard();
        // binding stages for the prompts and button presses
        private int remapSTG = 0;
        // new bindings for the keyboard/controller
        private int _cLeft, _cPause, _cStart, _cRight;

        //--- CONTROLLER AND XNA ---//
        Controller _ctrl = new Controller();
        private bool _xboxConnect = false;                   // bool to disable keyboard presses if controller is connected

        //--- LEVEL 5 BRICK BOSS ENCOUNTER ---//
        private List<Ball> _leBall = new List<Ball>();       // list of enemy balls
        private int _red, _green, _blue = 0;                 // the "health" of the brick boss

        private bool _frostOn = false;                       // frost aura
        private bool _invert = false;                        // inversion on/off
        private bool _colorful = false;                      // COLORFUL BACKGROUND~
        private bool _berserk = false;                       // enrage bool
        private bool _slowball = false;                      // bool that slows down the palyer ball ( doesnt affect enemy balls )
        private GraphicsPath _fp;                            // graphics path for frost aura

        // seperate timers for the berserk + abilty duration and set
        private Stopwatch _berserkTimer = new Stopwatch();
        private Stopwatch _colorSW = new Stopwatch();
        private Stopwatch _invertSW = new Stopwatch();
        private Stopwatch _slowSW = new Stopwatch();
        //--- LEVEL 5 BRICK BOSS ENCOUNTER ---//

        public Breakout()
        {
            InitializeComponent();
            _gr = this.CreateGraphics();

            // initialize a game
            _game = new Game();

            UI_TSL_Lives.Text = "Lives : " + _game._lives;
            UI_TSL_Score.Text = "Scores : " + _game._score;
            UI_TSL_Time.Text = "Game State : " + _game._state;

            // place paddle, and ball
            _ball = new Ball(new PointF(), System.Drawing.Color.Red, 18);
            _player = new Player(new PointF(350, 475), System.Drawing.Color.Green);
            // set the ball at starting position
            _ball.StartingBall(_player);

            timer.Enabled = true;
        }

        // The Tick handler will animate the ball and paddle moving.
        // Side Effects: It will also render out the bricks, paddle, ball, and handle game state changes such as pausing and remapping keys/buttons
        //              Also checks for controller connection and disables the keyboard presses if it is connected.
        private void timer_Tick(object sender, EventArgs e)
        {
            // using double buffering
            using (BufferedGraphicsContext bgc = new BufferedGraphicsContext())
            {
                using (BufferedGraphics bg = bgc.Allocate(_gr, this.DisplayRectangle))
                {                
                    // clear the graphics          
                    if (_game._level == 5 && _colorful)
                        bg.Graphics.Clear(System.Drawing.Color.FromArgb(_rng.Next(256), _rng.Next(256), _rng.Next(256)));
                    else if (_game._level != 5 || !_colorful)
                        bg.Graphics.Clear(System.Drawing.Color.White);
                    

                    Region ballRegion = new Region(_ball.GetPath());
                    Font font = new Font("Calibri", 25);

                    if (_game._state == Game.GameState.Remap)
                    {
                        bg.Graphics.Clear(System.Drawing.Color.White);
                        RemapScreen(bg.Graphics, font);
                    }

                    if (_game._state == Game.GameState.Running && _game._lives > 0)
                    {
                        int unbreakables = 0;

                        for (int i = 0; i < _lBricks.Count; ++i)
                        {
                            // check for brick collisions
                            if (_ball.BrickHit(_lBricks[i], bg.Graphics))
                            {
                                if (_lBricks[i]._type == Brick.Type.Pawn || _lBricks[i]._type == Brick.Type.Lord)
                                {
                                    if (_lBricks[i]._type == Brick.Type.Pawn)
                                        _lBricks[i].MarkofDeath();                      // instant kill any collided black bricks
                                    else
                                        BossHit();                                      // drain a health from the boss

                                    _game.AddScore(100);
                                }
                            }

                            if (_lBricks[i]._type == Brick.Type.Rook)                   // exclude unbreakable bricks
                                unbreakables++;
                        }

                        // check if the bricks are gone while excluding unbreakable bricks
                        if (_lBricks.Count - unbreakables == 0 || _lBricks.Count == 0)
                        {
                            _game.NextLevel();
                            StartUp();
                        }

                        // remove all death marked bricks
                        lock (_lBricks)
                            _lBricks.RemoveAll((A) => A is Brick ? ((Brick)A)._death : false);

                        // check if the ball hit the paddle
                        _ball.PaddleHit(_player, bg.Graphics);

                        // check if the ball is below the bottom of the paddle
                        if (_ball.BallDead(_player))
                        {
                            _game.LoseLife();
                            _leBall.Clear();

                            // reset player and ball
                            _ball = new Ball(new PointF(), System.Drawing.Color.Red, 18);
                            _player = new Player(new PointF(350, 475), System.Drawing.Color.Green);
                            _ball.StartingBall(_player);
                        }
                        
                        // animate the ball
                        _ball.Move(new Size(this.Width, this.Height));

                        // boss level stuff
                        BossLevel(bg.Graphics);

                        if (_game._level == 5)
                        {
                            // start two seperate timers for beserk and abilities
                            _colorSW.Start();
                            _invertSW.Start();
                            _berserkTimer.Start();
                            _slowSW.Start();
                        }

                        if (_game._level == 5 && _frostOn)
                        {
                            _fp = new GraphicsPath();
                            _fp.StartFigure();
                            // add a 250 radius around the paddle
                            _fp.AddEllipse((_player._pos.X + (_player._length / 2)) - 225, (_player._pos.Y + (_player._width / 2)) - 225, 450, 450);
                            _fp.CloseFigure();

                            //Region aura = new Region(fp);
                            //bg.Graphics.FillRegion(new SolidBrush(System.Drawing.Color.LightCyan), aura);
                            FrostAuraHit(bg.Graphics, _fp);
                        }
                    }

                    // display ability prompts in the boss level
                    if (_game._level == 5)
                    {
                        if (_frostOn)
                        {
                            if (_colorful)
                                bg.Graphics.DrawString("Frost Aura", font, new SolidBrush(System.Drawing.Color.White), new PointF(50, 500));
                            else
                                bg.Graphics.DrawString("Frost Aura", font, new SolidBrush(System.Drawing.Color.Blue), new PointF(50, 500));
                        }

                        if (_invert && _invertSW.Elapsed.Seconds < 4 && !_berserk)
                        {
                            if (_colorful)
                                bg.Graphics.DrawString("Inversion >:D", font, new SolidBrush(System.Drawing.Color.White), new PointF(290, 230));
                            else
                                bg.Graphics.DrawString("Inversion >:D", font, new SolidBrush(System.Drawing.Color.Red), new PointF(290, 230));
                        }
                        if (_slowball && _slowSW.Elapsed.Seconds < 4 && !_berserk)
                        {
                            if (_colorful)
                                bg.Graphics.DrawString("Slow Ball >:D", font, new SolidBrush(System.Drawing.Color.White), new PointF(290, 265));
                            else
                                bg.Graphics.DrawString("Slow Ball >:D", font, new SolidBrush(System.Drawing.Color.Red), new PointF(290, 265));
                        }
                        if (_colorful && _colorSW.Elapsed.Seconds < 4 && !_berserk)
                            bg.Graphics.DrawString("COLORFUL LOOK AT ALL THESE COLORS", font, new SolidBrush(System.Drawing.Color.White), new PointF(125, 300));
                        
                        if (_berserk)
                            bg.Graphics.DrawString("Berserk", new Font("Calibri", 48), new SolidBrush(System.Drawing.Color.White), new PointF(275, 280));

                        // stop the stopwatches if its paused or  remapping or what not
                        if (_game._state != Game.GameState.Running)
                        {
                            // start two seperate timers for beserk and abilities
                            _colorSW.Stop();
                            _invertSW.Stop();
                            _berserkTimer.Stop();
                            _slowSW.Stop();
                        }
                    }
       
                    // poll for controller connection and inputs
                    StateControls();
                    
                    // disable the keyboard movement IF a controller is connected
                    if (!_xboxConnect)
                        // move the paddle **** move the ball with the paddle before starting game
                        _player.Move(_keys._goingLeft, _keys._goingRight, this.Width, _ball, _game);

                    if (_game._state == Game.GameState.Initial)
                        _gSW.Stop();

                    if (_game._state != Game.GameState.Remap)
                    {
                        if (_leBall.Count > 0)
                            _leBall.ForEach(A => A.Render(bg.Graphics));

                        _lBricks.ForEach(A => A.Render(bg.Graphics));
                        // manually render out paddle and ball
                        _player.Render(bg.Graphics);
                        _ball.Render(bg.Graphics);
                    }

                    // game over if no more lives
                    if (_game._lives == 0)
                    {
                        string prompt = "Press ";
                        bg.Graphics.Clear(System.Drawing.Color.White);
                        bg.Graphics.DrawString("Game Over!!", font, new SolidBrush(System.Drawing.Color.Black), new PointF(300, 250));

                        // output the right key/button
                        if (!_xboxConnect)
                            prompt += (System.Windows.Forms.Keys)_keys._startGame;
                        else
                            prompt += (Microsoft.Xna.Framework.Input.Buttons)_ctrl._startGame;

                        prompt += " to restart the game :((((((";
                        
                        _gSW.Reset();
                        _colorSW.Reset();
                        _invertSW.Reset();
                        _berserkTimer.Reset();
                        _slowSW.Reset();

                        bg.Graphics.DrawString(prompt, font, new SolidBrush(System.Drawing.Color.Black), new PointF(175, 300));
                        _game.ChangeState(Game.GameState.Over);
                    }

                    // winner if they make it last the fitfh level
                    if (_game._lives > 0 && _game._state == Game.GameState.Over)
                    {
                        string prompt = "Press ";
                        bg.Graphics.Clear(System.Drawing.Color.White);
                        bg.Graphics.DrawString("You have defeated the LORD", font, new SolidBrush(System.Drawing.Color.Black), new PointF(200, 150));

                        // output the right key/button
                        if (!_xboxConnect)
                            prompt += (System.Windows.Forms.Keys)_keys._startGame;
                        else
                            prompt += (Microsoft.Xna.Framework.Input.Buttons)_ctrl._startGame;

                        prompt += " to restart the game ^_^";

                        _gSW.Reset();
                        _colorSW.Reset();
                        _invertSW.Reset();
                        _berserkTimer.Reset();
                        _slowSW.Reset();

                        bg.Graphics.DrawString(prompt, font, new SolidBrush(System.Drawing.Color.Black), new PointF(165, 300));
                    }

                    // pause the game if there are still lives
                    if (_game._state == Game.GameState.Paused && _game._lives > 0)
                        bg.Graphics.DrawString("Paused", font, new SolidBrush(System.Drawing.Color.Black), new PointF(330, 300));
                    
                    // if the game is FIRST loaded, show title + controls
                    if (_game._state == Game.GameState.Start)
                    {
                        Font startFont = new Font("Calibri", 40);
                        Font ctrl = new Font("Calibri", 25);
                        Font ctrlFont = new Font("Calibri", 18);
                        Font keys = new Font("Calibri", 14);
                        bg.Graphics.DrawString("Brick-Break", startFont, new SolidBrush(System.Drawing.Color.FromArgb(_rng.Next(256), _rng.Next(256), _rng.Next(256))), new PointF(260, 35));                     
                        bg.Graphics.DrawString("Controls", ctrl, new SolidBrush(System.Drawing.Color.Black), new PointF(325, 120));
                        bg.Graphics.DrawString("Keyboard -", ctrlFont, new SolidBrush(System.Drawing.Color.Black), new PointF(150, 145));
                        bg.Graphics.DrawString((System.Windows.Forms.Keys)_keys._left + " : Move Left\n\n" + (System.Windows.Forms.Keys)_keys._right + " : Move Right\n\n"
                            + (System.Windows.Forms.Keys)_keys._startGame + " : Start Game\n\n" + (System.Windows.Forms.Keys)_keys._pause + " : Pause Game\n\nV : Use Button\n\n"
                            + (System.Windows.Forms.Keys)_keys._rebind + " : Change Binding", keys, new SolidBrush(System.Drawing.Color.Black), new PointF(150, 185));
                        bg.Graphics.DrawString("GamePad -", ctrlFont, new SolidBrush(System.Drawing.Color.Black), new PointF(500, 145));
                        bg.Graphics.DrawString((Microsoft.Xna.Framework.Input.Buttons)_ctrl._left + " : Move Left\n\n" + (Microsoft.Xna.Framework.Input.Buttons)_ctrl._right + " : Move Right\n\n" +
                            (Microsoft.Xna.Framework.Input.Buttons)_ctrl._startGame + " : Start Game\n\n" + (Microsoft.Xna.Framework.Input.Buttons)_ctrl._pause + " : Pause Game\n\nRT : Use Button\n\n"
                            + (Microsoft.Xna.Framework.Input.Buttons)_ctrl._rebind + " : Change Binding", keys, new SolidBrush(System.Drawing.Color.Black), new PointF(500, 185));
                    }

                    // manually render out the items in the graphics
                    bg.Render();
                }
            }

            // update scores and lives
            UI_TSL_Lives.Text = "Lives : " + _game._lives;
            UI_TSL_Score.Text = "Scores : " + _game._score;
            UI_TSL_Time.Text = "Time : " + String.Format("{0:00}:{1:00}", _gSW.Elapsed.Minutes, _gSW.Elapsed.Seconds);
        }

        // The KeyDown handler will move the player handle IF the controller is not connected.
        // Side Effects: [what is the effect of the event handler when it perform its task]
        private void Breakout_KeyDown(object sender, KeyEventArgs e)
        {
            // disable keyboard controls if there is a controller connected
            if (!_xboxConnect)
            {
                if (_game._state == Game.GameState.Remap)
                {
                    // read new binding keys
                    if (remapSTG == 0)
                        _cLeft = _keys.RemapProcess((int)e.KeyCode, _game, ref remapSTG);
                    else if (remapSTG == 1)
                        _cRight = _keys.RemapProcess((int)e.KeyCode, _game, ref remapSTG);
                    else if (remapSTG == 2)
                        _cPause = _keys.RemapProcess((int)e.KeyCode, _game, ref remapSTG);
                    else if (remapSTG == 3)
                        _cStart = _keys.RemapProcess((int)e.KeyCode, _game, ref remapSTG);
                    else if (remapSTG == 4)
                    {
                        int dummy = 0;
                        dummy = _keys.RemapProcess((int)e.KeyCode, _game, ref remapSTG);
                    }

                    if (remapSTG == 5)
                    {
                        if (_keys._oldState == Game.GameState.Start && _game._state == Game.GameState.Remap)
                            _game.ChangeState(_keys._oldState);
                        else
                            _game.ChangeState(Game.GameState.Paused);
                    }
                }
                else
                {
                    // move the paddle
                    _keys.Movement((int)e.KeyCode, _game, _invert, true);

                    // start up a level
                    if (_keys.StartGame((int)e.KeyCode, _gSW, _ball, _player, _game, _lBricks))
                        if (_game._state == Game.GameState.Initial)
                            StartUp();

                    _keys.PauseGame((int)e.KeyCode, _gSW, _game);

                    remapSTG =_keys.RebindReq((int)e.KeyCode, _game, _gSW);
                }
            }
        }

        // The KeyUp handler will help the paddle move than using continuous key down keys
        // Side Effects: None
        private void Breakout_KeyUp(object sender, KeyEventArgs e)
        {
            _keys.Movement((int)e.KeyCode, _game, _invert, false);
        }

        //*********************************************************************
        //Method: internal void StartUp()
        //Purpose: Generate Bricks to play against, a Ball, and the paddle. It will generate bricks according to the current level.
        //Parameters: None.
        //Returns: None.
        //*********************************************************************
        internal void StartUp()
        {
            lock (_lBricks)
            {
                _lBricks.Clear();
                Random _rng = new Random();

                // first level 
                if (_game._level == 1)
                {
                    for (int row = 0; row < 5; ++row)
                        for (int col = 0; col < 7; ++col)
                            _lBricks.Add(new Brick(new PointF(55, 55), col, row, System.Drawing.Color.Black));
                }
                else if (_game._level == 2)
                {
                    for (int row = 0; row < 5; ++row)
                        for (int col = 0; col < 7; ++col)
                        {
                            if (col == 2 || col == 4)
                                _lBricks.Add(new Brick(new PointF(55, 55), col, row, System.Drawing.Color.Gray, Brick.Type.Rook));
                            else
                                _lBricks.Add(new Brick(new PointF(55, 55), col, row, System.Drawing.Color.Black));
                        }
                }
                else if (_game._level == 3)
                {
                    for (int row = 0; row < 5; ++row)
                        for (int col = 0; col < 7; ++col)
                        {
                            if (col == 1 && row < 3 || col == 2 && row > 2 || col == 5 && row < 3 || col == 4 && row > 2)
                                _lBricks.Add(new Brick(new PointF(55, 55), col, row, System.Drawing.Color.Gray, Brick.Type.Rook));
                            else
                                _lBricks.Add(new Brick(new PointF(55, 55), col, row, System.Drawing.Color.Black));
                        }
                }
                else if (_game._level == 4)
                {
                    for (int row = 0; row < 5; ++row)
                        for (int col = 0; col < 7; ++col)
                        {
                            if (col == 0 && row > 0|| row == 4 || col == 6 && row > 0)
                                _lBricks.Add(new Brick(new PointF(55, 55), col, row, System.Drawing.Color.Gray, Brick.Type.Rook));
                            else
                                _lBricks.Add(new Brick(new PointF(55, 55), col, row, System.Drawing.Color.Black));
                        }
                }
                else if (_game._level == 5)
                {
                    _red = 220;
                    _blue = 220;
                    _green = 220;

                    _lBricks.Add(new Brick(new PointF(), 0, 0, System.Drawing.Color.FromArgb(_red, _blue, _green), Brick.Type.Lord, 50, 795));
                }

                _colorSW.Reset();
                _invertSW.Reset();
                _berserkTimer.Reset();
            }

            _ball = new Ball(new PointF(), System.Drawing.Color.Red, 18);
            _player = new Player(new PointF(350, 475), System.Drawing.Color.Green);

            // set the ball at starting position
            _ball.StartingBall(_player);
        }

        //*********************************************************************
        //Method: private void StateControls()
        //Purpose: Check for Button presses on the Controller.
        //Parameters: None.
        //Returns: None.
        //*********************************************************************
        private void StateControls()
        {
            // obtain the state of the first player
            GamePadState gps = GamePad.GetState(PlayerIndex.One);

            if (gps.IsConnected)
            {
                if (_game._state == Game.GameState.Running && !_xboxConnect)
                    _game.ChangeState(Game.GameState.Paused);

                // disable keyboard presses
                _xboxConnect = true;
                // get the controller state
                _ctrl.ObtainControllerState(gps);

                UI_L_CtrlStatus.Text = "Connected";

                // if the player is remapping his/her bindings, go do the binding wizard and open up prompts
                if (_game._state == Game.GameState.Remap)
                    CtrlRebinding();
                else
                {
                    // controller movement
                    _ctrl.Movement(_player, _game, _invert, this.Width, _ball);

                    // start up a level
                    if (_ctrl.StartGame(_gSW, _ball, _player, _game, _lBricks))
                        if (_game._state == Game.GameState.Initial)
                            StartUp();

                    // pause game
                    _ctrl.PauseGame(_gSW, _game);

                    // obtain a request for rebinding
                    remapSTG = _ctrl.RebindReq(_game, _gSW);
                }
            }
            else
            {
                if (_game._state == Game.GameState.Running && _xboxConnect)
                    _game.ChangeState(Game.GameState.Paused);

                _xboxConnect = false;
                UI_L_CtrlStatus.Text = "Not Connected";
            }
        }

        //*********************************************************************
        //Method:  private void CtrlRebinding(GamePadState gps)
        //Purpose: Allows the user to rebind his/her buttons on the controller.
        //Parameters: GamePadState which is the state for the current controller.
        //Returns: None.
        //*********************************************************************
        private void CtrlRebinding()
        {
            // read new binding keys
            if (remapSTG == 0)
                _ctrl.ButtonPress(ref _cLeft, ref remapSTG);
            if (remapSTG == 1)
                _ctrl.ButtonPress(ref _cRight, ref remapSTG);
            if (remapSTG == 2)
                _ctrl.ButtonPress(ref _cPause, ref remapSTG);
            if (remapSTG == 3)
                _ctrl.ButtonPress(ref _cStart, ref remapSTG);
            if (remapSTG == 4)
            {
                int dummy = 0;
                _ctrl.ButtonPress(ref dummy, ref remapSTG);
            }
            if (remapSTG == 5)
            {
                if (_ctrl._oldState == Game.GameState.Start && _game._state == Game.GameState.Remap)
                    _game.ChangeState(_ctrl._oldState);
                else
                    _game.ChangeState(Game.GameState.Paused);
            }
        }

        //*********************************************************************
        //Method: private void RemapScreen(Graphics bg, Font font)
        //Purpose: Displays the rebinding screen and appropriate prompts to the player.
        //Parameters: Graphics being the double buffered graphics to draw out the prompts on and font being the font for the prompt.
        //Returns: None.
        //*********************************************************************
        private void RemapScreen(Graphics bg, Font font)
        {
            // show binding prompts
            if (remapSTG == 0)
                bg.DrawString("Press a Key/Button to bind LEFT MOVEMENT", font, new SolidBrush(System.Drawing.Color.Black), new PointF(110, 150));
            if (remapSTG == 1)
                bg.DrawString("Press a Key/Button to bind RIGHT MOVEMENT", font, new SolidBrush(System.Drawing.Color.Black), new PointF(110, 150));
            if (remapSTG == 2)
                bg.DrawString("Press a Key/Button to bind START", font, new SolidBrush(System.Drawing.Color.Black), new PointF(110, 150));
            if (remapSTG == 3)
                bg.DrawString("Press a Key/Button to bind PAUSE", font, new SolidBrush(System.Drawing.Color.Black), new PointF(110, 150));
            if (remapSTG == 4)
            {
                if (!_xboxConnect)
                {
                    // attempt to save bindings
                    if (_keys.ChangeBinding(_cLeft, _cRight, _cPause, _cStart))
                        bg.DrawString("Bindings SAVED!", font, new SolidBrush(System.Drawing.Color.LimeGreen), new PointF(255, 50));
                    else
                    {
                        bg.DrawString("\tBindings FAILED!\nREVERTING BACK TO DEFAULT BINDINGS", font, new SolidBrush(System.Drawing.Color.Red), new PointF(150, 50));
                        _keys.ChangeBinding(37, 39, 66, 32);
                    }

                    // show new/default bindings
                    bg.DrawString("LEFT MOVEMENT : " + (System.Windows.Forms.Keys)_keys._left
                        + "\nRIGHT MOVEMENT : " + (System.Windows.Forms.Keys)_keys._right
                        + "\nPAUSE : " + (System.Windows.Forms.Keys)_keys._pause
                        + "\nSTART : " + (System.Windows.Forms.Keys)_keys._startGame
                        , font, new SolidBrush(System.Drawing.Color.Black), new PointF(250, 150));

                    bg.DrawString("Press any key to go back!", font, new SolidBrush(System.Drawing.Color.DarkRed), new PointF(225, 450));
                }
                else
                {   
                    // attempt to save bindings
                    if (_ctrl.ChangeBinding(_cLeft, _cRight, _cPause, _cStart))
                        bg.DrawString("Bindings SAVED!", font, new SolidBrush(System.Drawing.Color.LimeGreen), new PointF(255, 50));
                    else
                    {
                        bg.DrawString("\tBindings FAILED!\nREVERTING BACK TO DEFAULT BINDINGS", font, new SolidBrush(System.Drawing.Color.Red), new PointF(150, 50));
                        _ctrl.ChangeBinding(4, 8, 16, 4096);
                    }
                    // show new/default bindings
                    bg.DrawString("LEFT MOVEMENT : " + (Microsoft.Xna.Framework.Input.Buttons)_ctrl._left
                        + "\nRIGHT MOVEMENT : " + (Microsoft.Xna.Framework.Input.Buttons)_ctrl._right
                        + "\nPAUSE : " + (Microsoft.Xna.Framework.Input.Buttons)_ctrl._pause
                        + "\nSTART : " + (Microsoft.Xna.Framework.Input.Buttons)_ctrl._startGame
                        , font, new SolidBrush(System.Drawing.Color.Black), new PointF(250, 150));

                    bg.DrawString("Press " + (Microsoft.Xna.Framework.Input.Buttons)_ctrl._pause + " to go back!", font, new SolidBrush(System.Drawing.Color.DarkRed), new PointF(225, 450));
                }
            }
        }

        #region boss_level_functions

        //*********************************************************************
        //Method:  private void BossLevel(Graphics bg)
        //Purpose: Purely for the fitfh level. This will generate the boss brick, handle boss abilties 
        //         and frost aura, and handle the collision with the ball and the boss.
        //Parameters: Graphics being the double buffered graphics to draw out the enemy balls and boss.
        //Returns: None.
        //*********************************************************************
        private void BossLevel(Graphics bg)
        {
            if (_game._level == 5)
            {
                BossAbilties();

                int frequency = 0;
                //_red = 0;
                //_blue = 0;
                //_green = 0;

                // generate random numbers where it shoots balls down and ability unlocks
                if (_red == 0 && _blue != 0)                                      // boss speeds up the ball shooting a tiny bit
                    frequency = _rng.Next(1, 151);
                else if (_red == 0 && _blue == 0 && _green != 0)
                {
                    frequency = _rng.Next(1, 101);                               // boss speeds up ball shooting when half way
                    _frostOn = true;                                             // frost aura enabled for paddle
                    _player.ChangeColor(System.Drawing.Color.LightCyan);        // indicate the user that there is an aura following the paddle
                }
                else if (_red == 0 && _blue == 0 && _green == 0 || _berserk)
                {
                    frequency = _rng.Next(10, 16);                               // last stand or berserk
                    _frostOn = false;                                            // disable all power ups
                    _invert = true;                                              // enable all boss abilities
                    _slowball = true;
                    _colorful = true;
                    _ball.SlowBall(true);

                    _player.ChangeColor(System.Drawing.Color.Green);
                }
                else
                    frequency = _rng.Next(1, 251);                              // boss acts normal when it's near full health

                // ALWAYS GENERATE BALLS WHERE EVER THE PADDLE IS LOCATED AT
                if (frequency == 10 && _lBricks.Count > 0)
                    _leBall.Add(new Ball(new PointF(_rng.Next((int)_player._pos.X - 10, (int)_player._pos.X + _player._width + 10),
                        _lBricks[0]._pos.Y + (int)_lBricks[0]._width),
                        System.Drawing.Color.FromArgb(_red, _green, _blue),
                        15, Ball.Type.Enemy));


                for (int i = 0; i < _leBall.Count; ++i)
                {
                    if (_game._state == Game.GameState.Running)
                        _leBall[i].Move(new Size(this.Width, this.Height));

                    if (_leBall[i].PaddleHit(_player, bg))
                    {
                        // change the ball color according to the boss' health
                        if (_red > 0)
                            _leBall[i].ChangeColor(System.Drawing.Color.Red);
                        else if (_blue > 0)
                            _leBall[i].ChangeColor(System.Drawing.Color.Blue);
                        else if (_green > 0)
                            _leBall[i].ChangeColor(System.Drawing.Color.Green);
                        else
                            _leBall[i].ChangeColor(System.Drawing.Color.Black);

                        // turn the ball to become friendly
                        if (_leBall[i]._type == Ball.Type.Enemy)
                            _leBall[i]._type = Ball.Type.Friendly;
                    }

                    // checks for if there is the boss brick alive
                    if (_lBricks.Count > 0)
                    {
                        // remove the balls when it hits the boss brick
                        if (_leBall[i].BrickHit(_lBricks[0], bg))
                        {
                            BossHit();
                            _leBall.RemoveAt(i);
                        }

                        // check if the ball is below the bottom of the paddle
                        else if (_leBall[i].BallDead(_player))
                        {
                            _game.LoseLife();
                            _leBall.Clear();
                            // reset player and ball
                            _ball = new Ball(new PointF(), System.Drawing.Color.Red, 18);
                            _player = new Player(new PointF(350, 475), System.Drawing.Color.Green);
                            _ball.StartingBall(_player);
                        }
                    }
                }
            }
        }

        //*********************************************************************
        //Method:  private void BossHit()
        //Purpose: Purely for the fitfh level. But it will decrease the boss' health each time a ball hits it.
        //Parameters: None.
        //Returns: None.
        //*********************************************************************
        private void BossHit()
        {
            // go through all colors in From.Argb till black (255,255,255) -> (0,0,0)
            // user have to hit the brick lord 66 times to get through
            if (_red != 0)
                _red -= 20;
            if (_red == 0 && _blue != 0)
                _blue -= 20;
            if (_red == 0 && _blue == 0 && _green != 0)
                _green -= 20;
            if (_red == 0 && _blue == 0 && _green == 0)
            {
                _lBricks[0].MarkofDeath();
                _game.ChangeState(Game.GameState.Over);
            }

            _lBricks[0].ChangeColor(System.Drawing.Color.FromArgb(_red, _blue, _green));
        }

        //*********************************************************************
        //Method: private void BossAbilties()
        //Purpose: Simply to reduce code for the boss' abilities. It will simply turn on or off the abilties according to the
        //         ability's duration/stop watches.
        //Parameters: None.
        //Returns: None.
        //*********************************************************************
        private void BossAbilties()
        {
            // each time a boss does an ability, reset the the ability timer and set the duration the ability
            if (_invertSW.Elapsed.Seconds == _rng.Next(10, 60) && !_berserk && !_invert)
            {
                _invert = true;    

                _invertSW.Restart();
            }
            if (_colorSW.Elapsed.Seconds == _rng.Next(20, 60) && !_berserk && !_colorful)
            {
                _colorful = true;
                _colorSW.Restart();
            }
            if (_slowSW.Elapsed.Seconds == _rng.Next(5, 60) && !_berserk && !_slowball)
            {
                _slowball = true;
                _ball.SlowBall(true);
                _slowSW.Restart();
            }

            // launch ability durations
            if (_invert && !_berserk && _invertSW.Elapsed.Seconds > 15)
                _invert = false;
            if (_colorful && !_berserk && _colorSW.Elapsed.Seconds > 10)
                _colorful = false;
            if (_slowball && !_berserk && _slowSW.Elapsed.Seconds > 20)
            {
                _slowball = false;
                _ball.SlowBall(false);
            }
            // if the boss hits beserk timer, everything goes on
            if (_berserkTimer.Elapsed.Minutes >= 3 && _berserkTimer.Elapsed.Seconds >= 0)
            {
                _berserk = true;
                _colorSW.Stop();
                _invertSW.Stop();
                _slowSW.Stop();
            }

            // if the berserk timer is on, turn on every ability
            if (_berserk)
            {
                _invert = true;
                _colorful = true;
                _slowball = true;
                _ball.SlowBall(true);
            }

            // go down peroidically after 30 seconds
            if (_berserkTimer.Elapsed.Seconds > 45)
                if (_lBricks.Count > 0 && _lBricks[0]._width < 250)
                    _lBricks[0].BossIncrease();
        }

        //*********************************************************************
        //Method: private void FrostAuraHit(Graphics bg, GraphicsPath frost)
        //Purpose: Create a frost aura around the paddle and handle with the balls getting in and out of the aura.
        //Parameters: Graphics being the double buffered graphics to draw out the enemy balls and boss and GraphicsPath for the frost aura radius for region creation.
        //Returns: None. 
        //*********************************************************************
        private void FrostAuraHit(Graphics bg, GraphicsPath frost)
        {
            for (int i = 0; i < _leBall.Count; ++i)
            {
                if (_leBall[i]._type == Ball.Type.Enemy)
                {
                    Region eBall = new Region(_leBall[i].GetPath());
                    Region aura = new Region(frost);

                    Region clone = aura.Clone();
                    clone.Intersect(eBall);

                    // if the ball hits the frost aura
                    if (!clone.IsEmpty(bg))
                    {
                        // slow the ball
                        _leBall[i].ChangeSpeed(3);
                        _leBall[i].ChangeColor(System.Drawing.Color.Cyan);
                    }
                    else
                    {
                        // if the ball isn't in the aura, normal speed
                        _leBall[i].ChangeSpeed(5);

                        if (_red > 0)
                            _leBall[i].ChangeColor(System.Drawing.Color.Red);
                        else if (_blue > 0)
                            _leBall[i].ChangeColor(System.Drawing.Color.Blue);
                        else if (_green > 0)
                            _leBall[i].ChangeColor(System.Drawing.Color.Green);
                        else
                            _leBall[i].ChangeColor(System.Drawing.Color.Black);
                    }
                }
                else
                    // if the ball was in the aura and hit the paddle, make it a friendly and revert to normal speed
                    _leBall[i].ChangeSpeed(-5);
            }
        }

        #endregion
    }
}
