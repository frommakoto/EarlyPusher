﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;
using EarlyPusher.Models;
using EarlyPusher.Modules.OrderTab.Interfaces;
using SFLibs.Core.Basis;

namespace EarlyPusher.Modules.OrderTab.ViewModels
{
    public class TeamOrderVM : ViewModelBase<TeamData>, IBackColorHolder
    {
        private OperateOrderVM parent;
        private ObservableCollection<OrderItemVM> sortedList = new ObservableCollection<OrderItemVM>();
        private bool isWinner;
        private int nextIndex = 0;
        private bool isCorrect = true;

        #region プロパティ

        public ObservableCollection<OrderItemVM> SortedList
        {
            get { return this.sortedList; }
        }

        public bool IsWinner
        {
            get { return this.isWinner; }
            set { SetProperty(ref this.isWinner, value, IsWinnerChanged); }
        }

        public Color BackColor
        {
            get { return this.Model.TeamColor; }
        }

        public bool IsCorrect
        {
            get { return this.isCorrect; }
            set { SetProperty(ref this.isCorrect, value); }
        }

        #endregion

        public TeamOrderVM(OperateOrderVM parent, TeamData data)
            : base(data)
        {
            this.parent = parent;
            for (int i = 0; i < 4; i++)
            {
                this.SortedList.Add(new OrderItemVM(this));
            }
        }

        private void IsWinnerChanged()
        {
            this.parent.CommandRaiseCanExecuteChanged();
        }

        public void Clear()
        {
            this.nextIndex = 0;
            foreach (var item in this.SortedList)
            {
                item.Choice = null;
            }
            this.IsCorrect = true;
        }

        public bool SetKey(Guid device, int key)
        {
            if (this.nextIndex > 3)
            {
                return false;
            }

            var data = this.Model.Members.FirstOrDefault(d => d.DeviceGuid == device && d.Key == key);
            if (data == null)
            {
                return false;
            }

            var choice = (Choice)this.Model.Members.IndexOf(data);

            if (this.SortedList.Any(i => i.Choice == choice))
            {
                return false;
            }

            this.SortedList[this.nextIndex].Choice = choice;
            this.nextIndex++;

            return true;
        }

        public void CheckCorrect(ChoiceOrderMediaVM media, int count)
        {
            if (this.SortedList.Any(i => i.Choice == null))
            {
                this.IsCorrect = false;
                return;
            }

            for (int i = 0; i < count; i++)
            {
                if (media.SortedList[i].Choice != this.SortedList[i].Choice)
                {
                    this.IsCorrect = false;
                }
            }
        }

        private int SortItemGetHash(OrderItemVMBase arg)
        {
            return arg.Choice.GetHashCode();
        }

        private bool SortItemEqual(OrderItemVMBase arg1, OrderItemVMBase arg2)
        {
            return arg1.Choice == arg2.Choice;
        }
    }
}
