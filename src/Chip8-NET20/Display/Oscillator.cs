/**
    The CHIP-8 emulator: an implementation in C# using .NET Framework 2.0.
    Copyright (C) 2020  Felipe BF  <smprg.6502@gmail.com>

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
using System.Windows.Forms;

namespace Display
{
    public class Oscillator : Chip8.Oscillator
    {
        private Timer timer;
        private int interval;

        public Oscillator()
            : this(0)
        {
        }

        public Oscillator(int freq)
            : base(freq)
        {
            interval = (Frequency == 0) ? 0 : 1000 / Frequency;

            timer = new Timer();
            timer.Interval = interval;
            timer.Tick += new EventHandler(OnTimerTick);
            timer.Enabled = false;
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            CycleCompleted();
        }

        public override void Start()
        {
            timer.Enabled = true;
        }

        public override void Stop()
        {
            timer.Enabled = false;
        }
    }
}
