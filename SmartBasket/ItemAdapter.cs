﻿using System;
using Android.App;
using Android.Views;
using Android.Widget;
using System.Collections.Generic;
using ArduinoSmartBasket;

namespace SmartBasket
{
    public class ItemAdapter : BaseAdapter<Item>
    {
        Activity activity;
        int layoutResourceId;
        List<Item> items = new List<Item>();

        public ItemAdapter(Activity activity, int layoutResourceId)
        {
            this.activity = activity;
            this.layoutResourceId = layoutResourceId;
        }

        //Returns the view for a specific item on the list
        public override View GetView(int position, Android.Views.View convertView, Android.Views.ViewGroup parent)
        {
            var row = convertView;
            var currentItem = this[position];
            CheckBox checkBox;

            if (row == null)
            {
                var inflater = activity.LayoutInflater;
                row = inflater.Inflate(layoutResourceId, parent, false);

                checkBox = row.FindViewById<CheckBox>(Resource.Id.checkToDoItem);

                checkBox.CheckedChange += async (sender, e) => {
                    var cbSender = sender as CheckBox;
                    if (cbSender != null && cbSender.Tag is ItemWrapper && cbSender.Checked)
                    {
                        cbSender.Enabled = false;
                        if (activity is ToDoActivity)
                            await ((ToDoActivity)activity).CheckItem((cbSender.Tag as ToDoItemWrapper).ToDoItem);
                    }
                };
            }
            else
                checkBox = row.FindViewById<CheckBox>(Resource.Id.checkToDoItem);

            checkBox.Text = currentItem.name;
            checkBox.Checked = false;
            checkBox.Enabled = true;
            checkBox.Tag = new ItemWrapper(currentItem);

            return row;
        }

        public void Add(Item item)
        {
            items.Add(item);
            NotifyDataSetChanged();
        }

        public void Clear()
        {
            items.Clear();
            NotifyDataSetChanged();
        }

        public void Remove(Item item)
        {
            items.Remove(item);
            NotifyDataSetChanged();
        }

        #region implemented abstract members of BaseAdapter

        public override long GetItemId(int position)
        {
            return position;
        }

        public override int Count
        {
            get
            {
                return items.Count;
            }
        }

        public override Item this[int position]
        {
            get
            {
                return items[position];
            }
        }

        #endregion
    }
}

