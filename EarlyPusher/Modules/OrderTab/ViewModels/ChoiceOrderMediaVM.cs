﻿using System;
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;
using EarlyPusher.Models;
using EarlyPusher.ViewModels;
using SFLibs.Core.Extensions;

namespace EarlyPusher.Modules.OrderTab.ViewModels
{
    public class ChoiceOrderMediaVM : MediaVM
    {
        private ObservableCollection<CurrectOrederItemVM> sortedList = new ObservableCollection<CurrectOrederItemVM>();
        private ChoiceOrderMediaData model;

        public ObservableCollection<CurrectOrederItemVM> SortedList
        {
            get { return this.sortedList; }
        }

        public ChoiceOrderMediaData Model
        {
            get { return this.model; }
        }

        public ChoiceOrderMediaVM(ChoiceOrderMediaData model) : base()
        {
            this.model = model;

            for (int i = 0; i < 4; i++)
            {
                var item = new CurrectOrederItemVM();
                item.Choice = this.model.ChoiceOrder[i];
                switch (item.Choice)
                {
                    case Choice.A:
                        if (!string.IsNullOrEmpty(this.model.ChoiceAImagePath))
                        {
                            item.Image = new BitmapImage(new Uri(this.model.ChoiceAImagePath));
                        }
                        break;
                    case Choice.B:
                        if (!string.IsNullOrEmpty(this.model.ChoiceBImagePath))
                        {
                            item.Image = new BitmapImage(new Uri(this.model.ChoiceBImagePath));
                        }
                        break;
                    case Choice.C:
                        if (!string.IsNullOrEmpty(this.model.ChoiceCImagePath))
                        {
                            item.Image = new BitmapImage(new Uri(this.model.ChoiceCImagePath));
                        }
                        break;
                    case Choice.D:
                        if (!string.IsNullOrEmpty(this.model.ChoiceDImagePath))
                        {
                            item.Image = new BitmapImage(new Uri(this.model.ChoiceDImagePath));
                        }
                        break;
                }

                this.SortedList.Add(item);
            }

            this.FilePath = this.model.MediaPath;
            this.LoadFile();
        }

        public void Clear()
        {
            this.SortedList.ForEach(i => i.IsVisible = false);
        }
    }
}
