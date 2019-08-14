using Unity.Collections;
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

			// 追加パネルか?
			Entities.ForEach( ( ref PuzzleGen gen ) => {
				if( gen.ReqAddPanelInit ) {
					gen.ReqAddPanelInit = false;
					reqAddInit = true;
				}
			} );

			// 追加パネル用に空きを探す.
			int2 blankCell = new int2( 3, 3 );
			if( reqAddInit ) {
				// 盤面情報用配列.
				NativeArray<int> InfoAry = new NativeArray<int>( 16, Allocator.Temp );
				// 盤面情報収集.
				Entities.ForEach( ( ref PanelInfo panel, ref Translation trans ) => {
					// 情報.
					int idx = panel.CellPos.x + panel.CellPos.y * 4;
					InfoAry[idx] = panel.Type;
				} );
				// 空きを探す.
				for( int j = 0; j < 4; ++j ) {
					bool isEnd = false;
					for( int i = 0; i < 4; ++i ) {
						int idx = i + j * 4;
						if( InfoAry[idx] == 0 ) {
							blankCell.x = i;
							blankCell.y = j;
							isEnd = true;
							break;
						}
					}
					if( isEnd )
						break;
				}
			}


			count = 0;
			Entities.ForEach( ( ref PanelInfo panel, ref Translation trans, ref Sprite2DRenderer sprite ) => {
				if( !panel.Initialized ) {
					panel.Initialized = true;

					// 追加パネル.
					if( reqAddInit ) {
						//panel.CellPos.x = 3;
						//panel.CellPos.y = 3;
						panel.CellPos = blankCell;
						panel.NextPos = panel.CellPos;
						float3 pos = new float3( panel.CellPos.x * 128f, -panel.CellPos.y * 128f, 0 );
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
