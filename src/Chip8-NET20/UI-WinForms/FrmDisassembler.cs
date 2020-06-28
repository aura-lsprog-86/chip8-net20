/**
    The CHIP-8 emulator: an implementation in C# using .NET Framework 2.0.
    Copyright (C) 2020  Felipe BF  <smprg.6502@gmail.com>

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
using Chip8.DataStructures;

namespace Chip8_NET20
{
    public partial class FrmDisassembler : Form
    {
        private Computer _comp;
        public Computer Source
        {
            get { return _comp; }
            set { _comp = value; }
        }

        private StringBuilder sb;

        public FrmDisassembler()
        {
            InitializeComponent();

            sb = new StringBuilder();
        }

        private void disassemble(ushort start, ushort end)
        {
            Generic.DataStructures.InstructionTemplate instTemplate;
            Instruction inst;

            Memory mem = (Memory)Source.Memory;
            InstructionDictionary instructions = ((Processor)Source.Processor).Instructions;

            sb.Length = 0;

            ushort opcode;
            for (int i = start; i <= end; i += 2)
            {
                if (i == end)
                {
                    sb.Append(String.Format("0x{0:X4}:  0x{1:X2}", i, mem[end]));
                    continue;
                }

                opcode = (ushort)((mem[i] << 8) | mem[i + 1]);

                instructions.TryGetValue(opcode, out instTemplate);
                inst = (Instruction)instTemplate.FormInstruction(opcode);

                object[] dumpArgs = new object[]
                        { i, opcode, inst.ToString() };

                sb.Append(String.Format("0x{0:X4}:  0x{1:X4}  {2}", dumpArgs));

                if (i < end)
                    sb.Append(Environment.NewLine);
            }

            txtDisasm.Text = sb.ToString();
        }

        private void computeEndAddr()
        {
            Memory mem = (Memory)Source.Memory;

            int endAddr = mem.Size - 1;
            while (endAddr > nudStartAddr.Value && mem[endAddr] == 0)
                endAddr--;

            nudEndAddr.Value = endAddr;
        }

        private void FrmDisassembler_Load(object sender, EventArgs e)
        {
            if (Owner != null)
            {
                Location = new Point(
                    Owner.Location.X + (Owner.Width - Width) / 2,
                    Owner.Location.Y + (Owner.Height - Height) / 2
                );
            }

            nudStartAddr.Value = 0x0200;
        }

        private void FrmDisassembler_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
            }
            else
            {
                if (chkAutoDisasm.Checked)
                    chkAutoDisasm.Checked = false;
            }
        }

        private void chkAutoSize_CheckedChanged(object sender, EventArgs e)
        {
            nudEndAddr.Enabled = !chkAutoSize.Checked;

            if (chkAutoSize.Checked)
                computeEndAddr();
        }

        private void chkAutoDisasm_CheckedChanged(object sender, EventArgs e)
        {
            Memory mem = (Memory)Source.Memory;

            btnDisassemble.Enabled = !chkAutoDisasm.Checked;

            if (chkAutoDisasm.Checked)
            {
                mem.MemoryRangeModified += new Generic.MemoryRangeModifiedEventHandler(OnMemoryRangeModified);
                mem.MemoryModified += new Generic.MemoryModifiedEventHandler(OnMemoryModified);

                disassemble(
                    Decimal.ToUInt16(nudStartAddr.Value),
                    Decimal.ToUInt16(nudEndAddr.Value)
                );
            }
            else
            {
                mem.MemoryRangeModified -= OnMemoryRangeModified;
                mem.MemoryModified -= OnMemoryModified;
            }
        }

        private void nudStartAddr_ValueChanged(object sender, EventArgs e)
        {
            if (nudEndAddr.Value < nudStartAddr.Minimum)
                nudEndAddr.Value = nudStartAddr.Minimum;

            nudEndAddr.Minimum = nudStartAddr.Value;

            if (chkAutoSize.Checked)
                computeEndAddr();

            if (chkAutoDisasm.Checked)
            {
                disassemble(
                    Decimal.ToUInt16(nudStartAddr.Value),
                    Decimal.ToUInt16(nudEndAddr.Value)
                );
            }
        }

        private void nudEndAddr_ValueChanged(object sender, EventArgs e)
        {
            if (chkAutoDisasm.Checked)
            {
                disassemble(
                    Decimal.ToUInt16(nudStartAddr.Value),
                    Decimal.ToUInt16(nudEndAddr.Value)
                );
            }
        }

        private void OnMemoryRangeModified(object sender, Generic.Events.MemoryRangeModifiedEventArgs e)
        {
            if (chkAutoSize.Checked)
                computeEndAddr();

            disassemble(
                Decimal.ToUInt16(nudStartAddr.Value),
                Decimal.ToUInt16(nudEndAddr.Value)
            );
        }

        private void OnMemoryModified(object sender, Generic.Events.MemoryModifiedEventArgs e)
        {
            if (chkAutoSize.Checked)
                computeEndAddr();

            disassemble(
                Decimal.ToUInt16(nudStartAddr.Value),
                Decimal.ToUInt16(nudEndAddr.Value)
            );
        }

        private void btnDisassemble_Click(object sender, EventArgs e)
        {
            if (chkAutoSize.Checked)
                computeEndAddr();

            disassemble(
                Decimal.ToUInt16(nudStartAddr.Value),
                Decimal.ToUInt16(nudEndAddr.Value)
            );
        }
    }
}