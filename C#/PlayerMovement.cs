using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    static public PlayerMovement H;

    public GameManager gm;
    public Event evt;

    [Header("Set in Inspector")]
    public float movePower = 1.0f;
    public float jumpPower = 1.0f;

    Rigidbody2D hRigid;
    SpriteRenderer hRend;
    Animator hAnimator;

    private AudioSource hAudioSource;

    Vector3 hMovement;
    private bool isJumping = false;

    //움직일 때 위아래를 동시에 입력받지 않기 위해 이러한 bool 타입 변수를 선언.
    //본 게임에서 기본적으로 상하좌우로 움직이지만, 특정 미니게임으로 진입하면 점프를 해야하는 등의 일이 생기기 때문.
    //gachul == "가출"
    private bool isMoveX = true;
    private bool isMoveY = true;
    public bool isMoving = false;
    private bool awMove = true;
    private bool hunger = true;
    private bool gachul = true;
    private bool full = true;

    //이동할 때 보여줄 이미지
    [Header("Set Motion")]
    public Sprite backMotionState;
    public Sprite frontMotionState;
    public Sprite sideMotionState;
    
    public bool cageCollision = false;
    public static bool startSource = false;
    
    int playerLayer, enemyLayer;
    Renderer rend;
    Color color;
    public bool coroutineAllowed = true;

    private void Awake()
    {
        if (H == null)
        {
            H = this;
        }
        else
        {
            Debug.LogError("<color=red>Player.Awake()</color> - attempted to assign second Player.H!");
        }
    }

    void Start()
    {
        hRigid = gameObject.GetComponent<Rigidbody2D>();
        hRend = gameObject.GetComponentInChildren<SpriteRenderer>();
        hAnimator = GetComponent<Animator>();
        hAudioSource = this.gameObject.GetComponent<AudioSource>();

        playerLayer = this.gameObject.layer;
        enemyLayer = LayerMask.NameToLayer("Enemy");
        Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, false);

        rend = GetComponent<Renderer>();
        color = rend.material.color;
    }

    void Update()
    {
        if (SceneManager.GetActiveScene().name == "RoomScene" || SceneManager.GetActiveScene().name == "ArrowScene")
        {
            hRigid.gravityScale = 0.0f;
        }
        else
        {
            if (Input.GetButtonDown("Jump"))
            {
                PlayerJump();
            }
        }

        Vector3 hPos = transform.position;

        Vector3 camPos = Camera.main.WorldToViewportPoint(transform.position);
        camPos.x = Mathf.Clamp01(camPos.x);
        camPos.y = Mathf.Clamp01(camPos.y);

        hPos = Camera.main.ViewportToWorldPoint(camPos);
        transform.position = hPos;

        //유니티 내부 함수 이용. 
        if (DropMinigameManager.enemyDestroy == true)
        {
            if (coroutineAllowed == true)
            {
                //Immortal이란 함수를 전달한다. Immortal() 함수는 최하단에 선언되어 있다.
                StartCoroutine("Immortal");
            }
            DropMinigameManager.enemyDestroy = false;
        }
    }

    private void FixedUpdate()
    {
        PlayerMove();
        PlayerMotionState();
        PlayerAudioSource();
    }

    public void PlayerMove()
    {
        //움직임이 필요 없는 화면에서는 움직이지 않도록 설정.
        if (SceneManager.GetActiveScene().name == "ArrowScene")
            return;
            
        Vector3 hMoveVelocity = Vector3.zero;
        
        if (SceneManager.GetActiveScene().name == "RoomScene")
        {
            if (Input.GetAxisRaw("Vertical") > 0)
            {
                hMoveVelocity = Vector3.up;
                hRend.flipY = true;
                gm.curFull -= 5 * Time.deltaTime;
            }
            else if (Input.GetAxisRaw("Vertical") < 0)
            {
                hMoveVelocity = Vector3.down;
                hRend.flipY = false;
                gm.curFull -= 5 * Time.deltaTime;
            }
            else
            {
                hRend.flipY = false;
            }
        }
        
        if (Input.GetAxisRaw("Horizontal") < 0)
        {
            hMoveVelocity = Vector3.left;
            hRend.flipY = false;
            hRend.flipX = true;
            if (SceneManager.GetActiveScene().name == "RoomScene")
                gm.curFull -= 5 * Time.deltaTime;
        }
        else if (Input.GetAxisRaw("Horizontal") > 0)
        {
            hMoveVelocity = Vector3.right;
            hRend.flipY = false;
            hRend.flipX = false;
            if (SceneManager.GetActiveScene().name == "RoomScene")
                gm.curFull -= 5 * Time.deltaTime;
        }
        
        transform.position += hMoveVelocity * movePower * Time.deltaTime * 7;
    }
    
    public void PlayerJump()
    {
        //점프가 필요한 장면에서 사용.
        if (!isJumping)
        {
            isJumping = true;

            hRigid.linearVelocity = Vector2.zero;

            Vector2 jumpVelocity = new Vector2(0, jumpPower);
            hRigid.AddForce(jumpVelocity * 5, ForceMode2D.Impulse);
        }
    }

    public void PlayerMotionState()
    {
        //애니메이션을 위해서 만들어진 함수.
        if (SceneManager.GetActiveScene().name == "ArrowScene")
        {
            awMove = true;
            isMoveX = false;
            isMoveY = false;

            hunger = false;
            gachul = false;
            full = false;
        }
        else if (SceneManager.GetActiveScene().name == "GameoverScene")
        {
            awMove = false;
            isMoveX = false;
            isMoveY = false;

            hunger = true;
            gachul = false;
            full = false;
        }
        else if (SceneManager.GetActiveScene().name == "GameClear_Gachul")
        {
            awMove = false;
            isMoveX = false;
            isMoveY = false;

            hunger = false;
            gachul = true;
            full = false;
        }
        else if (SceneManager.GetActiveScene().name == "GameClear_Stay")
        {
            awMove = false;
            isMoveX = false;
            isMoveY = false;

            hunger = false;
            gachul = false;
            full = true;
        }
        else
        {
            //기본적으로 움직이고 있는 상황
            awMove = false;
            hunger = false;
            gachul = false;
            full = false;

            if (Input.GetAxisRaw("Horizontal") != 0)
            {
                isMoveX = true;
            }
            else
            {
                isMoveX = false;
            }

            if (Input.GetAxisRaw("Vertical") != 0 && SceneManager.GetActiveScene().name == "RoomScene")
            {
                isMoveY = true;
            }
            else
            {
                isMoveY = false;
            }
        }
        
        hAnimator.SetBool("ArrowMove", awMove);
        hAnimator.SetBool("MoveX", isMoveX);
        hAnimator.SetBool("MoveY", isMoveY);
        hAnimator.SetBool("Hunger", hunger);
        hAnimator.SetBool("Gachul", gachul);
        hAnimator.SetBool("Full", full);
    }

    public void PlayerAudioSource()
    {
        //움직일 때 나는 소리
        if(Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
        {
            isMoving = true;
        }
        else
        {
            isMoving = false;
        }

        if(isMoving == true)
        {
            if(!hAudioSource.isPlaying)
            {
                hAudioSource.Play();
            }
        }
        else
        {
            hAudioSource.Stop();
        }
    }
    
    private void OnTriggerStay2D(Collider2D collision)
    {
        //충돌 안에 있을 때.
        if (collision.gameObject.name == "SceneChange" && (SceneManager.GetActiveScene().name == "CageScene") && Input.GetKey(KeyCode.Z))
        {
            startSource = true;
            SceneManager.LoadScene("RoomScene");
        }

        if (collision.gameObject.name == "Cage")
        {
            cageCollision = true;
        }
        else
        {
            cageCollision = false;
        }

        if (SceneManager.GetActiveScene().name == "RoomScene")
        {
            if (evt.randomValue >= evt.randomValueMin && Event.squirrelGame == true)
            {
                //메인 게임 화면에서 이벤트가 일어난 상황이라면, 아래의 switch 문을 실행하지 않도록 하기 위함.
                return;
            }
        }

        switch (collision.gameObject.name)
        {
            //씬 이동.
            case "Desk":
                if (Input.GetKey(KeyCode.Z))
                { SceneManager.LoadScene("DropScene"); }
                break;
            case "Closet":
                if (Input.GetKey(KeyCode.Z))
                { SceneManager.LoadScene("CardScene"); }
                break;
            case "Stringball":
                if (Input.GetKey(KeyCode.Z))
                { SceneManager.LoadScene("ArrowScene"); }
                break;
            case "Box":
                if (Input.GetKey(KeyCode.Z))
                { SceneManager.LoadScene("SpaceScene"); }
                break;
            case "Background":
                break;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isJumping = false;
        }
    }

    IEnumerator Immortal()
    {
        //return 위의 내용을 실행. return 이후 2초 뒤에(WaitForSeconds(2f)로 지정되어 있음) 아래의 내용을 실행한다.
        coroutineAllowed = false;
        Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, true);
        color.a = 0.5f;
        rend.material.color = color;
        yield return new WaitForSeconds(2f);

        Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, false);
        color.a = 1.0f;
        rend.material.color = color;
        coroutineAllowed = true;
    }
}
