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
				InfoAry[idx] = panel.Type;
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

			float t = 0.1f;		// 移動時間.
			float spd = 1f / t;	// 移動速度.

			float vx = (panel.NextPos.x - panel.CellPos.x) * 128f * spd * dt;
			float vy = -( panel.NextPos.y - panel.CellPos.y ) * 128f * spd * dt;


			var pos = trans.Value;
			pos.x += vx;
			pos.y += vy;
			trans.Value = pos;

			panel.Timer += dt;
			if( panel.Timer >= t ) {
				panel.CellPos = panel.NextPos;

				float3 orgPos = new float3();
				orgPos.x = -128f * 2f + 64f;
				orgPos.y = 128f * 2f - 64f;

				float3 newpos = new float3( panel.NextPos.x * 128f, -panel.NextPos.y * 128f, 0 );
				newpos += orgPos;
				trans.Value = newpos;

				panel.Timer = 0;
				panel.Step = 0;
			}

		}


		bool OverlapsObjectCollider( float3 position, float3 inputPosition, float2 size )
		{
			var rect = new Rect( position.x - size.x * 0.5f, position.y - size.y * 0.5f, size.x, size.y );
			return rect.Contains( inputPosition.xy );
		}
	}
}
