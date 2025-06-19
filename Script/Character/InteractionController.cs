using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.UI;
using UnityEngine.UI;

public class InteractionController : MonoBehaviour
{
    [SerializeField] LayerMask m_interactionMask;
    [SerializeField] Camera m_camera;
    [SerializeField] float m_distance = 4f;
    private RaycastHit m_hit;
    [SerializeField] KeyCode m_interactionKey = KeyCode.E;
    private float m_pressTime = 0;
    private float iTime = 0f;
    private bool m_doPlant = false;
    private bool m_blockDownAgain = false;
    private bool m_doActionStopCool = false;

    private InteractionType m_interactionType;
    private I_InteractionObejct m_currentInteraction;
    private Image m_interactionBar;
    private TextMeshProUGUI m_interactionText;

    private Animator m_animator;
    private Animator Animator
    {
        get
        {
            if (m_animator == null)
            {
                m_animator = GetComponentInChildren<Animator>();
            }

            return m_animator;
        }
    }

    public bool GetDoInterAction() => m_doPlant;  

    private void LateUpdate()
    {
        DoInteract();
    }

    public void DoInteract()
    {
        InteractionRay();
        if (m_currentInteraction != null)
        {
            if (Input.GetKey(m_interactionKey))
                TimerAction();
            else
            {
                if(IsCancelAction())
                {
                    TimerAction();
                }
                Interaction_CancelAction();
            }
        }
    }

    public void InitInteractionUI(Image interactionBar, TextMeshProUGUI interactionInfo)
    {
        m_interactionBar = interactionBar;
        m_interactionText = interactionInfo;

        ResetUITimer();
    }

    public void SetUITimer(float currentTime, float interactionTime)
    {
        if (m_interactionBar == null || m_interactionText == null) return;

        m_interactionBar.fillAmount = currentTime / interactionTime;
    }

    private void ResetUITimer()
    {
        if (m_interactionBar == null || m_interactionText == null) return;
        m_interactionBar.fillAmount = 0;
        m_interactionText.text = "";
    }

    public void ActionAnimationPlant()
    {
        if (m_doPlant == false)
        {
            Animator.SetBool("Plant", true);
            m_doPlant = true;
        }
        else
        {
            Animator.SetBool("Plant", false);
            m_doPlant = false;
        }
    }

    private bool IsCancelAction()
    {
        switch (m_interactionType)
        {
            case InteractionType.None:
            case InteractionType.InterActionDoor:
            default:
                return false;
            case InteractionType.Plant:
                return true;
        }
    }

    private void ActionAnimation()
    {
        switch (m_interactionType)
        {
            case InteractionType.None:
            case InteractionType.InterActionDoor:
            default:
                break;
            case InteractionType.Plant:
                ActionAnimationPlant();
                break;
        }
    }

    private void AnimationStop()
    {
        switch (m_interactionType)
        {
            case InteractionType.None:
            case InteractionType.InterActionDoor:
            default:
                break;
            case InteractionType.Plant:
                Animator.SetBool("Plant", false);
                m_doPlant = false;
                break;
        }
    }

    private void ResetAction()
    {
        m_pressTime = 0;
        ResetUITimer();
        m_interactionType = InteractionType.None;
        m_currentInteraction = null;
        m_blockDownAgain = false;
    }

    private void InteractionRay()
    {
        if (m_doPlant) return;
        if(m_doActionStopCool && Input.GetKeyUp(m_interactionKey))
        {
            m_doActionStopCool = false;
            return;
        }

        if (Physics.Raycast(m_camera.transform.position, m_camera.transform.forward, out m_hit, m_distance, m_interactionMask))
        {
            m_currentInteraction = m_hit.collider.GetComponent<I_InteractionObejct>();
            if (Input.GetKey(m_interactionKey))
            {
                m_currentInteraction = m_hit.collider.GetComponent<I_InteractionObejct>();
                iTime = m_currentInteraction.InteractionTime();
                m_interactionType = m_currentInteraction.interactionType();
                ActionAnimation();
                m_interactionText.text = m_currentInteraction.GetInteractionText();
                m_blockDownAgain = true;
            }
            else
            {
                m_interactionText.text = m_currentInteraction.GetCastText();
            }
            if (Input.GetKeyUp(m_interactionKey))
            {
                ResetAction();
            }
        }
        else if(m_currentInteraction != null)
        {
            ResetAction();
        }
    }

    private void Interaction_CancelAction()
    {
        if (IsCancelAction() == false) return;
        if (m_blockDownAgain && Input.GetKeyUp(m_interactionKey))
        {
            m_blockDownAgain = false;
            return;
        }

        if (Input.GetKeyUp(m_interactionKey))
        {
            AnimationStop();
            ResetAction();
            m_doActionStopCool = true;
        }
    }

    private void TimerAction()
    {
        m_pressTime += Time.deltaTime;
        SetUITimer(m_pressTime, iTime);
        if (m_pressTime > iTime)
        {
            m_currentInteraction.OnInteraction();
            AnimationStop();
            ResetAction();
            m_doActionStopCool = false;
        }
    }
}
