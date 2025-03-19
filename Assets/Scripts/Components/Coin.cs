using benjohnson;
using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] float startSpeed = 1;
    [SerializeField] float pickupY = -12;

    // Components
    Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = new Vector2(Random.Range(-startSpeed, startSpeed), Random.Range(startSpeed * 0.75f, startSpeed * 1.25f));
    }

    void Update()
    {
        //Get ride of the coin before it adds to the newgame
        if(Player.instance.Health.currentHealth <= 0)
        {
            Destroy(gameObject);
        }

        if (transform.position.y <= pickupY)
        {
            // Pick up
            Player.instance.Wallet.AddMoney(1);
            SoundManager.instance.PlaySound("Pick Up");
            Destroy(gameObject);
        }
    }
}
