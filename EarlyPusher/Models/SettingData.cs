﻿using StFrLibs.Core.Basis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EarlyPusher.Models
{
	public class SettingData : ObservableObject
	{
		public const string FileName = "conf.xml";

		private List<PanelData> keyBindCollection;

		public List<PanelData> KeyBindCollection
		{
			get { return keyBindCollection; }
		}

		private string anserSoundPath;

		public string AnserSoundPath
		{
			get { return anserSoundPath; }
			set { SetProperty( ref anserSoundPath, value ); }
		}

		public SettingData()
		{
			this.keyBindCollection = new List<PanelData>();
		}

	}
}
