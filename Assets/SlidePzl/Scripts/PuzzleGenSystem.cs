using Unity.Entities;
using Unity.Mathematics;
using Unity.Tiny.Core;
using Unity.Tiny.Core2D;
using Unity.Tiny.Debugging;
using Unity.Tiny.Scenes;

namespace SlidePzl
{
	/// <summary>
	/// エンティティ生成のみ.
	/// </summary>
	public class PuzzleGenSystem : ComponentSystem
	{
		Random _random;

		protected override void OnCreate()
		{
			_random = new Random();
			_random.InitState();
		}

		protected override void OnUpdate()
		{
			var env = World.TinyEnvironment();
			bool isGen = false;
			bool isAdd = false;
			int genCnt = 0;
			SceneReference panelBase = new SceneReference();


			Entities.ForEach( ( ref PuzzleGen gen ) => {
				if( gen.IsGenerate ) {
					isGen = true;
					gen.IsGenerate = false;
					genCnt = ++gen.GeneratedCnt;
				}
				else if( gen.IsGenAdditive ) {
					isAdd = true;
					gen.IsGenAdditive = false;
					gen.ReqAddPanelInit = true;
				}
			} );


			if( isGen ) {

				int redNum = math.min( genCnt, 6 );     // 最大で6個.

				int[] redIdices = new int[redNum];
#if true
				int ix = 0;
				int iy = 0;
				for( int i = 0; i < redNum; ++i ) {
					if( i == 0 ) {
						ix = getRandFromTime( 3 );
						iy = getRand( 3 );
						//Debug.LogFormatAlways( "1st {0} {1}", ix, iy );
					}
					else {
						ix = getRand( 4 );
						iy = getRand( 4 );
					}
					int idx = ix + iy * 4;
					if( idx >= 15 ) --idx;
					redIdices[i] = idx;
				}
#else
				for( int i = 0; i < redNum; ++i ) {
					redIdices[i] = i;
				}
#endif

				for( int i = 0; i < 15; ++i ) {
					bool isRed = false;
					for( int j = 0; j < redNum; ++j ) {
						if( i == redIdices[j] ) {
							isRed = true;
							break;
						}
					}

					if( isRed ) {
						panelBase = env.GetConfigData<PanelConfig>().PanelRed;
					}
					else {
						panelBase = env.GetConfigData<PanelConfig>().PanelWhite;
					}

					SceneService.LoadSceneAsync( panelBase );
				}
			}
			else if( isAdd ) {
				// 追加パネル.
				panelBase = env.GetConfigData<PanelConfig>().PanelWhite;
				SceneService.LoadSceneAsync( panelBase );
			}

		}

		int getRand( int max )
		{
			//float time = (float)World.TinyEnvironment().frameTime;
			//int rnd = (int)( time * 100f );
			//return rnd % max;

			return _random.NextInt( max );
		}

		int getRandFromTime( int max )
		{
			float time = (float)World.TinyEnvironment().frameTime;
			int rnd = (int)( time * 100f );
			Debug.LogFormatAlways( "rnd {0}", rnd );
			return rnd % max;
		}
	}
}
