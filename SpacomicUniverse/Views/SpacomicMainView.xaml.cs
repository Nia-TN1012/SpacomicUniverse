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
			try {
				// トーストのテンプレートから「イメージとテキスト」のXMLデータを取得します。
				tendonNotificationXml = ToastNotificationManager.GetTemplateContent( ToastTemplateType.ToastImageAndText01 );
				var toastBindingElement = tendonNotificationXml.DocumentElement.SelectSingleNode( "./visual/binding" );

				// トーストのテキストを設定します。
				var toastTextElement = toastBindingElement.SelectSingleNode( "./text" );
				toastTextElement.AppendChild( tendonNotificationXml.CreateTextNode( "Webで、すぱこーRSSフィードの最新話が公開されているよ。一覧画面から再取得してみてね！" ) );

				// トーストの画像を設定します。
				var toastImageAttribute = ( XmlElement )toastBindingElement.SelectSingleNode( "./image" );
				toastImageAttribute.SetAttribute( "src", "ms-appx:///Assets/pronama-tendon.png" );

				// トーストの表示の長さを設定します。
				var toastElement = tendonNotificationXml.DocumentElement;
				toastElement.SetAttribute( "duration", "short" );
			}
			catch( Exception ) { }
		}

		protected override void OnNavigatedTo( NavigationEventArgs e ) {
			base.OnNavigatedTo( e );

			SwitchSpacomicRSSFeedViewButton.IsChecked = true;
			SpacomicContentFrame.Navigate( typeof( SpacomicRSSCollectionView ) );

			if( e.NavigationMode == NavigationMode.New ) {
				// RSSフィードを取得するコマンドを実行します。
				spacomicMainViewModel.GetRSS.Execute( false );
			}
		}

		private void SpacomicContentFrame_Navigated( object sender, NavigationEventArgs e ) {
			if( e.SourcePageType != null ) {
				switch( e.SourcePageType.Name ) {
					case nameof( SpacomicRSSCollectionView ):
						SwitchSpacomicRSSFeedViewButton.IsChecked = true;
						break;
				}
			}
		}

		/// <summary>
		///		RSSフィード一覧ボタンをクリックした時に実行します。
		/// </summary>
		private void SwitchSpacomicRSSFeedViewButton_Click( object sender, RoutedEventArgs e ) {
			// RSSフィード一覧ページから、別のページに遷移していた場合、RSSフィード一覧ページに戻ります。
			var currentInlineFrame = ( SpacomicContentFrame?.Content as Page )?.Frame;
			while( currentInlineFrame?.CanGoBack ?? false ) {
				currentInlineFrame.GoBack();
			}
			SwitchSpacomicRSSFeedViewButton.IsChecked = true;
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
		private async void spacomicMainViewModel_GetRSSCompleted( object sender, TaskResultEventArgs e ) {
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
		private void spacomicMainViewModel_NewRSSContentsFound( object sender, EventArgs e ) {
			if( tendonNotificationXml != null ) {
				ToastNotification tendon = new ToastNotification( tendonNotificationXml );
				ToastNotificationManager.CreateToastNotifier().Show( tendon );
			}
		}
	}
}
