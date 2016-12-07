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

using System.Linq;
using Windows.UI.Core;
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
		///		<see cref="SpacomicRSSCollectionView"/>クラスの新しいインスタンスを生成します。
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
		///		<see cref="GridView"/>上のアイテムをタップ or クリックした時に実行します。
		/// </summary>
		private void SpacomicRSSList_ItemClick( object sender, ItemClickEventArgs e ) {
			if( ( SpacomicRSSList.Items?.Any() ?? false ) && SpacomicRSSList.SelectedIndex >= 0 ) {
				// この時点では、クリックしたアイテムのインデックスがSelectedIndexやSelectedItemなどに反映されていません。
				// まず、クリックしたアイテムと選択されたアイテムを比較して、同じインスタンスを参照していれば、コミックビューに遷移します。
				// ※キーボード操作で選択した場合、この条件文は必ず満たします。
				if( e.ClickedItem == SpacomicRSSList.SelectedItem ) {
					Frame.Navigate( typeof( SpacomicComicView ), SpacomicRSSList.SelectedIndex );
				}
			}
		}

		/// <summary>
		///		<see cref="GridView"/>上のアイテムをタップした時に実行します。
		/// </summary>
		/// <remarks>ItemClickイベントでページ遷移できた場合、このイベントは実行しません。</remarks>
		private void GridViewItem_Tapped( object sender, TappedRoutedEventArgs e ) {
			if( ( SpacomicRSSList.Items?.Any() ?? false ) && SpacomicRSSList.SelectedIndex >= 0 ) {
				Frame.Navigate( typeof( SpacomicComicView ), SpacomicRSSList.SelectedIndex );
			}
		}

		/// <summary>
		///		セマンティックズームコントロールのズームが変更された時に実行されます。
		/// </summary>
		private void SpacomicSemantics_ViewChangeStarted( object sender, SemanticZoomViewChangedEventArgs e ) {
			if( !e.IsSourceZoomedInView ) {
				var selectedGroupFirst = ( e.SourceItem.Item as SpacomicRSSItemsGroup )?.Items?.First();
				if( ( SpacomicRSSList.Items?.Any() ?? false ) && selectedGroupFirst != null ) {
					SpacomicRSSList.SelectedItem = selectedGroupFirst;
					SpacomicRSSList.ScrollIntoView( selectedGroupFirst );
				}
			}
		}

		/// <summary>
		///		<see cref="GridView"/>の先頭のアイテムにジャンプします。
		/// </summary>
		/// <remarks><see cref="SpacomicMainView"/>から呼び出します。</remarks>
		public void GridViewJumpToFirstItem() {
			if( SpacomicSemantics.IsZoomedInViewActive ) {
				if( SpacomicRSSList.Items?.Any() ?? false ) {
					SpacomicRSSList.SelectedIndex = 0;
					SpacomicRSSList.ScrollIntoView( SpacomicRSSList.Items[0] );
				}
			}
			else {
				if( SpacomicRSSDateList.Items?.Any() ?? false ) {
					SpacomicRSSDateList.SelectedIndex = 0;
					SpacomicRSSDateList.ScrollIntoView( SpacomicRSSDateList.Items[0] );
				}
			}
		}

		/// <summary>
		///		コミックビューにページ遷移し、現在選択されている話を開きます。
		/// </summary>
		/// <remarks><see cref="SpacomicMainView"/>から呼び出します。</remarks>
		public void NavigateToComicView() {
			if( SpacomicRSSList.Items?.Any() ?? false ) {
				Frame.Navigate( typeof( SpacomicComicView ), SpacomicRSSList.SelectedIndex >= 0 ? SpacomicRSSList.SelectedIndex : 0 );
			}
		}
	}
}
