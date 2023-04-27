using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Components")]

    private Rigidbody2D rb;

    [Header("Horizontal Movement")]
    [SerializeField] private float MoveSpeed = 10f;
    [SerializeField] private float MaxSpeed = 7f;
    [SerializeField] private float LinearDrag = 4f;
    private float HorizontalDirection;

    [Header("Jump Variables")]

    [SerializeField] private float JumpForce = 12f;
    [SerializeField] private float AirLinearDrag = 2.5f;
    [SerializeField] private float fallMultiplier = 8f;
    [SerializeField] private float LowJumpFallMultiplier = 5f;
    [SerializeField] private int ExtraJumps;
    private int ExtraJumpsValue;
    private bool CanJump => Input.GetKeyDown(KeyCode.Space) && (OnGround || ExtraJumpsValue > 0);

    [Header("Layer Mask")]
    [SerializeField] LayerMask GroundLayer;

    [Header("Ground Collision Variables")]
    [SerializeField] private float GroundRaycastLenght;
    [SerializeField] private Vector3 GroundRaycastOffSet;
    private bool OnGround;





    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        HorizontalDirection = GetInput().x;
       
        if (CanJump)
        {
            Jump();

        }
    }


    void FixedUpdate()
    {
        CheckCollision();
        MoveCharacter();
       
       
        if(OnGround)
        {
            ApplyLinearDrag();
            ExtraJumpsValue = ExtraJumps;
        }
        else
        {
            ApplyAirLinearDrag();
            FallMultiplier();
        }
    }




    private Vector2 GetInput()
    {
        return new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }

    void MoveCharacter()
    {
        rb.AddForce( new Vector2 (HorizontalDirection, 0f) * MoveSpeed);

        if (Mathf.Abs(rb.velocity.x) > MaxSpeed) //RigidBody'nin x eksenindeki hızının mutlak değeri max hızdan büyük mü?
        {
            rb.velocity = new Vector2(Mathf.Sign(rb.velocity.x) * MaxSpeed, rb.velocity.y);
            //karakterin x eksenindeki hızı MaxSpeed'i aşamaz.
        }
    }

    private void Jump()
    {
        if (!OnGround)
        {
            ExtraJumpsValue--;
        }
       

            rb.velocity = new Vector2(rb.velocity.x, 0f);
            rb.AddForce(Vector2.up * JumpForce, ForceMode2D.Impulse);
            //forceModer2D.Impulse kısmı anlık bir kuvvet olarak uygulanmasını sağlar.
            //ondan önceki kısım nesneye yukarı doğru kuvvet uygular.
        
    }

    void ApplyLinearDrag()
    { //velocity sürat demek.
        bool ChangingDirections = (HorizontalDirection > 0 && rb.velocity.x < 0) || (HorizontalDirection < 0 && rb.velocity.x > 0);

        if (Mathf.Abs(HorizontalDirection) < 0.4f || ChangingDirections)
        {
            
            rb.drag = LinearDrag;

        }
        else
        {
            
            rb.drag = 0f; //drag sürtünme demek.

        }
        rb.gravityScale = 0;
    }


private void ApplyAirLinearDrag()
    {
        rb.drag = AirLinearDrag;

    }
    private void CheckCollision()
    {
        OnGround = Physics2D.Raycast(transform.position + GroundRaycastOffSet, Vector2.down, GroundRaycastLenght, GroundLayer) ||
                   Physics2D.Raycast(transform.position - GroundRaycastOffSet, Vector2.down, GroundRaycastLenght, GroundLayer);
    }

    private void FallMultiplier()
    {

        if(rb.velocity.y < 0)
        {
            rb.gravityScale = fallMultiplier;

        }
        else if (rb.velocity.y > 0 && !Input.GetKey(KeyCode.Space))
        {
            rb.gravityScale = LowJumpFallMultiplier;

        }
        else
        {
            rb.gravityScale = 0f;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position + GroundRaycastOffSet, transform.position + GroundRaycastOffSet + Vector3.down * GroundRaycastLenght);
        Gizmos.DrawLine(transform.position - GroundRaycastOffSet, transform.position - GroundRaycastOffSet + Vector3.down * GroundRaycastLenght);
    }

}