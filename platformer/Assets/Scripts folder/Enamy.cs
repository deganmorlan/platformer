using UnityEngine;

public class Enamy : MonoBehaviour
{

    [SerializeField] private float moveSpeed = 0.1f;

    Rigidbody2D enamyCharicter;







    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
     enamyCharicter.GetComponent<Rigidbody2D>();
    }

    private void Awake()
    {
        
    }


    // Update is called once per frame
    void Update()
    {
        if (IsFacingRight() ) 
        {
            enamyCharicter.linearVelocity = new Vector2(moveSpeed, 0);
        }
        else
        {
            enamyCharicter.linearVelocity = new Vector2(-moveSpeed, 0);

        }

        
    }
    bool IsFacingRight()
    {
        return transform.localScale.x > 0;
            }


    private void OnTriggerExit2D(Collider2D collision)
    {
        transform.localScale = new Vector2(-(Mathf.Sign(enamyCharicter.linearVelocity.x)),1.0f );
    }
}
