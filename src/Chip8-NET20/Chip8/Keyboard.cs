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

namespace Chip8
{
    public class Keyboard : Generic.Keyboard
    {
        private Dictionary<int, int> _mappings;
        public Dictionary<int, int> Mappings
        {
            get { return _mappings; }
            private set { _mappings = value; }
        }

        public int NoKeyPress
        {
            get { return -1; }
        }

        public Keyboard()
            : base(16)
        {
            ClearLastIndex();

            Mappings = new Dictionary<int, int>();

            Mappings.Add('1', 1);
            Mappings.Add('2', 2);
            Mappings.Add('3', 3);
            Mappings.Add('4', 0xC);

            Mappings.Add('Q', 4);
            Mappings.Add('W', 5);
            Mappings.Add('E', 6);
            Mappings.Add('R', 0xD);

            Mappings.Add('A', 7);
            Mappings.Add('S', 8);
            Mappings.Add('D', 9);
            Mappings.Add('F', 0xE);

            Mappings.Add('Z', 0xA);
            Mappings.Add('X', 0);
            Mappings.Add('C', 0xB);
            Mappings.Add('V', 0xF);
        }

        public override bool GetKeyState(int index)
        {
            return key_states[index];
        }

        public override void SetKeyState(int keyCode, bool state)
        {
            int index = NoKeyPress;

            Mappings.TryGetValue(keyCode, out index);

            if (index == NoKeyPress)
                return;

            key_states[index] = state;
            LastKeyIndex = (state) ? index : NoKeyPress;
        }

        public override void ClearLastIndex()
        {
            LastKeyIndex = NoKeyPress;
        }
    }
}
