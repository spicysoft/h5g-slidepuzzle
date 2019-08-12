using Unity.Entities;
using Unity.Mathematics;

namespace SlidePzl
{
	public struct PanelInfo : IComponentData
	{
		public bool Initialized;
		public int2 CellPos;
		public int2 NextPos;
		public int Type;
		public int Status;
		public float Timer;

	}
}
