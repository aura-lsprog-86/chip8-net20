/**
    The CHIP-8 emulator: an implementation in C# using .NET Framework 2.0.
    Copyright (C) 2025, Aura Lesse Programmer <aurasys.7608@zohomail.com>

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Text;

namespace Generic
{
    public abstract class Computer
    {
        private Processor _proc;
        public Processor Processor
        {
            get { return _proc; }
            set { _proc = value; }
        }

        private Memory _mem;
        public Memory Memory
        {
            get { return _mem; }
            set { _mem = value; }
        }

        private Screen _scr;
        public Screen Screen
        {
            get { return _scr; }
            set { _scr = value; }
        }

        private Display _disp;
        public virtual Display Display
        {
            get { return _disp; }
            set { _disp = value; }
        }

        private Oscillator _osc;
        public virtual Oscillator Oscillator
        {
            get { return _osc; }
            set { _osc = value; }
        }

        private Keyboard _kb;
        public Keyboard Keyboard
        {
            get { return _kb; }
            set { _kb = value; }
        }

        private Buzzer _buzz;
        public virtual Buzzer Buzzer
        {
            get { return _buzz; }
            set { _buzz = value; }
        }

        public abstract void PowerCycle();
        public abstract void Restart();
    }
}
