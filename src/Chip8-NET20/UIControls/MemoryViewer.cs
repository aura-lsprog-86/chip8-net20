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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using Chip8;
using WindowsAPI;

namespace UIControls
{
    enum Direction
    {
        None = 0,
        Horizontal,
        Vertical,
        Both
    }

    public partial class MemoryViewer : UserControl
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private int _first_line;
        public int FirstLine
        {
            get { return _first_line; }
            set
            {
                _first_line = value;
                showVisibleLines();
            }
        }

        private int _no_lines;
        public int VisibleLineCount
        {
            get { return _no_lines; }
            set
            {
                _no_lines = value;
                showVisibleLines();
            }
        }

        private Memory _src = null;
        public Memory Source
        {
            get { return _src; }
            set
            {
                _src = value;
                dumpMemory(_src);
            }
        }

        private int _chunk_size;
        public int BytesPerLine
        {
            get { return _chunk_size; }
            set
            {
                _chunk_size = value;
                dumpMemory(Source);
            }
        }

        private int _max_bytes_line = 1;
        public int MaxByteCount
        {
            get { return _max_bytes_line; }
            private set
            {
                _max_bytes_line = value;
                OnPropertyChanged("MaxByteCount");
            }
        }

        private bool _auto_adjust_width = true;
        public bool AutoAdjustWidth
        {
            get { return _auto_adjust_width; }
            set
            {
                _auto_adjust_width = value;
                adjustDimensions(Direction.Horizontal);
            }
        }

        private StringBuilder sb;
        private List<string> lines;

        private string addressRepr = "XXXX:";
        private string byteRepr = "XX";
        private string spaceRepr = " ";

        public MemoryViewer()
        {
            InitializeComponent();

            sb = new StringBuilder();
            lines = new List<string>();

            adjustDimensions();
        }

        public void RefreshContents()
        {
            dumpMemory(Source);
        }

        private void dumpMemory(Memory src)
        {
            sb.Length = 0;

            if (Source == null)
            {
                txtContents.Text = "";
                return;
            }

            for (int offset = 0; offset < Source.Size; offset++)
            {
                int relChunkOffset = offset % BytesPerLine;

                if (relChunkOffset == 0)
                {
                    if (offset != 0)
                        sb.Append(Environment.NewLine);

                    sb.Append(String.Format("{0:X4}:", offset)).Append(spaceRepr);
                }

                sb.Append(String.Format("{0:X2}", Source[offset]));

                if (offset < Source.Size - 1 && relChunkOffset < BytesPerLine - 1)
                    sb.Append(spaceRepr);
            }

            lines.Clear();
            lines.AddRange(sb.ToString().Split(new String[] {Environment.NewLine}, StringSplitOptions.None));

            setVScrollRange();
            showVisibleLines();
        }

        private void adjustDimensions()
        {
            adjustDimensions(Direction.Both);
        }

        private void adjustDimensions(Direction dir)
        {
            if (dir == Direction.Horizontal || dir == Direction.Both)
                adjustLineToWidth();

            if (dir == Direction.Vertical || dir == Direction.Both)
                adjustLineCountToHeight();

            if (AutoAdjustWidth)
                BytesPerLine = MaxByteCount;
        }

        private void adjustLineToWidth()
        {
            Bitmap b = new Bitmap(1, 1);
            Graphics g = Graphics.FromImage(b);

            sb.Length = 0;

            int chunkSize = 1;

            sb.Append(addressRepr).Append(spaceRepr).Append(byteRepr);

            while (g.MeasureString(sb.ToString(), txtContents.Font).Width < txtContents.ClientSize.Width)
            {
                ++chunkSize;
                sb.Append(spaceRepr).Append(byteRepr);
            }

            MaxByteCount = chunkSize - 1;

            /*if (AutoAdjustWidth)
                BytesPerLine = MaxByteCount;*/

            g.Dispose();
            b.Dispose();
        }

        private void adjustLineCountToHeight()
        {
            // Original metric: inaccurate.
            // int clientHeight = txtContents.ClientSize.Height;

            // New metric: Direct USER32.dll call (Windows API).
            Functions.RECT r = new Functions.RECT();
            Functions.SendMessage(txtContents.Handle, Functions.EM_GETRECT, IntPtr.Zero, ref r);
            int clientHeight = r.Bottom - r.Top;
            
            VisibleLineCount = clientHeight / txtContents.Font.Height;

            vScrollBar.LargeChange = VisibleLineCount;

            setVScrollRange();
        }

        private void setVScrollRange()
        {
            // https://stackoverflow.com/a/2882878
            vScrollBar.Maximum = (Source == null) ? 1
                    : lines.Count - 1 + vScrollBar.LargeChange - 1;
        }

        private void showVisibleLines()
        {
            if (lines.Count == 0)
                return;

            // https://stackoverflow.com/a/1550330
            txtContents.SuspendLayout();

            sb.Length = 0;

            int possibleCount = Math.Min(VisibleLineCount, lines.Count - FirstLine);

            for (int i = 0; i < possibleCount; i++)
            {
                sb.Append(lines[FirstLine + i]);

                if (i < possibleCount - 1)
                    sb.Append(Environment.NewLine);
            }

            txtContents.Text = sb.ToString();

            txtContents.ResumeLayout();
        }

        protected void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, e);
        }

        protected void OnPropertyChanged(string propertyName)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        private void vScrollBar_ValueChanged(object sender, EventArgs e)
        {
            FirstLine = vScrollBar.Value;
        }

        private void MemoryViewer_Resize(object sender, EventArgs e)
        {
            adjustDimensions();
        }

        public void MemoryViewer_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.PageUp:
                    vScrollBar.Value -= Math.Min(vScrollBar.LargeChange, vScrollBar.Value - vScrollBar.Minimum);
                    break;

                case Keys.PageDown:
                    vScrollBar.Value += Math.Min(vScrollBar.LargeChange, lines.Count - 1 - vScrollBar.Value);
                    break;
            }
        }
    }
}