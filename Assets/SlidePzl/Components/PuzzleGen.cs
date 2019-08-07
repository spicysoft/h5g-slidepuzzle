using Unity.Entities;
using Unity.Mathematics;
using Unity.Tiny.Core2D;
using Unity.Tiny.Scenes;

namespace SlidePzl
{
	public struct PuzzleGen : IComponentData
	{
		public float2 OrginPos;
		public bool IsGenerate;
		public SceneReference PanalRed;


		public Entity panel;
	}
}
