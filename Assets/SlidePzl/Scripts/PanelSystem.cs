using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Tiny.Core;
using Unity.Tiny.Core2D;
using Unity.Tiny.Debugging;
using Unity.Tiny.Input;
using Unity.Tiny.Scenes;
using Unity.Tiny.UIControls;

namespace SlidePzl
{
	/// <summary>
	/// パネルの挙動.
	/// </summary>
	[UpdateAfter( typeof( InitPanelSystem ) )]
	public class PanelSystem : ComponentSystem
	{
		public const int PnlStAppear = 0;
		public const int PnlStNormal = 1;
		public const int PnlStMove = 2;
		public const int PnlStDisappear = 3;

		public const int PnlTypeNone = 0;
		public const int PnlTypeRed = 1;
		public const int PnlTypeWhite = 2;


		protected override void OnUpdate()
		{
			Entity delEntity = Entity.Null;
			var inputSystem = World.GetExistingSystem<InputSystem>();

			bool mouseOn = false;
			if( !isPause() ) {
				mouseOn = inputSystem.GetMouseButtonDown( 0 );
			}

			// 盤面情報収集.
			NativeArray<int> InfoAry = new NativeArray<int>( 16, Allocator.Temp );
			Entities.ForEach( ( ref PanelInfo panel ) => {
				int idx = panel.CellPos.x + panel.CellPos.y * 4;
				InfoAry[idx] = panel.Type;
			} );

			Entities.ForEach( ( Entity entity, ref PanelInfo panel, ref Translation trans, ref NonUniformScale scale, ref Sprite2DRenderer sprite ) => {
				if( !panel.Initialized )
					return;

				switch( panel.Status ) {
				case PnlStAppear:
					panelAppear( ref panel, ref sprite );
					break;
				case PnlStNormal:
					if( mouseOn ) {
						panelNorm( ref panel, ref trans, ref InfoAry );
					}
					break;

				case PnlStMove:
					panelMove( ref panel, ref trans );
					break;

				case PnlStDisappear:
					if( panelDisapper( ref panel, ref scale, ref sprite ) ) {
						delEntity = entity;
					}
					break;
				}
			} );

			InfoAry.Dispose();

			if( delEntity != Entity.Null ) {
				// エンティティ削除.
				SceneService.UnloadSceneInstance( delEntity );
				// パネル追加.
				Entities.ForEach( ( ref PuzzleGen gen ) => {
					gen.IsGenAdditive = true;
				} );
				setPause( false );
			}

		}

		bool isPause()
		{
			bool _isPause = false;
			Entities.ForEach( ( ref GameMngr mngr ) => {
				if( mngr.IsPause )
					_isPause = true;
			} );
			return _isPause;
		}

		void setPause( bool bPause )
		{
			Entities.ForEach( ( ref GameMngr mngr ) => {
				mngr.IsPause = bPause;
			} );
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
						panel.Status = PnlStMove;
						return;
					}
				}
				// down
				if( panel.CellPos.y < 3 ) {
					int idx = panel.CellPos.x + ( panel.CellPos.y + 1 ) * 4;
					if( infoAry[idx] == 0 ) {
						panel.NextPos = panel.CellPos;
						panel.NextPos.y++;
						panel.Status = PnlStMove;
						return;
					}
				}
				// left
				if( panel.CellPos.x > 0 ) {
					int idx = panel.CellPos.x - 1 + panel.CellPos.y * 4;
					if( infoAry[idx] == 0 ) {
						panel.NextPos = panel.CellPos;
						panel.NextPos.x--;
						panel.Status = PnlStMove;
						return;
					}
				}
				// right
				if( panel.CellPos.x < 3 ) {
					int idx = panel.CellPos.x + 1 + panel.CellPos.y * 4;
					if( infoAry[idx] == 0 ) {
						panel.NextPos = panel.CellPos;
						panel.NextPos.x++;
						panel.Status = PnlStMove;
						return;
					}
				}
			}
		}

		void panelMove( ref PanelInfo panel, ref Translation trans )
		{
			//Debug.LogFormatAlways( "nxt {0} {1}", panel.NextPos.x, panel.NextPos.y );
			var dt = World.TinyEnvironment().frameDeltaTime;

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
				panel.Status = PnlStNormal;

				//if( panel.Type == 1 && panel.NextPos.x == 3 && panel.NextPos.y == 3 ) {
				//	Debug.LogAlways("GOAL");
				//}
			}

		}

		void panelAppear( ref PanelInfo panel, ref Sprite2DRenderer sprite )
		{
			var dt = World.TinyEnvironment().frameDeltaTime;
			panel.Timer += dt;

//			var scl = scale.Value;
//			scl -= new float3( 0.9f * dt, 0.9f * dt, 0 );
//			scale.Value = scl;

			var col = sprite.color;
			col.a = panel.Timer * 2f;
			if( col.a > 1f )
				col.a = 1f;

			if( panel.Timer >= 0.5f ) {
				panel.Status = PnlStNormal;
				panel.Timer = 0;
				col.a = 1f;
			}
			sprite.color = col;
		}

		bool panelDisapper( ref PanelInfo panel, ref NonUniformScale scale, ref Sprite2DRenderer sprite )
		{
			//Debug.LogAlways("disapp");
			var dt = World.TinyEnvironment().frameDeltaTime;
			panel.Timer += dt;

			var scl = scale.Value;
			scl -= new float3( 1.9f*dt, 1.9f*dt, 0 );
			scale.Value = scl;

			var col = sprite.color;
			col.a -= 1.9f * dt;
			sprite.color = col;


			if( panel.Timer >= 0.5f ) {
				return true;
			}
			return false;
		}


		bool OverlapsObjectCollider( float3 position, float3 inputPosition, float2 size )
		{
			var rect = new Rect( position.x - size.x * 0.5f, position.y - size.y * 0.5f, size.x, size.y );
			return rect.Contains( inputPosition.xy );
		}
	}
}
