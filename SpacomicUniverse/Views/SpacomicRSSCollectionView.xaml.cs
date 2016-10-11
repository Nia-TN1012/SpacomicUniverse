#region バージョン情報
/**
*	@file SpacomicRSSCollectionView.xaml.cs
*	@brief すぱこーRSSフィード一覧を表示する画面「コレクションビュー」を表します。
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
using System.Linq;
using Windows.Data.Xml.Dom;
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

		///// <summary>
		/////		すぱこーRSSフィードの最新話を見つけた時に通知するトースト通知（天丼通知）用XMLデータを表します。
		///// </summary>
		//private XmlDocument tendonNotificationXml;

		/// <summary>
		///		SpacomicRSSCollectionViewクラスの新しいインスタンスを生成します。
		/// </summary>
		public SpacomicRSSCollectionView() {
			InitializeComponent();
			//CreateToast();
		}

		///// <summary>
		/////		すぱこーRSSフィードの最新話を見つけた時に通知する、トースト用XMLデータを作成します。
		///// </summary>
		//private void CreateToast() {
		//	try {
		//		// トーストのテンプレートから「イメージとテキスト」のXMLデータを取得します。
		//		tendonNotificationXml = ToastNotificationManager.GetTemplateContent( ToastTemplateType.ToastImageAndText01 );
		//		var toastBindingElement = tendonNotificationXml.DocumentElement.SelectSingleNode( "./visual/binding" );

		//		// トーストのテキストを設定します。
		//		var toastTextElement = toastBindingElement.SelectSingleNode( "./text" );
		//		toastTextElement.AppendChild( tendonNotificationXml.CreateTextNode( "Webで、すぱこーRSSフィードの最新話が公開されているよ。一覧画面から再取得してみてね！" ) );

		//		// トーストの画像を設定します。
		//		var toastImageAttribute = ( XmlElement )toastBindingElement.SelectSingleNode( "./image" );
		//		toastImageAttribute.SetAttribute( "src", "ms-appx:///Assets/pronama-tendon.png" );

		//		// トーストの表示の長さを設定します。
		//		var toastElement = tendonNotificationXml.DocumentElement;
		//		toastElement.SetAttribute( "duration", "short" );
		//	}
		//	catch( Exception ) { }
		//}

		/// <summary>
		///		別のページから遷移した時に実行します。
		/// </summary>
		/// <param name="e">イベント引数</param>
		protected override void OnNavigatedTo( NavigationEventArgs e ) {
			base.OnNavigatedTo( e );

			// この画面はホーム画面なので、戻るボタンは非表示にします。
			var currentView = SystemNavigationManager.GetForCurrentView();
			currentView.AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;

			if( e.NavigationMode == NavigationMode.New ) {
				// RSSフィードを取得するコマンドを実行します。
				//spacoRSSListViewModel.GetRSS.Execute( false );
			}
		}

		/// <summary>
		///		GridView上のアイテムをタップした時に実行します。
		/// </summary>
		private void GridViewItem_Tapped( object sender, TappedRoutedEventArgs e ) {
			if( SpacomicRSSList.Items.Any() && SpacomicRSSList.SelectedIndex >= 0 ) {
				Frame.Navigate( typeof( SpacomicComicView ), SpacomicRSSList.SelectedIndex );
			}
		}

		///// <summary>
		/////		設定ボタンが押された時に実行します。
		///// </summary>
		//private void SettingAboutButton_Click( object sender, RoutedEventArgs e ) {
		//	Frame.Navigate( typeof( AppSettingView ) );
		//	// ハンバーガーボタンのチェックを解除します。
		//	// ※これを忘れると、設定画面から戻る時に戻るボタンを2回押さなくてはならなくなります。
		//	// 　（1回目は、ハンバーガーボタンのチェック解除のイベント）
		//	// 注 : Frame.Navigateの前に行うと、SplitViewのコンテンツがフリーズします。
		//	HamburgerButton.IsChecked = false;
		//}

		///// <summary>
		/////		トップへ戻るボタンを押した時に実行します。
		///// </summary>
		//private void ToTheTopButton_Click( object sender, RoutedEventArgs e ) {
		//	if( SpacomicRSSList.Items?.Any() ?? false ) {
		//		SpacomicRSSList.SelectedIndex = 0;
		//		SpacomicRSSList.ScrollIntoView( SpacomicRSSList.Items[0] );
		//	}
		//	HamburgerButton.IsChecked = false;
		//}

		///// <summary>
		/////		すぱこーRSSフィードの取得が完了した時に実行します。
		///// </summary>
		//private async void spacoRSSListViewModel_GetRSSCompleted( object sender, TaskResultEventArgs e ) {
		//	// 失敗した場合、エラーダイアログを表示します。
		//	if( e.Result != TaskResult.Succeeded ) {
		//		await LoadErrorDialog.ShowAsync();
		//	}
		//}

		///// <summary>
		/////		エラーダイアログのボタンが押された時に実行します。
		///// </summary>
		//private void loadErrorDialog_PrimaryButtonClick( ContentDialog sender, ContentDialogButtonClickEventArgs args ) {
		//	LoadErrorDialog.Hide();
		//}

		///// <summary>
		/////		すぱこーRSSフィードの最新話を見つけた時に実行します。
		///// </summary>
		//private void spacoRSSListViewModel_NewRSSContentsFound( object sender, EventArgs e ) {
		//	if( tendonNotificationXml != null ) {
		//		ToastNotification toast = new ToastNotification( tendonNotificationXml );
		//		ToastNotificationManager.CreateToastNotifier().Show( toast );
		//	}
		//}
	}
}
