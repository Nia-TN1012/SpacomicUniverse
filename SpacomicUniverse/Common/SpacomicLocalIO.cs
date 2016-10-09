﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.Storage;

/// <summary>
///		すぱこみっく ユニバース
/// </summary>
namespace SpacomicUniverse {

	/// <summary>
	///		取得したすぱこーRSSフィードを、ローカルファイルへ保存・読み出しをします。
	/// </summary>
	class SpacomicLocalIO {

		/// <summary>
		///		取得したすぱこーRSSフィードのコンテンツを保存する、ローカルファイルのパスを表します。
		/// </summary>
		private const string rssListFilePath = "spacomic_rss.xml";

		/// <summary>
		///		取得したすぱこーRSSフィードのチャネル情報を保存する、ローカルファイルのパスを表します。
		/// </summary>
		private const string rssSauseFilePath = "spacomic_sause.xml";

		/// <summary>
		///		ローカルに保存したファイルから、すぱこーRSSフィードを読み込みます。
		/// </summary>
		/// <returns>結果情報とすぱこーRSSフィードコレクションのタプルオブジェクト</returns>
		public static async Task<Tuple<TaskResult, IEnumerable<SpacomicRSSItem>>> LoadRSSCollectionFile() {
			TaskResult result = TaskResult.Succeeded;
			List<SpacomicRSSItem> list = null;

			// ローカルファイルを正しく開いた時
			try {
				StorageFolder localFolder = ApplicationData.Current.LocalFolder;
				StorageFile spacoRSSListFile = await localFolder.GetFileAsync( rssListFilePath );
				if( spacoRSSListFile != null ) {
					XElement spacoXml = XDocument.Parse( await FileIO.ReadTextAsync( spacoRSSListFile ) ).Root;
					list = spacoXml.Elements( "item" ).Select( item =>
						new SpacomicRSSItem {
							Type = item.Attribute( "type" ).Value,
							Title = item.Attribute( "title" ).Value,
							Description = item.Attribute( "description" ).Value,
							Author = item.Attribute( "author" ).Value,
							PubDate = DateTime.Parse( item.Attribute( "pubDate" ).Value ),
							Link = item.Attribute( "link" ).Value,
							ModifiedDate = DateTime.Parse( item.Attribute( "modified" ).Value ),
							Volume = int.Parse( item.Attribute( "volume" ).Value ),
							ThumbnailURL = item.Attribute( "thumbnail" ).Value,
							MediaURL = item.Attribute( "content" ).Value,
							ID = item.Attribute( "id" ).Value,
							IsAvailable = bool.Parse( item.Attribute( "isAvailable" ).Value )
						}
					).ToList();
				}
				else {
					result = TaskResult.Failed;
				}
			}
			catch( Exception ) {
				result = TaskResult.Failed;
			}

			return new Tuple<TaskResult, IEnumerable<SpacomicRSSItem>>( result, list );
		}

		/// <summary>
		///		すぱこーRSSフィードのコレクションをローカルファイルに保存します。
		/// </summary>
		/// <param name="rssList">すぱこーRSSフィードのコレクション</param>
		public static async Task SaveRSSCollectionFile( IEnumerable<SpacomicRSSItem> rssList ) {
			XDocument spacoXml = new XDocument( new XDeclaration( "1.0", "utf-8", "yes" ) );

			spacoXml.Add(
				new XElement( "spaco_rss",
					rssList.Select( item =>
						new XElement( "item",
							new XAttribute( "type", item.Type ),
							new XAttribute( "title", item.Title ),
							new XAttribute( "description", item.Description ),
							new XAttribute( "author", item.Author ),
							new XAttribute( "pubDate", item.PubDate ),
							new XAttribute( "link", item.Link ),
							new XAttribute( "modified", item.ModifiedDate ),
							new XAttribute( "volume", item.Volume ),
							new XAttribute( "thumbnail", item.ThumbnailURL ),
							new XAttribute( "content", item.MediaURL ),
							new XAttribute( "id", item.ID ),
							new XAttribute( "isAvailable", item.IsAvailable )
						)
					)
				)
			);

			StorageFolder localFolder = ApplicationData.Current.LocalFolder;
			StorageFile spacoRSSCollectionFile = await localFolder.CreateFileAsync(
				rssListFilePath, CreationCollisionOption.ReplaceExisting
			);

			await FileIO.WriteTextAsync( spacoRSSCollectionFile, spacoXml.ToString() );
		}

		/// <summary>
		///		ローカルに保存したファイルから、すぱこーRSSフィードのチャネル情報を読み込みます。
		/// </summary>
		/// <returns>結果情報とすぱこーRSSフィードのチャネル情報のタプルオブジェクト</returns>
		public static async Task<Tuple<TaskResult, IEnumerable<KeyValuePair<string, SpacoRSSSause>>>> LoadSpacoRSSSauseFile() {
			TaskResult result = TaskResult.Succeeded;
			List<KeyValuePair<string, SpacoRSSSause>> list = null;

			try {
				StorageFolder localFolder = ApplicationData.Current.LocalFolder;
				StorageFile spacoRSSSauseFile = await localFolder.GetFileAsync( rssSauseFilePath );
				// ローカルファイルを正しく開いた時
				if( spacoRSSSauseFile != null ) {
					XElement sauseXml = XDocument.Parse( await FileIO.ReadTextAsync( spacoRSSSauseFile ) ).Root;
					list = sauseXml.Elements( "sause" ).Select( item =>
						new KeyValuePair<string, SpacoRSSSause>(
							item.Attribute( "type" ).Value,
							new SpacoRSSSause {
								Title = item.Attribute( "title" ).Value,
								Description = item.Attribute( "description" ).Value,
								Author = item.Attribute( "author" ).Value,
								PubDate = DateTime.Parse( item.Attribute( "pubDate" ).Value ),
								Link = item.Attribute( "link" ).Value,
								BannerURL = item.Attribute( "banner" ).Value
							}
						)
					).ToList();
				}
				else {
					result = TaskResult.Failed;
				}
			}
			catch( Exception ) {
				result = TaskResult.Failed;
			}

			return new Tuple<TaskResult, IEnumerable<KeyValuePair<string, SpacoRSSSause>>>( result, list );
		}

		/// <summary>
		///		すぱこーRSSフィードのチャネル情報をローカルファイルに保存します。
		/// </summary>
		/// <param name="sauseInfo">すぱこーRSSフィードのチャネル情報</param>
		public static async Task SaveSpacoRSSSauseFile( IEnumerable<KeyValuePair<string, SpacoRSSSause>> sauseInfo ) {
			XDocument sauseXml = new XDocument( new XDeclaration( "1.0", "utf-8", "yes" ) );

			sauseXml.Add(
				new XElement( "spaco_sause",
					sauseInfo.Select( item =>
						new XElement( "sause",
							new XAttribute( "type", item.Key ),
							new XAttribute( "title", item.Value.Title ),
							new XAttribute( "description", item.Value.Description ),
							new XAttribute( "author", item.Value.Author ),
							new XAttribute( "link", item.Value.Link ),
							new XAttribute( "pubDate", item.Value.PubDate ),
							new XAttribute( "banner", item.Value.BannerURL )
						)
					)
				)
			);

			StorageFolder localFolder = ApplicationData.Current.LocalFolder;
			StorageFile spacoRSSSauseInfoFile = await localFolder.CreateFileAsync(
				rssSauseFilePath, CreationCollisionOption.ReplaceExisting
			);

			await FileIO.WriteTextAsync( spacoRSSSauseInfoFile, sauseXml.ToString() );
		}
	}
}