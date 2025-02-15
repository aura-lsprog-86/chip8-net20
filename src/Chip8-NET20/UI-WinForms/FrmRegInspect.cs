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
    enum Counters
    {
        None,
        PC, SP, I
    }

    enum Timers
    {
        None,
        Delay, Sound
    }

    public partial class FrmRegInspect : Form
    {
        private Computer _comp;
        public Computer Source
        {
            private get { return _comp; }
            set
            {
                _comp = value;

                if (_comp != null)
                    updateAll();
            }
        }

        public FrmRegInspect()
        {
            InitializeComponent();
        }

        private void updateAll()
        {
            Processor proc = (Processor) Source.Processor;

            updateCounter(Counters.PC);
            updateCounter(Counters.SP);
            updateCounter(Counters.I);

            updateTimer(Timers.Delay);
            updateTimer(Timers.Sound);

            for (int i = 0; i < proc.V.Count; i++)
                updateRegister(i);
        }

        private void updateCounter(Counters c)
        {
            Processor proc = (Processor) Source.Processor;

            TextBox txtCtr = null;
            ushort val = 0;

            switch (c)
            {
                case Counters.PC:
                    txtCtr = txtPC;
                    val = proc.PC;
                    break;
                case Counters.SP:
                    txtCtr = txtSP;
                    val = (ushort) proc.SP;
                    break;
                case Counters.I:
                    txtCtr = txtI;
                    val = proc.I;
                    break;
            }

            if (txtCtr != null)
                txtCtr.Text = String.Format("0x{0:X4}", val);
        }

        private void updateTimer(Timers t)
        {
            Processor proc = (Processor) Source.Processor;

            TextBox txtTimer = null;
            byte val = 0;

            switch (t)
            {
                case Timers.Delay:
                    txtTimer = txtDelay;
                    val = proc.DelayTimer;
                    break;
                case Timers.Sound:
                    txtTimer = txtSound;
                    val = proc.SoundTimer;
                    break;
            }

            if (txtTimer != null)
                txtTimer.Text = String.Format("0x{0:X2}", val);
        }

        private void updateRegister(int index)
        {
            Processor proc = (Processor) Source.Processor;

            if (index < 0 || index >= proc.V.Count)
                return;

            TextBox txtVSel = this.Controls.Find("txtV" + String.Format("{0:X}", index), true)[0] as TextBox;

            txtVSel.Text = String.Format("0x{0:X2}", proc.V[index]);
        }

        private void FrmRegInspect_Load(object sender, EventArgs e)
        {
            if (Owner != null)
            {
                Location = new Point(
                    Owner.Location.X + (Owner.Width - Width) / 2,
                    Owner.Location.Y + (Owner.Height - Height) / 2
                );
            }
        }

        private void FrmRegInspect_FormClosing(object sender, FormClosingEventArgs e)
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

        private void OnCounterOrTimerChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "I":
                    updateCounter(Counters.I);
                    break;
                case "PC":
                    updateCounter(Counters.PC);
                    break;
                case "SP":
                    updateCounter(Counters.SP);
                    break;

                case "DelayTimer":
                    updateTimer(Timers.Delay);
                    break;
                case "SoundTimer":
                    updateTimer(Timers.Sound);
                    break;
            }
        }

        private void OnRegisterChanged(object sender, ItemStatusEventArgs<byte> e)
        {
            updateRegister(e.Index);
        }

        private void chkAutoUpdate_CheckedChanged(object sender, EventArgs e)
        {
            Processor proc = (Processor) Source.Processor;

            btnUpdate.Enabled = !chkAutoUpdate.Checked;

            if (chkAutoUpdate.Checked)
            {
                proc.PropertyChanged += new PropertyChangedEventHandler(OnCounterOrTimerChanged);
                proc.V.ItemReplaced += new NotifiableList<byte>.ItemStatusEventHandler(OnRegisterChanged);

                updateAll();
            }
            else
            {
                proc.PropertyChanged -= OnCounterOrTimerChanged;
                proc.V.ItemReplaced -= OnRegisterChanged;
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            updateAll();
        }
    }
}