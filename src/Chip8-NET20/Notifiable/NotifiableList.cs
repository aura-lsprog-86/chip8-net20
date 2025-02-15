using System;
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

using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace Notifiable
{
    public delegate void ListStatusEventHandler(object sender, ListStatusEventArgs e);

    public class NotifiableList<T> : Collection<T>
    {
        public delegate void ItemStatusEventHandler(object sender, ItemStatusEventArgs<T> e);

        public event ItemStatusEventHandler ItemAdding;
        public event ItemStatusEventHandler ItemAdded;
        public event ItemStatusEventHandler ItemRemoving;
        public event ItemStatusEventHandler ItemRemoved;
        public event ItemStatusEventHandler ItemReplacing;
        public event ItemStatusEventHandler ItemReplaced;
        public event ItemStatusEventHandler ItemUpdating;
        public event ItemStatusEventHandler ItemUpdated;

        public event ListStatusEventHandler ListClearing;
        public event ListStatusEventHandler ListCleared;

        public event ItemStatusEventHandler ItemActionBind;
        public event ItemStatusEventHandler ItemActionUnbind;

        protected void OnItemStatus(ItemStatusEventArgs<T> e)
        {
            ItemStatusEventHandler handler = null;

            switch (e.Status)
            {
                case ItemStatus.Adding:
                    handler = ItemAdding;
                    break;
                case ItemStatus.Added:
                    handler = ItemAdded;
                    break;

                case ItemStatus.Removing:
                    handler = ItemRemoving;
                    break;
                case ItemStatus.Removed:
                    handler = ItemRemoved;
                    break;

                case ItemStatus.Replacing:
                    handler = ItemReplacing;
                    break;
                case ItemStatus.Replaced:
                    handler = ItemReplaced;
                    break;

                case ItemStatus.Updating:
                    handler = ItemUpdating;
                    break;
                case ItemStatus.Updated:
                    handler = ItemUpdated;
                    break;

                case ItemStatus.ActionBind:
                    handler = ItemActionBind;
                    break;
                case ItemStatus.ActionUnbind:
                    handler = ItemActionUnbind;
                    break;
            }

            if (handler != null)
                handler(this, e);
        }

        protected void OnListStatus(ListStatusEventArgs e)
        {
            ListStatusEventHandler handler = null;

            switch (e.Status)
            {
                case ListStatus.Clearing:
                    handler = ListClearing;
                    break;
                case ListStatus.Cleared:
                    handler = ListCleared;
                    break;
            }

            if (handler != null)
                handler(this, e);
        }

        protected void OnItemAdding(int index, T item)
        {
            OnItemStatus(new ItemStatusEventArgs<T>(ItemStatus.Adding, index, item));
        }

        protected void OnItemAdded(int index, T item)
        {
            OnItemStatus(new ItemStatusEventArgs<T>(ItemStatus.Added, index, item));
        }

        protected void OnItemRemoving(int index, T item)
        {
            OnItemStatus(new ItemStatusEventArgs<T>(ItemStatus.Removing, index, item));
        }

        protected void OnItemRemoved(int index, T item)
        {
            OnItemStatus(new ItemStatusEventArgs<T>(ItemStatus.Removed, index, item));
        }

        protected void OnItemReplacing(int index, T oldItem, T newItem)
        {
            OnItemStatus(new ItemStatusEventArgs<T>(ItemStatus.Replacing, index, oldItem, newItem));
        }

        protected void OnItemReplaced(int index, T oldItem, T newItem)
        {
            OnItemStatus(new ItemStatusEventArgs<T>(ItemStatus.Replaced, index, oldItem, newItem));
        }

        protected void OnItemUpdating(int index, T item)
        {
            OnItemStatus(new ItemStatusEventArgs<T>(ItemStatus.Updating, index, item));
        }

        protected void OnItemUpdated(int index, T item)
        {
            OnItemStatus(new ItemStatusEventArgs<T>(ItemStatus.Updated, index, item));
        }

        protected void OnItemActionBind(int index, T item)
        {
            OnItemStatus(new ItemStatusEventArgs<T>(ItemStatus.ActionBind, index, item));
        }

        protected void OnItemActionUnbind(int index, T item)
        {
            OnItemStatus(new ItemStatusEventArgs<T>(ItemStatus.ActionUnbind, index, item));
        }

        protected void OnListClearing()
        {
            OnListStatus(new ListStatusEventArgs(ListStatus.Clearing));
        }

        protected void OnListCleared()
        {
            OnListStatus(new ListStatusEventArgs(ListStatus.Cleared));
        }

        protected override void InsertItem(int index, T item)
        {
            OnItemAdding(index, item);
            base.InsertItem(index, item);
            OnItemActionBind(index, item);
            OnItemAdded(index, item);
        }

        protected override void SetItem(int index, T item)
        {
            if (index < 0 || index >= Count)
                throw new ArgumentOutOfRangeException("index");

            T oldItem = this[index];

            OnItemReplacing(index, oldItem, item);
            OnItemActionUnbind(index, oldItem);
            base.SetItem(index, item);
            OnItemActionBind(index, item);
            OnItemReplaced(index, oldItem, item);
        }

        protected override void RemoveItem(int index)
        {
            if (index < 0 || index >= Count)
                throw new ArgumentOutOfRangeException("index");

            T item = this[index];

            OnItemRemoving(index, item);
            OnItemActionUnbind(index, item);
            base.RemoveItem(index);
            OnItemRemoved(index, item);
        }

        protected override void ClearItems()
        {
            OnListClearing();
            for (int i = 0; i < Count; i++)
                OnItemActionUnbind(i, this[i]);
            base.ClearItems();
            OnListCleared();
        }
    }
}
