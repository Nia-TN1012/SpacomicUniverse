#region バージョン情報
/**
*	@file SpacomicMainView.cs
*	@brief すぱこみっく！ユニバースのメインページを表します。
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
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

/// <summary>
///		すぱこみっく ユニバース
/// </summary>
namespace SpacomicUniverse {

	/// <summary>
	///		すぱこみっく！ユニバースのメインページを表します。
	/// </summary>
	public sealed partial class SpacomicMainView : Page {

		/// <summary>
		///		すぱこーRSSフィードの最新話を見つけた時に通知するトースト通知（天丼通知）用XMLデータを表します。
		/// </summary>
		private XmlDocument tendonNotificationXml;

		/// <summary>
		///		SpacomicMainViewクラスの新しいインスタンスを生成します。
		/// </summary>
		public SpacomicMainView() {
			InitializeComponent();
			CreateTendon();
		}

		/// <summary>
		///		すぱこーRSSフィードの最新話を見つけた時に通知する、トースト通知（天丼通知）用XMLデータを作成します。
		/// </summary>
		private void CreateTendon() {
			// トーストのテンプレートから「イメージとテキスト」のXMLデータを取得します。
			tendonNotificationXml = ToastNotificationManager.GetTemplateContent( ToastTemplateType.ToastImageAndText01 );
			var toastBindingElement = tendonNotificationXml.DocumentElement.SelectSingleNode( "./visual/binding" );

			// トーストのテキストを設定します。
			var toastTextElement = toastBindingElement.SelectSingleNode( "./text" );
			toastTextElement.AppendChild( tendonNotificationXml.CreateTextNode( "Webで、すぱこーの最新話が公開されているよ。一覧画面から再取得してみてね！" ) );

			// トーストの画像を設定します。
			var toastImageAttribute = ( XmlElement )toastBindingElement.SelectSingleNode( "./image" );
			toastImageAttribute.SetAttribute( "src", "ms-appx:///Assets/pronama-tendon.png" );

			// トーストの表示の長さを設定します。
			var toastElement = tendonNotificationXml.DocumentElement;
			toastElement.SetAttribute( "duration", "short" );
		}

		/// <summary>
		///		別のページから遷移した時に実行します。
		/// </summary>
		/// <param name="e">イベント引数</param>
		protected override void OnNavigatedTo( NavigationEventArgs e ) {
			base.OnNavigatedTo( e );

			// SplitView内のFrameのトップページに、すぱこーRSSフィード一覧をセットします。
			SpacomicContentFrame.Navigate( typeof( SpacomicRSSCollectionView ) );

			if( e.NavigationMode == NavigationMode.New ) {
				// RSSフィードを取得するコマンドを実行します。
				spacomicMainViewModel.GetRSS.Execute( false );
			}
		}

		/// <summary>
		///		SplitView内のFrameで、ナビゲートが発生した時に実行します。
		/// </summary>
		private void SpacomicContentFrame_Navigated( object sender, NavigationEventArgs e ) {
			if( e.SourcePageType != null ) {
				// Pageの型名と対応するラジオボタンにチェックを入れます。
				switch( e.SourcePageType.Name ) {
					case nameof( SpacomicRSSCollectionView ):
						SwitchSpacomicRSSFeedViewButton.IsChecked = true;
						break;
					case nameof( SpacomicComicView ):
						SwitchSpacomicComicViewButton.IsChecked = true;
						break;
					case nameof( UserGuideView ):
						SwitchUserGuideButton.IsChecked = true;
						break;
					case nameof( AppSettingView ):
						SwitchSettingAboutButton.IsChecked = true;
						break;
				}
			}
		}

		/// <summary>
		///		RSSフィード一覧ボタンをクリックした時に実行します。
		/// </summary>
		private void SwitchSpacomicRSSFeedViewButton_Click( object sender, RoutedEventArgs e ) {
			// 現在のページがRSSフィード一覧の時
			// ※このif文の条件を満たすのは、現在のページがRSSフィード一覧の時のみです。
			if( !SpacomicContentFrame.CanGoBack ) {
				// SpacomicRSSCollectionViewオブジェクトに、GridViewのロールアップを実行させます。
				( SpacomicContentFrame.Content as SpacomicRSSCollectionView )?.GridViewJumpToFirstItem();
			}
			// RSSフィード一覧ページから、別のページに遷移していた時
			else {
				// RSSフィード一覧ページまで戻ります。
				while( SpacomicContentFrame.CanGoBack ) {
					SpacomicContentFrame.GoBack();
				}
				SwitchSpacomicRSSFeedViewButton.IsChecked = true;
			}
			HamburgerButton.IsChecked = false;
		}

		/// <summary>
		///		コミックビューボタンをクリックした時に実行します。
		/// </summary>
		private void SwitchSpacomicComicViewButton_Click( object sender, RoutedEventArgs e ) {
			if( ( SpacomicContentFrame.Content as SpacomicComicView ) == null ) {
				// RSSフィード一覧ページまで戻ります。
				while( SpacomicContentFrame.CanGoBack ) {
					SpacomicContentFrame.GoBack();
				}
				var rssCollectionView = SpacomicContentFrame.Content as SpacomicRSSCollectionView;
				rssCollectionView?.NavigateToComicView();
			}
			HamburgerButton.IsChecked = false;
		}

		/// <summary>
		///		使い方ガイドボタンをクリックした時に実行します。
		/// </summary>
		private void SwitchUserGuideButton_Click( object sender, RoutedEventArgs e ) {
			SpacomicContentFrame.Navigate( typeof( UserGuideView ) );
			HamburgerButton.IsChecked = false;
		}

		/// <summary>
		///		設定ボタンをクリックした時に実行します。
		/// </summary>
		private void SwitchSettingAboutButton_Click( object sender, RoutedEventArgs e ) {
			SpacomicContentFrame.Navigate( typeof( AppSettingView ) );
			HamburgerButton.IsChecked = false;
		}

		/// <summary>
		///		すぱこーRSSフィードの取得が完了した時に実行します。
		/// </summary>
		private async void spacomicMainViewModel_GetRSSCompleted( object sender, GetRSSResult e ) {
			// 失敗した場合、エラーダイアログを表示します。
			if( e != GetRSSResult.Succeeded ) {
				await LoadErrorDialog.ShowAsync();
			}
		}

		/// <summary>
		///		エラーダイアログのボタンが押された時に実行します。
		/// </summary>
		private void loadErrorDialog_PrimaryButtonClick( ContentDialog sender, ContentDialogButtonClickEventArgs args ) {
			LoadErrorDialog.Hide();
		}

		/// <summary>
		///		すぱこーRSSフィードの最新話を見つけた時に実行します。
		/// </summary>
		private void spacomicMainViewModel_NewRSSContentsFound( object sender, EventArgs e ) {
			ToastNotification tendon = new ToastNotification( tendonNotificationXml );
			ToastNotificationManager.CreateToastNotifier().Show( tendon );
		}

		/// <summary>
		///		Webブラウザで開くボタンを押した時に実行します。
		/// </summary>
		private void OpenSpacoWithWebBrowserButton_Click( object sender, RoutedEventArgs e ) {
			var comicView = SpacomicContentFrame.Content as SpacomicComicView;
			comicView?.OpenSpacoWithWebBrowser();
		}
	}
}
