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
	///		すぱこーRSSフィード一覧を表示する画面を表します。
	/// </summary>
	public sealed partial class SpacoContentsListView : Page {

		private XmlDocument toastXml;

		/// <summary>
		///		SpacoContentsListViewクラスの新しいインスタンスを生成します。
		/// </summary>
		public SpacoContentsListView() {
			InitializeComponent();
			CreateToast();
		}

		/// <summary>
		///		すぱこーRSSフィードの最新話を見つけた時に通知するトースト用XMLデータを作成します。
		/// </summary>
		private void CreateToast() {
			try {
				toastXml = ToastNotificationManager.GetTemplateContent( ToastTemplateType.ToastImageAndText01 );
				var toastBindingElement = toastXml.DocumentElement.SelectSingleNode( "./visual/binding" );
				var toastTextElement = toastBindingElement.SelectSingleNode( "./text" );
				toastTextElement.AppendChild( toastXml.CreateTextNode( "すぱこーRSSフィードの最新話をWeb上で見つけたよ。" ) );
				var toastImageAttribute = ( XmlElement )toastBindingElement.SelectSingleNode( "./image" );
				toastImageAttribute.SetAttribute( "src", "ms-appx:///Assets/Square44x44Logo.scale-200.png" );
				toastImageAttribute.SetAttribute( "alt", "logo" );
			}
			catch( Exception ) { }
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

			if( e.NavigationMode == NavigationMode.New ) {
				// RSSフィードを取得するコマンドを実行します。
				spacoRSSListViewModel.GetRSS.Execute( false );
			}
		}

		/// <summary>
		///		GridViewをタップした時に実行します。
		/// </summary>
		private void GridView_Tapped( object sender, TappedRoutedEventArgs e ) {
			GridView gridView = sender as GridView;

			if( gridView != null && gridView.Items.Any() && gridView.SelectedIndex >= 0 ) {
				Frame.Navigate( typeof( SpacoContentsView ), gridView.SelectedIndex );
			}
		}

		/// <summary>
		///		設定ボタンが押された時に実行します。
		/// </summary>
		private void SettingAboutButton_Click( object sender, RoutedEventArgs e ) {
			Frame.Navigate( typeof( AppSettingView ) );
			// ハンバーガーボタンのチェックを解除します。
			// ※これを忘れると、設定画面から戻る時に戻るボタンを2回押さなくてはならなくなります。
			// 　（1回目は、ハンバーガーボタンのチェック解除のイベント）
			// 注 : Frame.Navigateの前に行うと、SplitViewのコンテンツがフリーズします。
			HamburgerButton.IsChecked = false;
		}

		/// <summary>
		///		トップへ戻るボタンを押した時に実行します。
		/// </summary>
		private void ToTheTopButton_Click( object sender, RoutedEventArgs e ) {
			if( SpacoRSSList.Items?.Any() ?? false ) {
				SpacoRSSList.SelectedIndex = 0;
				SpacoRSSList.ScrollIntoView( SpacoRSSList.Items[0] );
			}
			HamburgerButton.IsChecked = false;
		}

		/// <summary>
		///		すぱこーRSSフィードの取得が完了した時に実行します。
		/// </summary>
		private async void spacoRSSListViewModel_GetRSSCompleted( object sender, TaskResultEventArgs e ) {
			// 失敗した場合、エラーダイアログを表示します。
			if( e.Result != TaskResult.Succeeded ) {
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
		private void spacoRSSListViewModel_NewRSSContentsFound( object sender, EventArgs e ) {
			if( toastXml != null ) {
				ToastNotification toast = new ToastNotification( toastXml );
				ToastNotificationManager.CreateToastNotifier().Show( toast );
			}
		}
	}
}
