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
    public abstract class Keyboard
    {
        private int _no_keys;
        public int KeyCount
        {
            get { return _no_keys; }
            protected set { _no_keys = value; }
        }

        private int _last_key;
        public int LastKeyIndex
        {
            get { return _last_key; }
            protected set { _last_key = value; }
        }

        protected List<bool> key_states;

        public Keyboard(int noKeys)
        {
            KeyCount = noKeys;
            key_states = new List<bool>();

            for (int i = 0; i < KeyCount; i++)
                key_states.Add(false);
        }

        public abstract bool GetKeyState(int index);
        public abstract void SetKeyState(int keyCode, bool state);
        public abstract void ClearLastIndex();
    }
}
