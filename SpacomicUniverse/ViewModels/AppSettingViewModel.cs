using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;

/// <summary>
///		すぱこみっく ユニバース
/// </summary>
namespace SpacomicUniverse {

	/// <summary>
	///		アプリの設定画面用のViewModelクラスを表します。
	/// </summary>
	class AppSettingViewModel : INotifyPropertyChanged {

		/// <summary>
		///		SpacoRSSModelオブジェクトを表します。
		/// </summary>
		private SpacomicRSSCollectionModel spacomicRSSCollectionModel;

		/// <summary>
		///		すぱこーRSSフィードのチャネル情報のコレクションを取得します。
		/// </summary>
		public IEnumerable<SpacoRSSSause> SauseItems =>
			spacomicRSSCollectionModel.SauseItems.Values.AsEnumerable();

		/// <summary>
		///		AppSettingViewModelクラスの新しいインスタンスを生成します。
		/// </summary>
		public AppSettingViewModel() {
			// AppオブジェクトからModelを取得します。
			spacomicRSSCollectionModel = ( App.Current as App )?.SpacomicRSSCollectionModel;

			// Modelの参照の取得に失敗したら、例外をスローします。
			if( spacomicRSSCollectionModel == null ) {
				throw new Exception( $"Failed to get reference of Model's instance on {GetType().ToString()}" );
			}

			// プロパティの変更を通知します。
			spacomicRSSCollectionModel.PropertyChanged +=
				( sender, e ) =>
					PropertyChanged?.Invoke( sender, e );
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

		/// <summary>
		///		キャッシュ画像を削除するコマンドを表します。
		/// </summary>
		private ICommand deleteCache;
		/// <summary>
		///		キャッシュ画像を削除するコマンドを取得します。
		/// </summary>
		public ICommand DeleteCache =>
			deleteCache ?? ( deleteCache = new DeleteCacheCommand( this ) );

		/// <summary>
		///		コンテンツ、チャネル情報にあるキャッシュ済みの画像を削除するコマンドです。
		/// </summary>
		private class DeleteCacheCommand : ICommand {

			/// <summary>
			///		AppSettingViewModelの参照を格納します。
			/// </summary>
			private AppSettingViewModel viewModel;

			/// <summary>
			///		AppSettingViewModelの参照から、DeleteCacheCommandクラスの新しいインスタンスを生成します。
			/// </summary>
			/// <param name="_viewModel">AppSettingViewModelの参照</param>
			public DeleteCacheCommand( AppSettingViewModel _viewModel ) {
				viewModel = _viewModel;

				// コマンド実行の可否の変更を通知します。
				viewModel.PropertyChanged += ( sender, e ) =>
					CanExecuteChanged?.Invoke( sender, e );
			}

			/// <summary>
			///		コマンドを実行できるかどうかを取得します。
			/// </summary>
			/// <param name="parameter">パラメーター（使用しません）</param>
			/// <returns>コンテンツがある時 : true / コンテンツが空の時 : false</returns>
			public bool CanExecute( object parameter ) =>
				viewModel.spacomicRSSCollectionModel.Items.Any();

			/// <summary>
			///		コマンド実行の可否の変更した時のイベントハンドラーです。
			/// </summary>
			public event EventHandler CanExecuteChanged;

			/// <summary>
			///		コマンドを実行し、キャッシュ画像を削除します。
			/// </summary>
			/// <param name="parameter">パラメーター（使用しません）</param>
			public void Execute( object parameter ) {
				viewModel.spacomicRSSCollectionModel.DeleteCache();
			}

		}
	}
}
