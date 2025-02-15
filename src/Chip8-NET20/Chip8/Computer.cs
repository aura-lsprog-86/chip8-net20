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
    public class Computer : Generic.Computer
    {
        public enum Options
        {
            ClearScreenOnReset = 0,
            ModifyIOnFX55AndFX65
        }

        private Display _disp;
        public new Display Display
        {
            get { return _disp; }
            set
            {
                _disp = value;

                if (_disp == null)
                    return;

                _disp.HSize = 0.8;
                _disp.VSize = 0.8;

                _disp.BackgroundColor = new Generic.Display.Color(40, 40, 40);
                _disp.ForegroundColor = new Generic.Display.Color(255, 176, 0);

                _disp.Screen = Screen;
                _disp.Keyboard = (Keyboard) Keyboard;
            }
        }

        private Oscillator _osc;
        public new Oscillator Oscillator
        {
            get { return _osc; }
            set
            {
                _osc = value;

                if (_osc == null)
                    return;

                _osc.Display = Display;
                _osc.Processor = (Processor) Processor;
            }
        }

        private Buzzer _buzz;
        public new Buzzer Buzzer
        {
            get { return _buzz; }
            set
            {
                _buzz = value;

                if (_buzz == null)
                    return;

                ((Processor)Processor).Buzzer = _buzz;
            }
        }

        private String _font_path = null;
        public String FontPath
        {
            private get { return _font_path; }
            set
            {
                _font_path = value;

                if (_font_path == null)
                    return;

                FileIO.LoadFileToMemory(_font_path, (Memory) Memory, 0x0);
            }
        }

        private String _prog_path = null;
        public String ProgramPath
        {
            get { return _prog_path; }
            set
            {
                _prog_path = value;

                if (_prog_path == null)
                    return;

                FileIO.LoadFileToMemory(_prog_path, (Memory) Memory, 0x200);
            }
        }

        private List<bool> opts;
        private Random rnd;

        public Computer()
            : base()
        {
            opts = new List<bool>();
            opts.Add(true);
            opts.Add(false);

            rnd = new Random();

            Memory = new Memory(4096);

            Screen = new Screen();
            Screen.VideoRAM = new Memory(64 * 32);
            randomize((Memory) Screen.VideoRAM);

            Keyboard = new Keyboard();

            Processor = new Processor();
            ((Processor)Processor).MainRAM = (Memory) Memory;
            ((Processor)Processor).VideoRAM = (Memory) Screen.VideoRAM;
            ((Processor)Processor).Keyboard = (Keyboard) Keyboard;

            Oscillator = null;
            Display = null;
            Buzzer = null;
        }

        public override void PowerCycle()
        {
            Restart();

            if (Memory == null)
                return;

            Memory.SetValue(0, 0x200);
            ProgramPath = null;
        }

        public override void Restart()
        {
            if (Processor == null || Screen == null)
                return;

            Processor.Reset();
            Processor.Draw = true;

            if (opts[(int) Options.ClearScreenOnReset])
                Screen.VideoRAM.Clear();
            else
                randomize((Memory) Screen.VideoRAM);
        }

        public void SetOptionFlag(Options opt, bool state)
        {
            opts[(int)opt] = state;

            switch (opt)
            {
                case Options.ModifyIOnFX55AndFX65:
                    ((Processor)Processor).ModifyI = opts[(int)opt];
                    break;
            }
        }

        private void randomize(Memory mem)
        {
            if (mem == null)
                return;

            byte[] newContents = new byte[mem.Size];

            for (int i = 0; i < mem.Size; i++)
                newContents[i] = (byte) ((rnd.Next(10) < 3) ? 1 : 0);

            mem.SetRange(newContents, 0);
        }
    }
}
