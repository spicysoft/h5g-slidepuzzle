using Unity.Entities;
using Unity.Tiny.Scenes;
using Unity.Tiny.Debugging;
using Unity.Tiny.Core;
using Unity.Tiny.Text;

namespace SlidePzl
{
	[UpdateAfter( typeof( PanelSystem ) )]
	public class GoalSystem : ComponentSystem
	{
		protected override void OnUpdate()
		{
			bool isScored = false;
			bool isRefresh = false;
			int redCnt = 0;

			// 赤パネルの数.
			Entities.ForEach( ( Entity entity, ref PanelInfo panel ) => {
				if( !panel.Initialized )
					return;
				if( panel.Type == PanelSystem.PnlTypeRed )
					++redCnt;
			} );

			// ゴール判定（仮）.
			Entities.ForEach( ( Entity entity, ref PanelInfo panel ) => {
				if( !panel.Initialized )
					return;

				// red.
				if( panel.Type == PanelSystem.PnlTypeRed ) {
					if( panel.NextPos.x == 3 && panel.NextPos.y == 3 ) {
						if( panel.Status == PanelSystem.PnlStNormal ) {
							if( redCnt > 1 ) {
								// パネル消す.
								panel.Status = PanelSystem.PnlStDisappear;
								setPause();
							}
							else {
								// 盤面更新.
								isRefresh = true;
								panel.Status = PanelSystem.PnlStDisappear;
								setPause();
							}
							isScored = true;
						}
					}
				}
			} );


			//Entity gameMngrEntity = Entity.Null;
			if( isScored ) {
				int score = 0;
				Entities.ForEach( ( Entity entity, ref GameMngr gamemngr ) => {
					//gameMngrEntity = entity;
					gamemngr.Score += 100;
					score = gamemngr.Score;

					Debug.LogFormatAlways("input {0}", gamemngr.InputCnt );
				} );

				// スコア表示.
				Entities.WithAll<TextScoreTag>().ForEach( ( Entity entity ) =>
				{
					EntityManager.SetBufferFromString<TextString>( entity, score.ToString() );
				} );


				if( isRefresh ) {
#if false
					// パネル全消し.
					var env = World.TinyEnvironment();
					SceneService.UnloadAllSceneInstances( env.GetConfigData<PanelConfig>().PanelRed );
					SceneService.UnloadAllSceneInstances( env.GetConfigData<PanelConfig>().PanelWhite );
					// 盤面生成.
					Entities.ForEach( ( ref PuzzleGen gen ) => {
						gen.IsGenerate = true;
					} );
#else
					// ここで全消しせずにリクエストしておく.
					Entities.ForEach( ( ref GameMngr mngr ) => {
						mngr.ReqReflesh = true;
					} );
#endif
				}
			}

			/*
			if( gameMngrEntity != Entity.Null ) {
				GameMngr mngr = EntityManager.GetComponentData<GameMngr>( gameMngrEntity );
				int score = mngr.Score;
				Debug.LogFormatAlways( "sc {0}", score );
			}*/

		}

		void setPause()
		{
			Entities.ForEach( ( ref GameMngr mngr ) => {
				mngr.IsPause = true;
			} );
		}
	}
}
