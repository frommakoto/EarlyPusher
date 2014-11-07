﻿using EarlyPusher.Manager;
using EarlyPusher.Models;
using SlimDX.DirectInput;
using StFrLibs.Core.Basis;
using StFrLibs.Core.Commands;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Threading;
using System.Xml.Serialization;

namespace EarlyPusher.ViewModels
{
	public class MainVM : ViewModelBase
	{
		private SettingData data;

		private Dispatcher dispatcher = Dispatcher.CurrentDispatcher;
		private StringBuilder logBuilder;
		private DeviceManager manager;
		private DispatchObservableCollection<string> devices;
		private DispatchObservableCollection<ItemVM> items;
		private long updateTime;
		private bool isSettingMode = true;
		private int rank = 0;

		#region プロパティ

		public DelegateCommand SearchCommand { get; private set; }

		public DelegateCommand LoadedCommand { get; private set; }
		public DelegateCommand ClosingCommand { get; private set; }

		public DelegateCommand AddCommand { get; private set; }
		public DelegateCommand DelCommand { get; private set; }

		public DelegateCommand SelectCommand { get; private set; }

		public DelegateCommand StartCommand { get; private set; }

		public DispatchObservableCollection<string> Devices
		{
			get
			{
				return this.devices;
			}
		}

		public DispatchObservableCollection<ItemVM> Items
		{
			get
			{
				return this.items;
			}
		}

		public string Log
		{
			get { return this.logBuilder.ToString(); }
		}

		public long UpdateTime
		{
			get { return updateTime; }
			set { SetProperty( ref updateTime, value ); }
		}

		public bool IsSettingMode
		{
			get { return isSettingMode; }
			set { SetProperty( ref isSettingMode, value, SettingChanged ); }
		}

		#endregion

		public MainVM()
		{
			this.SearchCommand = new DelegateCommand( SearchDevice, null );
			this.LoadedCommand = new DelegateCommand( Inited, null );
			this.ClosingCommand = new DelegateCommand( Closing, null );
			this.AddCommand = new DelegateCommand( AddItem, null );
			this.DelCommand = new DelegateCommand( DelItem, CanDelItem );
			this.SelectCommand = new DelegateCommand( Select, null );
			this.StartCommand = new DelegateCommand( Start, null );
			this.manager = new DeviceManager();
			this.manager.Devices.CollectionChanged += Devices_CollectionChanged;
			this.manager.PropertyChanged += Manager_PropertyChanged;
			this.manager.KeyPushed += Manager_KeyPushed;
			this.devices = new DispatchObservableCollection<string>();
			this.items = new DispatchObservableCollection<ItemVM>();
			this.logBuilder = new StringBuilder();
		}

		#region コマンド関係

		private void Start( object obj )
		{
			DeselectAll();
			InitRank();
		}

		private void Select( object obj )
		{
			var item = obj as ItemVM;
			Contract.Assert( item != null );
			DeselectAll();
			item.IsSelected = true;
			this.DelCommand.RaiseCanExecuteChanged();
		}

		private void AddItem( object obj )
		{
			this.Items.Add( new ItemVM() );
		}

		private bool CanDelItem( object obj )
		{
			return this.Items.Where( i => i.IsSelected ).Count() == 1;
		}

		private void DelItem( object obj )
		{
			var item = this.Items.FirstOrDefault( i => i.IsSelected );
			Contract.Assert( item != null );

			this.Items.Remove( item );
		}

		private void Inited( object obj )
		{
			LoadData();
		}

		private void Closing( object obj )
		{
			SaveData();
		}

		private void SearchDevice( object obj )
		{
			this.manager.SearchDevice();
		}

		#endregion

		#region イベント関係

		private void Manager_KeyPushed( object sender, DeviceKeyEventArgs e )
		{
			WriteLogLine( e.Key.ToString() );
			if( this.IsSettingMode )
			{
				var item = this.Items.FirstOrDefault( i => i.IsSelected );
				if( item != null )
				{
					item.Data = new KeyBindData() { DeviceGuid = e.InstanceID, Key = e.Key };
				}
			}
			else
			{
				var item = this.Items.FirstOrDefault( i => i.Data.DeviceGuid == e.InstanceID && i.Data.Key == e.Key );
				if( item != null )
				{
					rank++;
					item.Rank = rank.ToString();
				}
			}
		}

		private void Manager_PropertyChanged( object sender, PropertyChangedEventArgs e )
		{
			if( e.PropertyName == "UpdateTime" )
			{
				this.UpdateTime = this.manager.UpdateTime;
			}
		}

		private void Devices_CollectionChanged( object sender, NotifyCollectionChangedEventArgs e )
		{
			switch( e.Action )
			{
				case NotifyCollectionChangedAction.Add:
				case NotifyCollectionChangedAction.Remove:
				case NotifyCollectionChangedAction.Replace:
					if( e.NewItems != null )
					{
						foreach( Device d in e.NewItems )
						{
							this.devices.Add( d.Information.InstanceName );
						}
					}
					if( e.OldItems != null )
					{
						foreach( Device d in e.OldItems )
						{
							this.devices.Remove( d.Information.InstanceName );
						}
					}
					break;
				case NotifyCollectionChangedAction.Reset:
					this.devices.Clear();
					break;
				default:
					break;
			}
		}

		#endregion

		#region 設定

		/// <summary>
		/// 設定を読み込みます。
		/// </summary>
		private void LoadData()
		{
			if( File.Exists( SettingData.FileName ) )
			{
				using( FileStream file = new FileStream( SettingData.FileName, FileMode.Open ) )
				{
					XmlSerializer xml = new XmlSerializer( typeof( SettingData ) );
					this.data = xml.Deserialize( file ) as SettingData;
				}
			}
			else
			{
				this.data = new SettingData();
			}
			foreach( var item in this.data.KeyBindCollection )
			{
				this.Items.Add( new ItemVM() { Data = item } );
			}
		}

		/// <summary>
		/// 設定を保存します。
		/// </summary>
		public void SaveData()
		{
			this.data.KeyBindCollection.Clear();
			foreach( var item in this.Items )
			{
				if( item.Data != null )
				{
					this.data.KeyBindCollection.Add( item.Data );
				}
				else
				{
					this.data.KeyBindCollection.Add( new KeyBindData() );
				}
			}
			using( Stream file = new FileStream( SettingData.FileName, FileMode.Create ) )
			{
				XmlSerializer xml = new XmlSerializer( typeof( SettingData ) );
				xml.Serialize( file, this.data );
			}
		}

		#endregion

		private void SettingChanged( bool isSucceed )
		{
			rank = 0;
			DeselectAll();
			InitRank();
		}

		private void DeselectAll()
		{
			foreach( var i in this.Items )
			{
				i.IsSelected = false;
			}
		}

		private void InitRank()
		{
			foreach( var i in this.Items )
			{
				i.Rank = string.Empty;
			}
		}

		private void WriteLogLine( string str )
		{
			this.logBuilder.AppendLine( str );
			this.NotifyPropertyChanged( "Log" );
		}

		public void DispatcherInvoke( Action action )
		{
			if( !this.dispatcher.CheckAccess() )
			{
				this.dispatcher.BeginInvoke( action );
			}
			else
			{
				action();
			}
		}
	}
}