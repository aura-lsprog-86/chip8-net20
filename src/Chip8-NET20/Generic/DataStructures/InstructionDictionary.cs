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
    public delegate void InstructionHandler(InstructionArgs args);

    public class InstructionDictionary
    {
        private Dictionary<string, InstructionTemplate> _data;
        public Dictionary<string, InstructionTemplate> Instructions
        {
            get { return _data; }
            private set { _data = value; }
        }

        private List<int> availableFilters;
        private StringBuilder sb;

        public InstructionTemplate this[string key]
        {
            set { Instructions[key] = value; }
        }

        public InstructionDictionary()
        {
            Instructions = new Dictionary<string, InstructionTemplate>();
            availableFilters = new List<int>();
            sb = new StringBuilder();

            computeFilters();
        }

        private int ascendentOrderByOnesCount(int x, int y)
        {
            if (countOnes(x) == countOnes(y))
                return x - y;

            return countOnes(x) - countOnes(y);
        }

        private void computeFilters()
        {
            for (int i = 0; i < 16; i++)
                availableFilters.Add(i);

            availableFilters.Sort(ascendentOrderByOnesCount);
        }

        private int countOnes(int num)
        {
            int count = 0;

            while (num != 0)
            {
                count++;
                num &= num - 1;
            }

            return count;
        }

        private int getNibble(ushort addr, int pattern, int shift)
        {
            return (addr & pattern) >> shift;
        }

        private string computeKey(ushort addr, int pattern)
        {
            sb.Length = 0;

            for (int nb = 0; nb < 4; nb++)
            {
                bool bit = (pattern & 1) == 1;

                if (bit)
                    sb.Insert(0, "X");
                else
                    sb.Insert(0, String.Format("{0:X}", getNibble(addr, 0xF << (4 * nb), 4 * nb)));

                pattern >>= 1;
            }

            return sb.ToString();
        }

        public bool TryGetValue(ushort opcode, out InstructionTemplate value)
        {
            string key = "";
            foreach (int filter in availableFilters)
            {
                key = computeKey(opcode, filter);
                if (Instructions.ContainsKey(key))
                    break;
            }

            if (key == "")
            {
                value = null;
                return false;
            }

            return Instructions.TryGetValue(key, out value);
        }
    }
}
