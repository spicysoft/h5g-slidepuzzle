using Unity.Entities;
using Unity.Mathematics;
using Unity.Tiny.Core;
using Unity.Tiny.Core2D;
using Unity.Tiny.Debugging;
using Unity.Tiny.Scenes;

namespace SlidePzl
{
	public class PuzzleGenSystem : ComponentSystem
	{
		protected override void OnUpdate()
		{
			var env = World.TinyEnvironment();
			bool isGen = false;
			float2 orgPos = new float2();
			SceneReference panelBase = new SceneReference();


			Entities.ForEach( ( ref PuzzleGen gen ) => {
				if( gen.IsGenerate ) {

					isGen = true;

					gen.OrginPos.x = -128f * 2f + 64f;
					gen.OrginPos.y = 128f * 2f + 64f;

					orgPos = gen.OrginPos;

					//SceneReference panelBase = new SceneReference();
					panelBase = gen.PanalRed;

					//var panelary = env.GetConfigBufferData<PanelConfig>().Reinterpret<SceneReference>().ToNativeArray( Unity.Collections.Allocator.Temp );
					//Debug.LogAlways(panelary[0].ToString());
					//Debug.LogAlways( "1111" );

					//SceneService.LoadSceneAsync( gen.PanalRed );

					//SceneService.LoadSceneAsync( panelary[0] );
					//Debug.LogAlways( "222" );

					//panelary.Dispose();

#if false
					var trans = EntityManager.GetComponentData<Translation>( gen.panel );
					float3 pos = new float3();
					pos.x = gen.OrginPos.x;
					pos.y = gen.OrginPos.y;
					pos.z = 0;
					trans.Value = pos;
					EntityManager.SetComponentData<Translation>( gen.panel, trans );
#endif
					gen.IsGenerate = false;
				}
			} );


			if( isGen ) {
				for( int i = 0; i < 15; ++i ) {
					if( i % 2 == 0 )
						panelBase = env.GetConfigData<PanelConfig>().PanelWhite;
					else
						panelBase = env.GetConfigData<PanelConfig>().PanelRed;
					SceneService.LoadSceneAsync( panelBase );
				}
			}

		}
	}
}
