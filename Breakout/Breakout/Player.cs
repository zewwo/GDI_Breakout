using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Breakout
{
    class Player : GameObjects
    {
        // graphics path for the paddle
        private GraphicsPath _gr;
        // incrementing movement number
        public const int _MOVE = 10;
        // dimensions of the paddle
        public int _length { get; private set; }
        public int _width { get; private set; }

        public Player(PointF pos, Color color)
            :base(pos, color)
        {
            _length = 75;
            _width = 10;
            _gr = new GraphicsPath();
        }

        //*********************************************************************
        //Method: public void Move(bool left, bool right, int gameWidth, Ball ball, Game game)
        //Purpose: Move the paddle left or right.
        //Parameters: Two bools representing the direction so the player can hold down the key than pressing it repeatedly.
        //            gameWidth being the size of the Game(form), Ball and Game are self-explanatory.
        //Returns: None.
        //*********************************************************************
        public void Move(bool left, bool right, int gameWidth, Ball ball, Game game)
        {
            // move the paddle and don't allow it to go out of bounds too
            if (left)
                if (this._pos.X >= _MOVE)
                {
                    ChangePos(new PointF(_pos.X - _MOVE, _pos.Y));

                    // move the ball while the paddle is moving before starting the game
                    if (game._state == Game.GameState.Initial)
                        ball.ChangePos(new PointF(ball._pos.X - _MOVE, ball._pos.Y));
                }
            if (right)
                if (this._pos.X + 75 <= gameWidth - _MOVE * 2)
                {
                    ChangePos(new PointF(_pos.X + _MOVE, _pos.Y));

                    // move the ball while the paddle is moving before starting the game
                    if (game._state == Game.GameState.Initial)
                        ball.ChangePos(new PointF(ball._pos.X + _MOVE, ball._pos.Y));
                }
        }

        //*********************************************************************
        //Method: public override GraphicsPath GetPath()
        //Purpose: Overriden method that returns a GraphicsPath of the Paddle.
        //Parameters: None.
        //Returns: None.
        //*********************************************************************
        public override GraphicsPath GetPath()
        {
            GraphicsPath newPath = new GraphicsPath();
            newPath.StartFigure();
            newPath.AddRectangle(new Rectangle((int)_pos.X, (int)_pos.Y, _length, _width));
            newPath.CloseFigure();
            _gr = newPath;

            return _gr;
        }
    }
}
