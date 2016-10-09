using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

/// <summary>
///		すぱこみっく ユニバース
/// </summary>
namespace SpacomicUniverse {

	/// <summary>
	///		漫画画像を表示する画面用のViewModelを表します。
	/// </summary>
	class SpacoRSSContentsViewModel : INotifyPropertyChanged {

		/// <summary>
		///		SpacoRSSModelオブジェクトを表します。
		/// </summary>
		private SpacoRSSModel spacoRSSModel;

		/// <summary>
		///		すぱこーRSSフィードのコンテンツのコレクションを取得します。
		/// </summary>
		public IEnumerable<SpacoRSSContent> Items =>
			spacoRSSModel?.Items;

		/// <summary>
		///		現在選択しているコンテンツを取得します。
		/// </summary>
		public SpacoRSSContent SelectedItem =>
			spacoRSSModel?.Items != null && spacoRSSModel.Items.Any() && SelectedIndex >= 0 ?
			spacoRSSModel.Items[SelectedIndex] : null;

		/// <summary>
		///		現在選択しているコレクションのインデックスを表します。
		/// </summary>
		private int selectedIndex;
		/// <summary>
		///		現在選択しているコレクションのインデックスを取得・設定します。
		/// </summary>
		public int SelectedIndex {
			get { return selectedIndex; }
			set {
				selectedIndex = value;
				NotifyPropertyChanged();
				NotifyPropertyChanged( nameof( SelectedItem ) );
			}
		}

		/// <summary>
		///		コンテンツの幅を表します。
		/// </summary>
		private double contentWidth;
		/// <summary>
		///		コンテンツの幅を取得・設定します。
		/// </summary>
		public double ContentWidth {
			get { return contentWidth; }
			set {
				if( contentWidth != value ) {
					contentWidth = value;
					NotifyPropertyChanged();
				}
			}
		}

		/// <summary>
		///		コンテンツの高さを表します。
		/// </summary>
		private double contentHeight;
		/// <summary>
		///		コンテンツの高さを取得・設定します。
		/// </summary>
		public double ContentHeight {
			get { return contentHeight; }
			set {
				if( contentHeight != value ) {
					contentHeight = value;
					NotifyPropertyChanged();
				}
			}
		}
		
		/// <summary>
		///		SpacoRSSContentsViewModelの新しいインスタンスを生成します。
		/// </summary>
		public SpacoRSSContentsViewModel() {
			// AppオブジェクトからModelを取得します。
			spacoRSSModel = ( App.Current as App )?.SpacoRSSModel;
			if( spacoRSSModel != null ) {
				spacoRSSModel.PropertyChanged += PropertyChanged;
			}
		}

		/// <summary>
		///		プロパティ変更後に発生させるイベントハンドラーです。
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		///		プロパティ変更を通知します。
		/// </summary>
		/// <param name="propertyName">プロパティ名</param>
		private void NotifyPropertyChanged( [CallerMemberName]string propertyName = null ) {
			PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( propertyName ) );
		}
	}
}
