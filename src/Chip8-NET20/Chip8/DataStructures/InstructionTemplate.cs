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
    public enum InstructionType
    {
        NoArguments,
        OneArgument4Bits,
        OneArgument12Bits,
        TwoArguments4BitsEach,
        TwoArguments4BitsAnd8Bits,
        ThreeArguments4BitsEach
    }

    public class InstructionTemplate : Generic.DataStructures.InstructionTemplate
    {
        private InstructionType _type;
        public InstructionType Type
        {
            get { return _type; }
            private set { _type = value; }
        }

        public InstructionTemplate(InstructionType type, string format, InstructionHandler handler)
        {
            Type = type;
            Format = format;
            Handler = handler;
        }

        public override Generic.DataStructures.Instruction FormInstruction(ushort opcode)
        {
            return (Generic.DataStructures.Instruction) new Instruction(this, opcode);
        }
    }
}
