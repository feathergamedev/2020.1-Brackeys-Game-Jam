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

    private PlayerState m_curPlayerState;

    public PlayerState CurPlayerState
    {
        get
        {
            return m_curPlayerState;
        }
        set
        {
            PlayerStateInitialize(value);
        }
    }

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

    [SerializeField]
    private GameObject m_digHoleParticle;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        CurPlayerState = PlayerState.Walk;
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Mechanic"))
        {
            var mechanic = collision.gameObject.GetComponent<IMechanic>();

            if (mechanic == null)
                Debug.LogError(collision.gameObject.name + " don't have IMechanic-related script!");

            mechanic.Triggered();
        }
    }

    void PlayerStateInitialize(PlayerState newState)
    {
        var curState = m_curPlayerState;

        switch (curState)
        {
            case PlayerState.Walk:
                break;
            case PlayerState.Dig:
                ResetDigIndicator();
                break;
        }

        switch (newState)
        {
            case PlayerState.Walk:
                DigIndicatorActiveToggle(false);

                gameObject.layer = LayerMask.NameToLayer("Player_Normal");
                m_renderer.sortingLayerName = "ForeGround";
                m_digHoleParticle.SetActive(false);
                break;
            case PlayerState.Dig:
                DigIndicatorActiveToggle(true);

                gameObject.layer = LayerMask.NameToLayer("Player_Sneak");
                m_renderer.sortingLayerName = "InGround";
                m_digHoleParticle.SetActive(true);
                break;
        }

        m_curPlayerState = newState;
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

    void ResetDigIndicator()
    {
        DigIndicatorActiveToggle(false);
        m_digIndicator.rotation = Quaternion.Euler(0, 0, 0);
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
