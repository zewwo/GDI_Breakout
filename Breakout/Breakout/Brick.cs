using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing.Drawing2D;
using System.Drawing;

namespace Breakout
{
    class Brick : GameObjects
    {
        private GraphicsPath _gr;
        public bool _death { get; private set; }
        public Type _type { get; private set; }
        public double _width { get; private set; }
        public int _length { get; private set; }

        public enum Type
        {
            Pawn,
            Rook,
            Lord,
        }

        public Brick(PointF pos, float col, float row, Color color, Type type = Type.Pawn, int width = 25, int length = 75)
            : base(new PointF(pos.X + (100 * col), pos.Y + (50 * row)), color)
        {
            _gr = new GraphicsPath();
            _width = width;
            _length = length;
            _type = type;
        }

        //*********************************************************************
        //Method: public void MarkofDeath()
        //Purpose: Marks the Brick for removal if it is hit.
        //Parameters: None.
        //Returns: None.
        //*********************************************************************
        public void MarkofDeath()
        {
            this._death = true;
        }

        //*********************************************************************
        //Method: public override GraphicsPath GetPath()
        //Purpose: Overriden to create a graphicspath for the Brick for rendering.
        //Parameters: None.
        //Returns: None.
        //*********************************************************************
        public override GraphicsPath GetPath()
        {
            GraphicsPath newPath = new GraphicsPath();
            newPath.StartFigure();
            newPath.AddRectangle(new Rectangle((int)_pos.X, (int)_pos.Y, _length, (int)_width));
            newPath.CloseFigure();

            _gr = newPath;

            return _gr;
        }

        //*********************************************************************
        //Method: public void BossIncrease()
        //Purpose: Simply for the fitfh level. Peroidically increase the height of the boss brick until the max threshold.
        //Parameters: None.
        //Returns: None.
        //*********************************************************************
        public void BossIncrease()
        {
            _width += 0.05;
        }
    }
}
