#region バージョン情報
/**
*	@file SpacomicRSSItem.cs
*	@brief すぱこーRSSフィードのコンテンツ情報を格納します。
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
using System.Collections.Generic;
using Windows.UI.Xaml.Media.Imaging;

using Chronoir_net.UniSPADA;
using Windows.UI.Xaml;

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
		///		種別とRSSフィードのURLから、<see cref="SpacoRSSSource"/>クラスの新しいインスタンスを生成します。
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
		/// <returns>画像を格納した<see cref="BitmapImage"/>インスタンス</returns>
		private BitmapImage DownloadImage( string url ) {
			BitmapImage bitmap = null;

			try {
				bitmap = new BitmapImage( new Uri( url ) );
				bitmap.ImageFailed += Bitmap_ImageFailed;
			}
			catch( Exception ) {}

			return bitmap;
		}

		/// <summary>
		///		画像の取得に失敗した時に実行します。
		/// </summary>
		private void Bitmap_ImageFailed( object sender, ExceptionRoutedEventArgs e ) {
            if( sender is BitmapImage bitmap ) {
                bitmap.ImageFailed -= Bitmap_ImageFailed;
                // ダミーの画像をセットします。
                bitmap.UriSource = new Uri( "ms-appx:///Assets/no_image.png" );
            }
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
		///		すぱこーRSSフィードの種別と<see cref="SpacoRSSItem"/>オブジェクトから、<see cref="SpacomicRSSItem"/>の新しいインスタンスを生成します。
		/// </summary>
		/// <param name="type">すぱこーRSSフィードの種別</param>
		/// <param name="item">SpacoRSSItemのオブジェクト</param>
		public SpacomicRSSItem( string type = null, SpacoRSSItem item = null ) : base( item ) {
			Type = type;
		}

		/// <summary>
		///		指定したURLから画像を取得します。
		/// </summary>
		/// <param name="url">画像ののURL</param>
		/// <returns>画像を格納した<see cref="BitmapImage"/>インスタンス</returns>
		private BitmapImage DownloadImage( string url ) {
			BitmapImage bitmap = null;

			try {
				bitmap = new BitmapImage( new Uri( url ) );
				bitmap.ImageFailed += Bitmap_ImageFailed;
			}
			catch( Exception ) {}

			return bitmap;
		}

		/// <summary>
		///		画像の取得に失敗した時に実行します。
		/// </summary>
		private void Bitmap_ImageFailed( object sender, ExceptionRoutedEventArgs e ) {
            if( sender is BitmapImage bitmap ) {
                bitmap.ImageFailed -= Bitmap_ImageFailed;
                // ダミーの画像をセットします。
                bitmap.UriSource = new Uri( "ms-appx:///Assets/no_image.png" );
            }
        }

		/// <summary>
		///		サムネイル画像と漫画画像のキャッシュを削除します。
		/// </summary>
		public void DeleteCache() {
			mediaCache = null;
			thumbnailCache = null;
		}
	}

	/// <summary>
	///		すぱこーRSSフィードのコンテンツをグルー化したしたものを格納します。
	/// </summary>
	class SpacomicRSSItemsGroup {
		/// <summary>
		///		グループ名を取得・設定します。
		/// </summary>
		public string GroupTitle { get; set; }

		/// <summary>
		///		グループ内のコレクションを取得・設定します。
		/// </summary>
		public IEnumerable<SpacomicRSSItem> Items { get; set; }
	}
}
