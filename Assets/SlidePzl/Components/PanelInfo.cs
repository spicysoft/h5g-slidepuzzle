using Unity.Entities;
using Unity.Mathematics;

namespace SlidePzl
{
	public struct PanelInfo : IComponentData
	{
		public int2 cellPos;

	}
}
