using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

/// <summary>
///		すぱこみっく ユニバース
/// </summary>
namespace SpacomicUniverse {

	/// <summary>
	///		bool値とVisibility値の相互変換を行います。
	/// </summary>
	public sealed class BoolToVisibilityConverter : IValueConverter {
		/// <summary>
		///		bool値から対応するVisibility値に変換します。
		/// </summary>
		/// <param name="value">bool値</param>
		/// <param name="targetType">ターゲットの型</param>
		/// <param name="parameter">パラメーター</param>
		/// <param name="language">言語</param>
		/// <returns>bool値と対応するVisibility値</returns>
		public object Convert( object value, Type targetType, object parameter, string language ) =>
			( value is bool && ( bool )value ) ? Visibility.Visible : Visibility.Collapsed;

		/// <summary>
		///		Visibility値から対応するbool値に変換します。
		/// </summary>
		/// <param name="value">Visibility値</param>
		/// <param name="targetType">ターゲットの型</param>
		/// <param name="parameter">パラメーター</param>
		/// <param name="language">言語</param>
		/// <returns>Visibility値と対応するbool値</returns>
		public object ConvertBack( object value, Type targetType, object parameter, string language ) =>
			value is Visibility && ( Visibility )value == Visibility.Visible;
	}

}
