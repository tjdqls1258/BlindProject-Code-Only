using Cysharp.Threading.Tasks;
using System.Linq;
using System.Threading;
using UnityEngine;

public class SonarManager : MonoSingleton<SonarManager>
{
    public class sonarData
    {
        private float radius = 0;
        private bool _isAction = false;
        public Color sonarpositionPixcel = Color.clear;
        public Color sonarDataPixcel = Color.clear;
        public bool startSonar
        {
            get { return _isAction; }
            set 
            {
                _isAction = value; 

                if(!value)
                {
                    radius = 0;
                    sonarDataPixcel = Color.clear;
                    sonarDataPixcel.a = 0;
                }
            }
        }

        public void SonarStart(Vector3 sonarPos,Color sonarPowerAColor)
        {
            startSonar = true;
            this.sonarDataPixcel = sonarPowerAColor;

            sonarpositionPixcel.r = sonarPos.x;
            sonarpositionPixcel.g = sonarPos.y;
            sonarpositionPixcel.b = sonarPos.z;
            sonarpositionPixcel.a = radius = 0;
        }

        public bool CheckSonar(float speed)
        {
            if (startSonar == false) return false;

            if(sonarDataPixcel.a <= radius)
                startSonar = false;
            else
                radius += Time.deltaTime * speed;

            sonarpositionPixcel.a = radius;

            return startSonar;
        }
    }
    public int MaxSonarCount = 100;
    public float sonarSpeed = 1;

    private Texture2D maskTexture;  // Dynamically generated mask texture
    private Texture2D maskdataTexture;  // Dynamically generated mask texture
    private sonarData[] maskPositions; // Array of mask positions (stored in Vector3)
    public EdgeDetection edgeDetectionFeature;
    private int m_currentSonarCount = 0;
    private CancellationTokenSource cancel = new CancellationTokenSource();

    private bool m_runUpdate = false;

    protected override void Awake()
    {
        _instance = this;
        Init();
    }

    protected override void Init()
    {
        base.Init();
        GenerateMaskTexture();

        edgeDetectionFeature.maskTexture = maskTexture;
        edgeDetectionFeature.maskdataTexture = maskdataTexture;
        edgeDetectionFeature.MaxSonarCount = MaxSonarCount;
        Shader.SetGlobalFloat("_numMasks", MaxSonarCount);
        Shader.SetGlobalTexture("_maskTex", maskTexture);
        Shader.SetGlobalTexture("_maskData", maskdataTexture);
        m_runUpdate = true;
        Update_sonarData().Forget();
    }
    
    private void GenerateMaskTexture()
    {
        maskPositions = new sonarData[MaxSonarCount];
        for(int i = 0; i < maskPositions.Length; i++)
        {
            maskPositions[i] = new sonarData();
        }

        maskTexture = new Texture2D(MaxSonarCount, 1, TextureFormat.RGBAFloat, false)
        {
            wrapMode = TextureWrapMode.Clamp,
            filterMode = FilterMode.Point
        };
        maskdataTexture = new Texture2D(MaxSonarCount, 1, TextureFormat.RGBAFloat, false)
        {
            wrapMode = TextureWrapMode.Clamp,
            filterMode = FilterMode.Point
        };

        UpdateMaskTexture();
    }

    private void UpdateMaskTexture()
    {
        for (int i = 0; i < MaxSonarCount; i++)
        {
            if (maskPositions[i].CheckSonar(sonarSpeed) == false)
            {
                maskTexture.SetPixel(i, 0, Color.clear);
                continue;
            }
            maskTexture.SetPixel(i, 0, maskPositions[i].sonarpositionPixcel);
        }

        maskTexture.Apply();
    }

    public void SonarAdd(Vector3 startPos, Color powerColor, LayerMask _layerTarget)
    {
        maskPositions.First(x=>x.startSonar == false).SonarStart(startPos, powerColor);

        if(_layerTarget > 0)
        {
            var layhit = Physics.SphereCastAll(startPos, powerColor.a, Vector3.one, powerColor.a, _layerTarget);
            if (layhit.Length > 0)
            {
                foreach(var lay in layhit)
                {
                    lay.transform.GetComponent<EnemyMove>().HitSonar(startPos);
                }
            }
        }

        for (int i = 0; i < MaxSonarCount; i++)
        {
            maskdataTexture.SetPixel(i, 0, maskPositions[i].sonarDataPixcel);
        }
        maskdataTexture.Apply();
    }

    private async UniTaskVoid Update_sonarData()
    {
        while (m_runUpdate)
        {
            UpdateMaskTexture();
            await UniTask.Yield(PlayerLoopTiming.Update, cancel.Token);
        }
    }

    private void OnDisable()
    {
        m_runUpdate = false;
        UniTask.FromCanceled(cancel.Token);
    }
}
