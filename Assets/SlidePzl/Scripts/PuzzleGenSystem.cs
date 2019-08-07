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
				for( int i = 0; i < 15; ++i ) {
					int type = _random.NextInt( 2 );
					//Debug.LogFormatAlways("rnd {0}", type);

					switch( type ) {
					case 0:
						panelBase = env.GetConfigData<PanelConfig>().PanelWhite;
						break;
					case 1:
						panelBase = env.GetConfigData<PanelConfig>().PanelRed;
						break;
					}

					SceneService.LoadSceneAsync( panelBase );
				}
			}

		}
	}
}
