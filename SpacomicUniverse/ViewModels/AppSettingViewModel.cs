﻿#region バージョン情報
/**
*	@file AppSettingViewModel.cs
*	@brief アプリの設定画面用のViewModelクラスを表します。
*
*	@par バージョン Version
*	1.2.5
*	@par 作成者 Author
*	智中ニア（Nia Tomonaka）
*	@par コピーライト Copyright
*	Copyright (C) 2016 Chronoir.net
*	@par 作成日
*	2016/10/09
*	@par 最終更新日
*	2016/12/08
*	@par ライセンス Licence
*	BSD Licence（ 2-caluse ）
*	@par 連絡先 Contact
*	@@nia_tn1012（ https://twitter.com/nia_tn1012/ ）
*	@par ホームページ Homepage
*	- http://chronoir.net/ (ホームページ)
*/
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Windows.ApplicationModel;

/// <summary>
///		すぱこみっく ユニバース
/// </summary>
namespace SpacomicUniverse {

	/// <summary>
	///		アプリの設定画面用のViewModelクラスを表します。
	/// </summary>
	class AppSettingViewModel : INotifyPropertyChanged {

		/// <summary>
		///		パッケージの情報を表します。
		/// </summary>
		private PackageId packageInfo;

		/// <summary>
		///		現在のバージョン番号を取得します。
		/// </summary>
		public string CurrentVersion =>
			packageInfo != null ? $"{packageInfo?.Version.Major}.{packageInfo.Version.Minor}.{packageInfo.Version.Build}" : "";

		/// <summary>
		///		SpacoRSSModelオブジェクトを表します。
		/// </summary>
		private SpacomicRSSCollectionModel spacomicRSSCollectionModel;

		/// <summary>
		///		すぱこーRSSフィードのチャネル情報のコレクションを取得します。
		/// </summary>
		public IEnumerable<SpacoRSSSause> SauseItems =>
			spacomicRSSCollectionModel.SauseItems.Values.AsEnumerable();

		/// <summary>
		///		AppSettingViewModelクラスの新しいインスタンスを生成します。
		/// </summary>
		public AppSettingViewModel() {
			// AppオブジェクトからModelを取得します。
			spacomicRSSCollectionModel = ( App.Current as App )?.SpacomicRSSCollectionModel;

			// Modelの参照の取得に失敗したら、例外をスローします。
			if( spacomicRSSCollectionModel == null ) {
				throw new Exception( $"Failed to get reference of Model's instance on {GetType().ToString()}" );
			}

			spacomicRSSCollectionModel.PropertyChanged += SpacomicRSSCollectionModel_PropertyChanged;
			spacomicRSSCollectionModel.ImageCachesDeleted += SpacomicRSSCollectionModel_ImageCachesDeleted;

			// パッケージ情報を取得します。
			packageInfo = Package.Current.Id;
		}

		/// <summary>
		///		プロパティの変更を通知します。
		/// </summary>
		private void SpacomicRSSCollectionModel_PropertyChanged( object sender, PropertyChangedEventArgs e ) =>
			PropertyChanged?.Invoke( sender, e );

		/// <summary>
		///		画像のキャッシュの削除が完了したことをView側に通知します。
		/// </summary>
		private void SpacomicRSSCollectionModel_ImageCachesDeleted( object sender, EventArgs e ) =>
			NotifyPropertyChanged( nameof( SauseItems ) );

		/// <summary>
		///		<see cref="SpacomicRSSCollectionModel"/>のイベントハンドラーに登録済みのこのクラスのイベントをすべて解除します。
		/// </summary>
		public void UnsubscribeAllEvents() {
			spacomicRSSCollectionModel.PropertyChanged -= SpacomicRSSCollectionModel_PropertyChanged;
			spacomicRSSCollectionModel.ImageCachesDeleted -= SpacomicRSSCollectionModel_ImageCachesDeleted;
		}

		/// <summary>
		///		プロパティ変更後に発生させるイベントハンドラーです。
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		///		プロパティ変更を通知します。
		/// </summary>
		/// <param name="propertyName">プロパティ名</param>
		private void NotifyPropertyChanged( [CallerMemberName]string propertyName = null ) {
			PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( propertyName ) );
		}

		/// <summary>
		///		キャッシュ画像を削除するコマンドを表します。
		/// </summary>
		private ICommand deleteCache;
		/// <summary>
		///		キャッシュ画像を削除するコマンドを取得します。
		/// </summary>
		public ICommand DeleteCache =>
			deleteCache ?? ( deleteCache = new DeleteCacheCommand( this ) );

		/// <summary>
		///		コンテンツ、チャネル情報にあるキャッシュ済みの画像を削除するコマンドです。
		/// </summary>
		private class DeleteCacheCommand : ICommand {

			/// <summary>
			///		AppSettingViewModelの参照を格納します。
			/// </summary>
			private AppSettingViewModel viewModel;

			/// <summary>
			///		AppSettingViewModelの参照から、DeleteCacheCommandクラスの新しいインスタンスを生成します。
			/// </summary>
			/// <param name="_viewModel">AppSettingViewModelの参照</param>
			public DeleteCacheCommand( AppSettingViewModel _viewModel ) {
				viewModel = _viewModel;

				// コマンド実行の可否の変更を通知します。
				viewModel.PropertyChanged += ( sender, e ) =>
					CanExecuteChanged?.Invoke( sender, e );
			}

			/// <summary>
			///		コマンドを実行できるかどうかを取得します。
			/// </summary>
			/// <param name="parameter">パラメーター（使用しません）</param>
			/// <returns>コンテンツがある時 : true / コンテンツが空の時 : false</returns>
			public bool CanExecute( object parameter ) =>
				viewModel.spacomicRSSCollectionModel.Items.Any();

			/// <summary>
			///		コマンド実行の可否の変更した時のイベントハンドラーです。
			/// </summary>
			public event EventHandler CanExecuteChanged;

			/// <summary>
			///		コマンドを実行し、キャッシュ画像を削除します。
			/// </summary>
			/// <param name="parameter">パラメーター（使用しません）</param>
			public void Execute( object parameter ) {
				viewModel.spacomicRSSCollectionModel.DeleteCache();
			}

		}
	}
}
