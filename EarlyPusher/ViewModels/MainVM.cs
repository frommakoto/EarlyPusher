﻿using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using EarlyPusher.Manager;
using EarlyPusher.Models;
using EarlyPusher.Modules.ChoiceTab.ViewModels;
using EarlyPusher.Modules.EarlyTab.ViewModels;
using EarlyPusher.Modules.Setting1Tab.ViewModels;
using EarlyPusher.Modules.Setting2Tab.ViewModels;
using EarlyPusher.Modules.OrderTab.ViewModels;
using EarlyPusher.Views;
using StFrLibs.Core.Basis;
using StFrLibs.Core.Commands;
using EarlyPusher.Modules.TimeShockTab.ViewModels;

namespace EarlyPusher.ViewModels
{
	public class MainVM : ViewModelBase
	{
		private SettingData data;

		private PlayWindow window;
		private DeviceManager manager;

		private List<OperateTabVMBase> operateVMs = new List<OperateTabVMBase>();

		private OperateTabVMBase selectedTab;

		#region プロパティ

		public DelegateCommand WindowCommand { get; private set; }
		public DelegateCommand WindowMaxCommand { get; private set; }

		public DelegateCommand LoadedCommand { get; private set; }
		public DelegateCommand ClosingCommand { get; private set; }

		/// <summary>
		/// ボタンデバイスの管理クラス
		/// </summary>
		public DeviceManager Manager => this.manager;

		public SettingData Data => this.data;

		public IEnumerable<OperateTabVMBase> Tabs => this.operateVMs;

		public OperateTabVMBase SelectedTab
		{
			get { return this.selectedTab; }
			set { SetProperty( ref this.selectedTab, value, SelectedTabChanged, SelectedTabChanging ); }
		}

		#endregion

		public MainVM()
		{
			this.WindowCommand = new DelegateCommand( ShowCloseWindow, CanShowCloseWindow );
			this.WindowMaxCommand = new DelegateCommand( MaximazeWindow, CanMaximaize );

			this.LoadedCommand = new DelegateCommand( Inited, null );
			this.ClosingCommand = new DelegateCommand( Closing, null );

			this.manager = new DeviceManager();

			this.operateVMs.Add( new OperateSetting1VM( this ) );
			this.operateVMs.Add( new OperateSetting2VM( this ) );
			this.operateVMs.Add( new OperateChoiceVM( this ) );
			this.operateVMs.Add( new OperateEarlyVM( this ) );
			this.operateVMs.Add( new OperateOrderVM( this ) );
			this.operateVMs.Add( new OperateTimeShockVM( this ) );
		}

		private void SelectedTabChanging()
		{
			if( this.SelectedTab != null )
			{
				this.SelectedTab.Deactivate();
			}
		}

		private void SelectedTabChanged()
		{
			if( this.SelectedTab != null )
			{
				this.SelectedTab.Activate();
			}
			this.WindowCommand.RaiseCanExecuteChanged();
		}

		#region コマンド

		private bool CanShowCloseWindow( object obj )
		{
			return this.SelectedTab.PlayView != null;
		}

		/// <summary>
		/// プレイウィンドウの開閉処理
		/// </summary>
		/// <param name="obj"></param>
		private void ShowCloseWindow( object obj )
		{
			if( this.window != null )
			{
				this.window.Close();
				this.window = null;

			}
			else
			{
				this.window = new PlayWindow();
				this.window.DataContext = this.SelectedTab;
				this.window.Show();
			}
			this.WindowMaxCommand.RaiseCanExecuteChanged();
		}

		/// <summary>
		/// プレイウィンドウの最大化可能かどうか
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		private bool CanMaximaize( object obj )
		{
			return this.window != null;
		}

		/// <summary>
		/// プレイウィンドウの最大化・通常化
		/// </summary>
		/// <param name="obj"></param>
		private void MaximazeWindow( object obj )
		{
			Contract.Assert( this.window != null );

			if( this.window.WindowState != System.Windows.WindowState.Maximized )
			{
				this.window.WindowState = System.Windows.WindowState.Maximized;
			}
			else
			{
				this.window.WindowState = System.Windows.WindowState.Normal;
			}
		}

		/// <summary>
		/// ウィンドウが開くときの処理
		/// </summary>
		/// <param name="obj"></param>
		private void Inited( object obj )
		{
			LoadData();
		}

		/// <summary>
		/// ウィンドウが閉じるときの処理
		/// </summary>
		/// <param name="obj"></param>
		private void Closing( object obj )
		{
			SaveData( SettingData.FileName );
			if( this.window != null )
			{
				this.window.Close();
			}

			this.manager.Dispose();
		}

		#endregion

		#region 設定読み書き

		/// <summary>
		/// 設定を読み込みます。
		/// </summary>
		private void LoadData()
		{
			try
			{
				if( File.Exists( SettingData.FileName ) )
				{
					using( FileStream file = new FileStream( SettingData.FileName, FileMode.Open ) )
					{
						XmlSerializer xml = new XmlSerializer( typeof( SettingData ) );
						this.data = xml.Deserialize( file ) as SettingData;
					}
				}
			}
			catch
			{
			}
			finally
			{
				if( this.data == null )
				{
					this.data = new SettingData();
				}
			}

			//4択用に必ず1チーム4人は確保する。
			foreach( var team in this.data.TeamList )
			{
				while( team.Members.Count < 4 )
				{
					team.Members.Add( new MemberData() );
				}
			}

			//各タブのロード処理
			foreach( var vm in this.operateVMs )
			{
				vm.LoadData();
			}
		}

		/// <summary>
		/// 設定を保存します。
		/// </summary>
		public void SaveData( string path )
		{
			//各タブのセーブ処理
			foreach( var vm in this.operateVMs )
			{
				vm.SaveData();
			}

			using( Stream file = new FileStream( path, FileMode.Create ) )
			{
				XmlSerializer xml = new XmlSerializer( typeof( SettingData ) );
				xml.Serialize( file, this.data );
			}
		}

		#endregion

	}
}
