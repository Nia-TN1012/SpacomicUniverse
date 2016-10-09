﻿using System;
using System.Linq;
using Windows.UI.Core;
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

		/// <summary>
		///		SpacoContentsListViewクラスの新しいインスタンスを生成します。
		/// </summary>
		public SpacoContentsListView() {
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
		///		設定ボタンが押された時に実行します。
		/// </summary>
		private void SettingAboutButton_Click( object sender, RoutedEventArgs e ) {
			Frame.Navigate( typeof( AppSettingView ) );
			// ハンバーガーボタンのチェックを解除します。
			// ※これを忘れると、設定画面から戻る時に戻るボタンを2回押さなくてはならなくなります。
			// 　（1回目は、ハンバーガーボタンのチェック解除のイベント）
			HamburgerButton.IsChecked = false;
		}
	}
}
