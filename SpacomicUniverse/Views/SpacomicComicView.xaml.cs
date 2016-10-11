#region バージョン情報
/**
*	@file SpacomicComicView.xaml.cs
*	@brief 漫画の画像を表示する画面「コミックビュー」を表します。
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

using System;
using System.Threading.Tasks;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

/// <summary>
///		すぱこみっく ユニバース
/// </summary>
namespace SpacomicUniverse {

	/// <summary>
	///		漫画の画像を表示する画面「コミックビュー」を表します。
	/// </summary>
	public sealed partial class SpacomicComicView : Page {

		/// <summary>
		///		FlipViewが初期化されたかどうかを表します。
		/// </summary>
		private bool flipViewInitialized = false;

		/// <summary>
		///		SpacoContentsViewクラスの新しいインスタンスを生成します。
		/// </summary>
		public SpacomicComicView() {
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

			// RSSフィード一覧のGridViewで選択したインデックスを、ViewModel側にも設定します。
			if( e.NavigationMode == NavigationMode.New ) {
				spacomicComicViewModel.SelectedIndex = e.Parameter is int ? ( int )e.Parameter : 0;
			}
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

		/// <summary>
		///		コンテンツをダブルタップした時に実行します。
		/// </summary>
		private void ScrollViewer_DoubleTapped( object sender, DoubleTappedRoutedEventArgs e ) {
			var scrollViewwer = sender as ScrollViewer;
			// ズームが1倍より大きい時、1倍に戻します。
			if( scrollViewwer.ZoomFactor > 1.0f ) {
				scrollViewwer.ChangeView( 0, 0, 1.0f, true );
			}
			// ズームが1倍の時、拡大します。
			else {
				var contentImage = ( scrollViewwer.Content as Image )?.Source as BitmapImage;
				var doubleTapPoint = e.GetPosition( scrollViewwer );

				if( contentImage != null ) {
					// 画像のサイズとScrollViewerのViewportとの比率を、縦横それぞれ求めます。
					float zoomFactorWidth = ( float )( contentImage.PixelWidth / scrollViewwer.ViewportWidth );
					float zoomFactorHeight = ( float )( contentImage.PixelHeight / scrollViewwer.ViewportHeight );

					// ZoomFactorは比率の大きい方を設定します。
					scrollViewwer.ChangeView( doubleTapPoint.X, doubleTapPoint.Y, Math.Max( zoomFactorWidth, zoomFactorHeight ), true );
				}
			}
		}

		/// <summary>
		///		FlipViewのサイズが変化した時に実行します。
		/// </summary>
		private void FlipView_SizeChanged( object sender, SizeChangedEventArgs e ) {
			spacomicComicViewModel.ContentWidth = e.NewSize.Width;
			spacomicComicViewModel.ContentHeight = e.NewSize.Height;
		}

		/// <summary>
		///		FlipViewのDataContextが変更された時に実行します。
		/// </summary>
		private void FlipView_DataContextChanged( FrameworkElement sender, DataContextChangedEventArgs args ) {
			// 初期化済みフラグがオフの時、FlipViewのインデックスをViewModelのインデックスに設定します。
			if( !flipViewInitialized ) {
				var flipView = sender as FlipView;
				flipView.SelectedIndex = spacomicComicViewModel.SelectedIndex;
				flipViewInitialized = true;
			}
		}

		/// <summary>
		///		FlipViewのコンテンツ選択が変化した時に実行します。
		/// </summary>
		private void FlipView_SelectionChanged( object sender, SelectionChangedEventArgs e ) {
			// 初期化済みフラグがオンの時、ViewModelのインデックスをFlipViewのインデックスに設定します。
			if( flipViewInitialized ) {
				var flipView = sender as FlipView;
				spacomicComicViewModel.SelectedIndex = flipView.SelectedIndex;
			}
		}

		/// <summary>
		///		
		/// </summary>
		public async Task OpenSpacoWithWebBrowser() {
			try {
				if( spacomicComicViewModel.SelectedItem != null ) {
					// リンク先を既定のブラウザで開きます。
					await Launcher.LaunchUriAsync( new Uri( spacomicComicViewModel.SelectedItem.Link ) );
				}
			}
			catch( Exception ) { }
		}
	}
}
