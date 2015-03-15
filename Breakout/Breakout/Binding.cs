using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Breakout
{
    abstract class Binding : Game
    {
        // basic bindings for both controller + keyboard
        public int _pause { get; private set; }
        public int _startGame { get; private set; }
        public int _left { get; private set; }
        public int _right { get; private set; }

        // variable to hold the old game state for use after the remapping wizard
        public GameState _oldState;

        // rebinding key cannot be changed in the rebinding wizard
        public readonly int _rebind;

        public Binding(int left, int right, int pause, int start, int rebind)
        {
            _left = left;
            _right = right;
            _pause = pause;
            _startGame = start;
            _rebind = rebind;
        }

        //*********************************************************************
        //Method: public bool ChangeBinding(int left, int right, int pause, int start)
        //Purpose: Changes the binding for the keyboard or controller. It will check for duplicate keys as well.
        //Parameters: ints being the respective controls except the rebind button : left, right, pause, start
        //Returns: Bool representing if there was any duplicate keys pressed or not.
        //*********************************************************************
        public bool ChangeBinding(int left, int right, int pause, int start)
        {
            if (left == right || left == pause || left == start || left == _rebind
                || right == pause || right == start || right == _rebind ||
                pause == start || pause == _rebind || start == _rebind)
                return false;

            _left = left;
            _right = right;
            _pause = pause;
            _startGame = start;
           
            return true;
        }
    }
}
