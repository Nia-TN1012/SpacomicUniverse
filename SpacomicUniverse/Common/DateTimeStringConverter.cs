#region バージョン情報
/**
*	@file DateTimeStringConverter.cs
*	@brief DateTimeの値を指定の書式に従った文字列に変換する、Converterクラスです。
*
*	@par バージョン Version
*	1.0.0
*	@par 作成者 Author
*	智中ニア（Nia Tomonaka）
*	@par コピーライト Copyright
*	Copyright (C) 2016 Chronoir.net
*	@par 作成日
*	2016/10/09
*	@par 最終更新日
*	2016/10/10
*	@par ライセンス Licence
*	BSD Licence（ 2-caluse ）
*	@par 連絡先 Contact
*	@@nia_tn1012（ https://twitter.com/nia_tn1012/ ）
*	@par ホームページ Homepage
*	- http://chronoir.net/ (ホームページ)
*/
#endregion

using System;
using System.Globalization;
using Windows.UI.Xaml.Data;

/// <summary>
///		すぱこみっく ユニバース
/// </summary>
namespace SpacomicUniverse {

	/// <summary>
	///		DateTimeの値を指定した書式に従って変換します。
	/// </summary>
	public sealed class DateTimeStringConverter : IValueConverter {

		/// <summary>
		///		DateTimeの値を指定した書式に従って変換します。
		/// </summary>
		/// <param name="value">DateTimeの値</param>
		/// <param name="targetType">ターゲットの値</param>
		/// <param name="parameter">書式指定文字列</param>
		/// <param name="language">カルチャ情報</param>
		/// <returns>書式指定によって変換されたDateTimeの値の文字列</returns>
		public object Convert( object value, Type targetType, object parameter, string language ) {
			if( value != null && value is DateTime ) {
				// 書式とカルチャー情報の指定
				if( parameter != null && language != null ) {
					return ( ( DateTime )value ).ToString( parameter.ToString(), new CultureInfo( language ) );
				}
				// 書式のみ指定
				else if( parameter != null ) {
					return ( ( DateTime )value ).ToString( parameter.ToString() );
				}
				// カルチャ情報のみ指定
				else if( language != null ) {
					return ( ( DateTime )value ).ToString( new CultureInfo( language ) );
				}
				// 書式、カルチャー教室ともになし（既定の書式）
				return value;
			}
			return null;
		}

		/// <summary>
		///		このメソッドは使用しません。常にnullを返します。
		/// </summary>
		public object ConvertBack( object value, Type targetType, object parameter, string language ) => null;
	}
}
