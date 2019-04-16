using UnityEngine;
using System.Collections.Generic;

[ExecuteAlways]
public partial class KekeCharacter : MonoBehaviour
{
    [System.Serializable]
    public class KekeCharacterAnimation
    {
        [SerializeField]
        [Range(0, 2)]
        private float m_MainBounceHeight;

        [SerializeField]
        [Range(1, 8)]
        private float m_MainBounceSpeed;

        [SerializeField]
        [Range(0, 8)]
        private float m_Stretch;

        [SerializeField]
        [Range(0, 1)]
        private float m_HeadDamp;

        [SerializeField]
        [Range(0, 1)]
        private float m_HeadDelay;

        [SerializeField]
        private float m_SecondaryDelay;

        private float prevTime = 0;
        private float time = 0;

        public void Update()
        {
            prevTime = time;
            time += m_MainBounceSpeed * Time.deltaTime;
        }

        public void GetMainBounce(out float height, out float stretch)
        {
            GetBounce(0, 0, out height, out stretch);
        }

        public void GetHeadBounce(out float height, out float stretch)
        {
            GetBounce(m_HeadDelay, m_HeadDamp, out height, out stretch);
        }

        private void GetBounce(float delay, float damp, out float height, out float stretch)
        {
            float bounce = Mathf.Pow(Mathf.Sin(time - delay), 4);
            float prevBounce = Mathf.Pow(Mathf.Sin(prevTime - delay), 4);

            height = (1 - bounce) * m_MainBounceHeight * (1 - damp);
            stretch = (bounce - prevBounce) * m_Stretch * (1 - damp);
        }
    }

    [System.Serializable]
    public class KekeCharacterSettings
    {
        [SerializeField]
        private Material m_MainMaterial;

        [SerializeField]
        private Mesh m_MainShape;
        [Header("Head")]
        [SerializeField]
        [Range(0, 5)]
        private float m_BodySize;

        [SerializeField]
        [Range(-1, 1)]
        private float m_BodyTaper;

        [SerializeField]
        [Range(0, 1)]
        private float m_BodyFlatten;

        [Header("Head")]
        [SerializeField]
        [Range(0, 5)]
        private float m_HeadSize;

        [SerializeField]
        [Range(0, 3)]
        private float m_EarSize;

        [SerializeField]
        [Range(0, 1)]
        private float m_EarSpread;

        [Header("Arms")]
        [Range(8, 20)]
        private int m_ArmResolution;

        [SerializeField]
        [Range(0, 5)]
        private float m_ArmLength;

        [SerializeField]
        [Range(0, 1)]
        private float m_ArmThickness;

        [SerializeField]
        private AnimationCurve m_ArmThicknessCurve = new AnimationCurve(new Keyframe(0, 1), new Keyframe(1, 1));

        [Header("Legs")]
        [SerializeField]
        [Range(8, 20)]
        private int m_LegResolution;

        [SerializeField]
        [Range(0, 5)]
        private float m_LegLength;

        [SerializeField]
        [Range(0, 1)]
        private float m_LegThickness;

        [SerializeField]
        private AnimationCurve m_LegThicknessCurve = new AnimationCurve(new Keyframe(0, 1), new Keyframe(1, 1));

        [SerializeField]
        [Range(0, 1)]
        private float m_LegSpread;

        [SerializeField]
        [Range(-1, 1)]
        private float m_LegFrontBack;

        [SerializeField]
        [Range(0, 1)]
        private float m_ResolutionSpread = 0.0f;

        [Header("Debug")]
        [SerializeField]
        private bool m_DebugLegs = false;


        public Material MainMaterial
        {
            get
            {
                return m_MainMaterial;
            }

            set
            {
                m_MainMaterial = value;
            }
        }

        public Mesh MainShape
        {
            get
            {
                return m_MainShape;
            }

            set
            {
                m_MainShape = value;
            }
        }

        public float BodySize
        {
            get
            {
                return m_BodySize;
            }

            set
            {
                m_BodySize = value;
            }
        }

        public float HeadSize
        {
            get
            {
                return m_HeadSize;
            }

            set
            {
                m_HeadSize = value;
            }
        }

        public float EarSize
        {
            get
            {
                return m_EarSize;
            }

            set
            {
                m_EarSize = value;
            }
        }

        public float EarSpread
        {
            get
            {
                return m_EarSpread;
            }

            set
            {
                m_EarSpread = value;
            }
        }

        public float ArmLength
        {
            get
            {
                return m_ArmLength;
            }

            set
            {
                m_ArmLength = value;
            }
        }

        public float LegLength
        {
            get
            {
                return m_LegLength;
            }

            set
            {
                m_LegLength = value;
            }
        }

        public float LegSpread
        {
            get
            {
                return m_LegSpread;
            }

            set
            {
                m_LegSpread = value;
            }
        }

        public float LegFrontBack
        {
            get
            {
                return m_LegFrontBack;
            }

            set
            {
                m_LegFrontBack = value;
            }
        }

        public int LegResolution
        {
            get
            {
                return m_LegResolution;
            }

            set
            {
                m_LegResolution = value;
            }
        }

        public float LegThickness
        {
            get
            {
                return m_LegThickness;
            }

            set
            {
                m_LegThickness = value;
            }
        }

        public AnimationCurve LegThicknessCurve
        {
            get
            {
                return m_LegThicknessCurve;
            }

            set
            {
                m_LegThicknessCurve = value;
            }
        }

        public float ResolutionSpread
        {
            get
            {
                return m_ResolutionSpread;
            }

            set
            {
                m_ResolutionSpread = value;
            }
        }

        public int ArmResolution
        {
            get
            {
                return m_ArmResolution;
            }

            set
            {
                m_ArmResolution = value;
            }
        }

        public float ArmThickness
        {
            get
            {
                return m_ArmThickness;
            }

            set
            {
                m_ArmThickness = value;
            }
        }

        public AnimationCurve ArmThicknessCurve
        {
            get
            {
                return m_ArmThicknessCurve;
            }

            set
            {
                m_ArmThicknessCurve = value;
            }
        }

        public bool DebugLegs
        {
            get
            {
                return m_DebugLegs;
            }

            set
            {
                m_DebugLegs = value;
            }
        }

        public float BodyTaper
        {
            get
            {
                return m_BodyTaper;
            }

            set
            {
                m_BodyTaper = value;
            }
        }

        public float BodyFlatten
        {
            get
            {
                return m_BodyFlatten;
            }

            set
            {
                m_BodyFlatten = value;
            }
        }
    }

    [SerializeField]
    private KekeCharacterSettings m_CharacterSettings;

    [SerializeField]
    private KekeCharacterAnimation m_CharacterAnimation;
    private Matrix4x4 bodyMatrix = Matrix4x4.identity;
    private Matrix4x4 headMatrix = Matrix4x4.identity;

    public KekeCharacterSettings CharacterSettings
    {
        get
        {
            return m_CharacterSettings;
        }

        set
        {
            m_CharacterSettings = value;
        }
    }

    public KekeCharacterAnimation CharacterAnimation
    {
        get
        {
            return m_CharacterAnimation;
        }

        set
        {
            m_CharacterAnimation = value;
        }
    }

    List<IDrawable> drawables = new List<IDrawable>(8);

    public void CreateLegs()
    {
        leftLegSpline = new SplineMesh(6, CharacterSettings.LegResolution)
        {
            Thickness = CharacterSettings.LegThickness,
            ThicknessCurve = CharacterSettings.LegThicknessCurve,
            ResolutionSpread = CharacterSettings.ResolutionSpread,
            Material = CharacterSettings.MainMaterial,
            Parent = transform
        };

        rightLegSpline = new SplineMesh(6, CharacterSettings.LegResolution)
        {
            Thickness = CharacterSettings.LegThickness,
            ThicknessCurve = CharacterSettings.LegThicknessCurve,
            ResolutionSpread = CharacterSettings.ResolutionSpread,
            Material = CharacterSettings.MainMaterial,
            Parent = transform
        };
        drawables.Add(leftLegSpline);
        drawables.Add(rightLegSpline);
    }

    private void CreateArms()
    {
        leftArmSpline = new SplineMesh(6, CharacterSettings.ArmResolution)
        {
            Thickness = CharacterSettings.ArmThickness,
            ThicknessCurve = CharacterSettings.ArmThicknessCurve,
            ResolutionSpread = CharacterSettings.ResolutionSpread,
            Material = CharacterSettings.MainMaterial,
            Parent = transform
        };

        rightArmSpline = new SplineMesh(6, CharacterSettings.ArmResolution)
        {
            Thickness = CharacterSettings.ArmThickness,
            ThicknessCurve = CharacterSettings.ArmThicknessCurve,
            ResolutionSpread = CharacterSettings.ResolutionSpread,
            Material = CharacterSettings.MainMaterial,
            Parent = transform
        };
        drawables.Add(leftArmSpline);
        drawables.Add(rightArmSpline);
    }

    private void CreateBody()
    {
        bodyMesh = new BodyMesh(CharacterSettings.MainShape, transform)
        {
            Material = CharacterSettings.MainMaterial,
        };
        drawables.Add(bodyMesh);
    }

    private void OnEnable()
    {
        CreateBody();
        CreateLegs();
        CreateArms();
    }

    private void Update()
    {
        UpdateCharacter();
        DrawCharacter();
    }

    public void UpdateCharacter()
    {
        if (!CharacterSettings.MainShape)
        {
            return;
        }

        CharacterAnimation.Update();
        UpdateBody();
        UpdateLegs();
        UpdateArms();
        UpdateHead();
    }

    private void DrawCharacter()
    {
        foreach (var drawable in drawables)
        {
            drawable.Draw();
        }
    }

    private void UpdateArms()
    {
        if (CharacterSettings.ArmLength > 0)
        {

        }
    }

    private Vector3 flipX = new Vector3(-1, 1, 1);
    private BodyMesh bodyMesh;
    private SplineMesh leftLegSpline, rightLegSpline;
    private SplineMesh leftArmSpline, rightArmSpline;

    private void UpdateLegs()
    {
        if (CharacterSettings.LegLength > 0 && CharacterSettings.LegThickness > 0)
        {
            Vector3 legOrigin = (Quaternion.Euler(CharacterSettings.LegFrontBack * -90, 0, CharacterSettings.LegSpread * 90) * Vector3.down * CharacterSettings.BodySize / 2 * 0.5f);

            Vector3 leftLegOrigin = bodyMatrix.MultiplyPoint(Vector3.Scale(legOrigin, flipX));
            Vector3 rightLegOrigin = bodyMatrix.MultiplyPoint(legOrigin);


            Vector3 leftLegGoal = Vector3.Scale(legOrigin, flipX);
            Vector3 rightLegGoal = legOrigin;
            leftLegGoal.y = 0;
            rightLegGoal.y = 0;

            float leftBend = (CharacterSettings.LegLength / Vector3.Distance(leftLegOrigin, leftLegGoal));
            float rightBend = (CharacterSettings.LegLength / Vector3.Distance(rightLegOrigin, rightLegGoal));

            Vector3 kneeVector = Quaternion.Euler(0, CharacterSettings.LegSpread * 90, 0) * Vector3.forward;

            Vector3 leftMidpoint = (leftLegOrigin + leftLegGoal) / 2 + Vector3.Scale(kneeVector, flipX) * leftBend;
            Vector3 rightMidpoint = (rightLegOrigin + rightLegGoal) / 2 + kneeVector * rightBend;


            leftLegSpline.ResolutionV = rightLegSpline.ResolutionV = CharacterSettings.LegResolution;
            leftLegSpline.Material = rightLegSpline.Material = CharacterSettings.MainMaterial;
            leftLegSpline.Thickness = rightLegSpline.Thickness = CharacterSettings.LegThickness;
            leftLegSpline.ThicknessCurve = rightLegSpline.ThicknessCurve = CharacterSettings.LegThicknessCurve;
            leftLegSpline.ResolutionSpread = rightLegSpline.ResolutionSpread = CharacterSettings.ResolutionSpread;

            leftLegSpline.Update(leftLegOrigin, leftMidpoint - leftLegOrigin, leftLegGoal, leftMidpoint - leftLegGoal);
            rightLegSpline.Update(rightLegOrigin, rightMidpoint - rightLegOrigin, rightLegGoal, rightMidpoint - rightLegGoal);

            if (CharacterSettings.DebugLegs)
            {
                Debug.DrawLine(transform.TransformPoint(bodyMatrix.MultiplyPoint(Vector3.zero)), transform.TransformPoint(leftLegOrigin), Color.blue, 0, false);
                Debug.DrawLine(transform.TransformPoint(bodyMatrix.MultiplyPoint(Vector3.zero)), transform.TransformPoint(rightLegOrigin), Color.blue, 0, false);

                leftLegSpline.DrawDebug();
                rightLegSpline.DrawDebug();
            }

        }
    }

    private void UpdateHead()
    {
        if (CharacterSettings.HeadSize > 0)
        {
            float bounce;
            float stretch;

            CharacterAnimation.GetHeadBounce(out bounce, out stretch);

            Vector3 pos = Vector3.up * (CharacterSettings.BodySize / 2 + bounce);
            Vector3 scale = Vector3.one * CharacterSettings.HeadSize + Vector3.up * stretch;
            headMatrix = Matrix4x4.Translate(pos);


            Graphics.DrawMesh(CharacterSettings.MainShape, transform.localToWorldMatrix * bodyMatrix * Matrix4x4.TRS(pos, Quaternion.identity, scale), CharacterSettings.MainMaterial, gameObject.layer);
        }
    }

    private void UpdateBody()
    {
        if (CharacterSettings.BodySize > 0)
        {
            float bounce;
            float stretch;

            CharacterAnimation.GetMainBounce(out bounce, out stretch);

            bodyMesh.LocalPosition = Vector3.up * (CharacterSettings.LegLength + CharacterSettings.BodySize / 2 + bounce);
            bodyMesh.Size = CharacterSettings.BodySize;
            bodyMesh.Taper = CharacterSettings.BodyTaper;
            bodyMesh.Flatten = CharacterSettings.BodyFlatten;
            bodyMesh.SquashStretch = stretch;

            bodyMatrix = bodyMesh.LocalMatrix;
            //Vector3 scale = Vector3.one * CharacterSettings.BodySize + Vector3.up * stretch;

            //bodyMatrix = Matrix4x4.Translate(pos);

            //Graphics.DrawMesh(CharacterSettings.MainShape, transform.localToWorldMatrix * Matrix4x4.TRS(pos, Quaternion.identity, scale), CharacterSettings.MainMaterial, gameObject.layer);
        }
    }
}
