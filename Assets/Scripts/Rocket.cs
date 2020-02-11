using System;
using UnityEngine;

/// <summary>
/// to add :
///   -  blast radius
/// </summary>
public class Rocket : MonoBehaviour
{
    [SerializeField] private RocketSettings settings;
   
    private int owner;
    private Rigidbody rigid;
    private readonly float speed = 100; // vitesse des roquette (ne pas modifier)
    private int teamId;
    private bool available;
    public bool Available => this.available;


    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
    }

    public void GoToSleep()
    {
        this.available = true;
        gameObject.SetActive(false);
    }

    public void Launch(Bot user, Vector3 dir)
    {
        available = false;
        this.owner = user.ID;
        var userTransform = user.transform;
        var rocketTransform = this.transform;
        rocketTransform.position = userTransform.position;
        rocketTransform.rotation = Quaternion.LookRotation(dir, Vector3.up);
        gameObject.SetActive(true);
    }

    private void FixedUpdate()
    {
        rigid.velocity = Vector3.zero;
        rigid.AddForce(transform.forward * speed, ForceMode.VelocityChange);
    }

    private void OnTriggerEnter(Collider col)
    {
        if(col.GetComponent<Bot>() && col.GetComponent<Bot>().ID == this.owner)
            return;
        int botLayer = 1 << LayerMask.NameToLayer("TeamRed");
        botLayer |= 1 << LayerMask.NameToLayer("TeamBlue");
        var bots = Physics.OverlapSphere(transform.position, this.settings.RocketBlastRadius, botLayer);
        foreach (var bot in bots)
        {
            var dam = this.settings.RocketDamage;
            var distance = Vector3.Distance(this.transform.position, bot.ClosestPoint(this.transform.position));
            dam = this.settings.RocketDamage;
            if (distance > 1.0f) dam = this.settings.RocketDamage - 2;
            if (distance > 3.0f) dam = this.settings.RocketDamage - 3;
            if (distance > 5.0f) dam = this.settings.RocketDamage - 1;
            bot.GetComponent<Bot>().TakeDamage(dam);
        }

        if (Helper.GetInstance() != null)
        {
            Helper.GetInstance().ShowBlast(this.transform.position);
        }
        
        GoToSleep();
        
    }
}