using Unity.Entities;
using Unity.Mathematics;
using Unity.Tiny.Core2D;
using Unity.Tiny.Scenes;

namespace SlidePzl
{
	public struct PuzzleGen : IComponentData
	{
		public bool IsGenerate;			// 盤面生成.
		public bool IsGenAdditive;		// 追加パネル生成.
		public bool ReqAddPanelInit;	// 追加パネル初期化リクエスト.
	}
}
