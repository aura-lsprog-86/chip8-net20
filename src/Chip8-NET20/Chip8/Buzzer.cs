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
using System.Media;

namespace Chip8
{
    public class Buzzer : Generic.Buzzer
    {
        private string _sound_path;
        public new string SoundPath
        {
            get { return _sound_path; }
            set
            {
                _sound_path = value;
                player.SoundLocation = _sound_path;
                player.Load();
            }
        }

        private SoundPlayer player;

        public Buzzer()
            : base()
        {
            player = new SoundPlayer();
        }

        public override void Play()
        {
            player.PlayLooping();
        }

        public override void Stop()
        {
            player.Stop();
        }
    }
}
