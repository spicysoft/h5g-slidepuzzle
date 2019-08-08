using Unity.Entities;
using Unity.Mathematics;
using Unity.Tiny.Core;
using Unity.Tiny.Core2D;
using Unity.Tiny.Debugging;
using Unity.Tiny.Scenes;

namespace SlidePzl
{
	// エンティティ生成のみ.
	public class PuzzleGenSystem : ComponentSystem
	{
		Random _random;

		protected override void OnCreate()
		{
			_random = new Random();
			_random.InitState();
		}

		protected override void OnUpdate()
		{
			var env = World.TinyEnvironment();
			bool isGen = false;
			SceneReference panelBase = new SceneReference();


			Entities.ForEach( ( ref PuzzleGen gen ) => {
				if( gen.IsGenerate ) {
					isGen = true;
					gen.IsGenerate = false;
				}
			} );


			if( isGen ) {

				int num = _random.NextInt( 3 );
				Debug.LogFormatAlways( "rand {0}", num );

				float time = (float)World.TinyEnvironment().frameTime;
				int rnd = (int)(time * 100f);
				int ix = rnd % 3;
				rnd = World.TinyEnvironment().frameNum;
				int iy = rnd % 3;
				int idx = ix + iy * 4;

				for( int i = 0; i < 15; ++i ) {

					if( idx == i ) {
						panelBase = env.GetConfigData<PanelConfig>().PanelRed;
					}
					else {
						panelBase = env.GetConfigData<PanelConfig>().PanelWhite;
					}

					SceneService.LoadSceneAsync( panelBase );
				}
			}

		}
	}
}
