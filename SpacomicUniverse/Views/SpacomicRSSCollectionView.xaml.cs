#region バージョン情報
/**
*	@file SpacomicRSSCollectionView.xaml.cs
*	@brief すぱこーRSSフィード一覧を表示する画面「コレクションビュー」を表します。
*
*	@par バージョン Version
*	1.1.0
*	@par 作成者 Author
*	智中ニア（Nia Tomonaka）
*	@par コピーライト Copyright
*	Copyright (C) 2016 Chronoir.net
*	@par 作成日
*	2016/10/09
*	@par 最終更新日
*	2016/10/11
*	@par ライセンス Licence
*	BSD Licence（ 2-caluse ）
*	@par 連絡先 Contact
*	@@nia_tn1012（ https://twitter.com/nia_tn1012/ ）
*	@par ホームページ Homepage
*	- http://chronoir.net/ (ホームページ)
*/
#endregion

using System;
using System.Linq;
using Windows.Data.Xml.Dom;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

/// <summary>
///		すぱこみっく ユニバース
/// </summary>
namespace SpacomicUniverse {

	/// <summary>
	///		すぱこーRSSフィード一覧を表示する画面「コレクションビュー」を表します。
	/// </summary>
	public sealed partial class SpacomicRSSCollectionView : Page {

		/// <summary>
		///		SpacomicRSSCollectionViewクラスの新しいインスタンスを生成します。
		/// </summary>
		public SpacomicRSSCollectionView() {
			InitializeComponent();
		}

		/// <summary>
		///		別のページから遷移した時に実行します。
		/// </summary>
		/// <param name="e">イベント引数</param>
		protected override void OnNavigatedTo( NavigationEventArgs e ) {
			base.OnNavigatedTo( e );

			// この画面はホーム画面なので、戻るボタンは非表示にします。
			var currentView = SystemNavigationManager.GetForCurrentView();
			currentView.AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
		}

		/// <summary>
		///		GridView上のアイテムをタップ or クリックした時に実行します。
		/// </summary>
		private void SpacomicRSSList_ItemClick( object sender, ItemClickEventArgs e ) {
			if( SpacomicRSSList.Items.Any() && SpacomicRSSList.SelectedIndex >= 0 ) {
				Frame.Navigate( typeof( SpacomicComicView ), SpacomicRSSList.SelectedIndex );
			}
		}

		/// <summary>
		///		GridViewの先頭のアイテムにジャンプします。
		/// </summary>
		public void GridViewJumpToFirstItem() {
			if( SpacomicRSSList.Items?.Any() ?? false ) {
				SpacomicRSSList.SelectedIndex = 0;
				SpacomicRSSList.ScrollIntoView( SpacomicRSSList.Items[0] );
			}
		}
	}
}
