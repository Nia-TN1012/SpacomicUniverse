#region バージョン情報
/**
*	@file SpacoRSSCollectionViewModel.cs
*	@brief すぱこーRSSフィードの一覧を表示する画面用のViewModelを表します。
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

/// <summary>
///		すぱこみっく ユニバース
/// </summary>
namespace SpacomicUniverse {

	/// <summary>
	///		すぱこーRSSフィードの一覧を表示する画面用のViewModelを表します。
	/// </summary>
	class SpacoRSSCollectionViewModel : INotifyPropertyChanged {

		/// <summary>
		///		SpacoRSSModelオブジェクトを表します。
		/// </summary>
		private SpacomicRSSCollectionModel spacomicRSSCollectionModel;

		/// <summary>
		///		すぱこーRSSフィードのコンテンツのコレクションを取得します。
		/// </summary>
		public IEnumerable<SpacomicRSSItemsGroup> Items { get; private set; }

		/// <summary>
		///		すぱこーRSSフィード取得中のフラグを表します。
		/// </summary>
		private bool isProgress = false;
		/// <summary>
		///		すぱこーRSSフィード取得中のフラグを取得します。
		/// </summary>
		public bool IsProgress {
			get { return isProgress; }
			private set {
				isProgress = value;
				NotifyPropertyChanged();
			}
		}

		/// <summary>
		///		SpacoRSSCollectionViewModelの新しいインスタンスを生成します。
		/// </summary>
		public SpacoRSSCollectionViewModel() {
			// AppオブジェクトからModelを取得します。
			spacomicRSSCollectionModel = ( App.Current as App )?.SpacomicRSSCollectionModel;

			// Modelの参照の取得に失敗したら、例外をスローします。
			if( spacomicRSSCollectionModel == null ) {
				throw new Exception( $"Failed to get reference of Model's instance on {GetType().ToString()}" );
			}

			// プロパティの変更を通知します。
			spacomicRSSCollectionModel.PropertyChanged +=
				( sender, e ) =>
					PropertyChanged?.Invoke( sender, e );

			// RSSフィードの取得を開始したことをView側に通知します。
			spacomicRSSCollectionModel.GetRSSStarted +=
				( sender, e ) => {
					IsProgress = true;
					Items = null;
					NotifyPropertyChanged( nameof( Items ) );
					GetRSSStarted?.Invoke( this, e );
				};

			// RSSフィードの取得が完了したことをView側に通知します。
			spacomicRSSCollectionModel.GetRSSCompleted +=
				( sender, e ) => {
					// RSSフィード取得中のフラグをオフにします。
					IsProgress = false;
					DivideItemsIntoTheGroupsOfMonth();
					// RSSフィード取得完了したことをView側に通知します。
					GetRSSCompleted?.Invoke( this, e );
				};

			// 画像のキャッシュの削除が完了したことをView側に通知します。
			spacomicRSSCollectionModel.ImageCachesDeleted +=
				( sender, e ) => {
					NotifyPropertyChanged( nameof( Items ) );
				};

			if( spacomicRSSCollectionModel.Items != null ) {
				DivideItemsIntoTheGroupsOfMonth();
			}
		}

		/// <summary>
		///		RSSフィードのコレクションを年月ごとにグループ分けします。
		/// </summary>
		private void DivideItemsIntoTheGroupsOfMonth() {
			Items = spacomicRSSCollectionModel.Items
				.GroupBy( _ => _.PubDate.ToString( "yyyy年MM月" ) )
				.Select( _ => new SpacomicRSSItemsGroup { GroupTitle = _.Key, Items = _.AsEnumerable() } );
			NotifyPropertyChanged( nameof( Items ) );
		}

		/// <summary>
		///		すぱこーRSSフィードの取得開始時に発生させるイベントハンドラーです。
		/// </summary>
		public event EventHandler GetRSSStarted;

		/// <summary>
		///		すぱこーRSSフィードの取得完了後に発生させるイベントハンドラーです。
		/// </summary>
		public event EventHandler<GetRSSResult> GetRSSCompleted;

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
	}
}
