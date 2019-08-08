using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Tiny.Core;
using Unity.Tiny.Core2D;
using Unity.Tiny.Debugging;
using Unity.Tiny.Input;
using Unity.Tiny.UIControls;

namespace SlidePzl
{
	public class PanelSystem : ComponentSystem
	{
		protected override void OnUpdate()
		{
			var inputSystem = World.GetExistingSystem<InputSystem>();

			bool mouseOn = inputSystem.GetMouseButtonDown( 0 );
			//int cnt = 0;

			// 盤面情報収集.
			NativeArray<int> InfoAry = new NativeArray<int>( 16, Allocator.Temp );
			Entities.ForEach( ( ref PanelInfo panel ) => {
				int idx = panel.CellPos.x + panel.CellPos.y * 4;
				InfoAry[idx] = 1;
			} );

			Entities.ForEach( ( ref PanelInfo panel, ref Translation trans ) => {
				if( !panel.Initialized )
					return;

				if( panel.Step == 0 ) {
					if( mouseOn ) {
						panelNorm( ref panel, ref trans, ref InfoAry );
					}
				}
				else if( panel.Step == 1 ) {
					panelMove( ref panel, ref trans );
				}

#if false
				if( mouseOn ) {

					//float2 pos = inputSystem.GetInputPosition();
					//var wpos = GetTouchWorldPosition( pos );

					float2 size = new float2();
					size.x = size.y = 128f;

					float3 mypos = trans.Value;
					float3 mousePos = inputSystem.GetWorldInputPosition();
					//Debug.LogFormatAlways( "x {0} y {1} z {2}", mousePos.x, mousePos.y, mousePos.z );

					bool res = OverlapsObjectCollider( mypos, mousePos, size );
					if( res ) {
						//Debug.LogFormatAlways( "hit {0}, {1}", panel.CellPos.x, panel.CellPos.y );

					}
					cnt++;
				}
#endif

			} );


			InfoAry.Dispose();

			//if( mouseOn ) {
			//	Debug.LogFormatAlways( "cnt {0}", cnt );
			//}

		}


		void panelNorm( ref PanelInfo panel, ref Translation trans, ref NativeArray<int> infoAry )
		{
			float2 size = new float2();
			size.x = size.y = 128f;

			float3 mypos = trans.Value;
			var inputSystem = World.GetExistingSystem<InputSystem>();
			float3 mousePos = inputSystem.GetWorldInputPosition();
			//Debug.LogFormatAlways( "x {0} y {1} z {2}", mousePos.x, mousePos.y, mousePos.z );

			// マウスとのあたり.
			// todo 複数パネル移動対応.
			bool res = OverlapsObjectCollider( mypos, mousePos, size );
			if( res ) {
				//Debug.LogFormatAlways( "hit {0}, {1}", panel.CellPos.x, panel.CellPos.y );

				// 動けるかチェック.
				// up
				if( panel.CellPos.y > 0 ) {
					int idx = panel.CellPos.x + ( panel.CellPos.y - 1 ) * 4;
					if( infoAry[idx] == 0 ) {
						panel.NextPos = panel.CellPos;
						panel.NextPos.y--;
						panel.Step = 1;
						return;
					}
				}
				// down
				if( panel.CellPos.y < 3 ) {
					int idx = panel.CellPos.x + ( panel.CellPos.y + 1 ) * 4;
					if( infoAry[idx] == 0 ) {
						panel.NextPos = panel.CellPos;
						panel.NextPos.y++;
						panel.Step = 1;
						return;
					}
				}
				// left
				if( panel.CellPos.x > 0 ) {
					int idx = panel.CellPos.x - 1 + panel.CellPos.y * 4;
					if( infoAry[idx] == 0 ) {
						panel.NextPos = panel.CellPos;
						panel.NextPos.x--;
						panel.Step = 1;
						return;
					}
				}
				// right
				if( panel.CellPos.x < 3 ) {
					int idx = panel.CellPos.x + 1 + panel.CellPos.y * 4;
					if( infoAry[idx] == 0 ) {
						panel.NextPos = panel.CellPos;
						panel.NextPos.x++;
						panel.Step = 1;
						return;
					}
				}
			}
		}

		void panelMove( ref PanelInfo panel, ref Translation trans )
		{
			//Debug.LogFormatAlways( "nxt {0} {1}", panel.NextPos.x, panel.NextPos.y );
			var dt = (float)World.TinyEnvironment().frameDeltaTime;

			float vx = (panel.NextPos.x - panel.CellPos.x) * 128f * 4f * dt;
			float vy = -( panel.NextPos.y - panel.CellPos.y ) * 128f * 4f * dt;


			var pos = trans.Value;
			pos.x += vx;
			pos.y += vy;
			trans.Value = pos;

			panel.Timer += dt;
			if( panel.Timer >= 0.25f ) {
				panel.CellPos = panel.NextPos;

				float3 orgPos = new float3();
				orgPos.x = -128f * 2f + 64f;
				orgPos.y = 128f * 2f + 64f;

				float3 newpos = new float3( panel.NextPos.x * 128f, -panel.NextPos.y * 128f, 0 );
				newpos += orgPos;
				trans.Value = newpos;

				panel.Timer = 0;
				panel.Step = 0;
			}

		}


			/*float3 GetTouchWorldPosition( float2 mpos )
			{
				var env = World.TinyEnvironment();

				var displayInfo = World.TinyEnvironment().GetConfigData<DisplayInfo>();
				var inputSystem = World.GetExistingSystem<InputSystem>();

				var cameraEntity = Entity.Null;
				Entities.WithAll<Camera2D>().ForEach( ( Entity entity ) => { cameraEntity = entity; } );
				var windowPosition = new float2( mpos.x, mpos.y );
				var windowSize = new float2( displayInfo.width, displayInfo.height );

				return TransformHelpers.WindowToWorld( this, cameraEntity, windowPosition, windowSize );
			}*/


		bool OverlapsObjectCollider( float3 position, float3 inputPosition, float2 size )
		{
			var rect = new Rect( position.x - size.x * 0.5f, position.y - size.y * 0.5f, size.x, size.y );
			return rect.Contains( inputPosition.xy );
		}
	}
}
