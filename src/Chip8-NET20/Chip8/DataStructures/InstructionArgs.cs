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

using Generic.DataStructures;

namespace Chip8.DataStructures
{
    public class InstructionArgs : Generic.DataStructures.InstructionArgs
    {
        private InstructionType _type;
        public InstructionType Type
        {
            get { return _type; }
            private set { _type = value; }
        }

        public InstructionArgs(InstructionType type)
        {
            Arguments = new List<object>();
            Type = type;
        }

        private int count_ones(int value)
        {
            int count = 0;

            while (value != 0)
            {
                value &= value - 1;
                count++;
            }

            return count;
        }

        private object get_nibbles(int opcode, int pattern, int shift)
        {
            if (count_ones(pattern) <= 8)
                return (byte)((opcode & pattern) >> shift);
            else
                return (ushort)((opcode & pattern) >> shift);
        }

        public void SetArguments(ushort opcode)
        {
            switch (Type)
            {
                case InstructionType.NoArguments:
                    break;
                case InstructionType.OneArgument4Bits:
                    Arguments.Add(get_nibbles(opcode, 0x0F00, 8));
                    break;
                case InstructionType.OneArgument12Bits:
                    Arguments.Add(get_nibbles(opcode, 0x0FFF, 0));
                    break;
                case InstructionType.TwoArguments4BitsEach:
                    Arguments.Add(get_nibbles(opcode, 0x0F00, 8));
                    Arguments.Add(get_nibbles(opcode, 0x00F0, 4));
                    break;
                case InstructionType.TwoArguments4BitsAnd8Bits:
                    Arguments.Add(get_nibbles(opcode, 0x0F00, 8));
                    Arguments.Add(get_nibbles(opcode, 0x00FF, 0));
                    break;
                case InstructionType.ThreeArguments4BitsEach:
                    Arguments.Add(get_nibbles(opcode, 0x0F00, 8));
                    Arguments.Add(get_nibbles(opcode, 0x00F0, 4));
                    Arguments.Add(get_nibbles(opcode, 0x000F, 0));
                    break;
            }
        }

        protected override object GetArgument(int index)
        {
            if (index < 0 || index >= 3)
                return null;

            if (Type == InstructionType.NoArguments)
                return null;

            if (index == 0 && Type == InstructionType.OneArgument12Bits)
                return (ushort)Arguments[index];

            return (byte)Arguments[index];
        }
    }
}
