#region バージョン情報
/**
*	@file SpacomicMainViewModel.cs
*	@brief すぱこみっく！ユニバースのメインページのViewModelを表します。
*
*	@par バージョン Version
*	1.2.5
*	@par 作成者 Author
*	智中ニア（Nia Tomonaka）
*	@par コピーライト Copyright
*	Copyright (C) 2016 Chronoir.net
*	@par 作成日
*	2016/10/09
*	@par 最終更新日
*	2016/12/08
*	@par ライセンス Licence
*	BSD Licence（ 2-caluse ）
*	@par 連絡先 Contact
*	@@nia_tn1012（ https://twitter.com/nia_tn1012/ ）
*	@par ホームページ Homepage
*	- http://chronoir.net/ (ホームページ)
*/
#endregion

using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;

/// <summary>
///		すぱこみっく ユニバース
/// </summary>
namespace SpacomicUniverse {

	/// <summary>
	///		すぱこみっく！ユニバースのメインページのViewModelを表します。
	/// </summary>
	class SpacomicMainViewModel : INotifyPropertyChanged {

		/// <summary>
		///		SpacoRSSModelオブジェクトを表します。
		/// </summary>
		private SpacomicRSSCollectionModel spacomicRSSCollectionModel;

		/// <summary>
		///		コミックビューを開けるかどうか判別する値を取得します。
		/// </summary>
		public bool CanSwitchComicView =>
			!IsProgress && spacomicRSSCollectionModel.Items.Any();

		/// <summary>
		///		すぱこーRSSフィード取得中のフラグを表します。
		/// </summary>
		private bool isProgress = false;
		/// <summary>
		///		すぱこーRSSフィード取得中のフラグを取得します。
		/// </summary>
		public bool IsProgress {
			get { return isProgress; }
			private set {
				isProgress = value;
				NotifyPropertyChanged();
			}
		}

		/// <summary>
		///		SpacomicMainViewModelクラスの新しいインスタンスを生成します。
		/// </summary>
		public SpacomicMainViewModel() {
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

			// RSSフィードの取得が完了したことをView側に通知します。
			spacomicRSSCollectionModel.GetRSSCompleted +=
				( sender, e ) => {
					// RSSフィード取得中のフラグをオフにします。
					IsProgress = false;
					NotifyPropertyChanged( nameof( CanSwitchComicView ) );
					// RSSフィード取得完了したことをView側に通知します。
					GetRSSCompleted?.Invoke( this, e );
				};

			// すぱこーRSSフィードの最新話が見つかったことを通知します。
			spacomicRSSCollectionModel.NewRSSContentsFound +=
				( sender, e ) =>
					NewRSSContentsFound?.Invoke( sender, e );
		}

		/// <summary>
		///		すぱこーRSSフィードを取得します。
		/// </summary>
		/// <param name="forceReload">強制的にWebから取得するフラグ</param>
		private void GetSpacoRSS( bool forceReload = false ) {
			// RSSフィード取得中のフラグをオンにします。
			IsProgress = true;
			NotifyPropertyChanged( nameof( CanSwitchComicView ) );
			spacomicRSSCollectionModel.GetRSS( forceReload );
		}

		/// <summary>
		///		すぱこーRSSフィードの取得完了後に発生させるイベントハンドラーです。
		/// </summary>
		public event EventHandler<GetRSSResult> GetRSSCompleted;

		/// <summary>
		///		すぱこーRSSフィードの新しい話が見つかった時に発生させるイベントハンドラーです。
		/// </summary>
		public event EventHandler NewRSSContentsFound;

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
		///		RSSフィードを取得するコマンドを表します。
		/// </summary>
		private ICommand getRSS;
		/// <summary>
		///		RSSフィードを取得するコマンドを取得します。
		/// </summary>
		public ICommand GetRSS =>
			getRSS ?? ( getRSS = new GetRSSCommand( this ) );

		/// <summary>
		///		RSSフィードを取得を中止するコマンドを表します。
		/// </summary>
		private ICommand cancelGetRSS;
		/// <summary>
		///		RSSフィードを取得を中止するコマンドを取得します。
		/// </summary>
		public ICommand CancelGetRSS =>
			cancelGetRSS ?? ( cancelGetRSS = new CancelGetRSSCommand( this ) );

		/// <summary>
		///		RSSフィードを取得するコマンドです。
		/// </summary>
		private class GetRSSCommand : ICommand {

			/// <summary>
			///		SpacomicMainViewModelの参照を表します。
			/// </summary>
			private SpacomicMainViewModel viewModel;

			/// <summary>
			///		SpacomicMainViewModelの参照から、GetRSSCommandクラスの新しいインスタンスを生成します。
			/// </summary>
			/// <param name="_viewModel">SpacomicMainViewModelの参照</param>
			public GetRSSCommand( SpacomicMainViewModel _viewModel ) {
				viewModel = _viewModel;

				// コマンド実行の可否の変更を通知します。
				viewModel.PropertyChanged += ( sender, e ) =>
					CanExecuteChanged?.Invoke( sender, e );
			}

			/// <summary>
			///		コマンドを実行できるかどうかを取得します。
			/// </summary>
			/// <param name="parameter">パラメーター（使用しません）</param>
			/// <returns>RSSフィードを取得していない時 : true / RSSフィードを取得している時 : false</returns>
			public bool CanExecute( object parameter ) =>
				!viewModel.IsProgress;

			/// <summary>
			///		コマンド実行の可否の変更した時のイベントハンドラーです。
			/// </summary>
			public event EventHandler CanExecuteChanged;

			/// <summary>
			///		コマンドを実行し、RSSフィードを取得します。
			/// </summary>
			/// <param name="parameter">パラメーター（強制的にWebから取得するフラグ）</param>
			public void Execute( object parameter ) {
				bool forceReload;
				if( !bool.TryParse( parameter.ToString(), out forceReload ) ) {
					forceReload = false;
				}

				// RSSフィード取得します。
				viewModel.GetSpacoRSS( forceReload );
			}
		}

		/// <summary>
		///		RSSフィードを取得を中止するコマンドです。
		/// </summary>
		private class CancelGetRSSCommand : ICommand {

			/// <summary>
			///		SpacomicMainViewModelの参照を表します。
			/// </summary>
			private SpacomicMainViewModel viewModel;

			/// <summary>
			///		SpacomicMainViewModelの参照から、CancelGetRSSCommandクラスの新しいインスタンスを生成します。
			/// </summary>
			/// <param name="_viewModel">SpacomicMainViewModelの参照</param>
			public CancelGetRSSCommand( SpacomicMainViewModel _viewModel ) {
				viewModel = _viewModel;

				// コマンド実行の可否の変更を通知します。
				viewModel.PropertyChanged += ( sender, e ) =>
					CanExecuteChanged?.Invoke( sender, e );
			}

			/// <summary>
			///		コマンドを実行できるかどうかを取得します。
			/// </summary>
			/// <param name="parameter">パラメーター（使用しません）</param>
			/// <returns>RSSフィードを取得している時 : true / RSSフィードを取得していない時 : false</returns>
			public bool CanExecute( object parameter ) =>
				viewModel.IsProgress;

			/// <summary>
			///		コマンド実行の可否の変更した時のイベントハンドラーです。
			/// </summary>
			public event EventHandler CanExecuteChanged;

			/// <summary>
			///		コマンドを実行し、RSSフィードの取得を中止します。
			/// </summary>
			/// <param name="parameter">パラメーター（使用しません）</param>
			public void Execute( object parameter ) {
				if( viewModel.IsProgress ) {
					// RSSフィード取得を中止します。
					viewModel.spacomicRSSCollectionModel.CancelGetRSS();
					// RSSフィード取得中のフラグをオフにします。
					viewModel.IsProgress = false;
				}
			}
		}
	}
}
