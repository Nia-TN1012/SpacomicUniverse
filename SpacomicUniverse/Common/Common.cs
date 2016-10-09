using System;

/// <summary>
///		すぱコミック ユニバース
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
