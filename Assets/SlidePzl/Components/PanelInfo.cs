using Unity.Entities;
using Unity.Mathematics;

namespace SlidePzl
{
	public struct PanelInfo : IComponentData
	{
		public bool Initialized;	// 初期化したか.
		public int2 CellPos;		// 現在のセル単位の位置.
		public int2 NextPos;		// 移動先セル.
		public int Type;			// 1:赤 2:白.
		public int Status;			// 状態.
		public float Timer;			// タイマー.

	}
}
