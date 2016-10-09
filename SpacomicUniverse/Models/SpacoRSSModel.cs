using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
	public class SpacoRSSModel : INotifyPropertyChanged {

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
		public ObservableCollection<SpacoRSSContent> Items { get; private set; }

		/// <summary>
		///		SpacoRSSListModelの新しいインスタンスを生成します。
		/// </summary>
		public SpacoRSSModel() {
			SauseItems = new Dictionary<string, SpacoRSSSause>();
			Items = new ObservableCollection<SpacoRSSContent>();
		}

		/// <summary>
		///		WebからRSSフィードを取得します。
		/// </summary>
		/// <returns>すぱこーRSSフィードのコンテンツを格納したリスト</returns>
		private async Task<IEnumerable<SpacoRSSContent>> GetRSSCore() {
			List<SpacoRSSContent> list = new List<SpacoRSSContent>();

			foreach( var sause in spacoSause ) {
				// オフセット位置
				int offset = 0;
				// 続けて取得する必要があるかを表すフラグ
				bool isContinue = true;

				do {
					string url = $"{sause.RSSFeedURL}?offset={offset}";
					using( XmlReader reader = await Task.Run( () => SpacoRSSClient.GetXmlReaderAsync( url, cancellationTokenSource.Token ) ) ) {
						SpacoRSSReader srr = await Task.Run( () => SpacoRSSReader.LoadAsync( reader, cancellationTokenSource.Token ) );

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
									.Select( _ => new SpacoRSSContent( sause.Type, _ ) )
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
			TaskResult result = TaskResult.Succeeded;
			SauseItems.Clear();
			Items.Clear();
			cancellationTokenSource = new CancellationTokenSource();

			try {
				// Webからの再取得をリクエストされた時
				if( forceReload ) {
					IEnumerable<SpacoRSSContent> list = await GetRSSCore();
					foreach( var item in list.OrderByDescending( _ => _.PubDate ) ) {
						Items.Add( item );
					}
					// ローカルファイルに保存します。
					await SpacoUniverseIO.SaveSpacoRSSSauseInfoFile( SauseItems );
					await SpacoUniverseIO.SaveRSSTempFile( Items );
				}
				else {
					// 保存済みのローカルファイルから読み込み
					var spacoRSSauseFromLocal = await SpacoUniverseIO.LoadSpacoRSSSauseFile();
					var spacoRSSListFromLocal = await SpacoUniverseIO.LoadRSSListFile();

					// 保存済みのローカルファイルからの読み込みが成功した時
					if( spacoRSSauseFromLocal.Item1 == TaskResult.Succeeded && spacoRSSListFromLocal.Item1 == TaskResult.Succeeded ) {
						foreach( var item in spacoRSSauseFromLocal.Item2 ) {
							SauseItems[item.Key] = item.Value;
						}
						IEnumerable<SpacoRSSContent> list = spacoRSSListFromLocal.Item2;
						foreach( var item in list ) {
							Items.Add( item );
						}
					}
					// ローカルファイルからの読み込みに失敗した場合、Webから取得します
					else {
						IEnumerable<SpacoRSSContent> list = await GetRSSCore();
						foreach( var item in list.OrderByDescending( _ => _.PubDate ) ) {
							Items.Add( item );
						}
						// ローカルファイルに保存します。
						await SpacoUniverseIO.SaveSpacoRSSSauseInfoFile( SauseItems );
						await SpacoUniverseIO.SaveRSSTempFile( Items );
					}
				}
			}
			catch( OperationCanceledException ) {
				SauseItems.Clear();
				Items.Clear();
				result = TaskResult.Canceled;
			}
			catch( Exception ) {
				SauseItems.Clear();
				Items.Clear();
				result = TaskResult.Failed;
			}
			finally {
				cancellationTokenSource = null;
			}

			// すぱこーRSSフィードの取得が完了したことをViewModel側に通知します。
			GetRSSCompleted?.Invoke( this, new TaskResultEventArgs( result ) );
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
		///		RSSの取得完了後に発生させるイベントハンドラーです。
		/// </summary>
		public event TaskResultEventHandler GetRSSCompleted;

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
