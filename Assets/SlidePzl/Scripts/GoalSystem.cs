using Unity.Entities;
using Unity.Tiny.Scenes;
using Unity.Tiny.Debugging;

namespace SlidePzl
{
	public class GoalSystem : ComponentSystem
	{
		protected override void OnUpdate()
		{
			Entity panelEntity = Entity.Null;
			bool isGoal = false;

			// ゴール判定（仮）.
			Entities.ForEach( ( Entity entity, ref PanelInfo panel ) => {
				if( panel.Type == 1 && panel.NextPos.x == 3 && panel.NextPos.y == 3 ) {
					if( panel.Step == 0 ) {
						isGoal = true;
						panel.Step = 2;
						panelEntity = entity;
					}
				}
			} );

			if( isGoal ) {
				/*
				var panel = EntityManager.GetComponentData<PanelInfo>( panelEntity );
				Debug.LogFormatAlways( "st {0}", panel.Step );
				if( panel.Step == 0 ) {
					panel.Step = 2;
					Debug.LogAlways( "GOAL" );
					setPause();
				}*/
				// パネル削除.
				//SceneService.UnloadSceneInstance( panelEntity );


				//Entities.ForEach( ( ref PuzzleGen gen ) => {
				//	gen.IsGenAdditive = true;
				//} );
			}

		}

		void setPause()
		{
			Entities.ForEach( ( ref GameMngr mngr ) => {
				mngr.IsPause = true;
			} );
		}
	}
}
