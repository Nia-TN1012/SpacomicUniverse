using System;
using Windows.UI.Xaml.Media.Imaging;

using Chronoir_net.UniSPADA;

/// <summary>
///		すぱこみっく ユニバース
/// </summary>
namespace SpacomicUniverse {

	/// <summary>
	///		すぱこーRSSフィードの種別とRSSフィードのURLを格納します。
	/// </summary>
	public class SpacoRSSSource {
		/// <summary>
		///		すぱこーRSSフィードの種別を取得します。
		/// </summary>
		public string Type { get; private set; }

		/// <summary>
		///		RSSフィードのURLを取得します。
		/// </summary>
		public string RSSFeedURL { get; private set; }

		/// <summary>
		///		種別とRSSフィードのURLから、SpacoRSSSourceクラスの新しいインスタンスを生成します。
		/// </summary>
		/// <param name="type">すぱこーRSSフィードの種別</param>
		/// <param name="url">RSSフィードのURL</param>
		public SpacoRSSSource( string type, string url ) {
			Type = type;
			RSSFeedURL = url;
		}
	}

	/// <summary>
	///		すぱこーRSSフィードのチャネル情報を格納します。
	/// </summary>
	public class SpacoRSSSause {
		/// <summary>
		///		タイトル名を取得・設定します。
		/// </summary>
		public string Title { get; set; }

		/// <summary>
		///		概要を取得・設定します。
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		///		作者を取得・設定します。
		/// </summary>
		public string Author { get; set; }

		/// <summary>
		///		リンクを取得・設定します。
		/// </summary>
		public string Link { get; set; }

		/// <summary>
		///		RSSフィードの最新投稿日を取得・設定します。
		/// </summary>
		public DateTime PubDate { get; set; }

		/// <summary>
		///		バナー画像のURLを取得・設定します。
		/// </summary>
		public string BannerURL { get; set; }

		/// <summary>
		///		バナー画像のキャッシュを表します。
		/// </summary>
		private BitmapImage bannerCache;
		/// <summary>
		///		バナー画像のキャッシュを取得します。
		/// </summary>
		public BitmapImage BannerCache =>
			bannerCache ?? ( bannerCache = DownloadImage( BannerURL ) );

		/// <summary>
		///		指定したURLから画像を取得します。
		/// </summary>
		/// <param name="url">画像ののURL</param>
		/// <returns>画像を格納したBitmapImageインスタンス</returns>
		private BitmapImage DownloadImage( string url ) {
			BitmapImage bitmap = null;

			try {
				bitmap = new BitmapImage( new Uri( url ) );
			}
			catch( Exception ) {}

			return bitmap;
		}

		/// <summary>
		///		バナー画像のキャッシュを削除します。
		/// </summary>
		public void DeleteCache() {
			bannerCache = null;
		}
	}


	/// <summary>
	///		すぱこーRSSフィードのコンテンツ情報を格納します。
	/// </summary>
	public class SpacomicRSSItem : SpacoRSSItem {

		/// <summary>
		///		すぱこーRSSフィードの種別を取得・設定します。
		/// </summary>
		public string Type { get; set; }

		/// <summary>
		///		サムネイル画像のキャッシュを表します。
		/// </summary>
		private BitmapImage thumbnailCache;
		/// <summary>
		///		サムネイル画像のキャッシュを取得します。
		/// </summary>
		public BitmapImage ThumbnailCache =>
			thumbnailCache ?? ( thumbnailCache = DownloadImage( ThumbnailURL ) );

		/// <summary>
		///		漫画画像のキャッシュを表します。
		/// </summary>
		private BitmapImage mediaCache;
		/// <summary>
		///		漫画画像のキャッシュを取得します。
		/// </summary>
		public BitmapImage MediaCache =>
			mediaCache ?? ( mediaCache = DownloadImage( MediaURL ) );

		/// <summary>
		///		すぱこーRSSフィードの種別とSpacoRSSItemオブジェクトから、SpacomicRSSItemの新しいインスタンスを生成します。
		/// </summary>
		/// <param name="type">すぱこーRSSフィードの種別</param>
		/// <param name="item">SpacoRSSItemのオブジェクト</param>
		public SpacomicRSSItem( string type = null, SpacoRSSItem item = null ) : base() {
			Type = type;
			if( item != null ) {
				Title = item.Title;
				Description = item.Description;
				Author = item.Author;
				PubDate = item.PubDate;
				Link = item.Link;
				ModifiedDate = item.ModifiedDate;
				Volume = item.Volume;
				IsAvailable = item.IsAvailable;
				MediaURL = item.MediaURL;
				ThumbnailURL = item.ThumbnailURL;
				ID = item.ID;
			}
		}

		/// <summary>
		///		指定したURLから画像を取得します。
		/// </summary>
		/// <param name="url">画像ののURL</param>
		/// <returns>画像を格納したBitmapImageインスタンス</returns>
		private BitmapImage DownloadImage( string url ) {
			BitmapImage bitmap = null;

			try {
				bitmap = new BitmapImage( new Uri( url ) );
			}
			catch( Exception ) {}

			return bitmap;
		}

		/// <summary>
		///		サムネイル画像と漫画画像のキャッシュを削除します。
		/// </summary>
		public void DeleteCache() {
			mediaCache = null;
			thumbnailCache = null;
		}
	}
}
