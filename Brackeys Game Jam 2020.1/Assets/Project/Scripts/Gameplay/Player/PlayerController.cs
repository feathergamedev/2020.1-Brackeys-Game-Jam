using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public enum PlayerState
{
    Freeze,
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

    [SerializeField]
    private float m_normalScale, m_sneakScale;

    [Header("UI")]

    #region UI

    [SerializeField]
    private Image m_curEnergyImage;

    #endregion

    [Header("Juicy")]

    [SerializeField]
    private GameObject m_dieParticle;

    [SerializeField]
    private ParticleSystem m_digHoleParticle;

    [Header("Component")]

    #region Component

    [SerializeField]
    Rigidbody2D m_rigid;

    [SerializeField]
    SpriteRenderer m_renderer;

    [SerializeField]
    Collider2D m_collider;

    [SerializeField]
    private Animator m_animator;

    [SerializeField]
    private Transform m_digIndicator;

    [SerializeField]
    private Transform m_digDirection;

    #endregion

    #region BadCode

    private GameObject m_finishPoint;

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

    }

    // Update is called once per frame
    void Update()
    {
        if (CurPlayerState != PlayerState.Freeze)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                CurPlayerState = PlayerState.Dig;
                m_collider.enabled = false;
                m_collider.enabled = true;
                m_animator.SetBool("IsWalking", false);
            }

            if (Input.GetKeyUp(KeyCode.Space))
            {
                if (CurPlayerState == PlayerState.Dig)
                {
                    StartCoroutine(BackToGroundPerform());
                }
            }
        }

        switch (CurPlayerState)
        {
            case PlayerState.Walk:
                var walkVec = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
                Walk(walkVec);

                RefillEnergy();

                if (m_rigid.velocity == Vector2.zero)
                    m_animator.SetBool("IsWalking", false);
                else
                    m_animator.SetBool("IsWalking", true);

                RotateIndicatorByAxisMovement();

                if (AudioManager.instance.IsPlaying(SoundEffectType.DigHole))
                {
                    AudioManager.instance.StopSoundEffect(SoundEffectType.DigHole);
                }

                break;
            case PlayerState.Dig:
                if (m_curEnergy <= 0)
                {
                    if (CurPlayerState == PlayerState.Dig)
                        StartCoroutine(BackToGroundPerform());
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
        EventEmitter.Add(GameEvent.LevelFail, OnLevelFail);
        EventEmitter.Add(GameEvent.LevelComplete, OnLevelComplete);
    }

    void UnregisterEvent()
    {
        EventEmitter.Remove(GameEvent.LevelStart, OnLevelStart);
        EventEmitter.Remove(GameEvent.LevelFail, OnLevelFail);
        EventEmitter.Remove(GameEvent.LevelComplete, OnLevelComplete);
    }

    void OnLevelStart(IEvent @event)
    {
        var initPlayerPos = LevelManager.instance.GetInitPlayerPos();
        PlayerReset(initPlayerPos);
    }

    void OnLevelFail(IEvent @event)
    {
        StartCoroutine(LevelFailPerform());
    }

    void OnLevelComplete(IEvent @event)
    {
        CurPlayerState = PlayerState.Freeze;
        m_collider.enabled = false;
        m_animator.SetBool("IsWalking", false);
        m_animator.SetBool("IsDigging", false);
        m_animator.SetBool("IsDead", false);

        StartCoroutine(LevelCompletePerform());
    }

    IEnumerator LevelCompletePerform()
    {
        var EnterDoorPos = m_finishPoint.transform.position + new Vector3(0, -0.36f, 0);
        transform.DOMove(EnterDoorPos, 1.0f).SetEase(Ease.Linear);

        transform.DOScale(new Vector3(0.5f, 0.5f, 0.5f), 1.0f).SetEase(Ease.Linear);
        m_renderer.DOColor(new Color(255, 255, 255, 0), 1.0f).SetEase(Ease.Linear);

        var randomRotateZ = 45 * Random.Range(-3f, 3f);

        var endRotate = Quaternion.Euler(0, 0, randomRotateZ);
        transform.DORotateQuaternion(endRotate, 1.0f);

        yield return new WaitForSeconds(1.0f);
    }

    IEnumerator LevelFailPerform()
    {
        AudioManager.instance.PlaySoundEffect(SoundEffectType.PlayerDie);

        CurPlayerState = PlayerState.Freeze;
        m_collider.enabled = false;

        m_animator.SetBool("IsDead", true);
        m_renderer.DOColor(new Color(255, 255, 255, 0), 1.0f).SetEase(Ease.Linear);

        m_dieParticle.SetActive(true);

        yield return new WaitForSeconds(1.02f);

        m_dieParticle.SetActive(false);
        LevelManager.instance.LevelRestart();
        yield return null;        
    }

    IEnumerator BackToGroundPerform()
    {
//        CurPlayerState = PlayerState.Freeze;

        DigIndicatorActiveToggle(false);
        AudioManager.instance.StopSoundEffect(SoundEffectType.DigHole);
        AudioManager.instance.PlaySoundEffect(SoundEffectType.BackToGround);

        m_digHoleParticle.Stop();

        CurPlayerState = PlayerState.Walk;
        gameObject.layer = LayerMask.NameToLayer("Player_Normal");

        m_collider.enabled = false;
        m_collider.enabled = true;

        yield return new WaitForSeconds(0.12f);

        m_renderer.transform.DORotateQuaternion(Quaternion.Euler(0, 0, 0), 0.1f).SetEase(Ease.Linear);

        yield return null;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Mechanic"))
        {
            if (CurPlayerState == PlayerState.Freeze)
                return;

            var mechanic = collision.gameObject.GetComponent<IMechanic>();

            if (mechanic == null)
                Debug.LogError(collision.gameObject.name + " don't have IMechanic-related script!");

            var canTrigger = ((CurPlayerState == PlayerState.Walk) && (collision.gameObject.tag == "OnGround"))
                            || ((CurPlayerState == PlayerState.Dig) && (collision.gameObject.tag == "InGround"));

            if (collision.GetComponent<Mechanic_FinishPoint>())
                m_finishPoint = collision.gameObject;

            if (canTrigger)
                mechanic.Triggered();
            else
            {
//                Debug.Log(CurPlayerState + ", " + collision.gameObject.tag);
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
        StartCoroutine(PlayerResetSequence(initPos));
    }

    IEnumerator PlayerResetSequence(Vector3 initPos)
    {
        //Component
        transform.localPosition = initPos;
        transform.localScale = Vector3.zero;
        transform.localRotation = Quaternion.Euler(0, 0, 0);
        m_renderer.transform.localRotation = Quaternion.Euler(0, 0, 0);

        m_animator.SetBool("IsWalking", false);
        m_animator.SetBool("IsDigging", false);
        m_animator.SetBool("IsDead", false);

        //Gameplay
        m_curEnergy = m_maxEnergy;

        //Juicy
        m_dieParticle.SetActive(false);
        m_digHoleParticle.Stop();

        var transistTime = 0.5f;
        var transistEase = Ease.InSine;
        transform.DOScale(new Vector3(100, 100, 100), transistTime).SetEase(transistEase);
        m_renderer.DOColor(new Color32(255, 255, 255, 255), transistTime).SetEase(transistEase);

        yield return new WaitForSeconds(transistTime + 0.2f);

        CurPlayerState = PlayerState.Walk;
        m_collider.enabled = true;

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
                m_animator.SetBool("IsDigging", false);

                break;
        }

        switch (newState)
        {
            case PlayerState.Walk:

                m_renderer.DOColor(m_normalColor, 0.5f).SetEase(Ease.Linear);
                m_renderer.transform.DOScale(new Vector3(m_normalScale, m_normalScale, m_normalScale), 0.5f).SetEase(Ease.Linear);

                break;
            case PlayerState.Dig:
                DigIndicatorActiveToggle(true);
                gameObject.layer = LayerMask.NameToLayer("Player_Sneak");

                m_renderer.DOColor(m_sneakColor, 0.5f).SetEase(Ease.Linear);
                m_renderer.transform.DOScale(new Vector3(m_sneakScale, m_sneakScale, m_sneakScale), 0.5f).SetEase(Ease.Linear);

                m_digHoleParticle.Play();

                AudioManager.instance.PlaySoundEffect(SoundEffectType.DigHole);

                m_animator.SetBool("IsDigging", true);

                break;
            case PlayerState.Freeze:
                m_rigid.velocity = Vector2.zero;
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
                if (CurPlayerState == PlayerState.Walk)
                    angle = 0f;
            }
        }

        var newEuler = Quaternion.Euler(0, 0, angle);
        m_digIndicator.transform.DORotateQuaternion(newEuler, m_axisMovementTransistTime).SetEase(Ease.Linear);

        if (CurPlayerState == PlayerState.Dig)
            m_renderer.transform.DORotateQuaternion(newEuler, m_axisMovementTransistTime).SetEase(Ease.Linear);
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
