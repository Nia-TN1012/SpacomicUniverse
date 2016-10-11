#region バージョン情報
/**
*	@file Common.cs
*	@brief すぱこみっく！ユニバースで使用する、その他のクラスなどを定義しています。
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

/// <summary>
///		すぱこみっく ユニバース
/// </summary>
namespace SpacomicUniverse {

	/// <summary>
	///		任意のタスクの成功・失敗を表す列挙体です。
	/// </summary>
	public enum TaskResult {
		/// <summary>
		///		成功
		/// </summary>
		Succeeded,
		/// <summary>
		///		中止
		/// </summary>
		Canceled,
		/// <summary>
		///		失敗
		/// </summary>
		Failed
	}

	/// <summary>
	///		任意のタスクの成功・失敗のフラグを扱うイベント引数です。		
	/// </summary>
	public class TaskResultEventArgs : EventArgs {
		/// <summary>
		///		結果を表す値を取得・設定します。
		/// </summary>
		public TaskResult Result { get; set; }

		/// <summary>
		///		結果を表す値から、TaskResultEventArgsクラスの新しいインスタンスを生成します。
		/// </summary>
		/// <param name="r">結果を表す値</param>
		public TaskResultEventArgs( TaskResult r ) {
			Result = r;
		}
	}

	/// <summary>
	///		任意のタスクの成功・失敗のフラグを扱うイベント用デリゲートです。
	/// </summary>
	public delegate void TaskResultEventHandler( object sender, TaskResultEventArgs e );
}
