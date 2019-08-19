using Unity.Entities;
using Unity.Mathematics;
using Unity.Tiny.Core2D;
using Unity.Tiny.Scenes;
using Unity.Collections;

namespace SlidePzl
{
	public struct GameMngr : IComponentData
	{
		public bool IsTitleFinished;    // タイトル終了したか.
		public bool IsPause;        // ポーズするか.
		public bool ReqReflesh;     // 盤面更新リクエスト.
		public float GameTimer;		// 時間.
		public int Score;			// スコア.
		public int InputCnt;		// 有効な手数.
		public int InputCntGoal;	// ゴールした手数.
		public int ComboCnt;		// コンボ数 (0, 1, 2, 3).
	}
}
