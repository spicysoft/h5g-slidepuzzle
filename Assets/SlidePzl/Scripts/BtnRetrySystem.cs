using Unity.Entities;
using Unity.Tiny.Debugging;
using Unity.Tiny.UIControls;

namespace SlidePzl
{
	public class BtnRetrySystem : ComponentSystem
	{
		protected override void OnUpdate()
		{
			Entities.WithAll<BtnRetryTag>().ForEach( ( Entity entity, ref PointerInteraction pointerInteraction ) => {
				if( pointerInteraction.clicked ) {
					Debug.LogAlways("btn click");
				}
			} );
		}
	}
}
