using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private bool CanJump;
    private bool OnGround;
    private bool CanMove = true;




    [SerializeField] private float MovementSpeed; 
    [SerializeField] private float JumpSpeed;
    [SerializeField] private float WallSpeed;

    [SerializeField] private float RayLength; //ray ışın demek.
    [SerializeField] private float RayPositionOffSet;
    //SerializeField ifadesi normalde private olan değişkenin unity'de görüntülenebilir ve düzenlenebilir olmasını sağlar.


    //Ground Variables
    Vector3 RayPositionCenter;
    Vector3 RayPositionLeft;
    Vector3 RayPositionRight;

    RaycastHit2D[] GroundHitsCenter;
    RaycastHit2D[] GroundHitsLeft;
    RaycastHit2D[] GroundHitsRight;

    RaycastHit2D[][] AllRaycastHits = new RaycastHit2D[3][];


    //Wall Jump Variables
    private bool OnWallRight;
    private bool OnWallLeft;

    Vector3 WallRayPositionLeft;
    Vector3 WallRayPositionRight;

    RaycastHit2D[] WallHitsLeft;
    RaycastHit2D[] WallHitsRight;



    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }


    private void Update()
    {
        Movement();
        Jump();
    }


    private void RaySetup()
    {
        //Ground Check/Standart Jump Ray Setup
        RayPositionCenter = transform.position + new Vector3(0, -.5f, 0);
        RayPositionLeft = transform.position + new Vector3(-RayPositionOffSet, -.5f, 0);
        RayPositionRight = transform.position + new Vector3(RayPositionOffSet, -.5f, 0);

        GroundHitsCenter = Physics2D.RaycastAll(RayPositionCenter, Vector2.down, RayLength);
        GroundHitsLeft = Physics2D.RaycastAll(RayPositionLeft, Vector2.down, RayLength);
        GroundHitsRight = Physics2D.RaycastAll(RayPositionRight, Vector2.down, RayLength);

        AllRaycastHits[0] = GroundHitsCenter;
        AllRaycastHits[1] = GroundHitsLeft;
        AllRaycastHits[2] = GroundHitsRight;

        OnGround = GroundCheck(AllRaycastHits);
        CanJump = OnGround;



        //Wall Jump/Slide Ray Setup
        WallRayPositionLeft = transform.position + new Vector3(-RayPositionOffSet, 0, 0);
        WallRayPositionRight = transform.position + new Vector3(RayPositionOffSet, 0, 0);

        WallHitsLeft = Physics2D.RaycastAll(WallRayPositionLeft, Vector2.left, RayLength);
        WallHitsRight = Physics2D.RaycastAll(WallRayPositionRight, -Vector2.left, RayLength);

        OnWallLeft = RayCheck(WallHitsLeft);
        OnWallRight = RayCheck(WallHitsRight);

        DrawRay();
    }
    private bool RayCheck (RaycastHit2D [] RayHits)
    {
        foreach(RaycastHit2D hit in RayHits)
        {
            if(hit.collider != null)
            {
                if(hit.collider.tag != "PlayerCollider")
                {
                    return true;
                }
            }
        }
        return false;
    }


    private void Jump()
    {

        RaySetup();

        

        //Ground Jump
        if (Input.GetKey(KeyCode.Space) && CanJump)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);
            rb.velocity = new Vector2(rb.velocity.x, JumpSpeed);
            
        }
        //aynı anda hem zıplayıp hem sağ sola hareket ettirebilmek için.

        if(!OnGround)
        {
            if(CanMove)
            {
                if(Input.GetAxisRaw("Horizontal") < 0 && OnWallLeft)
                {
                    rb.velocity = new Vector2(rb.velocity.x, -WallSpeed);
                }
                else if (Input.GetAxisRaw("Horizontal") > 0 && OnWallRight)
                {
                    rb.velocity = new Vector2(rb.velocity.x, -WallSpeed);
                }
            }
            if(Input.GetKeyDown(KeyCode.Space) && OnWallLeft)
            {
                rb.velocity = new Vector2(JumpSpeed * 0.5f, JumpSpeed);
                StartCoroutine(WallJumpCoolDown());
            }
            if (Input.GetKeyDown(KeyCode.Space) && OnWallRight)
            {
                rb.velocity = new Vector2(-JumpSpeed * 0.5f, JumpSpeed);
                StartCoroutine(WallJumpCoolDown());
            }

        }


    }
      IEnumerator WallJumpCoolDown()
    {
        CanMove = false;
        yield return new WaitForSeconds(0.25f);
        CanMove = true;
        //0.25 sn durması, beklemesi için.
    }


    private bool GroundCheck(RaycastHit2D [][] GroundHits)
    {
        foreach(RaycastHit2D [] HitList in GroundHits)
        {
            foreach(RaycastHit2D hit in HitList)
            {
                if(hit.collider !=null )
                {
                    if(hit.collider.tag !="PlayerCollider")
                    {
                        return true;
                    }
                }
            }
        }
        return false;
        //karakterin altında zemin var mı yok mu onu kontrol etmek için.

    }
    



    private void Movement()
    {
        if (CanMove)
        {
            if (Input.GetAxisRaw("Horizontal") > 0)
            //unity'de edit kısmından project setting kısmında input manager içinde horizontal var
            //orda hangi bilgisayar tuşlarıyla hareket ettirmek istiyorsan onlar yer alıyor.
            //Yani bu kod yön tuşlarını belirlemek amacıyla kullanılır.
            {
                rb.velocity = new Vector2(MovementSpeed * Time.fixedDeltaTime, rb.velocity.y);
            }
            else if (Input.GetAxisRaw("Horizontal") < 0)
            {
                rb.velocity = new Vector2(-MovementSpeed * Time.fixedDeltaTime, rb.velocity.y);
            }
           else
            {
                rb.velocity = new Vector2(0, rb.velocity.y);

            }
           
            //pozitif basılmışsa sağa doğru hareket eder. Negatifse sola doğru hareket eder.
            //Hiçbir şeye basılmamışsa hareketsiz kalır.
            //pozitiften kastı sağ yön tuşu, negatif dediği de sol yön tuşu.
        }
    }
    private void DrawRay()
    {
        //Ground Rays
        Debug.DrawRay(RayPositionCenter, Vector2.down * RayLength, Color.red);
        Debug.DrawRay(RayPositionLeft, Vector2.down * RayLength, Color.red);
        Debug.DrawRay(RayPositionRight, Vector2.down * RayLength, Color.red);
        //Wall Rays
        Debug.DrawRay(WallRayPositionLeft, Vector2.left * RayLength, Color.red);
        Debug.DrawRay(WallRayPositionRight, -Vector2.left * RayLength, Color.red);
    }
}
