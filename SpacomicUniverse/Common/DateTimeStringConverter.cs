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
