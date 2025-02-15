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
using System.ComponentModel;

namespace Chip8
{
    public delegate void CycleSteppedEventHandler(object sender);
    public delegate void EmulationHaltedEventHandler(object sender, Exception ex);

    public abstract class Oscillator : Generic.Oscillator
    {
        public event CycleSteppedEventHandler CycleStepped;
        public event EmulationHaltedEventHandler EmulationHalted;

        private int countFreq = 0;
        private int countInstr = 0;

        private int _no_instr = 0;
        private int InstructionCount
        {
            get { return _no_instr; }
            set { _no_instr = value; }
        }

        private Display _disp;
        public Display Display
        {
            get { return _disp; }
            set { _disp = value; }
        }

        private Processor _proc;
        public Processor Processor
        {
            get { return _proc; }
            set
            {
                if (_proc != null)
                    _proc.PropertyChanged -= OnProcSpeedChanged;

                _proc = value;

                if (_proc != null)
                {
                    _proc.PropertyChanged += new PropertyChangedEventHandler(OnProcSpeedChanged);
                    updateInstrCount();
                }
            }
        }

        private bool _emu_start;
        public bool EmulationStarted
        {
            get { return _emu_start; }
            set { _emu_start = value; }
        }

        private bool _stop_halted = false;
        public bool StopWhenHalted
        {
            get { return _stop_halted; }
            set { _stop_halted = value; }
        }

        public Oscillator()
            : this(0)
        {
        }

        public Oscillator(int freq)
            : base(freq)
        {
            _emu_start = false;
        }

        public override void CycleCompleted()
        {
            if (Processor == null)
                return;

            if (EmulationStarted)
            {
                int noInstr = (Monostable) ? 1 : InstructionCount;
                bool updTimer = (Monostable)
                        ? (countInstr == InstructionCount - 1)
                        : true;

                if (Monostable)
                    countInstr = (countInstr + 1) % InstructionCount;

                Processor.ExecuteCycles(noInstr, updTimer);
            }

            if (Processor.Draw)
            {
                Processor.Draw = false;

                if (Display != null)
                    Display.Update();
            }

            if (++countFreq == Frequency)
                countFreq = 0;

            if (EmulationStarted && Monostable)
                OnCycleStepped();
        }

        protected void OnCycleStepped()
        {
            CycleSteppedEventHandler handler = CycleStepped;
            if (handler != null)
                handler(this);
        }

        private void updateInstrCount()
        {
            InstructionCount = (int)(Processor.Speed / Frequency);
        }

        private void OnProcSpeedChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "Speed")
                return;

            updateInstrCount();
        }

        public void HaltEmulation(Exception ex)
        {
            if (StopWhenHalted)
                EmulationStarted = false;

            OnEmulationHalted(ex);
        }

        private void OnEmulationHalted(Exception ex)
        {
            EmulationHaltedEventHandler handler = EmulationHalted;
            if (handler != null)
                handler(this, ex);
        }

        public abstract void Start();
        public abstract void Stop();
    }
}
