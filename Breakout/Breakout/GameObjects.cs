using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Breakout
{
    abstract class GameObjects : Game
    {
        // position of an object
        public PointF _pos { get; private set; } 
        // color of the object
        public Color _color { get; private set; }

        public GameObjects(PointF pos, Color color)
        {
            _pos = pos;
            _color = color;
        }

        //*********************************************************************
        //Method: public void ChangePos(PointF newPos)
        //Purpose: Changes the position of an object.
        //Parameters: PointF being the new postion.
        //Returns: None.
        //*********************************************************************
        public void ChangePos(PointF newPos)
        {
            _pos = newPos;
        }

        //*********************************************************************
        //Method: public void ChangeColor(Color n)
        //Purpose: Change the Color of an object.
        //Parameters: Color being the new color.
        //Returns: None.
        //*********************************************************************
        public void ChangeColor(Color n)
        {
            _color = n;
        }

        //*********************************************************************
        //Method: public void Render(Graphics gr)
        //Purpose: Create a region and renders out an object given a GraphicsPath return.
        //Parameters: Graphics being the double buffered Graphics.
        //Returns: None.
        //*********************************************************************
        public void Render(Graphics gr)
        {
            // obtain the region from the graphics path of a shape
            Region reg = new Region(this.GetPath());
            // fill the region in
            gr.FillRegion(new SolidBrush(_color), reg);
        }

        //*********************************************************************
        //Method: public abstract GraphicsPath GetPath();
        //Purpose: Allows derived classes after this to use it to create a GraphicsPath of its own object.
        //Parameters: None.
        //Returns: None.
        //*********************************************************************
        public abstract GraphicsPath GetPath();
    }
}
