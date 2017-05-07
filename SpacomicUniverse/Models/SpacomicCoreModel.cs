using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

using Chronoir_net.UniSPADA;

namespace SpacomicUniverse {

	public class SpacomicCoreModel {

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
		public SpacomicCoreModel() {
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
		public async Task<GetRSSResult> GetRSS( bool forceReload = false ) {
			// 初期化
			GetRSSResult result = GetRSSResult.Succeeded;
			SauseItems.Clear();
			Items.Clear();
			cancellationTokenSource = new CancellationTokenSource();

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
						if( spacoRSSauseFromLocal.Item1 == GetRSSResult.Succeeded && spacoRSSListFromLocal.Item1 == GetRSSResult.Succeeded ) {
							foreach( var item in spacoRSSauseFromLocal.Item2 ) {
								SauseItems[item.Key] = item.Value;
							}
							Items.AddRange( spacoRSSListFromLocal.Item2 );
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
					if( spacoRSSauseFromLocal.Item1 == GetRSSResult.Succeeded && spacoRSSListFromLocal.Item1 == GetRSSResult.Succeeded ) {
						foreach( var item in spacoRSSauseFromLocal.Item2 ) {
							SauseItems[item.Key] = item.Value;
						}
						Items.AddRange( spacoRSSListFromLocal.Item2 );

						// Web上に最新話があるかどうかチェックします。
						await CheckNewContents();
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

			return result;
		}

		/// <summary>
		///		すぱこーRSSフィードの新しい話がWeb上にあるかどうか判別します。
		/// </summary>
		public async Task<bool> CheckNewContents() {
			// 最新話が見つかったフラグ
			bool newContentsFound = false;

			try {
				foreach( var sause in spacoSause ) {
					string url = $"{sause.RSSFeedURL}?count=2";
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
			}
			catch( Exception ) { }

			return newContentsFound;
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
		}

		/// <summary>
		///		RSSの取得を中止します。
		/// </summary>
		public void CancelGetRSS() {
			cancellationTokenSource?.Cancel();
		}
	}
}
