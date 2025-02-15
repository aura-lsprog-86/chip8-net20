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

using Generic.Events;

namespace Generic
{
    public delegate void MemoryModifiedEventHandler(object sender, MemoryModifiedEventArgs e);
    public delegate void MemoryRangeModifiedEventHandler(object sender, MemoryRangeModifiedEventArgs e);

    public class Memory
    {
        private int _size;
        public int Size
        {
            get { return _size; }
            protected set
            {
                int new_size = value;
                int new_buf_size = (new_size == 0) ? 0 :
                    ((buf_size == 0) ? 1 : buf_size);

                if (new_buf_size == 0)
                    Data.Clear();
                else
                {
                    if (new_size > _size)
                    {
                        while (new_buf_size < new_size)
                            new_buf_size *= 2;
                    }
                    else
                    {
                        while (new_buf_size > new_size)
                            new_buf_size /= 2;
                    }

                    if (buf_size != new_buf_size)
                    {
                        Data.Capacity = new_buf_size;

                        if (new_size > _size)
                        //if (new_buf_size > buf_size)
                        {
                            for (int i = buf_size; i < new_buf_size; i++)
                                Data.Add(0);
                        }
                        //else // if (new_buf_size < buf_size)
                        //{
                        //    while (Data.Count > new_buf_size)
                        //        Data.RemoveAt(Data.Count - 1);
                        //}
                    }
                }

                _size = new_size;
                buf_size = new_buf_size;
            }
        }

        private List<byte> _data;
        public List<byte> Data
        //private NotifiableList<byte> _data;
        //public NotifiableList<byte> Data
        {
            get { return _data; }
            private set { _data = value; }
        }

        private int buf_size;

        public event MemoryModifiedEventHandler MemoryModified;
        public event MemoryRangeModifiedEventHandler MemoryRangeModified;

        public byte this[int idx]
        {
            get { return Data[idx]; }
            set
            {
                Data[idx] = value;
                OnMemoryModified(idx, value);
            }
        }

        public Memory()
            : this(0)
        {
        }

        public Memory(int size)
        {
            Data = new List<byte>();
            //Data = new NotifiableList<byte>();
            Size = size;
        }

        public void Clear()
        {
            Clear(0);
        }

        public void Clear(byte value)
        {
            SetValue(value, 0);
        }

        public void SetValue(byte value, int start)
        {
            for (int i = start; i < Size; i++)
                Data[i] = value;

            OnMemoryRangeModified(start, Size - 1);
        }

        public void SetRange(byte[] range, int start)
        {
            if (start < 0 || start >= Size)
                throw new Exception("Start position is out of bounds.");

            if (start + range.Length > Size)
                throw new Exception("Byte data is too large.");

            for (int i = 0; i < range.Length; i++)
                Data[start + i] = range[i];

            OnMemoryRangeModified(start, start + range.Length - 1);
        }

        private void OnMemoryModified(MemoryModifiedEventArgs e)
        {
            MemoryModifiedEventHandler handler = MemoryModified;
            if (handler != null)
                handler(this, e);
        }

        private void OnMemoryRangeModified(MemoryRangeModifiedEventArgs e)
        {
            MemoryRangeModifiedEventHandler handler = MemoryRangeModified;
            if (handler != null)
                handler(this, e);
        }

        private void OnMemoryModified(int index, byte value)
        {
            OnMemoryModified(new MemoryModifiedEventArgs(index, value));
        }

        private void OnMemoryRangeModified(int start, int end)
        {
            OnMemoryRangeModified(new MemoryRangeModifiedEventArgs(start, end));
        }
    }
}
