using Unity.Entities;
using Unity.Mathematics;
using Unity.Tiny.Core;
using Unity.Tiny.Core2D;
using Unity.Tiny.Debugging;
using Unity.Tiny.Input;

namespace SlidePzl
{
	public class PanelSystem : ComponentSystem
	{
		protected override void OnUpdate()
		{
			var inputSystem = World.GetExistingSystem<InputSystem>();

			bool mouseOn = inputSystem.GetMouseButtonDown( 0 );
			int cnt = 0;

			Entities.ForEach( ( ref PanelInfo panel, ref Translation trans ) => {
				if( !panel.Initialized )
					return;

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
						Debug.LogFormatAlways("hit {0}, {1}", panel.CellPos.x, panel.CellPos.y);
					}
					else {
						//Debug.LogAlways( "not hit" );
					}

					cnt++;
				}

			} );


			if( mouseOn ) {
				Debug.LogFormatAlways( "cnt {0}", cnt );
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
