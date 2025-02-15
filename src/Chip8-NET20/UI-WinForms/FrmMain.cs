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
using System.IO;

namespace Chip8_NET20
{
    enum Command
    {
        LoadProgram = 0,
        Start,
        Stop,
        ColdReset,
        WarmReset
    }

    public partial class FrmMain : Form
    {
        private Chip8.Computer comp;

        private Dictionary<Command, ToolStripItem[]> compositeCommands;

        private ToolStripMenuItem[] availableFonts;
        private ToolStripMenuItem[] availableForeColors;
        private ToolStripMenuItem[] availableBuzzers;
        private ToolStripMenuItem[] availableSpeeds;

        private FrmMemViewer frmMemViewer;
        private FrmRegInspect frmRegInspect;
        private FrmStackViewer frmStackViewer;
        private FrmDisassembler frmDisassembler;

        private Form[] devTools;

        public FrmMain()
        {
            InitializeComponent();

            availableFonts = new ToolStripMenuItem[]
            {
                itemFontDefault, itemFontAlt, itemFont7Seg, itemFontLowercase
            };

            availableForeColors = new ToolStripMenuItem[]
            {
                subitemForeColorAmber, subitemForeColorStrongGreen, subitemForeColorLightGreen
            };

            availableBuzzers = new ToolStripMenuItem[]
            {
                itemBuzz736Square, itemBuzz787Sine
            };

            availableSpeeds = new ToolStripMenuItem[]
            {
                itemSpeedSlow, itemSpeedNormal, itemSpeedFast
            };

            compositeCommands = new Dictionary<Command,ToolStripItem[]>();
            compositeCommands.Add(Command.LoadProgram, new ToolStripItem[] {itemLoadProg, btnLoadProg});
            compositeCommands.Add(Command.Start, new ToolStripItem[] { itemStart, btnStart });
            compositeCommands.Add(Command.Stop, new ToolStripItem[] { itemStop, btnStop });
            compositeCommands.Add(Command.ColdReset, new ToolStripItem[] { itemColdReset, btnColdReset });
            compositeCommands.Add(Command.WarmReset, new ToolStripItem[] { itemWarmReset, btnWarmReset });

            init_computer();

            frmMemViewer = new FrmMemViewer();
            frmMemViewer.Source = comp;

            frmRegInspect = new FrmRegInspect();
            frmRegInspect.Source = comp;

            frmStackViewer = new FrmStackViewer();
            frmStackViewer.Source = comp;

            frmDisassembler = new FrmDisassembler();
            frmDisassembler.Source = comp;

            devTools = new Form[]
            {
                frmMemViewer, frmRegInspect, frmStackViewer, frmDisassembler
            };
        }

        private void init_computer()
        {
            comp = new Chip8.Computer();

            comp.Buzzer = new Chip8.Buzzer();
            comp.Display = disp.Display;
            comp.Oscillator = new Display.Oscillator(60);

            itemClearScrStart.PerformClick();

            itemFontDefault.PerformClick();
            subitemForeColorAmber.PerformClick();
            itemBuzz736Square.PerformClick();
            itemSpeedNormal.PerformClick();

            comp.Oscillator.StopWhenHalted = false;
            comp.Oscillator.EmulationHalted += new Chip8.EmulationHaltedEventHandler(OnEmulationHalted);
            comp.Oscillator.Start();

            lblStatus.Text = "This is the CHIP-8 emulator.";
        }

        private void enable_cmd(Command cmd, bool enabled)
        {
            ToolStripItem[] items = new ToolStripItem[] { };
            compositeCommands.TryGetValue(cmd, out items);

            foreach (ToolStripItem item in items)
                item.Enabled = enabled;
        }

        private void exclusive_activate_menu(ToolStripMenuItem active, ToolStripMenuItem[] group)
        {
            if (active == null || group == null)
                return;

            foreach (ToolStripMenuItem item in group)
                item.Checked = false;

            active.Checked = true;
        }

        private void FrmMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            frmMemViewer.Dispose();
            comp.Oscillator.Stop();
            disp.MemoryCleanup();
        }

        private void OnLoadProgram(object sender, EventArgs e)
        {
            bool previousState = comp.Oscillator.EmulationStarted;

            if (previousState)
                itemStop.PerformClick();

            DialogResult result = openFileDialog.ShowDialog(this);

            if (result == DialogResult.Cancel)
            {
                if (previousState)
                    itemStart.PerformClick();

                return;
            }

            itemColdReset.PerformClick();

            comp.ProgramPath = openFileDialog.FileName;

            enable_cmd(Command.Start, true);

            lblStatus.Text = "Program \"" + Path.GetFileName(comp.ProgramPath) + "\" successfully loaded!";
        }

        private void OnComputerStart(object sender, EventArgs e)
        {
            enable_cmd(Command.Start, false);
            enable_cmd(Command.Stop, true);

            comp.Oscillator.EmulationStarted = true;

            lblStatus.Text = "CHIP-8 emulation successfully started.";
        }

        private void OnComputerStop(object sender, EventArgs e)
        {
            enable_cmd(Command.Start, true);
            enable_cmd(Command.Stop, false);

            comp.Oscillator.EmulationStarted = false;

            lblStatus.Text = "CHIP-8 emulation successfully stopped.";
        }

        private void OnComputerColdReset(object sender, EventArgs e)
        {
            if (comp.Oscillator.EmulationStarted)
                itemStop.PerformClick();

            enable_cmd(Command.Start, false);

            comp.PowerCycle();

            lblStatus.Text = "The CHIP-8 has been cold reset.";
        }

        private void OnComputerWarmReset(object sender, EventArgs e)
        {
            bool emuStarted = comp.Oscillator.EmulationStarted;

            enable_cmd(Command.Start, !emuStarted && comp.ProgramPath != null);
            enable_cmd(Command.Stop, emuStarted && comp.ProgramPath != null);

            comp.Restart();

            lblStatus.Text = "The CHIP-8 has been warm reset.";
        }

        private void OnOscCycleStepped(object sender)
        {
            itemStop.PerformClick();

            lblStatus.Text = "Execution cycle completed. The CHIP-8 has stopped.";
        }

        private void OnEmulationHalted(object sender, Exception ex)
        {
            Chip8.Processor proc = ((Chip8.Processor)comp.Processor);

            itemStop.PerformClick();

            MessageBox.Show(
                "An exception occurred when executing the program.\n\n" +
                "Error: " + ex.Message + "\n" +
                String.Format("Address: 0x{0:X4}\n\n", proc.PC) +
                "Emulation has been automatically stopped.\n" +
                "Please use the Register Inspector (F12, Ctrl+R, Alt+U) to check the state of the processor.",
                "Processor error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            );

            lblStatus.Text = "Exception \"" + ex.Message + "\" occurred at address " +
                    String.Format("0x{0:X4}", proc.PC) + ". Emulation has been halted.";
        }

        private void itemExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void itemClearScrStart_CheckedChanged(object sender, EventArgs e)
        {
            comp.SetOptionFlag(Chip8.Computer.Options.ClearScreenOnReset, itemClearScrStart.Checked);

            lblStatus.Text = (itemClearScrStart.Checked)
                    ? "The screen will now be cleared on reset."
                    : "The screen will NOT be cleared on reset.";
        }

        private void itemModifyI_CheckedChanged(object sender, EventArgs e)
        {
            comp.SetOptionFlag(Chip8.Computer.Options.ModifyIOnFX55AndFX65, itemModifyI.Checked);

            lblStatus.Text = (itemModifyI.Checked)
                    ? "The I register will now be modified after instructions FX55 and FX65 execute."
                    : "The I register will now be left intact after instructions FX55 and FX65 execute.";
        }

        private void itemFont_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem tsmiSender = sender as ToolStripMenuItem;
            exclusive_activate_menu(tsmiSender, availableFonts);

            comp.FontPath = Application.StartupPath + "\\" + (string) tsmiSender.Tag;

            lblStatus.Text = "System font \"" + tsmiSender.Text + "\" has been successfully loaded!";
        }

        private void itemForeColor_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem tsmiSender = sender as ToolStripMenuItem;
            exclusive_activate_menu(tsmiSender, availableForeColors);

            comp.Display.ForegroundColor = new Generic.Display.Color(Convert.ToInt32(tsmiSender.Tag.ToString(), 16));
        }

        private void itemBackColor_Click(object sender, EventArgs e)
        {
        }

        private void itemBuzzer_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem tsmiSender = sender as ToolStripMenuItem;
            exclusive_activate_menu(tsmiSender, availableBuzzers);

            comp.Buzzer.SoundPath = Application.StartupPath + "\\" + (string) tsmiSender.Tag;

            lblStatus.Text = "Buzzer \"" + tsmiSender.Text + "\" has been successfully set!";
        }

        private void itemSpeed_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem tsmiSender = sender as ToolStripMenuItem;
            exclusive_activate_menu(tsmiSender, availableSpeeds);

            comp.Processor.Speed = Int32.Parse(tsmiSender.Tag.ToString());

            lblStatus.Text = "Processor speed successfully set at " + comp.Processor.Speed.ToString() + " Hz.";
        }

        private void itemShowDevTools_CheckedChanged(object sender, EventArgs e)
        {
            menuDev.Visible = itemShowDevTools.Checked;

            foreach (ToolStripItem item in menuDev.DropDownItems)
                item.Enabled = itemShowDevTools.Checked;

            if (itemShowDevTools.Checked)
                lblStatus.Text = "Developer tools have been activated. Please check the \"Developer\" menu.";
            else
            {
                foreach (Form f in devTools)
                    if (f.Visible)
                        f.Close();

                lblStatus.Text = "Developer tools have been disabled.";
            }
        }

        private void itemDevMemViewer_Click(object sender, EventArgs e)
        {
            if (!frmMemViewer.Visible)
                frmMemViewer.Show(this);
            else
                frmMemViewer.Focus();
        }

        private void itemDevRegInspect_Click(object sender, EventArgs e)
        {
            if (!frmRegInspect.Visible)
                frmRegInspect.Show(this);
            else
                frmRegInspect.Focus();
        }

        private void itemDevStackViewer_Click(object sender, EventArgs e)
        {
            if (!frmStackViewer.Visible)
                frmStackViewer.Show(this);
            else
                frmStackViewer.Focus();
        }

        private void itemDevDisassembler_Click(object sender, EventArgs e)
        {
            if (!frmDisassembler.Visible)
                frmDisassembler.Show(this);
            else
                frmDisassembler.Focus();
        }

        private void itemDevCycleStep_CheckedChanged(object sender, EventArgs e)
        {
            comp.Oscillator.Monostable = itemDevCycleStep.Checked;

            if (itemDevCycleStep.Checked)
            {
                comp.Oscillator.CycleStepped += new Chip8.CycleSteppedEventHandler(OnOscCycleStepped);

                lblStatus.Text = "Cycle stepping is now active.";
            }
            else
            {
                comp.Oscillator.CycleStepped -= OnOscCycleStepped;

                lblStatus.Text = "Cycle stepping has been deactivated.";
            }
        }

        private void itemAbout_Click(object sender, EventArgs e)
        {
            FrmAbout frmAbout = new FrmAbout();
            frmAbout.ShowDialog(this);
        }
    }
}