using Unity.Entities;
using Unity.Tiny.Debugging;
using Unity.Collections;
using Unity.Tiny.Core;
using Unity.Tiny.Input;
using Unity.Tiny.Scenes;
using Unity.Tiny.Text;

namespace SlidePzl
{
	public class GameMngrSystem : ComponentSystem
	{
		public const float GameTimeLimit = 10f;

		protected override void OnUpdate()
		{
			bool isTitleFinished = false;
			Entities.ForEach( ( ref GameMngr mngr ) => {
				isTitleFinished = mngr.IsTitleFinished;
				if( !isTitleFinished ) {
					mngr.IsTitleFinished = true;
					mngr.IsPause = true;
				}
			} );

			if( !isTitleFinished ) {
				SceneReference panelBase = new SceneReference();
				panelBase = World.TinyEnvironment().GetConfigData<PanelConfig>().TitleScn;
				SceneService.LoadSceneAsync( panelBase );
				Debug.LogAlways( "title scn load" );
				return;
			}


			float timer = 0;
			int score = 0;
			bool isEnd = false;

			Entities.ForEach( ( ref GameMngr mngr ) => {
				if( mngr.IsPause )
					return;

				score = mngr.Score;

				// タイマー.
				mngr.GameTimer += World.TinyEnvironment().frameDeltaTime;
				timer = mngr.GameTimer;
				if( timer >= GameTimeLimit ) {
					isEnd = true;
					//mngr.GameTimer = 0;
					mngr.IsPause = true;
				}
			} );


			if( isEnd ) {
				// リザルト表示.
				SceneReference panelBase = new SceneReference();
				panelBase = World.TinyEnvironment().GetConfigData<PanelConfig>().ResultScn;
				SceneService.LoadSceneAsync( panelBase );
				Debug.LogAlways("result scn load");
			}

			// タイマー表示.
			Entities.WithAll<TextTimerTag>().ForEach( ( Entity entity ) =>
			{
				int t = (int)( GameTimeLimit - timer );
				EntityManager.SetBufferFromString<TextString>( entity, t.ToString() );
			} );

#if false
			Entities.WithAll<TextComboTag>().ForEach( ( Entity entity ) => {
				if( timer < 3f )
					EntityManager.SetBufferFromString<TextString>( entity, "" );
				else
					EntityManager.SetBufferFromString<TextString>( entity, "Combo" );
			} );
#endif
		}

	}
}
