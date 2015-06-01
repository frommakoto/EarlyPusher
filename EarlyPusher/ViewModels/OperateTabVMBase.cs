﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using System.Xml.Serialization;
using EarlyPusher.Basis;
using EarlyPusher.Manager;
using EarlyPusher.Models;
using EarlyPusher.Views;
using Microsoft.Win32;
using Ookii.Dialogs.Wpf;
using SlimDX.DirectInput;
using StFrLibs.Core.Basis;
using StFrLibs.Core.Commands;

namespace EarlyPusher.ViewModels
{
	public abstract class OperateTabVMBase : ViewModelBase
	{
		private MainVM parent;

		private MediaVM answerSound;

		#region プロパティ

		public MainVM Parent
		{
			get { return this.parent; }
		}

		/// <summary>
		/// 解答音
		/// </summary>
		public MediaVM AnswerSound
		{
			get { return answerSound; }
			set { SetProperty( ref answerSound, value ); }
		}

		public abstract UIElement PlayView
		{
			get;
		}
		
		#endregion

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public OperateTabVMBase( MainVM parent )
		{
			this.parent = parent;
		}

		#region 設定読み書き

		/// <summary>
		/// 設定を読み込みます。
		/// </summary>
		public virtual void LoadData()
		{
			this.AnswerSound = new MediaVM() { FilePath = this.parent.Data.AnswerSoundPath };
		}

		/// <summary>
		/// 設定を保存します。
		/// </summary>
		public virtual void SaveData()
		{
		}

		#endregion

		public virtual void Activate()
		{
		}

		public virtual void Deactivate()
		{
		}
	}
}
