using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

/// <summary>
///		すぱこみっく ユニバース
/// </summary>
namespace SpacomicUniverse {

	/// <summary>
	///		parameterで指定された文字列がソースの型名と一致するかマッチングして、Visibility値を求めます。
	/// </summary>
	public sealed class TypeNameMatchingToVisibilityConverter : IValueConverter {

		/// <summary>
		///		parameterで指定された文字列がソースの型名と一致するかマッチングして、Visibility値を求めます。
		/// </summary>
		/// <param name="value">Pageオブジェクト</param>
		/// <param name="targetType">ターゲットの型</param>
		/// <param name="parameter">マッチング文字列</param>
		/// <param name="language">言語</param>
		/// <returns>parameterで指定された文字列がソースの型名と一致した時 : Visibility.Visible / それ以外 : Visibility.Collapsed</returns>
		public object Convert( object value, Type targetType, object parameter, string language ) =>
			value != null && parameter is string &&
			// ※派生クラスの場合、そのクラスの型名を取り出すことができます。
			( string )parameter == value.GetType().Name ?
			Visibility.Visible : Visibility.Collapsed;

		/// <summary>
		///		このメソッドは使用しません。常にnullを返します。
		/// </summary>
		public object ConvertBack( object value, Type targetType, object parameter, string language ) => null;
	}
}
