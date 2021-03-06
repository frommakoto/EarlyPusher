﻿using EarlyPusher.Models;
using SFLibs.Core.Basis;

namespace EarlyPusher.ViewModels
{
    public class MemberViewModel : ViewModelBase<MemberData>
    {
        public TeamViewModel Parent { get; }

        private bool answerable;

        public bool Answerable
        {
            get { return this.answerable; }
            set { SetProperty(ref this.answerable, value); }
        }

        public MemberViewModel(TeamViewModel parent, MemberData model) : base(model)
        {
            this.Parent = parent;
        }
    }
}
