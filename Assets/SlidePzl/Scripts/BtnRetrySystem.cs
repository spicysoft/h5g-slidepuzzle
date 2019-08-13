using Unity.Entities;
using Unity.Tiny.Core;
using Unity.Tiny.Debugging;
using Unity.Tiny.Scenes;
using Unity.Tiny.UIControls;

namespace SlidePzl
{
	public class BtnRetrySystem : ComponentSystem
	{
//		bool isLoaded;


		protected override void OnUpdate()
		{

			bool btnOn = false;
			Entities.WithAll<BtnRetryTag>().ForEach( ( Entity entity, ref PointerInteraction pointerInteraction ) => {
				if( pointerInteraction.clicked ) {
					Debug.LogAlways("btn ret click");
					btnOn = true;
				}
			} );


			if( btnOn ) {
				// 盤面リフレッシュ.
				var env = World.TinyEnvironment();
				SceneService.UnloadAllSceneInstances( env.GetConfigData<PanelConfig>().PanelRed );
				SceneService.UnloadAllSceneInstances( env.GetConfigData<PanelConfig>().PanelWhite );

				Entities.ForEach( ( ref PuzzleGen gen ) => {
					gen.IsGenerate = true;
				} );
				SceneReference panelBase = new SceneReference();
				panelBase = env.GetConfigData<PanelConfig>().ResultScn;
				SceneService.UnloadAllSceneInstances( panelBase );

				// ポーズ解除.
				setPause( false );
			}

		}

		void setPause( bool bPause )
		{
			Entities.ForEach( ( ref GameMngr mngr ) => {
				mngr.IsPause = bPause;
			} );
		}
	}
}
