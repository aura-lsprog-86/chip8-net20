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

namespace Chip8.DataStructures
{
    public class InstructionDictionary : Generic.DataStructures.InstructionDictionary
    {
        private Processor _proc;
        public Processor Processor
        {
            get { return _proc; }
            set { _proc = value; }
        }

        public InstructionDictionary(Processor proc)
            : base()
        {
            Processor = proc;

            Instructions["00E0"] = new InstructionTemplate(InstructionType.NoArguments, "CLS", (Generic.DataStructures.InstructionHandler)clearScreen);
            Instructions["00EE"] = new InstructionTemplate(InstructionType.NoArguments, "RET", (Generic.DataStructures.InstructionHandler)returnFromSubroutine);
            Instructions["1XXX"] = new InstructionTemplate(InstructionType.OneArgument12Bits, "JP 0x{0:X4}", (Generic.DataStructures.InstructionHandler)jumpToAddress);
            Instructions["2XXX"] = new InstructionTemplate(InstructionType.OneArgument12Bits, "CALL 0x{0:X4}", (Generic.DataStructures.InstructionHandler)callSubroutineAtAddress);
            Instructions["3XXX"] = new InstructionTemplate(InstructionType.TwoArguments4BitsAnd8Bits, "SE V{0:X}, V{1:X2}", (Generic.DataStructures.InstructionHandler)skipIfRegEqualToConst);
            Instructions["4XXX"] = new InstructionTemplate(InstructionType.TwoArguments4BitsAnd8Bits, "SNE V{0:X}, V{1:X2}", (Generic.DataStructures.InstructionHandler)skipIfRegNotEqualToConst);
            Instructions["5XX0"] = new InstructionTemplate(InstructionType.TwoArguments4BitsEach, "SE V{0:X}, V{1:X}", (Generic.DataStructures.InstructionHandler)skipIfRegEqualToReg);
            Instructions["6XXX"] = new InstructionTemplate(InstructionType.TwoArguments4BitsAnd8Bits, "LD V{0:X}, 0x{1:X2}", (Generic.DataStructures.InstructionHandler)loadConstToReg);
            Instructions["7XXX"] = new InstructionTemplate(InstructionType.TwoArguments4BitsAnd8Bits, "ADD V{0:X}, 0x{1:X2}", (Generic.DataStructures.InstructionHandler)addConstToRegNoCarry);
            Instructions["8XX0"] = new InstructionTemplate(InstructionType.TwoArguments4BitsEach, "LD V{0:X}, V{1:X}", (Generic.DataStructures.InstructionHandler)loadRegToReg);
            Instructions["8XX1"] = new InstructionTemplate(InstructionType.TwoArguments4BitsEach, "OR V{0:X}, V{1:X}", (Generic.DataStructures.InstructionHandler)bitwiseOrRegToReg);
            Instructions["8XX2"] = new InstructionTemplate(InstructionType.TwoArguments4BitsEach, "AND V{0:X}, V{1:X}", (Generic.DataStructures.InstructionHandler)bitwiseAndRegToReg);
            Instructions["8XX3"] = new InstructionTemplate(InstructionType.TwoArguments4BitsEach, "XOR V{0:X}, V{1:X}", (Generic.DataStructures.InstructionHandler)bitwiseXorRegToReg);
            Instructions["8XX4"] = new InstructionTemplate(InstructionType.TwoArguments4BitsEach, "ADD V{0:X}, V{1:X}", (Generic.DataStructures.InstructionHandler)addRegToRegWithCarry);
            Instructions["8XX5"] = new InstructionTemplate(InstructionType.TwoArguments4BitsEach, "SUB V{0:X}, V{1:X}", (Generic.DataStructures.InstructionHandler)subtractRegToTegWithBorrow);
            Instructions["8XX6"] = new InstructionTemplate(InstructionType.TwoArguments4BitsEach, "SHR V{0:X}, V{1:X}", (Generic.DataStructures.InstructionHandler)rightShiftRegSpillCarry);
            Instructions["8XX7"] = new InstructionTemplate(InstructionType.TwoArguments4BitsEach, "SUBN V{0:X}, V{1:X}", (Generic.DataStructures.InstructionHandler)invertedSubtractRegToRegWithBorrow);
            Instructions["8XXE"] = new InstructionTemplate(InstructionType.TwoArguments4BitsEach, "SHL V{0:X}, V{1:X}", (Generic.DataStructures.InstructionHandler)leftShiftRegSpillCarry);
            Instructions["9XX0"] = new InstructionTemplate(InstructionType.TwoArguments4BitsEach, "SNE V{0:X}, V{1:X}", (Generic.DataStructures.InstructionHandler)skipIfRegNotEqualToReg);
            Instructions["AXXX"] = new InstructionTemplate(InstructionType.OneArgument12Bits, "LD I, 0x{0:X4}", (Generic.DataStructures.InstructionHandler)loadIWithAddress);
            Instructions["BXXX"] = new InstructionTemplate(InstructionType.OneArgument12Bits, "JP V0, 0x{0:X4}", (Generic.DataStructures.InstructionHandler)jumpToAddressWithOffsetInReg0);
            Instructions["CXXX"] = new InstructionTemplate(InstructionType.TwoArguments4BitsAnd8Bits, "RND V{0:X}, 0x{1:X2}", (Generic.DataStructures.InstructionHandler)bitwiseAndRandomNumberAndConst);
            Instructions["DXXX"] = new InstructionTemplate(InstructionType.ThreeArguments4BitsEach, "DRW V{0:X}, V{1:X}, 0x{2:X2}", (Generic.DataStructures.InstructionHandler)drawSpriteAndDetectCollision);
            Instructions["EX9E"] = new InstructionTemplate(InstructionType.OneArgument4Bits, "SKP V{0:X}", (Generic.DataStructures.InstructionHandler)skipIfKeypressNameInRegIsPressed);
            Instructions["EXA1"] = new InstructionTemplate(InstructionType.OneArgument4Bits, "SKNP V{0:X}", (Generic.DataStructures.InstructionHandler)skipIfKeypressNameInRegIsNotPressed);
            Instructions["FX07"] = new InstructionTemplate(InstructionType.OneArgument4Bits, "LD V{0:X}, DT", (Generic.DataStructures.InstructionHandler)loadRegWithDelayValue);
            Instructions["FX0A"] = new InstructionTemplate(InstructionType.OneArgument4Bits, "LD V{0:X}, K", (Generic.DataStructures.InstructionHandler)waitForKeypressAndStoreNameInReg);
            Instructions["FX15"] = new InstructionTemplate(InstructionType.OneArgument4Bits, "LD DT, V{0:X}", (Generic.DataStructures.InstructionHandler)loadDelayValueWithReg);
            Instructions["FX18"] = new InstructionTemplate(InstructionType.OneArgument4Bits, "LD ST, V{0:X}", (Generic.DataStructures.InstructionHandler)loadSoundTimerWithReg);
            Instructions["FX1E"] = new InstructionTemplate(InstructionType.OneArgument4Bits, "ADD I, V{0:X}", (Generic.DataStructures.InstructionHandler)addRegToI);
            Instructions["FX29"] = new InstructionTemplate(InstructionType.OneArgument4Bits, "LD F, V{0:X}", (Generic.DataStructures.InstructionHandler)loadFontSpriteStartToI);
            Instructions["FX33"] = new InstructionTemplate(InstructionType.OneArgument4Bits, "LD B, V{0:X}", (Generic.DataStructures.InstructionHandler)loadBCDOfRegToStartI);
            Instructions["FX55"] = new InstructionTemplate(InstructionType.OneArgument4Bits, "LD [I], V{0:X}", (Generic.DataStructures.InstructionHandler)dumpRegsToMemoryStartingAtI);
            Instructions["FX65"] = new InstructionTemplate(InstructionType.OneArgument4Bits, "LD V{0:X}, [I]", (Generic.DataStructures.InstructionHandler)populateRegsFromMemoryStartingAtI);
            Instructions["XXXX"] = new InstructionTemplate(InstructionType.NoArguments, "", (Generic.DataStructures.InstructionHandler)unrecognizedInstruction);
        }

        private void clearScreen(Generic.DataStructures.InstructionArgs args)
        {
            // 00E0: Clears the screen.
            if (Processor.VideoRAM != null)
                Processor.VideoRAM.Clear();

            Processor.Draw = true;
            Processor.PC += 2;
        }

        private void returnFromSubroutine(Generic.DataStructures.InstructionArgs args)
        {
            // 00EE: Returns from subroutine.
            if (Processor.SP == 0)
                throw new Exception("Stack underflow.");

            Processor.PC = Processor.Stack[--Processor.SP];
            Processor.PC += 2;
        }

        private void jumpToAddress(Generic.DataStructures.InstructionArgs args)
        {
            // 1NNN: Jump to location NNN.
            Processor.PC = (ushort)(args[0]);
        }

        private void callSubroutineAtAddress(Generic.DataStructures.InstructionArgs args)
        {
            // 2NNN: Calls the subroutine at address NNN.
            if (Processor.SP == 15)
                throw new Exception("Stack overflow.");

            Processor.Stack[Processor.SP++] = Processor.PC;
            Processor.PC = (ushort)(args[0]);
        }

        private void skipIfRegEqualToConst(Generic.DataStructures.InstructionArgs args)
        {
            // 3XKK: If register VX contains the value KK, increment PC by 2. 
            if (Processor.V[(byte)(args[0])] == (byte)(args[1]))
                Processor.PC += 2;

            Processor.PC += 2;
        }

        private void skipIfRegNotEqualToConst(Generic.DataStructures.InstructionArgs args)
        {
            // 4XKK: If register VX doesn't contain the value KK, increment PC by 2.
            if (Processor.V[(byte)(args[0])] != (byte)(args[1]))
                Processor.PC += 2;

            Processor.PC += 2;
        }

        private void skipIfRegEqualToReg(Generic.DataStructures.InstructionArgs args)
        {
            // 5XY0: If registers VX and VY are equal, increment PC by 2.
            if (Processor.V[(byte)(args[0])] == Processor.V[(byte)(args[1])])
                Processor.PC += 2;

            Processor.PC += 2;
        }

        private void loadConstToReg(Generic.DataStructures.InstructionArgs args)
        {
            // 6XKK: Put the value KK into register VX.
            Processor.V[(byte)(args[0])] = (byte)(byte)(args[1]);
            Processor.PC += 2;
        }

        private void addConstToRegNoCarry(Generic.DataStructures.InstructionArgs args)
        {
            // 7XKK: Add the value KK to register VX. (No carry generated.)
            Processor.V[(byte)(args[0])] += (byte)(byte)(args[1]);
            Processor.PC += 2;
        }

        private void loadRegToReg(Generic.DataStructures.InstructionArgs args)
        {
            // 8XY0: Stores the value of register VY in register VX.
            Processor.V[(byte)(args[0])] = Processor.V[(byte)(args[1])];
            Processor.PC += 2;
        }

        private void bitwiseOrRegToReg(Generic.DataStructures.InstructionArgs args)
        {
            // 8XY1: Perform bitwise OR between registers VX and VY.
            //       Store the result in register VX.
            Processor.V[(byte)(args[0])] |= Processor.V[(byte)(args[1])];
            Processor.PC += 2;
        }

        private void bitwiseAndRegToReg(Generic.DataStructures.InstructionArgs args)
        {
            // 8XY2: Perform bitwise AND between registers VX and VY.
            //       Store the result in register VX.
            Processor.V[(byte)(args[0])] &= Processor.V[(byte)(args[1])];
            Processor.PC += 2;
        }

        private void bitwiseXorRegToReg(Generic.DataStructures.InstructionArgs args)
        {
            // 8XY3: Perform bitwise XOR between registers VX and VY.
            //       Store the result in register VX.
            Processor.V[(byte)(args[0])] ^= Processor.V[(byte)(args[1])];
            Processor.PC += 2;
        }

        private void addRegToRegWithCarry(Generic.DataStructures.InstructionArgs args)
        {
            // 8XY4: Add the values of registers VX and VY.
            //       Store the result in register VX.
            //       If VX + VY > 0xFF, set VF = 1; otherwise, set VF = 0.
            if ((ushort)Processor.V[(byte)(args[0])] + Processor.V[(byte)(args[1])] > 0xFF)
                Processor.V[0xF] = 1;
            else
                Processor.V[0xF] = 0;

            Processor.V[(byte)(args[0])] += Processor.V[(byte)(args[1])];
            Processor.PC += 2;
        }

        private void subtractRegToTegWithBorrow(Generic.DataStructures.InstructionArgs args)
        {
            // 8XY5: Subtract the value of register VY from register VX.
            //       Store the result in register VX.
            //       If VX > VY, set VF = 1; otherwise, set VF = 0.
            if (Processor.V[(byte)(args[0])] > Processor.V[(byte)(args[1])])
                Processor.V[0xF] = 1;
            else
                Processor.V[0xF] = 0;

            Processor.V[(byte)(args[0])] -= Processor.V[(byte)(args[1])];
            Processor.PC += 2;
        }

        private void rightShiftRegSpillCarry(Generic.DataStructures.InstructionArgs args)
        {
            // 8XY6: Shift register VX 1 position to the right.
            //       Set VF to the LSB of VX before the shift.
            Processor.V[0xF] = (byte)((Processor.V[(byte)(args[0])]) & 0x1);

            Processor.V[(byte)(args[0])] >>= 1;
            Processor.PC += 2;
        }

        private void invertedSubtractRegToRegWithBorrow(Generic.DataStructures.InstructionArgs args)
        {
            // 8XY7: Subtract the value of register VX from register VY.
            //       Store the result in register VX.
            //       If VY > VX, set VF = 1; otherwise, set VF = 0.
            if (Processor.V[(byte)(args[1])] > Processor.V[(byte)(args[0])])
                Processor.V[0xF] = 1;
            else
                Processor.V[0xF] = 0;

            Processor.V[(byte)(args[0])] = (byte)(Processor.V[(byte)(args[1])] - Processor.V[(byte)(args[0])]);
            Processor.PC += 2;
        }

        private void leftShiftRegSpillCarry(Generic.DataStructures.InstructionArgs args)
        {
            // 8XYE: Shift register VX 1 position to the left.
            //       Set VF to the MSB of VX before the shift.
            Processor.V[0xF] = (byte)(((Processor.V[(byte)(args[0])]) & 0x80) >> 7);
            Processor.V[(byte)(args[0])] <<= 1;
            Processor.PC += 2;
        }

        private void skipIfRegNotEqualToReg(Generic.DataStructures.InstructionArgs args)
        {
            // 9XY0: If registers VX and VY are not equal, increment PC by 2.
            if (Processor.V[(byte)(args[0])] != Processor.V[(byte)(args[1])])
                Processor.PC += 2;

            Processor.PC += 2;
        }

        private void loadIWithAddress(Generic.DataStructures.InstructionArgs args)
        {
            // ANNN: Sets I to the address NNN.
            Processor.I = (ushort)(args[0]);
            Processor.PC += 2;
        }

        private void jumpToAddressWithOffsetInReg0(Generic.DataStructures.InstructionArgs args)
        {
            // BNNN: Jump to location NNN + the value of register V0.
            Processor.PC = (ushort)((ushort)(args[0]) + Processor.V[0]);
        }

        private void bitwiseAndRandomNumberAndConst(Generic.DataStructures.InstructionArgs args)
        {
            // CXKK: Generate a random number between 0x00 and 0xFF.
            //       Perform a bitwise AND between this number and the value KK.
            //       Store the result in register VX.
            int randNum = Processor.RandomGenerator.Next(256);
            Processor.V[(byte)(args[0])] = (byte)(randNum & (byte)(args[1]));

            Processor.PC += 2;
        }

        private void drawSpriteAndDetectCollision(Generic.DataStructures.InstructionArgs args)
        {
            // DXYN: Draw a sprite on the screen.
            //       Sprite data starts at the address stored in I.
            //       "N" bytes of data are read in total.
            //       The sprite is painted at coordinates (VX, VY).
            //       The screen contents are XORed with the sprite data.
            //       If any pixels are erased, set VF = 1; otherwise, set VF = 0.
            //       Any part of the sprite going outside the screen wraps around.


            if (Processor.VideoRAM == null)
            {
                Processor.PC += 2;
                return;
            }

            byte x0 = Processor.V[(byte)(args[0])];
            byte y0 = Processor.V[(byte)(args[1])];
            byte h = (byte)(args[2]);

            byte pixel;
            int screen_pos;

            int xf, yf;

            Processor.V[0xF] = 0;

            for (int y = 0; y < h; y++)
            {
                pixel = Processor.MainRAM[Processor.I + y];

                for (int x = 0; x < 8; x++)
                {
                    if ((pixel & (0x80 >> x)) != 0)
                    {
                        xf = (x0 + x) % 64;
                        yf = (y0 + y) % 32;

                        screen_pos = (64 * yf) + xf;

                        if (Processor.VideoRAM[screen_pos] == 1)
                            Processor.V[0xF] = 1;

                        Processor.VideoRAM[screen_pos] ^= 1;
                    }
                }
            }

            Processor.Draw = true;
            Processor.PC += 2;
        }

        private void skipIfKeypressNameInRegIsPressed(Generic.DataStructures.InstructionArgs args)
        {
            // EX9E: Checks if a key is pressed and its name is in register VX.
            //       If so, the next instruction is skipped.
            if (Processor.Keyboard == null)
            {
                Processor.PC += 2;
                return;
            }

            if (Processor.Keyboard.GetKeyState(Processor.V[(byte)(args[0])]))
                Processor.PC += 2;

            Processor.PC += 2;
        }

        private void skipIfKeypressNameInRegIsNotPressed(Generic.DataStructures.InstructionArgs args)
        {
            // EXA1: Checks if the key with name in register VX is NOT pressed.
            //       If so, the next instruction is skipped.
            if (Processor.Keyboard == null)
            {
                Processor.PC += 2;
                return;
            }

            if (!Processor.Keyboard.GetKeyState(Processor.V[(byte)(args[0])]))
                Processor.PC += 2;

            Processor.PC += 2;
        }

        private void loadRegWithDelayValue(Generic.DataStructures.InstructionArgs args)
        {
            // FX07: Sets register VX to the current delay timer value.
            Processor.V[(byte)(args[0])] = Processor.DelayTimer;
            Processor.PC += 2;
        }

        private void waitForKeypressAndStoreNameInReg(Generic.DataStructures.InstructionArgs args)
        {
            // FX0A: Wait for a key press, then store its name in register VX.
            if (Processor.Keyboard == null)
            {
                Processor.PC += 2;
                return;
            }

            int last_keycode;
            if ((last_keycode = Processor.Keyboard.LastKeyIndex) == Processor.Keyboard.NoKeyPress)
                return; //break;

            Processor.V[(byte)(args[0])] = (byte)last_keycode;

            // Avoid getting the key "stuck" for an entire display refresh!
            Processor.Keyboard.ClearLastIndex();

            Processor.PC += 2;
        }

        private void loadDelayValueWithReg(Generic.DataStructures.InstructionArgs args)
        {
            // FX15: Sets the delay timer to the value stored in register VX.
            Processor.DelayTimer = Processor.V[(byte)(args[0])];
            Processor.PC += 2;
        }

        private void loadSoundTimerWithReg(Generic.DataStructures.InstructionArgs args)
        {
            // FX18: Sets the sound timer to the value stored in register VX.
            Processor.SoundTimer = Processor.V[(byte)(args[0])];

            // The sound should immedately start playing,
            // BUT only if the value is different from zero.
            if (Processor.SoundTimer != 0)
            {
                Processor.Playing = true;

                if (Processor.Buzzer != null)
                    Processor.Buzzer.Play();
            }

            Processor.PC += 2;
        }

        private void addRegToI(Generic.DataStructures.InstructionArgs args)
        {
            // FX1E: Adds the value of the register VX to I.

            Processor.I += Processor.V[(byte)(args[0])];
            Processor.PC += 2;
        }

        private void loadFontSpriteStartToI(Generic.DataStructures.InstructionArgs args)
        {
            // FX29: Sets I to the start of a predefined sprite's data.
            //       Predefined sprites are hexadecimal digits 0-F.
            //       The value of register VX indicates the sprite to use.

            // As digits are 5 bytes long...
            Processor.I = (ushort)(5 * Processor.V[(byte)(args[0])]);
            Processor.PC += 2;
        }

        private void loadBCDOfRegToStartI(Generic.DataStructures.InstructionArgs args)
        {
            // FX33: Store the BCD representation of VX in memory.
            //       The hundreds digit is stored at address I.
            //       The tens digit is stored at address I, +1.
            //       The units digit is stored at address I, +2.
            Processor.MainRAM[Processor.I] = (byte)(Processor.V[(byte)(args[0])] / 100);
            Processor.MainRAM[Processor.I + 1] = (byte)((Processor.V[(byte)(args[0])] / 10) % 10);
            Processor.MainRAM[Processor.I + 2] = (byte)(Processor.V[(byte)(args[0])] % 10);
            Processor.PC += 2;
        }

        private void dumpRegsToMemoryStartingAtI(Generic.DataStructures.InstructionArgs args)
        {
            // FX55: Copy the values of registers V0 through VX in memory.
            //       The destination starts at the address stored in I.
            int noRegs = (byte)(args[0]);

            for (int r = 0; r <= noRegs; r++)
                Processor.MainRAM[Processor.I + r] = Processor.V[r];

            // Per mattmik, I reg. should reflect the counting.
            // BEWARE: Game TICTAC doesn't work if counting is reflected!
            if (Processor.ModifyI)
                Processor.I += (ushort)(noRegs + 1);

            Processor.PC += 2;
        }

        private void populateRegsFromMemoryStartingAtI(Generic.DataStructures.InstructionArgs args)
        {
            // FX65: Populate registers V0 through VX with data in memory.
            //       The source starts at the address stored in I.
            int noRegs = (byte)(args[0]);

            for (int r = 0; r <= noRegs; r++)
                Processor.V[r] = Processor.MainRAM[Processor.I + r];

            // Per mattmik, I reg. should reflect the counting.
            // BEWARE: Game TICTAC doesn't work if counting is reflected!
            if (Processor.ModifyI)
                Processor.I += (ushort)(noRegs + 1);

            Processor.PC += 2;
        }

        private void unrecognizedInstruction(Generic.DataStructures.InstructionArgs args)
        {
            Processor.PC += 2;
        }
    }
}
