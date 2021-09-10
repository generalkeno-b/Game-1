using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator an;
    private enum State {idle,running,jumping,falling,hurt}
    private State state = State.idle;
    private Collider2D col;
    [SerializeField] private LayerMask ground;
    [SerializeField] private float speed=10f;
    [SerializeField] private int gems=0;
    [SerializeField] private Text Score; 
    [SerializeField] private Text Health; 
    [SerializeField] private int health=100; 
    private void Start()
    {
        rb=GetComponent<Rigidbody2D>();
        an=GetComponent<Animator>();
        col=GetComponent<Collider2D>();
    }
      private void Update()
      {
        if(state!=State.hurt)
            Move();
        VelocitySwitch();
        an.SetInteger("state",(int)state);
        if(health<=0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
      }
      private void OnTriggerEnter2D(Collider2D collision)
      {
          if(collision.tag=="Collectible")
          {
              Destroy(collision.gameObject);
              gems++;
              Score.text=gems.ToString();
          }
      }
      private void OnCollisionEnter2D(Collision2D other)
      {
          if(other.gameObject.tag=="Enemy")
          {
              if(state==State.falling)
              {
                    rb.velocity=new Vector2(rb.velocity.x,20);
                    state = State.jumping;
                    Destroy(other.gameObject);
              }
              else
              {
                  state=State.hurt;
                  if(other.gameObject.transform.position.x > transform.position.x) //enemy on right
                  {
                      rb.velocity=new Vector2(-10,rb.velocity.y);
                      health-=20;
                  }
                  else //enemy on left
                  {
                      rb.velocity=new Vector2(10,rb.velocity.y);
                      health-=20;
                  }
                  Health.text=health.ToString();
              }
          }
      }
      private void Move()
    {
        float hdirection = Input.GetAxis("Horizontal");
        if((hdirection<0))
        {
            rb.velocity=new Vector2(-speed, rb.velocity.y);
            transform.localScale = new Vector2(-1,1);
        }
        else if((hdirection>0))
        {
            rb.velocity=new Vector2(speed, rb.velocity.y);
            transform.localScale = new Vector2(1,1);
        }
        if(Input.GetButtonDown("Jump")&&col.IsTouchingLayers(ground))
        {
            rb.velocity=new Vector2(rb.velocity.x,20);
            state = State.jumping;
        }
    }
    private void VelocitySwitch()
    {
        if(state==State.jumping)
        {
            if(rb.velocity.y<0.1f)
            {
                state=State.falling;
            }
        }
        else if(state==State.falling)
        {
            if(col.IsTouchingLayers(ground))
            {
                state=State.idle;
            }
        }
        else if(state==State.hurt)
        {
            if(Mathf.Abs(rb.velocity.x)<0.1f)
            {
                state=State.idle;
            }
        }
        else if(Mathf.Abs(rb.velocity.x)>2f)
        {
            state=State.running;
        }
        else
        {
            state=State.idle;
        }
    }

}
