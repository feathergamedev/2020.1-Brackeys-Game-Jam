using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public enum PlayerState
{
    Prepare,
    Walk,
    Dig,
}

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

    [SerializeField]
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

    [Header("Speed")]

    [SerializeField]
    private float m_walkSpeed;

    [SerializeField]
    private float m_digSpeed;

    [SerializeField]
    private float m_indicatorRotateSpeed;

    [SerializeField]
    private float m_axisMovementTransistTime;

    [Header("Energy")]

    [SerializeField]
    private float m_maxEnergy;

    private float m_curEnergy;

    [SerializeField]
    private float m_energyConsumeRate, m_energyRefillRate;

    [Header("Color")]

    [SerializeField]
    private Color m_normalColor, m_sneakColor;

    [Header("UI")]

    #region UI

    [SerializeField]
    private Image m_curEnergyImage;

    #endregion


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

    private void OnEnable()
    {
        RegisterEvent();
    }

    private void OnDisable()
    {
        UnregisterEvent();
    }

    private void Awake()
    {
        instance = this;
    }

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
            CurPlayerState = PlayerState.Dig;
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            CurPlayerState = PlayerState.Walk;
        }

        switch (CurPlayerState)
        {
            case PlayerState.Walk:
                var walkVec = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
                Walk(walkVec);

                RefillEnergy();
                break;
            case PlayerState.Dig:
                if (m_curEnergy <= 0)
                {
                    CurPlayerState = PlayerState.Walk;
                }

                var direction = (m_digDirection.position - transform.position).normalized;

                //RotateIndicatorByKeyboard();
                //RotateIndicatorByMouse();
                RotateIndicatorByAxisMovement();

                Dig(direction);

                ComsumeEnergy();
                break;                
        }


    }

    void RegisterEvent()
    {
        EventEmitter.Add(GameEvent.LevelStart, OnLevelStart);
    }

    void UnregisterEvent()
    {
        EventEmitter.Remove(GameEvent.LevelStart, OnLevelStart);
    }

    void OnLevelStart(IEvent @event)
    {
        var initPlayerPos = LevelManager.instance.GetInitPlayerPos();
        PlayerReset(initPlayerPos);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Mechanic"))
        {
            var mechanic = collision.gameObject.GetComponent<IMechanic>();

            if (mechanic == null)
                Debug.LogError(collision.gameObject.name + " don't have IMechanic-related script!");

            var canTrigger = ((CurPlayerState == PlayerState.Walk) && (collision.gameObject.tag == "OnGround"))
                            || ((CurPlayerState == PlayerState.Dig) && (collision.gameObject.tag == "InGround"));

            if (canTrigger)
                mechanic.Triggered();
            else
            {
                Debug.Log(CurPlayerState + ", " + collision.gameObject.tag);
            }
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("ViewSensor"))
        {
            var sensor = collision.gameObject.GetComponent<LevelViewSensor>();

            if (sensor == null)
                Debug.LogError(collision.gameObject.name + " don't have ViewSensor script!");

            sensor.TriggerSensor();
        }
    }

    public void GetCaught()
    {
        if (CurPlayerState == PlayerState.Dig)
            return;

        //TODO: perform

        EventEmitter.Emit(GameEvent.LevelFail);
    }

    void PlayerReset(Vector3 initPos)
    {
        transform.position = initPos;
        m_curEnergy = m_maxEnergy;
    }

    void PlayerStateInitialize(PlayerState newState)
    {
        if (m_curPlayerState == newState)
            return;

        switch (m_curPlayerState)
        {
            case PlayerState.Walk:
                break;
            case PlayerState.Dig:
                DigIndicatorActiveToggle(false);
                break;
        }

        switch (newState)
        {
            case PlayerState.Walk:
                gameObject.layer = LayerMask.NameToLayer("Player_Normal");
                m_renderer.DOColor(m_normalColor, 0.5f).SetEase(Ease.Linear);
                m_digHoleParticle.SetActive(false);
                break;
            case PlayerState.Dig:
                DigIndicatorActiveToggle(true);
                gameObject.layer = LayerMask.NameToLayer("Player_Sneak");
                m_renderer.DOColor(m_sneakColor, 0.5f).SetEase(Ease.Linear);
                m_digHoleParticle.SetActive(true);
                break;
        }

        m_curPlayerState = newState;
    }

    void RotateIndicatorByAxisMovement()
    {
        var angle = m_digIndicator.rotation.eulerAngles.z;

        if (Input.GetAxisRaw("Horizontal") == 1)
        {
            if (Input.GetAxisRaw("Vertical") == -1)
                angle = -135f;
            else if (Input.GetAxisRaw("Vertical") == 1)
                angle = -45f;
            else if (Input.GetAxisRaw("Vertical") == 0)
                angle = -90f;
        }
        else if (Input.GetAxisRaw("Horizontal") == -1)
        {
            if (Input.GetAxisRaw("Vertical") == -1)
                angle = 135f;
            else if (Input.GetAxisRaw("Vertical") == 1)
                angle = 45f;
            else if (Input.GetAxisRaw("Vertical") == 0)
                angle = 90f;
        }
        else
        {
            if (Input.GetAxisRaw("Vertical") == -1)
                angle = -180f;
            else if (Input.GetAxisRaw("Vertical") == 1)
                angle = 0f;
            else if (Input.GetAxisRaw("Vertical") == 0)
            {
                //Do nothing
            }
        }

        var newEuler = Quaternion.Euler(0, 0, angle);
        m_digIndicator.transform.DORotateQuaternion(newEuler, m_axisMovementTransistTime).SetEase(Ease.Linear);

        //m_digIndicator.transform.rotation
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

    void ComsumeEnergy()
    {
        m_curEnergy -= (m_energyConsumeRate * Time.deltaTime);

        m_curEnergy = Mathf.Max(0, m_curEnergy);
        m_curEnergyImage.fillAmount = m_curEnergy / m_maxEnergy;
    }

    void RefillEnergy()
    {
        m_curEnergy += (m_energyRefillRate * Time.deltaTime);

        m_curEnergy = Mathf.Min(m_curEnergy, m_maxEnergy);
        m_curEnergyImage.fillAmount = m_curEnergy / m_maxEnergy;
    }

    public void RefillAllEnergy()
    {
        m_curEnergy = m_maxEnergy;
        m_curEnergyImage.fillAmount = m_curEnergy / m_maxEnergy;

        //TODO: Sound Effect.
    }
}
