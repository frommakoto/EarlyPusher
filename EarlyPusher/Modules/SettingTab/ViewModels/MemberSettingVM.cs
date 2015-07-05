﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EarlyPusher.Models;
using StFrLibs.Core.Basis;

namespace EarlyPusher.Modules.SettingTab.ViewModels
{
	public class MemberSettingVM : ViewModelBase<MemberData>
	{
		private TeamSettingVM parent;

		private bool isKeyLock = true;

		public bool IsKeyLock
		{
			get { return this.isKeyLock; }
			set { SetProperty( ref this.isKeyLock, value ); }
		}
		
		public MemberSettingVM( TeamSettingVM parent, MemberData data )
			: base( data )
		{
			this.parent = parent;
		}

		public TeamSettingVM Parent
		{
			get { return this.parent; }
		}

	}
}