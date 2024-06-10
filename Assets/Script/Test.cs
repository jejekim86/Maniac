using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class Test : Controller
{
    Vector3 translation;
    CapsuleCollider collier;

    private void Start()
    {
        collier = GetComponent<CapsuleCollider>();
        isride = false;
        HP_image = null;
    }
    public override void Move()
    {
        float horizontalMove = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        float speed = 10;

        translation = new Vector3(horizontalMove, 0, vertical);

        translation *= speed * Time.deltaTime;
        transform.Translate(translation, Space.World);
    }

    IEnumerator ClickButton(Vehicle item)
    {
        switch (isride)
        {
            case true: // 차에서 내릴때
                yield return null;
                item.CheckRide(false);
                transform.SetParent(null);
                transform.position = item.transform.position + (Vector3.right * 3);
                isride = false;
                break;
            case false:
                if (!item) yield break; // 차에서 탑승할때
                Debug.Log("ClickButton");
                yield return new WaitForSeconds(3f);
                isride = true;
                item.CheckRide(true);
                gameObject.SetActive(false);
                //transform.SetParent(item.gameObject.transform);
                break;
        }
        yield break;
    }
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Vehicle"))
        {
            if (Input.GetKeyDown(KeyCode.E))
                StartCoroutine(ClickButton(collision.gameObject.GetComponent<Vehicle>())); 
        }
    }
    private void Update()
    {
        Move();
    }
    public void SetColliderEnabled(bool check)
    {
        collier.enabled = check;
        meshRenderer.enabled = check;
    }
}
