using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// 空白ページのアイテム テンプレートについては、http://go.microsoft.com/fwlink/?LinkId=234238 を参照してください

namespace SpacomicUniverse {
	/// <summary>
	/// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
	/// </summary>
	public sealed partial class SpacomicMainView : Page {
		public SpacomicMainView() {
			InitializeComponent();
		}

		protected override void OnNavigatedTo( NavigationEventArgs e ) {
			base.OnNavigatedTo( e );

			SwitchSpacomicRSSFeedViewButton.IsChecked = true;
		}

		private void SwitchSpacomicRSSFeedViewButton_Checked( object sender, RoutedEventArgs e ) {
			if( SpacomicContentFrame.Content == null ) {
				SpacomicContentFrame.Navigate( typeof( SpacomicRSSCollectionView ) );
			}
			else {
				
			}
		}

		private void SwitchSpacomicComicViewButton_Checked( object sender, RoutedEventArgs e ) {
			
		}

		private void SwitchSettingAboutButton_Click( object sender, RoutedEventArgs e ) {
			SpacomicContentFrame.Navigate( typeof( AppSettingView ) );
		}
	}
}
