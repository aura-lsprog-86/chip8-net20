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
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Generic.Events;
using Chip8;

namespace Chip8_NET20
{
    public partial class FrmMemViewer : Form
    {
        private Computer _comp = null;
        public Computer Source
        {
            private get { return _comp; }
            set { _comp = value; }
        }

        private Memory _use = null;
        private Memory CurrentMemory
        {
            get { return _use; }
            set
            {
                if (_use != null && chkAutoUpdate.Checked)
                {
                    _use.MemoryModified -= Memory_MemoryModified;
                    _use.MemoryRangeModified -= Memory_MemoryRangeModified;
                }

                _use = value;

                if (_use != null && chkAutoUpdate.Checked)
                {
                    _use.MemoryModified += new Generic.MemoryModifiedEventHandler(Memory_MemoryModified);
                    _use.MemoryRangeModified += new Generic.MemoryRangeModifiedEventHandler(Memory_MemoryRangeModified);
                }

                memViewer.Source = _use;
            }
        }

        public FrmMemViewer()
        {
            InitializeComponent();

            adjustByteCount();
        }

        private void adjustByteCount()
        {
            nudNoBytes.Maximum = memViewer.MaxByteCount;

            if (!chkManualSize.Checked || nudNoBytes.Value > memViewer.MaxByteCount)
                nudNoBytes.Value = memViewer.MaxByteCount;
        }

        private void FrmMemViewer_Load(object sender, EventArgs e)
        {
            if (Owner != null)
            {
                Location = new Point(
                    Owner.Location.X + (Owner.Width - Width) / 2,
                    Owner.Location.Y + (Owner.Height - Height) / 2
                );
            }

            cbMemType.SelectedIndex = 0;
        }

        private void FrmMemViewer_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
            }
            else
            {
                if (chkAutoUpdate.Checked)
                    chkAutoUpdate.Checked = false;
            }
        }

        private void cbMemType_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Do not attempt to check memory of an undefined source!
            if (Source == null)
                return;

            switch (cbMemType.SelectedIndex)
            {
                // Main memory
                case 0:
                    CurrentMemory = (Memory)Source.Memory;
                    break;
                
                // Video memory
                case 1:
                    CurrentMemory = (Memory)Source.Screen.VideoRAM;
                    break;
            }
        }

        private void FrmMemViewer_KeyDown(object sender, KeyEventArgs e)
        {
            if (cbMemType.Focused || chkManualSize.Focused || nudNoBytes.Focused)
                return;

            memViewer.MemoryViewer_KeyDown(sender, e);
        }

        private void cbManualSize_CheckedChanged(object sender, EventArgs e)
        {
            nudNoBytes.Enabled = chkManualSize.Checked;
            memViewer.AutoAdjustWidth = !chkManualSize.Checked;
        }

        private void memViewer_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "MaxByteCount")
                adjustByteCount();
        }

        private void nudNoBytes_ValueChanged(object sender, EventArgs e)
        {
            if (!chkManualSize.Checked)
                return;

            memViewer.BytesPerLine = Decimal.ToInt32(nudNoBytes.Value);
        }

        private void chkAutoUpdate_CheckedChanged(object sender, EventArgs e)
        {
            btnUpdate.Enabled = !chkAutoUpdate.Checked;

            if (chkAutoUpdate.Checked)
            {
                CurrentMemory.MemoryModified += new Generic.MemoryModifiedEventHandler(Memory_MemoryModified);
                CurrentMemory.MemoryRangeModified += new Generic.MemoryRangeModifiedEventHandler(Memory_MemoryRangeModified);

                memViewer.RefreshContents();
            }
            else
            {
                CurrentMemory.MemoryModified -= Memory_MemoryModified;
                CurrentMemory.MemoryRangeModified -= Memory_MemoryRangeModified;
            }
        }

        private void Memory_MemoryModified(object sender, MemoryModifiedEventArgs e)
        {
            memViewer.RefreshContents();
        }

        private void Memory_MemoryRangeModified(object sender, MemoryRangeModifiedEventArgs e)
        {
            memViewer.RefreshContents();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            memViewer.RefreshContents();
        }
    }
}