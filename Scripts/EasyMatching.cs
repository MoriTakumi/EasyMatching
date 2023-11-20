// 2023 TakumiMori
/// <summary>
/// OpenCVの処理を簡易に書くことができる自作モジュール\n
/// 前提アセットとしてOpenCV for Unity必須
/// </summary>
namespace OpenCVForUnity.EasyMatching
{

    using UnityEngine;
    using System.Collections.Generic;
    using OpenCVForUnity.CoreModule;
    using OpenCVForUnity.UnityUtils;
    using OpenCVForUnity.Features2dModule;

#if UNITY_EDITOR
    using UnityEditor;
#endif

    /*
    特徴点抽出アルゴリズム 
    */
    /// <summary>
    /// 特徴点検出アルゴリズムの種類
    /// </summary>
    /// <remarks>
    /// @link FeatureDetect @endlinkの引数として使用 \n
    /// 各特徴点検出器の違いは以下の通り
    /// </remarks>
    /*! 
        <table class="fieldtable">
            <tbody>
                <tr>
                    <th style="border-right: 1px solid var(--memdef-border-color); border-radius: 1px; text-align: center;"><p>要素</p></th>
                    <th style="border-right: 1px solid var(--memdef-border-color); border-radius: 1px; text-align: center;"><p>アルゴリズム</p></th>
                    <th style="border-right: 1px solid var(--memdef-border-color); border-radius: 1px; text-align: center;"><p>特徴のタイプ</p></th>
                    <th style="text-align: center;"><p>スケールに非依存</p></th>
                </tr>

                <tr>
                    <td class="fieldname"><p>SIFT=0</p></td>
                    <td class="fieldname"><p>DoG</p></td>
                    <td class="fieldname"><p>ブロブ</p></td>
                    <td class="fieldname"><p>あり</p></td>
                </tr>

                <tr>
                    <td class="fieldname"><p>AKAZE=1</p></td>
                    <td class="fieldname"><p>AOSスキーム</p></td>
                    <td class="fieldname"><p>コーナー</p></td>
                    <td class="fieldname"><p>なし</p></td>
                </tr>

                <tr>
                    <td class="fieldname"><p>ORB=2</p></td>
                    <td class="fieldname"><p>OrientedFAST</p></td>
                    <td class="fieldname"><p>コーナー</p></td>
                    <td class="fieldname"><p>なし</p></td>
                </tr>

                <tr>
                    <td class="fieldname"><p>BRISK=3</p></td>
                    <td class="fieldname"><p>スケール不変FAST</p></td>
                    <td class="fieldname"><p>コーナー</p></td>
                    <td class="fieldname"><p>あり</p></td>
                </tr>

                <tr>
                    <td class="fieldname"><p>FastFeatureDetector=4</p></td>
                    <td class="fieldname"><p>FAST</p></td>
                    <td class="fieldname"><p>コーナー</p></td>
                    <td class="fieldname"><p>なし</p></td>
                </tr>

                <tr>
                    <td class="fieldname"><p>AgastFeatureDetector=5</p></td>
                    <td class="fieldname"><p>不明</p></td>
                    <td class="fieldname"><p>コーナー</p></td>
                    <td class="fieldname"><p>不明</p></td>
                </tr>
            </tbody>
        </table>
    */
    public enum FeatureDetectAlgorithm
    {
        SIFT, 
        AKAZE, 
        ORB, 
        BRISK, 
        FastFeatureDetector, 
        AgastFeatureDetector
    }

    /*
    特徴量記述のアルゴリズム　マッチングするための正規化のようなもの
    */
    //! 特徴量記述のアルゴリズムの種類

    /*! \class DescriptorExtractor
    *
    *  Docs for DescriptorExtractor
    */

    /// <remarks>
    /// マッチングするための正規化のようなもの \n
    /// @link DescriptorExtractor @endlinkの引数として使用 \n
    /// 特徴量記述を指定するものであり，各特徴量記述の違いは以下の通り
    /// </remarks>
    
    /*! 
        <table class="fieldtable">
            <tbody>
                <tr>
                    <th style="border-right: 1px solid var(--memdef-border-color); border-radius: 1px; text-align: center;"><p>要素</p></th>
                    <th style="border-right: 1px solid var(--memdef-border-color); border-radius: 1px; text-align: center;"><p>アルゴリズム</p></th>
                    <th style="text-align: center;"><p>データ構造</p></th>
                </tr>

                <tr>
                    <td class="fieldname"><p>SIFT=0</p></td>
                    <td class="fieldname"><p>勾配方向ヒストグラム</p></td>
                    <td class="fieldname"><p>実数ベクトル</p></td>
                </tr>

                <tr>
                    <td class="fieldname"><p>AKAZE=1</p></td>
                    <td class="fieldname"><p>Modified-Local Difference Binary(M-LDB)</p></td>
                    <td class="fieldname"><p>バイナリ</p></td>
                </tr>

                <tr>
                    <td class="fieldname"><p>ORB=2</p></td>
                    <td class="fieldname"><p>回転不変BRIEF</p></td>
                    <td class="fieldname"><p>バイナリ</p></td>
                </tr>

                <tr>
                    <td class="fieldname"><p>BRISK=3</p></td>
                    <td class="fieldname"><p>回転不変BRIEF</p></td>
                    <td class="fieldname"><p>バイナリ</p></td>
                </tr>
            </tbody>
        </table>
    */
    public enum DescriptorExtractorAlgorithm
    {
        SIFT,
        AKAZE, 
        ORB,
        BRISK
    }

    /*
    マッチングの手法をまとめたもの
    FLANNBASED = 近傍探索
    BRUTEFORCE = 全探索
    */
    /// <summary>
    /// マッチング時の探索手法の指定
    /// </summary>
    /// <remarks>
    /// マッチする特徴点同士を探索するための手法を指定
    /// </remarks>
    /*! 
        <table class="fieldtable">
            <tbody>
                <tr>
                    <th style="border-right: 1px solid var(--memdef-border-color); border-radius: 1px; text-align: center;"><p>要素</p></th>
                    <th style="border-right: 1px solid var(--memdef-border-color); border-radius: 1px; text-align: center;"><p>探索手法</p></th>
                    <th style="text-align: center;"><p>対応するデータ構造</p></th>
                </tr>

                <tr>
                    <td class="fieldname"><p>FLANNBASED=1</p></td>
                    <td class="fieldname"><p>最近傍探索</p></td>
                    <td class="fieldname"><p>実数ベクトル</p></td>
                </tr>

                <tr>
                    <td class="fieldname"><p>BRUTEFORCE=2</p></td>
                    <td class="fieldname"><p>全探索</p></td>
                    <td class="fieldname"><p>全て</p></td>
                </tr>

                <tr>
                    <td class="fieldname"><p>BRUTEFORCE_L1=3</p></td>
                    <td class="fieldname"><p>全探索（マンハッタン距離）</p></td>
                    <td class="fieldname"><p>全て</p></td>
                </tr>

                <tr>
                    <td class="fieldname"><p>BRUTEFORCE_HAMMING=4</p></td>
                    <td class="fieldname"><p>全探索（ハミング距離）</p></td>
                    <td class="fieldname"><p>バイナリ</p></td>
                </tr>

                <tr>
                    <td class="fieldname"><p>BRUTEFORCE_HAMMINGLUT=5</p></td>
                    <td class="fieldname"><p>全探索（ハミング距離）</p></td>
                    <td class="fieldname"><p>バイナリ</p></td>
                </tr>

                <tr>
                    <td class="fieldname"><p>BRUTEFORCE_SL2=6</p></td>
                    <td class="fieldname"><p>全探索（ユークリッド2乗距離）</p></td>
                    <td class="fieldname"><p>全て</p></td>
                </tr>
            </tbody>
        </table>
    */

    public enum DescriptorMatcherMode
    {
        FLANNBASED = 1,// 最近傍探索．実数ベクトルのデータ構造に対応
        BRUTEFORCE = 2, // 全探索．全てのデータ構造に対応
        BRUTEFORCE_L1 = 3, //  マンハッタン距離・全探索．全てのデータ構造に対応 
        BRUTEFORCE_HAMMING = 4, //  ハミング距離・全探索．バイナリのデータ構造に対応 
        BRUTEFORCE_HAMMINGLUT = 5, //  ハミング距離・全探索．バイナリのデータ構造に対応 
        BRUTEFORCE_SL2 = 6//  ユークリッド2乗距離・全探索．全てのデータ構造に対応 
    }

    /*
    public const double AkazeThreshold = 0.00100000004749745;

    public static readonly int OrbFastThreshold = 20;
    public static readonly int OrbEdgeThreshold = 31;

    public static readonly int BriskThreshold = 30;

    public static readonly int FastThreshold = 10;

    public static readonly int AgastThreshold = 10;
    */
    /// <summary>
    /// 特徴点検出で設定する各パラメータをまとめてあるクラス
    /// </summary>
    public class FeatureDetectThreshold
    {
        //! AKAZE検出器の閾値
        public double akazeThreshold;
        //! ORB-Fast検出器の閾値
        public int orbFastThreshold;
        //! 特徴が検出されない境界の閾値
        public int orbEdgeThreshold;
        //! BRISK-AGAST検出器の閾値
        public int briskThreshold;
        //! Fast検出器の閾値
        public int fastThreshold;
        //! AGAST検出器の閾値
        public int agastThreshold;
        //! メンバ変数初期値用コンストラクタ
        public FeatureDetectThreshold()
        {
            akazeThreshold = 0.00100000004749745;
            orbFastThreshold = 20;
            orbEdgeThreshold = 31;
            briskThreshold = 30;
            fastThreshold = 10;
            agastThreshold = 10;
        }
    }
    // 特徴点の抽出
    /// <summary>
    /// 各特徴点検出アルゴリズムを扱いやすいようにまとめた構造体 \n
    /// @link DescriptorExtractor @endlinkの引数として使用 \n
    /// 
    /// </summary>
    public struct FeatureDetect
    {
        private Mat matImg;
        //! Mat型の画像データ（読み取り専用）
        public Mat MatImg { get { return matImg; } }

        private MatOfKeyPoint keyPoint;
        //! 画像データのキーポイント（読み取り専用）
        public MatOfKeyPoint KeyPoint { get { return keyPoint; } }
        
        private Mat descriptor;
        //! 記述子（読み取り専用）
        public Mat Descriptor { get { return descriptor; } }

        private FeatureDetectAlgorithm algorithm;
        //! 検出アルゴリズム（読み取り専用）
        public FeatureDetectAlgorithm Algorithm { get { return algorithm; } }

        private FeatureDetectThreshold threshold;
        // 特徴点抽出アルゴリズムを追加するなら要追記
        private void SelectAlgorithm()
        {
            switch (algorithm)
            {
                case FeatureDetectAlgorithm.SIFT:
                    {
                        var sift = SIFT.create();
                        var detector = sift;
                        //　特徴量の検出
                        detector.detect(matImg, keyPoint);
                    }
                    break;

                case FeatureDetectAlgorithm.AKAZE:
                    {
                        var akaze = AKAZE.create();
                        var detector = akaze;

                        detector.setThreshold(threshold.akazeThreshold);

                        //　特徴量の検出
                        detector.detect(matImg, keyPoint);
                    }
                    break;

                case FeatureDetectAlgorithm.ORB:
                    {
                        var orb = ORB.create();
                        var detector = orb;

                        detector.setFastThreshold(threshold.orbFastThreshold);
                        detector.setEdgeThreshold(threshold.orbEdgeThreshold);

                        //　特徴量の検出
                        detector.detect(matImg, keyPoint);
                    }
                    break;

                case FeatureDetectAlgorithm.BRISK:
                    {
                        var brisk = BRISK.create();
                        var detector = brisk;

                        detector.setThreshold(threshold.briskThreshold);

                        //　特徴量の検出
                        detector.detect(matImg, keyPoint);
                    }
                    break;

                case FeatureDetectAlgorithm.FastFeatureDetector:
                    {
                        var Fast = FastFeatureDetector.create();
                        var detector = Fast;

                        detector.setThreshold(threshold.fastThreshold);
                        //　特徴量の検出
                        detector.detect(matImg, keyPoint);
                        break;
                    }
                case FeatureDetectAlgorithm.AgastFeatureDetector:
                    {
                        var Agast = AgastFeatureDetector.create();
                        var detector = Agast;
                        detector.setThreshold(threshold.agastThreshold);
                        //　特徴量の検出
                        detector.detect(matImg, keyPoint);
                    }
                    break;

                default:
                    Debug.LogError("FeatureDetectMode ERROR!!");
                    break;
            }
        }
        //! Mat型の画像データを引数にとるコンストラクタ
        public FeatureDetect(Mat img, FeatureDetectAlgorithm algorithm, FeatureDetectThreshold threshold)
        {
            matImg = new Mat();
            img.copyTo(matImg);
            this.algorithm = algorithm;
            this.threshold = threshold;

            keyPoint = new MatOfKeyPoint();
            descriptor = new Mat();
            SelectAlgorithm();
        }
        //! Texture2D型の画像データを引数にとるコンストラクタ
        public FeatureDetect(Texture2D img, FeatureDetectAlgorithm algorithm, FeatureDetectThreshold threshold)
        {
            matImg = new Mat(img.height, img.width, CvType.CV_8UC4);
            Utils.texture2DToMat(img, matImg, true);
            this.algorithm = algorithm;
            this.threshold = threshold;

            keyPoint = new MatOfKeyPoint();
            descriptor = new Mat();
            SelectAlgorithm();
        }
    }
    // 特徴量の検出
    /// <summary>
    /// 各特徴量記述を扱いやすいようにまとめた構造体 \n
    /// 特徴量の検出を行う \n
    /// @link MatchResult @endlinkの引数としても使用 \n
    /// 
    /// </summary>
    /// 

    public struct DescriptorExtractor
    {
        private Mat matImg;
        //! Mat型の画像データ　検出結果(読み取り専用)
        public Mat MatImg { get { return matImg; } }

        private MatOfKeyPoint keyPoint;
        //! Mat型の画像データ(読み取り専用)
        public MatOfKeyPoint KeyPoint { get { return keyPoint; } }

        private Mat descriptor;
        //! 記述子(読み取り専用)
        public Mat Descriptor { get { return descriptor; } }

        private DescriptorExtractorAlgorithm algorithm;
        //! 特徴量アルゴリズム(読み取り専用)
        public DescriptorExtractorAlgorithm Algorithm { get { return algorithm; } }

        private void SelectAlgorithm()
        {
            switch (algorithm)
            {
                case DescriptorExtractorAlgorithm.SIFT:
                    {
                        var sift = SIFT.create();
                        var extractor = sift;

                        extractor.compute(matImg, keyPoint, descriptor);
                    }
                    break;

                case DescriptorExtractorAlgorithm.AKAZE:
                    {
                        var akaze = AKAZE.create();
                        var extractor = akaze;
                        extractor.compute(matImg, keyPoint, descriptor);
                    }
                    break;

                case DescriptorExtractorAlgorithm.ORB:
                    {
                        var orb = ORB.create();
                        var extractor = orb;

                        //　特徴量の検出
                        extractor.compute(matImg, keyPoint, descriptor);
                    }
                    break;

                case DescriptorExtractorAlgorithm.BRISK:
                    {
                        var brisk = BRISK.create();
                        var extractor = brisk;

                        //　特徴量の検出
                        extractor.compute(matImg, keyPoint, descriptor);
                    }
                    break;

                default:
                    Debug.LogError("FeatureDetectMode ERROR!!");
                    break;
            }
        }
        //! コンストラクタ
        public DescriptorExtractor(FeatureDetect featureDetect, DescriptorExtractorAlgorithm algorithm)
        {
            matImg = new Mat();
            featureDetect.MatImg.copyTo(matImg);

            keyPoint = new MatOfKeyPoint();
            featureDetect.KeyPoint.copyTo(keyPoint);
            descriptor = new Mat();
            featureDetect.Descriptor.copyTo(descriptor);

            this.algorithm = algorithm;
            SelectAlgorithm();
        }
    }
    /// <summary>
    /// マッチング結果をTexture2Dで出力するための構造体 \n
    /// </summary>
    public struct MatchResult
    {
        //! マッチング結果の画像
        private Texture2D texture;
        
        public Texture2D Texture { get { return texture; } }
        private DescriptorExtractor before;
        //! マッチングしたい1つ目の画像データを持つDescriptorExtractor
        public DescriptorExtractor Before { get { return before; } }
        private int beforeNum;
        public int BeforeNum { get { return beforeNum; } }

        private DescriptorExtractor after;
        //! マッチングしたい2つ目の画像データを持つDescriptorExtractor
        public DescriptorExtractor After { get { return after; } }
        private int afterNum;
        public int AfterNum { get { return afterNum; } }

        private MatOfDMatch matches;
        //! マッチングによる特徴点同士の対応などが格納されている
        public MatOfDMatch Matches { get { return matches; } }
        
        private float matchRate;
        //! マッチングによる特徴点同士の対応などが格納されている
        public float MatchRate { get { return matchRate; } }

        public MatchResult(Texture2D texture, DescriptorExtractor before, int beforeNum, DescriptorExtractor after, int afterNum, MatOfDMatch matches, float matchRate)
        {
            this.texture = texture;
            this.before = before;
            this.beforeNum = beforeNum;
            this.after = after;
            this.afterNum = afterNum;
            this.matches = matches;
            this.matchRate = matchRate;
        }
    }

}
