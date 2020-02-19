using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerState
{
    Walk,
    Dig,
}

public class PlayerController : MonoBehaviour
{
    public PlayerState CurPlayerState;

    [Header("Walk")]

    [SerializeField]
    private float m_walkSpeed;

    [Header("Dig")]
    [SerializeField]
    private float m_digSpeed;

    [SerializeField]
    private float m_indicatorRotateSpeed;

    [Header("Component")]

    #region Component

    [SerializeField]
    Rigidbody2D m_rigid;

    [SerializeField]
    SpriteRenderer m_renderer;

    [SerializeField]
    private Transform m_digIndicator;

    [SerializeField]
    private Transform m_digDirection;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ChangePlayerState(PlayerState.Dig);
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            ChangePlayerState(PlayerState.Walk);
        }

        switch (CurPlayerState)
        {
            case PlayerState.Walk:
                var walkVec = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
                Walk(walkVec);
                break;
            case PlayerState.Dig:
                var direction = (m_digDirection.position - transform.position).normalized;
                RotateIndicatorByKeyboard();
                Dig(direction);
                break;                
        }


    }

    public void ChangePlayerState(PlayerState newState)
    {
        switch(newState)
        {
            case PlayerState.Walk:
                DigIndicatorActiveToggle(false);
                break;
            case PlayerState.Dig:
                m_digIndicator.Rotate(0, 0, 0);
                DigIndicatorActiveToggle(true);
                break;
        }

        CurPlayerState = newState;
    }

    void RotateIndicatorByKeyboard()
    {
        if (Input.GetAxis("Horizontal") > 0)
        {
            m_digIndicator.Rotate(0, 0, -m_indicatorRotateSpeed);
        }
        else if (Input.GetAxis("Horizontal") < 0)
        {
            m_digIndicator.Rotate(0, 0, m_indicatorRotateSpeed);
        }
    }

    void RotateIndicatorByMouse()
    {
        var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var angle = Mathf.Atan2(mousePos.y - transform.position.y, mousePos.x - transform.position.x);
        angle *= Mathf.Rad2Deg;
        angle -= 90f;
        if (angle < -180f)
            angle += 360f;

        m_digIndicator.rotation = Quaternion.Euler(0, 0, angle);
    }

    void DigIndicatorActiveToggle(bool isActive)
    {
        m_digIndicator.gameObject.SetActive(isActive);
    }

    void Walk(Vector3 moveVec)
    {
        m_rigid.velocity = moveVec * m_walkSpeed;   
    }

    void Dig(Vector3 digVec)
    {
        m_rigid.velocity = digVec * m_digSpeed;
    }
}
