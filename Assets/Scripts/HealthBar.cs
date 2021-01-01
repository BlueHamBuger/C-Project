using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{

    public float HealthPoint
    {
        get
        {
            return m_Slider.value;
        }
        set
        {
            m_Slider.value = value;
        }
    }

    public float MaxHealthPoint
    {
        get
        {
            return m_Slider.maxValue;
        }
        set
        {
            m_Slider.maxValue = value;
        }
    }
    public float offset = -0.5f;

    void Start()
    {
        if (Canvas == null) Debug.LogError("Please set a canvas for health bar.");
        m_SliderRectTransform = gameObject.GetComponent<Slider>().transform as RectTransform;
        //transform.parent = Canvas.transform;
        transform.SetParent(Canvas.transform);
        // 获取物体高度
        //FollowTarget.GetComponent<SkinnedMeshRenderer>().bounds.size.y;
        m_ObjectHeight = FollowTarget.GetComponent<Renderer>().bounds.size.y;
        Debug.Log("object height." + m_ObjectHeight);
        m_Slider = gameObject.GetComponent<Slider>();
    }

    void Update()
    {
        UpdateHealthBarPosition();
    }

    private void LateUpdate()
    {
        UpdateHealthBarColor();
    }

    void UpdateHealthBarPosition()
    {
        Vector3 worldPosition = new Vector3(FollowTarget.transform.position.x,
            FollowTarget.transform.position.y + m_ObjectHeight + offset  , FollowTarget.transform.position.z);
        // 根据NPC头顶的3D坐标换算成它在2D屏幕中的坐标
        Vector2 position = Camera.main.WorldToScreenPoint(worldPosition);
        m_SliderRectTransform.position = position;
    }

    void UpdateHealthBarColor()
    {
        if (m_Slider.value < m_Slider.maxValue * 0.5f)
        {
            m_FillImage.color = Color.red;
        }
        else
        {
            m_FillImage.color = Color.green;
        }
        if (m_Slider.value <= 0)
        {
            Debug.Log(gameObject.name + " is dead");
        }
    }

    public Canvas Canvas;
    public GameObject FollowTarget; // 血条跟踪的对象
    private RectTransform m_SliderRectTransform;
    private Slider m_Slider;

    [SerializeField] private Image m_FillImage;
    private float m_ObjectHeight;
}
