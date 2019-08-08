using Unity.Entities;
using Unity.Mathematics;
using Unity.Tiny.Core2D;
using Unity.Tiny.Debugging;

namespace SlidePzl
{
	// 位置調節. PuzzleGenSystemで生成してから呼ばれる.
	public class InitPanelSystem : ComponentSystem
	{
		protected override void OnUpdate()
		{
			int count = 0;
			float3 orgPos = new float3();
			orgPos.x = -128f * 2f + 64f;
			orgPos.y = 128f * 2f - 64f;
			orgPos.z = 0;


			// 15個揃うまで待つ(仮).
			Entities.ForEach( ( ref PanelInfo panel ) => {
				++count;
			} );
			if( count != 15 )
				return;

			count = 0;
			Entities.ForEach( ( ref PanelInfo panel, ref Translation trans ) => {
				if( !panel.Initialized ) {
					panel.Initialized = true;

					int v = count / 4;
					int h = count % 4;
					float3 pos = new float3( h * 128f, -v * 128f, 0 );
					pos += orgPos;

					panel.CellPos.x = h;
					panel.CellPos.y = v;
					panel.NextPos = panel.CellPos;

					trans.Value = pos;
					++count;
				}

			} );

		}
	}
}
