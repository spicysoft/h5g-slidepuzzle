using Unity.Entities;
using Unity.Mathematics;

namespace SlidePzl
{
	public struct PanelInfo : IComponentData
	{
		/*public enum ePanelType
		{
			White,
			Red,
		};*/
		public bool Initialized;
		public int2 CellPos;
		public int Type;
	}
}
