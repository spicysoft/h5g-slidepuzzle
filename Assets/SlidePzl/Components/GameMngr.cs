using Unity.Entities;
using Unity.Mathematics;
using Unity.Tiny.Core2D;
using Unity.Tiny.Scenes;
using Unity.Collections;

namespace SlidePzl
{
	public struct GameMngr : IComponentData
	{
		public bool IsTitleFinished;
		public float GameTimer;
		public int Score;
		public bool IsPause;
	}
}
