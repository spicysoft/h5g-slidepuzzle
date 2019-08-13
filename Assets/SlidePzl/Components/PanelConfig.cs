using Unity.Entities;
using Unity.Tiny.Scenes;

namespace SlidePzl
{
	public struct PanelConfig : IComponentData
	{
		public SceneReference PanelWhite;
		public SceneReference PanelRed;

		public SceneReference ResultScn;

	}
}
