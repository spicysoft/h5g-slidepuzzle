using Unity.Entities;
using Unity.Mathematics;
using Unity.Tiny.Core2D;
using Unity.Tiny.Debugging;

namespace SlidePzl
{
	public class InitPanelSystem : ComponentSystem
	{
		protected override void OnUpdate()
		{
			int count = 0;
			float3 orgPos = new float3();
			orgPos.x = -128f * 2f + 64f;
			orgPos.y = 128f * 2f + 64f;
			orgPos.z = 0;

			Entities.ForEach( ( ref PanelInfo panel, ref Translation trans ) => {
				if( !panel.Initialized ) {
					panel.Initialized = true;

					int v = count / 4;
					int h = count % 4;
					float3 pos = new float3( h * 128f, -v * 128f, 0 );
					pos += orgPos;

					panel.CellPos.x = h;
					panel.CellPos.y = v;

					trans.Value = pos;
					++count;
				}

			} );

		}
	}
}
