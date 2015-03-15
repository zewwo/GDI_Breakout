using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Breakout
{
    class Ball : GameObjects
    {
        // ball velocities
        private double _xSpeed;
        private double _ySpeed;
        private GraphicsPath _gr;
        private int _size;

        public Type _type;
        public PointF tempVelos;
        public enum Type
        {
            Friendly,
            Enemy,
        }

        public Ball(PointF pos, Color color, int size, Type type = Type.Friendly)
            :base(pos, color)
        {
            Random _rng = new Random();

            while (_xSpeed == 0 || _xSpeed == -1 || _xSpeed == 1)
                _xSpeed = _rng.Next(-5, 5);

            _ySpeed = -5.0;

            if (type == Type.Enemy)
            {
                _ySpeed = 5.0;
                _xSpeed = 0.0;
            }

            _type = type;
            _size = size;
            tempVelos = new PointF((float)_xSpeed, (float)_ySpeed);

            _gr = new GraphicsPath();
        }

        //*********************************************************************
        //Method: public void StartingBall(Player _player)
        //Purpose: Set the Ball's position slightly above the paddle when it is starting a level or game.
        //Parameters: Player being the paddle.
        //Returns: None.
        //*********************************************************************
        public void StartingBall(Player _player)
        {
            ChangePos(new PointF(_player._pos.X + 28, _player._pos.Y - 25)); 
        }

        //*********************************************************************
        //Method: public void Move(Size boundary)
        //Purpose: Move the paddle within the Game window.
        //Parameters: Size being the Game window dimensions.
        //Returns: None.
        //*********************************************************************
        public void Move(Size boundary)
        {
            // check for out of bounds
            if (_pos.X + _size > boundary.Width - _size || _pos.X < 0)
                _xSpeed *= -1.0;
            if (_pos.Y < 10 + _size)
                _ySpeed *= -1.0;

            // move the ball
            ChangePos(new PointF(_pos.X + (float)_xSpeed, _pos.Y + (float)_ySpeed));
        }

        //*********************************************************************
        //Method: public bool BrickHit(Brick brick, Graphics bg)
        //Purpose: Check for collision with a brick which will make the ball bounce back according to where the collision happened.
        //Parameters: Brick being the checking brick and Graphics being the double buffered Graphics.
        //Returns: None.
        //*********************************************************************
        public bool BrickHit(Brick brick, Graphics bg)
        {
            // calculate the distance from the paddle
            double distFromBrick = Math.Sqrt(Math.Pow(brick._pos.X - _pos.X, 2) + Math.Pow(brick._pos.Y - _pos.Y, 2));
            
            // create a region if the ball is within the range of the brick
            if (distFromBrick <= Math.Pow(_size / 2, 2) || brick._type == Brick.Type.Lord)
            {
                Region brickRegion = new Region(brick.GetPath());
                Region hit = brickRegion.Clone();

                // perform intersection test on two regions
                hit.Intersect(new Region(this._gr));

                // if there was a small portion of intersection
                if (!hit.IsEmpty(bg))
                {
                    // ball hits on the left side of the brick
                    if (_pos.X + (_size / 2) < brick._pos.X 
                        && _pos.Y + (_size / 2) >= brick._pos.Y 
                        && _pos.Y + (_size / 2) <= brick._pos.Y + brick._width)
                    {
                        if (_xSpeed > 0)
                            _xSpeed *= -1;
                    }
                    // ball hits on the right side of the brick
                    else if (_pos.X + (_size / 2) > brick._pos.X + brick._length 
                        && _pos.Y + (_size / 2) >= brick._pos.Y
                        && _pos.Y + (_size / 2) <= brick._pos.Y + brick._width)
                    {
                        if (_xSpeed < 0)
                            _xSpeed *= -1;
                    }

                    // ball hits the top/bottom of a brick
                    else if (_pos.Y <= brick._pos.Y + brick._width || _pos.Y + _size >= brick._pos.Y + brick._width
                        && _pos.X + (_size / 2) >= brick._pos.X
                        && _pos.X + (_size / 2) <= brick._pos.X + brick._length)
                    {
                        if (brick._type == Brick.Type.Lord)
                            ChangePos(new PointF(_pos.X, _pos.Y + 5));

                        _ySpeed *= -1;
                    }
                    
                    return true;
                }
            }
            return false;
        }

        //*********************************************************************
        //Method: public bool PaddleHit(Player player, Graphics bg)
        //Purpose: Check for collision with the paddle which make the ball bounce back. 
        //Parameters: Player being the paddle and Graphics being the double buffered Graphics.
        //Returns: None
        //*********************************************************************
        public bool PaddleHit(Player player, Graphics bg)
        {
            // calculate the distance from the paddle
            double distFromPaddle = Math.Sqrt(Math.Pow(player._pos.X - _pos.X, 2) + Math.Pow(player._pos.Y - _pos.Y, 2));

            // create a region if the ball is within the range of the paddle
            if (distFromPaddle <= Math.Pow(_size / 2, 2))
            {
                Region paddleRegion = new Region(player.GetPath());
                Region playerHit = paddleRegion.Clone();

                // check if the ball hit the paddle
                playerHit.Intersect(new Region(this._gr));

                if (!playerHit.IsEmpty(bg))
                {
                    // check if it lands on the paddle
                    if (_pos.Y < player._pos.Y
                        && _pos.X + _size > player._pos.X
                        && _pos.X < player._pos.X + player._length)
                    {
                        float offset = _pos.Y;
                        offset = player._pos.Y - 15;
                        ChangePos(new PointF(_pos.X, offset));

                        this._ySpeed *= -1;

                        if (_type == Type.Enemy)
                            _type = Type.Friendly;
                    }
                    // ball hits on the right side of the paddle, not on top
                    else if (_pos.Y >= player._pos.Y
                        && _pos.X > player._pos.X + player._length)
                    {
                        if (_xSpeed < 0)
                            _xSpeed *= -1;
                    }
                    // ball hits on the left side of the paddle, not on top
                    else if (_pos.Y >= player._pos.Y
                        && _pos.X < player._pos.X)
                    {
                        if (_xSpeed > 0)
                            _xSpeed *= -1;
                    }

                    return true;
                }
            }

            return false;
        }

        //*********************************************************************
        //Method: public bool BallDead(Player player)
        //Purpose: Checks if the ball goes below the paddle.
        //Parameters: Player being the paddle.
        //Returns: Returns True if it does which means the player will lose a life or false if it doesnt.
        //*********************************************************************
        public bool BallDead(Player player)
        {
            return this._pos.Y + _size > (player._pos.Y + 75);
        }

        //*********************************************************************
        //Method: public override GraphicsPath GetPath()
        //Purpose: Overriden to create a graphicspath for the ball for rendering.
        //Parameters: None.
        //Returns: None.
        //*********************************************************************
        public override GraphicsPath GetPath()
        {
            GraphicsPath newPath = new GraphicsPath();
            newPath.StartFigure();
            newPath.AddEllipse(new Rectangle((int)_pos.X, (int)_pos.Y, _size, _size));
            newPath.CloseFigure();
            _gr = newPath;

            return _gr;
        }

        //*********************************************************************
        //Method: public void ChangeSpeed(int y)
        //Purpose: Changes the speed of the ball.
        //Parameters: int being the new speed.
        //Returns: None.
        //*********************************************************************
        public void ChangeSpeed(int y)
        {
            _ySpeed = y;
        }

        //*********************************************************************
        //Method: public void SlowBall(bool on)
        //Purpose: Simply for the fitfh level. It will slow the ball down if the boss' ability is on or keep it default if it isn't on.
        //Parameters: Bool reprsenting whether the ability is on or off.
        //Returns: None.
        //*********************************************************************
        public void SlowBall(bool on)
        {
            if (_ySpeed > 0)
            {
                if (on)
                    _ySpeed = 2;
                else
                    _ySpeed = 5;
            }
            else
            {
                if (on)
                    _ySpeed = -2;
                else
                    _ySpeed = -5;
            }
        }
    }
}
