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

			var inputSystem = World.GetExistingSystem<InputSystem>();
			if( inputSystem.GetMouseButtonDown(0) ) {
				Debug.LogFormatAlways("t {0}", timer );
			}

		}
	}
}
