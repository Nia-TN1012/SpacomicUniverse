#region バージョン情報
/**
*	@file BoolToVisibilityConverter.cs
*	@brief bool値とVisibility値の相互変換を行う、Converterクラスです。
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
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

/// <summary>
///		すぱこみっく ユニバース
/// </summary>
namespace SpacomicUniverse {

	/// <summary>
	///		bool値と<see cref="Visibility"/>値の相互変換を行います。
	/// </summary>
	public sealed class BoolToVisibilityConverter : IValueConverter {
		/// <summary>
		///		bool値から対応する<see cref="Visibility"/>値に変換します。
		/// </summary>
		/// <param name="value">bool値</param>
		/// <param name="targetType">ターゲットの型</param>
		/// <param name="parameter">パラメーター</param>
		/// <param name="language">言語</param>
		/// <returns>bool値と対応する<see cref="Visibility"/>値</returns>
		public object Convert( object value, Type targetType, object parameter, string language ) =>
			( value is bool && ( bool )value ) ? Visibility.Visible : Visibility.Collapsed;

		/// <summary>
		///		<see cref="Visibility"/>値から対応するbool値に変換します。
		/// </summary>
		/// <param name="value"><see cref="Visibility"/>値</param>
		/// <param name="targetType">ターゲットの型</param>
		/// <param name="parameter">パラメーター</param>
		/// <param name="language">言語</param>
		/// <returns><see cref="Visibility"/>値と対応するbool値</returns>
		public object ConvertBack( object value, Type targetType, object parameter, string language ) =>
			value is Visibility && ( Visibility )value == Visibility.Visible;
	}

}
