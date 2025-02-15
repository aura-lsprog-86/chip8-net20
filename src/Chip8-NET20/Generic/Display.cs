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
    public abstract class Display
    {
        public class Color
        {
            private byte red;
            public byte Red
            {
                get { return red; }
                set { red = value; }
            }

            private byte green;
            public byte Green
            {
                get { return green; }
                set { green = value; }
            }

            private byte blue;
            public byte Blue
            {
                get { return blue; }
                set { blue = value; }
            }

            public int BGR
            {
                get
                {
                    return (Blue << 16) | (Green << 8) | Red;
                }

                set
                {
                    Blue = (byte) ((value >> 16) & 0xFF);
                    Green = (byte) ((value >> 8) & 0xFF);
                    Red = (byte) (value & 0xFF);
                }
            }

            public Color()
                : this(0, 0, 0)
            {
            }

            public Color(byte red, byte green, byte blue)
            {
                Red = red;
                Green = green;
                Blue = blue;
            }

            public Color(int bgr)
            {
                BGR = bgr;
            }
        }

        private Screen _scr;
        public Screen Screen
        {
            get { return _scr; }
            set { _scr = value; }
        }

        private double _h_size;
        public double HSize
        {
            get { return _h_size; }
            set { _h_size = value; }
        }

        private double _v_size;
        public double VSize
        {
            get { return _v_size; }
            set { _v_size = value; }
        }

        private Color _fore_color;
        public virtual Color ForegroundColor
        {
            get { return _fore_color; }
            set { _fore_color = value; }
        }

        private Color _back_color;
        public virtual Color BackgroundColor
        {
            get { return _back_color; }
            set { _back_color = value; }
        }

        public Display()
        {
            Screen = null;

            HSize = 1.0;
            VSize = 1.0;

            ForegroundColor = new Color();
            BackgroundColor = new Color();
        }

        public abstract void DrawFrame(object arg);
    }
}
