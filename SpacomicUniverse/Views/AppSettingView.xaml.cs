#region バージョン情報
/**
*	@file AppSettingView.xaml.cs
*	@brief アプリの設定画面を表します。
*
*	@par バージョン Version
*	1.0.0
*	@par 作成者 Author
*	智中ニア（Nia Tomonaka）
*	@par コピーライト Copyright
*	Copyright (C) 2016 Chronoir.net
*	@par 作成日
*	2016/10/09
*	@par 最終更新日
*	2016/10/10
*	@par ライセンス Licence
*	BSD Licence（ 2-caluse ）
*	@par 連絡先 Contact
*	@@nia_tn1012（ https://twitter.com/nia_tn1012/ ）
*	@par ホームページ Homepage
*	- http://chronoir.net/ (ホームページ)
*/
#endregion

using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

/// <summary>
///		すぱこみっく ユニバース
/// </summary>
namespace SpacomicUniverse {

	/// <summary>
	///		アプリの設定画面を表します。
	/// </summary>
	public sealed partial class AppSettingView : Page {

		/// <summary>
		///		AppSettingViewクラスの新しいインスタンスを生成します。
		/// </summary>
		public AppSettingView() {
			InitializeComponent();
		}

		/// <summary>
		///		別のページから遷移した時に実行します。
		/// </summary>
		/// <param name="e">イベント引数</param>
		protected override void OnNavigatedTo( NavigationEventArgs e ) {
			base.OnNavigatedTo( e );

			// 現在のViewを取得し、戻るボタンを表示させ、そのボタンを推した時のイベントを格納します。
			var currentView = SystemNavigationManager.GetForCurrentView();
			currentView.AppViewBackButtonVisibility =
				Frame.CanGoBack ? AppViewBackButtonVisibility.Visible : AppViewBackButtonVisibility.Collapsed;
			currentView.BackRequested += Page_BackRequested;
		}

		/// <summary>
		///		別の画面に遷移する時に実行します。
		/// </summary>
		/// <param name="e">イベント引数</param>
		protected override void OnNavigatingFrom( NavigatingCancelEventArgs e ) {
			base.OnNavigatingFrom( e );

			// 戻る場合、イベントハンドラーを削除します。
			if( e.NavigationMode == NavigationMode.Back ) {
				var currentView = SystemNavigationManager.GetForCurrentView();
				currentView.BackRequested -= Page_BackRequested;
			}
		}

		/// <summary>
		///		戻るボタンが押された時に実行します。
		/// </summary>
		private void Page_BackRequested( object sender, BackRequestedEventArgs e ) {
			if( Frame.CanGoBack ) {
				Frame.GoBack();
				e.Handled = true;
			}
		}
	}
}
