using Unity.Entities;
using Unity.Mathematics;
using Unity.Tiny.Core;
using Unity.Tiny.Core2D;
using Unity.Tiny.Debugging;
using Unity.Tiny.Scenes;


namespace SlidePzl
{
	/// <summary>
	/// 位置調節等初期化. PuzzleGenSystemで生成してから呼ばれる.
	/// </summary>
	[UpdateAfter( typeof( PuzzleGenSystem ) )]
	public class InitPanelSystem : ComponentSystem
	{
		public const float OrgX = -128f * 2f + 64f;
		public const float OrgY = 128f * 2f - 64f - 64f;


		protected override void OnUpdate()
		{
			int count = 0;
			bool reqAddInit = false;

			float3 orgPos = new float3( OrgX, OrgY, 0 );
			//orgPos.x = -128f * 2f + 64f;
			//orgPos.y = 128f * 2f - 64f;
			//orgPos.z = 0;

			// 15個揃うまで待つ(仮).
			Entities.ForEach( ( ref PanelInfo panel ) => {
				++count;
			} );
			if( count != 15 )
				return;

			Entities.ForEach( ( ref PuzzleGen gen ) => {
				if( gen.ReqAddPanelInit ) {
					gen.ReqAddPanelInit = false;
					reqAddInit = true;
				}
			} );


			count = 0;
			Entities.ForEach( ( ref PanelInfo panel, ref Translation trans, ref Sprite2DRenderer sprite ) => {
				if( !panel.Initialized ) {
					panel.Initialized = true;

					if( reqAddInit ) {
						panel.CellPos.x = 3;
						panel.CellPos.y = 3;
						panel.NextPos = panel.CellPos;
						float3 pos = new float3( 3f * 128f, -3f * 128f, 0 );
						pos += orgPos;
						trans.Value = pos;
					}
					else {
						int v = count / 4;
						int h = count % 4;
						float3 pos = new float3( h * 128f, -v * 128f, 0 );
						pos += orgPos;

						panel.CellPos.x = h;
						panel.CellPos.y = v;
						panel.NextPos = panel.CellPos;

						trans.Value = pos;
						++count;
					}

					var col = sprite.color;
					col.a = 0;
					sprite.color = col;

				}
			} );

		}
	}
}
