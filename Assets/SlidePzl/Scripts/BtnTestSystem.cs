using Unity.Entities;
using Unity.Tiny.Core;
using Unity.Tiny.Debugging;
using Unity.Tiny.Scenes;
using Unity.Tiny.UIControls;

namespace SlidePzl
{
	public class BtnTestSystem : ComponentSystem
	{
		bool isLoaded;


		protected override void OnUpdate()
		{
			bool btnOn = false;
			Entities.WithAll<BtnTestTag>().ForEach( ( Entity entity, ref PointerInteraction pointerInteraction ) => {
				if( pointerInteraction.clicked ) {
					//Debug.LogAlways("btn click");
					btnOn = true;
				}
			} );


			if( btnOn ) {
#if false
				var env = World.TinyEnvironment();
				SceneService.UnloadAllSceneInstances( env.GetConfigData<PanelConfig>().PanelRed );
				SceneService.UnloadAllSceneInstances( env.GetConfigData<PanelConfig>().PanelWhite );

				Entities.ForEach( ( ref PuzzleGen gen ) => {
					gen.IsGenerate = true;
				} );
#endif
				SceneReference panelBase = new SceneReference();
				var env = World.TinyEnvironment();
				panelBase = env.GetConfigData<PanelConfig>().ResultScn;
				if( !isLoaded ) {
					SceneService.LoadSceneAsync( panelBase );
					isLoaded = true;
				}
				else {
					SceneService.UnloadAllSceneInstances( panelBase );
					isLoaded = false;
				}

			}

		}
	}
}
