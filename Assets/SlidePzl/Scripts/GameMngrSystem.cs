using Unity.Entities;
using Unity.Tiny.Debugging;
using Unity.Collections;
using Unity.Tiny.Core;
using Unity.Tiny.Input;

namespace SlidePzl
{
	public class GameMngrSystem : ComponentSystem
	{
		protected override void OnUpdate()
		{
			float timer = 0;

			Entities.ForEach( ( ref GameMngr mngr ) => {
				mngr.GameTimer += World.TinyEnvironment().frameDeltaTime;
				timer = mngr.GameTimer;
			} );

			//var inputSystem = World.GetExistingSystem<InputSystem>();
			//if( inputSystem.GetMouseButtonDown(0) ) {
			//	Debug.LogFormatAlways("t {0}", timer );
			//}

		}

		/*
		void goalCheck( ref GameMngr mngr )
		{
			Entity panelEntity = Entity.Null;
			bool isGoal = false;

			// ゴール判定（仮）.
			Entities.ForEach( ( Entity entity, ref PanelInfo panel ) => {
				if( panel.Type == 1 && panel.NextPos.x == 3 && panel.NextPos.y == 3 ) {
					isGoal = true;
					panelEntity = entity;
				}
			} );
		}*/

	}
}
