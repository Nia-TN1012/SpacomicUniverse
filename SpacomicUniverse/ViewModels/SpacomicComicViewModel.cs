#region バージョン情報
/**
*	@file SpacomicComicViewModel.cs
*	@brief 漫画の画像を表示する画面用のViewModelを表します。
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
	///		漫画の画像を表示する画面用のViewModelを表します。
	/// </summary>
	class SpacomicComicViewModel : INotifyPropertyChanged {

		/// <summary>
		///		SpacoRSSModelオブジェクトを表します。
		/// </summary>
		private SpacomicRSSCollectionModel spacomicRSSCollectionModel;

		/// <summary>
		///		すぱこーRSSフィードのコンテンツのコレクションを取得します。
		/// </summary>
		public IEnumerable<SpacomicRSSItem> Items =>
			spacomicRSSCollectionModel.Items;

		/// <summary>
		///		現在選択しているコンテンツを取得します。
		/// </summary>
		public SpacomicRSSItem SelectedItem =>
			spacomicRSSCollectionModel.Items.Any() &&
			SelectedIndex >= 0 && SelectedIndex < spacomicRSSCollectionModel.Items.Count ?
			spacomicRSSCollectionModel.Items[SelectedIndex] : null;

		/// <summary>
		///		現在選択しているコレクションのインデックスを表します。
		/// </summary>
		private int selectedIndex;
		/// <summary>
		///		現在選択しているコレクションのインデックスを取得・設定します。
		/// </summary>
		public int SelectedIndex {
			get { return selectedIndex; }
			set {
				selectedIndex = value;
				NotifyPropertyChanged();
				NotifyPropertyChanged( nameof( SelectedItem ) );
			}
		}

		/// <summary>
		///		コンテンツの幅を表します。
		/// </summary>
		private double contentWidth;
		/// <summary>
		///		コンテンツの幅を取得・設定します。
		/// </summary>
		public double ContentWidth {
			get { return contentWidth; }
			set {
				if( contentWidth != value ) {
					contentWidth = value;
					NotifyPropertyChanged();
				}
			}
		}

		/// <summary>
		///		コンテンツの高さを表します。
		/// </summary>
		private double contentHeight;
		/// <summary>
		///		コンテンツの高さを取得・設定します。
		/// </summary>
		public double ContentHeight {
			get { return contentHeight; }
			set {
				if( contentHeight != value ) {
					contentHeight = value;
					NotifyPropertyChanged();
				}
			}
		}

		/// <summary>
		///		SpacomicComicViewModelの新しいインスタンスを生成します。
		/// </summary>
		public SpacomicComicViewModel() {
			// AppオブジェクトからModelを取得します。
			spacomicRSSCollectionModel = ( App.Current as App )?.SpacomicRSSCollectionModel;

			// Modelの参照の取得に失敗したら、例外をスローします。
			if( spacomicRSSCollectionModel == null ) {
				throw new Exception( $"Failed to get reference of Model's instance on {GetType().ToString()}" );
			}

			spacomicRSSCollectionModel.PropertyChanged += SpacomicRSSCollectionModel_PropertyChanged;
		}

		/// <summary>
		///		プロパティの変更を通知します。
		/// </summary>
		private void SpacomicRSSCollectionModel_PropertyChanged( object sender, PropertyChangedEventArgs e ) =>
			PropertyChanged?.Invoke( sender, e );

		/// <summary>
		///		<see cref="SpacomicRSSCollectionModel"/>のイベントハンドラーに登録済みのこのクラスのイベントをすべて解除します。
		/// </summary>
		public void UnsubscribeAllEvents() {
			spacomicRSSCollectionModel.PropertyChanged -= SpacomicRSSCollectionModel_PropertyChanged;
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
	}
}
