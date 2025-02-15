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

namespace Generic.DataStructures
{
    public abstract class Instruction
    {
        private InstructionArgs _args;
        public InstructionArgs Arguments
        {
            get { return _args; }
            protected set { _args = value; }
        }

        private InstructionHandler _handler;
        public InstructionHandler Handler
        {
            get { return _handler; }
            protected set { _handler = value; }
        }

        private string _fmt;
        public String Format
        {
            get { return _fmt; }
            set { _fmt = value; }
        }
        
        public abstract void Execute();
    }
}
