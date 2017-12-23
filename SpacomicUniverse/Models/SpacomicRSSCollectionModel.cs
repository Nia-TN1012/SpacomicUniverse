#region バージョン情報
/**
*	@file SpacomicRSSCollectionModel.cs
*	@brief すぱこーRSSフィードのコンテンツを管理します。
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
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

using Chronoir_net.UniSPADA;

/// <summary>
///		すぱこみっく ユニバース
/// </summary>
namespace SpacomicUniverse {

	/// <summary>
	///		すぱこーRSSフィードのコンテンツを管理します。
	/// </summary>
	public class SpacomicRSSCollectionModel : INotifyPropertyChanged {

		/// <summary>
		///		処理を取り消すためのトークンを表します。
		/// </summary>
		private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

		/// <summary>
		///		すぱこーRSSフィードの種別とRSSフィードのURLのリストを表します。
		/// </summary>
		private static SpacoRSSSource[] spacoSause = new SpacoRSSSource[] {
			new SpacoRSSSource( "すぱこー", "https://pronama-api.azurewebsites.net/feed/spaco" ),
			new SpacoRSSSource( "すぱこー 焼きそば編", "https://pronama-api.azurewebsites.net/feed/spacoyakisoba" )
			
			//new SpacoRSSSource( "すぱこー", "https://apis.chronoir.net/spaco-feed/?spaco=spaco" ),
			//new SpacoRSSSource( "すぱこー 焼きそば編", "https://apis.chronoir.net/spaco-feed/?spaco=spaco-yakisoba" )
		};

		/// <summary>
		///		すぱこーRSSフィードのチャネル情報を格納するコレクションを取得します。
		/// </summary>
		public Dictionary<string, SpacoRSSSause> SauseItems { get; private set; }

		/// <summary>
		///		すぱこーRSSフィードのコンテンツを格納するコレクションを取得します。
		/// </summary>
		public List<SpacomicRSSItem> Items { get; private set; }

		/// <summary>
		///		SpacoRSSListModelの新しいインスタンスを生成します。
		/// </summary>
		public SpacomicRSSCollectionModel() {
			SauseItems = new Dictionary<string, SpacoRSSSause>();
			Items = new List<SpacomicRSSItem>();
		}

		/// <summary>
		///		WebからRSSフィードを取得します。
		/// </summary>
		/// <returns>すぱこーRSSフィードのコンテンツを格納したリスト</returns>
		private async Task<IEnumerable<SpacomicRSSItem>> GetRSSCore() {
			List<SpacomicRSSItem> list = new List<SpacomicRSSItem>();

			foreach( var sause in spacoSause ) {
				// オフセット位置
				int offset = 0;
				// 続けて取得する必要があるかを表すフラグ
				bool isContinue = true;

				do {
					string url = $"{sause.RSSFeedURL}?offset={offset}";
					using( XmlReader reader = await Task.Run( () => SpacoRSSClient.GetXmlReaderAsync( url, cancellationTokenSource.Token ) ) ) {
						SpacoRSSReader srr = await Task.Run( () => SpacoRSSReader.LoadAsync( reader, cancellationTokenSource.Token ) );

						// すぱこーRSSフィードのチャネル情報を設定します。
						if( !SauseItems.ContainsKey( sause.Type ) ) {
							SauseItems[sause.Type] = new SpacoRSSSause {
								Title = srr.Title,
								Description = srr.Description,
								Author = srr.Author,
								Link = srr.Link,
								PubDate = srr.PubDate,
								BannerURL = srr.BannerURL
							};
						}

						// 取得したコンテンツの数を取得します。
						int count = srr.Items.Count();
						if( count > 0 ) {
							offset += count;
						}
						// なければ、現在ソースからののRSSフィードの取得を終了します。
						else {
							isContinue = false;
						}

						// 利用可能なコンテンツを抽出します。
						list.AddRange(
							srr.Items.Where( _ => _.IsAvailable )
									.Select( _ => new SpacomicRSSItem( sause.Type, _ ) )
									.ToList()
						);
					}
				} while( isContinue );
			}
			return list;
		}

		/// <summary>
		///		RSSフィードを取得（保存済みのローカルファイルから読み込み or Webから取得）します。
		/// </summary>
		/// <param name="forceReload">強制的にWebから取得するフラグ</param>
		public async Task GetRSS( bool forceReload = false ) {
			// 初期化
			GetRSSResult result = GetRSSResult.Succeeded;
			SauseItems.Clear();
			Items.Clear();
			cancellationTokenSource = new CancellationTokenSource();
			GetRSSStarted( this, null );

			try {
				// Webからの再取得をリクエストされた時
				if( forceReload ) {
					try {
						Items.AddRange( ( await GetRSSCore() ).OrderByDescending( _ => _.PubDate ) );
						// 取得したすぱこーRSSフィードのデータをローカルファイルに保存します。
						await SpacomicLocalIO.SaveSpacoRSSSauseFile( SauseItems );
						await SpacomicLocalIO.SaveRSSCollectionFile( Items );
					}
					// Webからの再取得に失敗した場合、ローカルファイルからリストアします。
					// Itemsが空であれば、保存済みのローカルファイルは、再取得する前のすぱこーRSSフィードのデータが残っています。
					catch( Exception ) when( !Items.Any() ) {
						// 保存済みのローカルファイルからリストア
						var spacoRSSauseFromLocal = await SpacomicLocalIO.LoadSpacoRSSSauseFile();
						var spacoRSSListFromLocal = await SpacomicLocalIO.LoadRSSCollectionFile();

						// 保存済みのローカルファイルからのリストアが成功した時
						if( spacoRSSauseFromLocal.ResultCode == GetRSSResult.Succeeded && spacoRSSListFromLocal.ResultCode == GetRSSResult.Succeeded ) {
							foreach( var item in spacoRSSauseFromLocal.Items ) {
								SauseItems[item.Key] = item.Value;
							}
							Items.AddRange( spacoRSSListFromLocal.Items );
							result = GetRSSResult.LocalDataRestored;
						}
						// リストアに失敗した場合、例外をリスローします。
						else {
							throw;
						}
					}
				}
				else {
					// 保存済みのローカルファイルから読み込み
					var spacoRSSauseFromLocal = await SpacomicLocalIO.LoadSpacoRSSSauseFile();
					var spacoRSSListFromLocal = await SpacomicLocalIO.LoadRSSCollectionFile();

					// 保存済みのローカルファイルからの読み込みが成功した時
					if( spacoRSSauseFromLocal.ResultCode == GetRSSResult.Succeeded && spacoRSSListFromLocal.ResultCode == GetRSSResult.Succeeded ) {
						foreach( var item in spacoRSSauseFromLocal.Items ) {
							SauseItems[item.Key] = item.Value;
						}
						Items.AddRange( spacoRSSListFromLocal.Items );

						// Web上に最新話があるかどうかチェックします。
						CheckNewContents();
					}
					// ローカルファイルからの読み込みに失敗した場合、Webから取得します
					else {
						Items.AddRange( ( await GetRSSCore() ).OrderByDescending( _ => _.PubDate ) );
						await SpacomicLocalIO.SaveSpacoRSSSauseFile( SauseItems );
						await SpacomicLocalIO.SaveRSSCollectionFile( Items );
					}
				}
			}
			catch( OperationCanceledException ) {
				SauseItems.Clear();
				Items.Clear();
				result = GetRSSResult.Canceled;
			}
			catch( Exception ) {
				SauseItems.Clear();
				Items.Clear();
				result = GetRSSResult.Failed;
			}
			finally {
				cancellationTokenSource.Dispose();
			}
			// すぱこーRSSフィードの取得が完了したことをViewModel側に通知します。
			GetRSSCompleted?.Invoke( this, result );
		}

		/// <summary>
		///		すぱこーRSSフィードの新しい話がWeb上にあるかどうか判別します。
		/// </summary>
		private async Task CheckNewContents() {
			// 最新話が見つかったフラグ
			bool newContentsFound = false;

			try {
				foreach( var sause in spacoSause ) {
					string url = $"{sause.RSSFeedURL}&count=1";
					// このforeachブロック専用のCancellationTokenを生成します。
					using( CancellationTokenSource cancellationTokenSourceInstant = new CancellationTokenSource() ) {
						// タイムアウトは5秒間に設定します。
						cancellationTokenSourceInstant.CancelAfter( 5000 );
						using( XmlReader reader = await Task.Run( () => SpacoRSSClient.GetXmlReaderAsync( url, cancellationTokenSourceInstant.Token ) ) ) {
							SpacoRSSReader srr = await Task.Run( () => SpacoRSSReader.LoadAsync( reader, cancellationTokenSourceInstant.Token ) );
							// 最新話のVolumeが、Items上の同じソースの最新のVolumeより大きい時、フラグをオンにします。
							if( srr.Items.First().Volume > Items.Where( _ => _.Type == sause.Type ).Max( _ => _.Volume ) ) {
								newContentsFound = true;
							}
						}
					}
				}
				// 最新話が見つかったら、ViewModelに通知します。
				if( newContentsFound ) {
					NewRSSContentsFound?.Invoke( this, new EventArgs() );
				}
			}
			catch( Exception ) {}
		}

		/// <summary>
		///		すぱこーRSSフィードのコンテンツ及びチャネル情報にある、キャッシュ済みの画像を全て削除します。
		/// </summary>
		public void DeleteCache() {
			foreach( var item in SauseItems ) {
				item.Value.DeleteCache();
			}
			foreach( var item in Items ) {
				item.DeleteCache();
			}
			ImageCachesDeleted?.Invoke( this, new EventArgs() );
		}

		/// <summary>
		///		すぱこーRSSフィードの取得開始時に発生させるイベントハンドラーです。
		/// </summary>
		public event EventHandler GetRSSStarted;

		/// <summary>
		///		すぱこーRSSフィードの取得完了後に発生させるイベントハンドラーです。
		/// </summary>
		public event EventHandler<GetRSSResult> GetRSSCompleted;

		/// <summary>
		///		すぱこーRSSフィードの新しい話が見つかった時に発生させるイベントハンドラーです。
		/// </summary>
		public event EventHandler NewRSSContentsFound;

		/// <summary>
		///		画像のキャッシュを削除した時に発生させるイベントハンドラーです。
		/// </summary>
		public event EventHandler ImageCachesDeleted;

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
		///		RSSの取得を中止します。
		/// </summary>
		public void CancelGetRSS() {
			cancellationTokenSource?.Cancel();
		}
	}
}
