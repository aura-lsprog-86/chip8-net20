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
using System.Diagnostics;

using Notifiable;
using Chip8.DataStructures;

namespace Chip8
{
    public class Processor : Generic.Processor
    {
        private ushort opcode;

        private InstructionDictionary _inst;
        public InstructionDictionary Instructions
        {
            get { return _inst; }
            private set { _inst = value; }
        }

        private ushort _i;
        public ushort I
        {
            get { return _i; }
            set
            {
                _i = value;
                OnPropertyChanged("I");
            }
        }

        private ushort _pc;
        public ushort PC
        {
            get { return _pc; }
            set
            {
                _pc = value;
                OnPropertyChanged("PC");
            }
        }

        private byte _delay_timer;
        public byte DelayTimer
        {
            get { return _delay_timer; }
            set
            {
                _delay_timer = value;
                OnPropertyChanged("DelayTimer");
            }
        }

        private byte _sound_timer;
        public byte SoundTimer
        {
            get { return _sound_timer; }
            set
            {
                _sound_timer = value;
                OnPropertyChanged("SoundTimer");
            }
        }

        private byte _sp;
        public byte SP
        {
            get { return _sp; }
            set
            {
                _sp = value;
                OnPropertyChanged("SP");
            }
        }

        private NotifiableList<ushort> _stack;
        public NotifiableList<ushort> Stack
        {
            get { return _stack; }
            private set { _stack = value; }
        }

        private NotifiableList<byte> _v;
        public NotifiableList<byte> V
        {
            get { return _v; }
            private set { _v = value; }
        }

        private bool _modify_I;
        public bool ModifyI
        {
            get { return _modify_I; }
            set { _modify_I = value; }
        }

        private bool _printed;
        public bool Printed
        {
            get { return _printed; }
            set { _printed = value; }
        }

        private Memory _main_RAM;
        public Memory MainRAM
        {
            get { return _main_RAM; }
            set { _main_RAM = value; }
        }

        private Memory _video_RAM;
        public Memory VideoRAM
        {
            get { return _video_RAM; }
            set { _video_RAM = value; }
        }

        private Keyboard _kb;
        public Keyboard Keyboard
        {
            get { return _kb; }
            set { _kb = value; }
        }

        private bool _playing;
        public bool Playing
        {
            get { return _playing; }
            set { _playing = value; }
        }

        private Buzzer _buzz;
        public Buzzer Buzzer
        {
            get { return _buzz; }
            set { _buzz = value; }
        }

        private Random _rnd;
        public Random RandomGenerator
        {
            get { return _rnd; }
            private set { _rnd = value; }
        }

        public Processor()
            : base()
        {
            Instructions = new InstructionDictionary(this);

            Draw = true;

            ModifyI = false;

            Printed = false;
            Playing = false;

            Stack = new NotifiableList<ushort>();
            for (int i = 0; i < 16; i++)
                Stack.Add(0);

            V = new NotifiableList<byte>();
            for (int i = 0; i < 16; i++)
                V.Add(0);

            // Reset random number generator.
            RandomGenerator = new Random();
        }

        public override void Reset()
        {
            // Reset pointers and opcode register.
            PC = 0x200;
            opcode = 0;
            I = 0;
            SP = 0;

            // Clear stack and registers.
            for (int i = 0; i < 16; i++)
                Stack[i] = 0;
            for (int i = 0; i < 16; i++)
                V[i] = 0;

            // Reset timers.
            DelayTimer = 0;
            SoundTimer = 0;
        }

        public override void ExecuteCycles(int noCycles)
        {
            ExecuteCycles(noCycles, true);
        }

        public void ExecuteCycles(int noCycles, bool updateTimers)
        {
            if (noCycles <= 0)
                return;

            for (int i = 0; i < noCycles; i++)
                cycle_step();

            if (updateTimers)
                update_timers();
        }

        private void cycle_step()
        {
            // No code can be executed if no RAM is present at all! :P
            if (MainRAM == null)
                return;

            // Fetch opcode. (Big Endian, no boundary checking.)
            opcode = (ushort)((MainRAM[PC] << 8) | MainRAM[PC + 1]);

            // Decode and execute opcode.
            Generic.DataStructures.InstructionTemplate instTemplate = null;
            Instructions.TryGetValue(opcode, out instTemplate);

            Instruction inst = (Instruction)instTemplate.FormInstruction(opcode);
            inst.Execute();
        }

        private void update_timers()
        {
            if (DelayTimer > 0)
                --DelayTimer;

            if (SoundTimer > 0)
                --SoundTimer;
            else if (Playing)
            {
                Playing = false;

                if (Buzzer != null)
                    Buzzer.Stop();
            }
        }
    }
}
