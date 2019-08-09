using Unity.Entities;
using Unity.Tiny.Scenes;
using Unity.Tiny.Debugging;

namespace SlidePzl
{
	public class GoalSystem : ComponentSystem
	{
		protected override void OnUpdate()
		{

			// ゴール判定（仮）.
			Entities.ForEach( ( Entity entity, ref PanelInfo panel ) => {
				if( !panel.Initialized )
					return;

				if( panel.Type == 1 && panel.NextPos.x == 3 && panel.NextPos.y == 3 ) {
					if( panel.Status == PanelSystem.PnlNormal ) {
						panel.Status = PanelSystem.PnlDisappear;
						setPause();
					}
				}
			} );

		}

		void setPause()
		{
			Entities.ForEach( ( ref GameMngr mngr ) => {
				mngr.IsPause = true;
			} );
		}
	}
}
