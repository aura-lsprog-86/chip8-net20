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
    public class Instruction : Generic.DataStructures.Instruction
    {
        public Instruction(InstructionTemplate template, ushort opcode)
        {
            Handler = template.Handler;
            Format = template.Format;

            Arguments = (Generic.DataStructures.InstructionArgs) new InstructionArgs(template.Type);
            ((InstructionArgs)Arguments).SetArguments(opcode);
        }

        public override void Execute()
        {
            Handler(Arguments);
        }

        public override string ToString()
        {
            switch (((InstructionArgs)Arguments).Type)
            {
                case InstructionType.NoArguments:
                    return Format;
                case InstructionType.OneArgument4Bits:
                    return String.Format(Format, (byte)Arguments[0]);
                case InstructionType.OneArgument12Bits:
                    return String.Format(Format, (ushort)Arguments[0]);
                case InstructionType.TwoArguments4BitsEach:
                case InstructionType.TwoArguments4BitsAnd8Bits:
                    return String.Format(Format, (byte)Arguments[0], (byte)Arguments[1]);
                case InstructionType.ThreeArguments4BitsEach:
                    return String.Format(Format, (byte)Arguments[0], (byte)Arguments[1], (byte)Arguments[2]);
                default:
                    return base.ToString();
            }
        }
    }
}
