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

using Notifiable;

namespace Generic
{
    public abstract class Processor : NotifiableObject
    {
        private int _speed;
        public int Speed
        {
            get { return _speed; }
            set
            {
                _speed = value;
                OnPropertyChanged("Speed");
            }
        }

        private bool _draw;
        public bool Draw
        {
            get { return _draw; }
            set { _draw = value; }
        }

        public abstract void Reset();
        public abstract void ExecuteCycles(int noCycles);
    }
}
