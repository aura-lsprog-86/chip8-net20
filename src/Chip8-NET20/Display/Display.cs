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
using System.Drawing;
using System.Text;

namespace Display
{
    public class Display : Chip8.Display
    {
        private DisplayUI ui;

        public Display(DisplayUI ui)
            : base()
        {
            this.ui = ui;
        }

        public override void DrawFrame(object arg)
        {
            DisplayInfo di = (DisplayInfo) arg;
            Graphics g = di.Graphics;

            g.Clear(System.Drawing.Color.FromArgb(
                BackgroundColor.Red,
                BackgroundColor.Green,
                BackgroundColor.Blue
            ));

            Brush foreBrush = new SolidBrush(System.Drawing.Color.FromArgb(
                ForegroundColor.Red,
                ForegroundColor.Green,
                ForegroundColor.Blue
            ));

            Size pixelDims = new Size(
                (int) ((di.Bounds.Width * HSize) / Screen.ColumnCount),
                (int) ((di.Bounds.Height * VSize) / Screen.RowCount)
            );

            Size displayArea = new Size(
                pixelDims.Width * Screen.ColumnCount,
                pixelDims.Height * Screen.RowCount
            );

            Point displayOffset = new Point(
                ((int) (di.Bounds.Width - displayArea.Width)) / 2,
                ((int) (di.Bounds.Height - displayArea.Height)) / 2
            );

            Rectangle pixel = new Rectangle();
            pixel.Size = pixelDims;

            for (int y = 0; y < Screen.RowCount; y++)
            {
                for (int x = 0; x < Screen.ColumnCount; x++)
                {
                    if (Screen.VideoRAM[Screen.ColumnCount * y + x] == 0)
                        continue;

                    pixel.X = displayOffset.X + pixelDims.Width * x;
                    pixel.Y = displayOffset.Y + pixelDims.Height * y;

                    g.FillRectangle(foreBrush, pixel);
                }
            }

            foreBrush.Dispose();
        }

        public override void Update()
        {
            ui.Invalidate();
        }
    }
}
