The CHIP-8 emulator
===================

**Current version:** 1.0-alpha.20200629

Overview
--------

This is a CHIP-8 emulator, implemented in C# and using .NET Framework 2.0.

Features
--------

* Customizable configuration.
    - Selectable system font.
    - Two buzzer sounds available.
    - Selectable processor speed.
    - Selectable foreground color.
* Fine tuning for program compatibility.
    - Automatic screen clear on reset.
    - Selectable behavior of `I` register on instructions `FX55` and `FX65`.
* Developer tools.
    - Basic cycle stepping mode (one processor cycle per invocation).
    - Memory viewer.
    - Register inspector.
    - Stack viewer.
    - Basic disassembler.

How to build
------------

This project has been coded using Visual Studio 2005. Later versions may work, but they are untested.

In order to produce a working program, build the project `UI-WinForms` or build the entire solution via `Ctrl+Shift+B`, using the "Release" configuration.

The folder `src\Chip8-NET20\UI-WinForms\bin\Release` will contain all components required to successfully run the program:

```
src\Chip8-NET20\UI-WinForms\bin\Release\
├── fonts
│   ├── 7seg.ch8
│   ├── alt.ch8
│   ├── default.ch8
│   └── lowercase.ch8
├── programs
│   ├── Clock Program [Bill Fisher, 1981].ch8
│   ├── IBM Logo.ch8
│   ├── invaders.ch8
│   ├── Keypad Test [Hap, 2006].ch8
│   ├── MAZE.ch8
│   ├── MERLIN.ch8
│   ├── overstack.ch8
│   ├── pong2.c8
│   ├── TANK.ch8
│   ├── test_opcode.ch8
│   ├── TICTAC.ch8
│   ├── UFO.ch8
│   └── WALL.ch8
├── sounds
│   ├── 736hz-square.wav
│   └── 787hz-sine.wav
├── COPYING
├── Chip8.dll
├── Display.dll
├── Generic.dll
├── Notifiable.dll
├── UIControls.dll
├── WindowsAPI.dll
└── Chip8-NET20.exe
```

Any other files not included in this tree may be safely removed.

Planned
-------

* Implementation correctness.
    - Allow the user to select whether or not to use RAM for the stack.
* Customizable configuration.
    - Screen background color.
* Developer tools.
    - Make the disassembler follow the address being executed when cycle-stepping.
    - Provide interfaces to write values directly to RAM, the stack or registers/timers.

License
-------

The CHIP-8 emulator: an implementation in C# using .NET Framework 2.0.

Copyright (C) 2025, Aura Lesse Programmer \<aurasys.7608@zohomail.com>

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
