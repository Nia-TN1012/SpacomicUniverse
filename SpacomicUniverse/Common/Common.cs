#region バージョン情報
/**
*	@file Common.cs
*	@brief すぱこみっく！ユニバースで使用する、その他のクラスなどを定義しています。
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

/// <summary>
///		すぱこみっく ユニバース
/// </summary>
namespace SpacomicUniverse {

	/// <summary>
	///		任意のタスクの成功・失敗を表す列挙体です。
	/// </summary>
	public enum GetRSSResult {
		/// <summary>
		///		RSSフィードの取得 / 読み込みに成功しました。
		/// </summary>
		Succeeded,
		/// <summary>
		///		RSSフィードの取得を中止しました。
		/// </summary>
		Canceled,
		/// <summary>
		///		WebからRSSフィードの取得に失敗したため、保存済みのローカルファイルからリストアしました。
		/// </summary>
		LocalDataRestored,
		/// <summary>
		///		RSSフィードの取得 / 読み込みに失敗しました。
		/// </summary>
		Failed
	}
}
