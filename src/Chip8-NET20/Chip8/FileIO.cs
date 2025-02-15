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
using System.IO;

namespace Chip8
{
    public static class FileIO
    {
        public static void LoadFileToMemory(string filePath, Memory mem, int pos)
        {
            if (filePath == null || mem == null)
                throw new Exception("No arguments must be NULL.");

            if (pos >= mem.Size)
                throw new Exception("Memory position out of bounds.");

            FileInfo fi = new FileInfo(filePath);

            if (pos + fi.Length >= mem.Size)
                throw new Exception("File size too large.");

            BinaryReader br = new BinaryReader(new FileStream(filePath, FileMode.Open));

            mem.SetRange(br.ReadBytes((int) fi.Length), pos);

            /*for (int i = 0; i < fi.Length; i++)
                mem[pos + i] = br.ReadByte();*/

            br.Close();
        }
    }
}
