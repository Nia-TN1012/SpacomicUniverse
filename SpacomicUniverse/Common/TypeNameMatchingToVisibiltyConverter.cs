#region バージョン情報
/**
*	@file TypeNameMatchingToVisibilityConverter.cs
*	@brief parameterで指定された文字列がソースの型名と一致するかマッチングして、Visibility値を求めます。
*
*	@par バージョン Version
*	1.1.0
*	@par 作成者 Author
*	智中ニア（Nia Tomonaka）
*	@par コピーライト Copyright
*	Copyright (C) 2016 Chronoir.net
*	@par 作成日
*	2016/10/09
*	@par 最終更新日
*	2016/10/11
*	@par ライセンス Licence
*	BSD Licence（ 2-caluse ）
*	@par 連絡先 Contact
*	@@nia_tn1012（ https://twitter.com/nia_tn1012/ ）
*	@par ホームページ Homepage
*	- http://chronoir.net/ (ホームページ)
*/
#endregion

using System;
using Windows.UI.Xaml;
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
