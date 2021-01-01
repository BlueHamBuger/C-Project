using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarLoaderSample : MonoBehaviour
{
    public GameObject FollowTarget;
    public Canvas Canvas;
    private GameObject m_HealthBarGo;
    private HealthBar m_HealthBar;
    int m_ChangeAmount;

    void Start()
    {
        m_HealthBarGo = Instantiate(Resources.Load<GameObject>("Prefabs/HealthBar"));
        m_HealthBar = m_HealthBarGo.GetComponent<HealthBar>();
        m_HealthBar.gameObject.SetActive(true);
        m_HealthBar.FollowTarget = FollowTarget;
        m_HealthBar.Canvas = Canvas;
    }

    private void Update()
    {
        if (m_HealthBar.HealthPoint <= 0)
        {
            m_ChangeAmount = 1;
        }
        else if (m_HealthBar.HealthPoint >= m_HealthBar.MaxHealthPoint)
        {
            m_ChangeAmount = -1;
        }
        m_HealthBar.HealthPoint += m_ChangeAmount;
    }
}
