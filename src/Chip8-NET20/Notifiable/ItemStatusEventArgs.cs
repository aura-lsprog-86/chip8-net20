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

namespace Notifiable
{
    public enum ItemStatus
    {
        None,
        Adding, Added,
        Removing, Removed,
        Replacing, Replaced,
        Updating, Updated,
        ActionBind, ActionUnbind
    }

    public class ItemStatusEventArgs<T>
    {
        private ItemStatus _status;
        public ItemStatus Status
        {
            get { return _status; }
            private set { _status = value; }
        }

        private int _index;
        public int Index
        {
            get { return _index; }
            private set { _index = value; }
        }

        private T _old_item;
        public T OldItem
        {
            get { return _old_item; }
            private set { _old_item = value; }
        }

        private T _item;
        public T Item
        {
            get { return _item; }
            private set { _item = value; }
        }

        public ItemStatusEventArgs(ItemStatus status, int index, T oldItem, T newItem)
        {
            Status = status;
            Index = index;
            OldItem = oldItem;
            Item = newItem;
        }

        public ItemStatusEventArgs(ItemStatus status, T oldItem, T newItem)
            : this(status, -1, oldItem, newItem)
        {
        }

        public ItemStatusEventArgs(ItemStatus status, int index, T item)
            : this(status, index, default(T), item)
        {
        }

        public ItemStatusEventArgs(ItemStatus status, T item)
            : this(status, -1, item)
        {
        }
    }
}
