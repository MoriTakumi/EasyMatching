// 2022 TakumiMori
using UnityEngine;
using OpenCVForUnity.CoreModule;
using OpenCVForUnity.UnityUtils;
using OpenCVForUnity.Features2dModule;
using OpenCVForUnity.EasyMatching;

public class SampleMatching : MonoBehaviour
{
    [SerializeField]
    private Texture2D beforeTarget;
    [SerializeField]
    private Texture2D afterTarget;

    // Start is called before the first frame update
    void Start()
    {
        // 特徴点マッチングを行う

        // 閾値の作成
        var threshold = new FeatureDetectThreshold();

        // 閾値設定の例　各アルゴリズムごとに設定可能
        //threshold.akazeThreshold = 0.001;
        threshold.orbFastThreshold = 20;
        threshold.orbEdgeThreshold = 31;
        
        // 検出アルゴリズムの設定
        var beforeDetector = new FeatureDetect(beforeTarget, FeatureDetectAlgorithm.ORB, threshold);
        // 分類アルゴリズムの設定
        var beforeDescriptor = new DescriptorExtractor(beforeDetector, DescriptorExtractorAlgorithm.ORB);

        var afterDetector = new FeatureDetect(afterTarget, FeatureDetectAlgorithm.ORB, threshold);
        var afterDescriptor = new DescriptorExtractor(afterDetector, DescriptorExtractorAlgorithm.ORB);

        // マッチングで行う探索アルゴリズムの設定
        var matcher = DescriptorMatcher.create((int)DescriptorMatcherMode.BRUTEFORCE);
        // マッチング結果の格納変数
        var matches = new MatOfDMatch();
        // マッチング
        matcher.match(beforeDescriptor.Descriptor, afterDescriptor.Descriptor, matches);
        
        var resultImg = new Mat();
        // マッチング結果描画用関数
        Features2d.drawMatches(beforeDescriptor.MatImg, beforeDescriptor.KeyPoint, afterDescriptor.MatImg, afterDescriptor.KeyPoint, matches, resultImg);
        
        // MatからUnityのTexture2Dへの変換
        var resultTexture = new Texture2D(resultImg.cols(), resultImg.rows(), TextureFormat.RGBA32, false);
        Utils.matToTexture2D(resultImg, resultTexture);
        // Updateで使用する際（ループ時）は決してGetcomponentしないこと！！！ 負荷の原因になります
        gameObject.GetComponent<Renderer>().sharedMaterial.mainTexture = resultTexture;
        // 出力画像のサイズに変換
        transform.localScale = new Vector3(resultTexture.width, resultTexture.height , 1);
    }
}
