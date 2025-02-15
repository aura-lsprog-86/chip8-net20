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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Display
{
    public partial class DisplayUI : UserControl
    {
        private Display _disp;
        public Display Display
        {
            get { return _disp; }
            private set { _disp = value; }
        }

        private BufferedGraphicsContext bgc;
        private BufferedGraphics bg;

        public DisplayUI()
        {
            InitializeComponent();

            Display = new Display(this);

            bgc = BufferedGraphicsManager.Current;
            bg = null;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            //base.OnPaint(e);

            if (bg == null)
            {
                //Rectangle bounds = new Rectangle(0, 0, Width, Height);
                //bg = bgc.Allocate(CreateGraphics(), bounds);
                bg = bgc.Allocate(CreateGraphics(), DisplayRectangle);
            }

            // Unconditional drawing occurs here!
            if (_disp.Screen != null)
                _disp.DrawFrame(new DisplayInfo(bg.Graphics, DisplayRectangle));

            bg.Render();
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            //base.OnPaintBackground(e);
        }

        private void DisplayUI_Resize(object sender, EventArgs e)
        {
            if (bg != null)
            {
                bg.Dispose();
                bg = null;
            }

            Invalidate();
        }

        public void MemoryCleanup()
        {
            if (bg != null)
                bg.Dispose();
        }

        private void DisplayUI_KeyDown(object sender, KeyEventArgs e)
        {
            _disp.Keyboard.SetKeyState((int) e.KeyCode, true);
        }

        private void DisplayUI_KeyUp(object sender, KeyEventArgs e)
        {
            _disp.Keyboard.SetKeyState((int) e.KeyCode, false);
        }
    }
}