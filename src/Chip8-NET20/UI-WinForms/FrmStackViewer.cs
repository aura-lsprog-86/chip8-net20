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

using Chip8;
using Notifiable;

namespace Chip8_NET20
{
    public partial class FrmStackViewer : Form
    {
        private Computer _comp;
        public Computer Source
        {
            private get { return _comp; }
            set
            {
                _comp = value;

                if (_comp != null)
                {
                    updateAll();
                }
            }
        }

        public FrmStackViewer()
        {
            InitializeComponent();
        }

        private void updateAll()
        {
            Processor proc = (Processor) Source.Processor;

            txtSP.Text = String.Format("0x{0:X4}", proc.SP);

            lstStack.BeginUpdate();

            lstStack.Items.Clear();

            for (int i = 0; i < proc.SP; i++)
                lstStack.Items.Add(String.Format("0x{0:X2}: 0x{1:X4}", lstStack.Items.Count, proc.Stack[i]));

            lstStack.EndUpdate();
        }

        private void FrmStackViewer_Load(object sender, EventArgs e)
        {
            if (Owner != null)
            {
                Location = new Point(
                    Owner.Location.X + (Owner.Width - Width) / 2,
                    Owner.Location.Y + (Owner.Height - Height) / 2
                );
            }
        }

        private void FrmStackViewer_FormClosing(object sender, FormClosingEventArgs e)
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

        private void OnSPChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "SP")
                return;

            updateAll();
        }

        private void OnStackItemReplaced(object sender, ItemStatusEventArgs<ushort> e)
        {
            // Processor artifact: item assignment before SP increment/decrement.
            if (e.Index >= lstStack.Items.Count)
                return;

            lstStack.Items[e.Index] = String.Format("0x{0:X2}: 0x{1:X4}", e.Index, e.Item);
        }

        private void chkAutoUpdate_CheckedChanged(object sender, EventArgs e)
        {
            Processor proc = (Processor) Source.Processor;

            btnUpdate.Enabled = !chkAutoUpdate.Checked;

            if (chkAutoUpdate.Checked)
            {
                proc.PropertyChanged += new PropertyChangedEventHandler(OnSPChanged);
                proc.Stack.ItemReplaced += new NotifiableList<ushort>.ItemStatusEventHandler(OnStackItemReplaced);

                updateAll();
            }
            else
            {
                proc.PropertyChanged -= OnSPChanged;
                proc.Stack.ItemReplaced -= OnStackItemReplaced;
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            updateAll();
        }
    }
}