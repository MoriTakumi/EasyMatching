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
        // �����_�}�b�`���O���s��

        // 臒l�̍쐬
        var threshold = new FeatureDetectThreshold();

        // 臒l�ݒ�̗�@�e�A���S���Y�����Ƃɐݒ�\
        //threshold.akazeThreshold = 0.001;
        threshold.orbFastThreshold = 20;
        threshold.orbEdgeThreshold = 31;
        
        // ���o�A���S���Y���̐ݒ�
        var beforeDetector = new FeatureDetect(beforeTarget, FeatureDetectAlgorithm.ORB, threshold);
        // ���ރA���S���Y���̐ݒ�
        var beforeDescriptor = new DescriptorExtractor(beforeDetector, DescriptorExtractorAlgorithm.ORB);

        var afterDetector = new FeatureDetect(afterTarget, FeatureDetectAlgorithm.ORB, threshold);
        var afterDescriptor = new DescriptorExtractor(afterDetector, DescriptorExtractorAlgorithm.ORB);

        // �}�b�`���O�ōs���T���A���S���Y���̐ݒ�
        var matcher = DescriptorMatcher.create((int)DescriptorMatcherMode.BRUTEFORCE);
        // �}�b�`���O���ʂ̊i�[�ϐ�
        var matches = new MatOfDMatch();
        // �}�b�`���O
        matcher.match(beforeDescriptor.Descriptor, afterDescriptor.Descriptor, matches);
        
        var resultImg = new Mat();
        // �}�b�`���O���ʕ`��p�֐�
        Features2d.drawMatches(beforeDescriptor.MatImg, beforeDescriptor.KeyPoint, afterDescriptor.MatImg, afterDescriptor.KeyPoint, matches, resultImg);
        
        // Mat����Unity��Texture2D�ւ̕ϊ�
        var resultTexture = new Texture2D(resultImg.cols(), resultImg.rows(), TextureFormat.RGBA32, false);
        Utils.matToTexture2D(resultImg, resultTexture);
        // Update�Ŏg�p����ہi���[�v���j�͌�����Getcomponent���Ȃ����ƁI�I�I ���ׂ̌����ɂȂ�܂�
        gameObject.GetComponent<Renderer>().sharedMaterial.mainTexture = resultTexture;
        // �o�͉摜�̃T�C�Y�ɕϊ�
        transform.localScale = new Vector3(resultTexture.width, resultTexture.height , 1);
    }
}
