using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Effect : MonoBehaviour
{
    [SerializeField] float effectDuration;
    public delegate bool ReturnToPoolDelegate(Effect effect);
    private ReturnToPoolDelegate returnToPool;

    public void SetEffect(Transform transform, ReturnToPoolDelegate returnToPool)
    {
        this.transform.SetPositionAndRotation(transform.position, transform.rotation);
        this.returnToPool = returnToPool;
        StartCoroutine(DurationCheck());
    }

    IEnumerator DurationCheck()
    {
        yield return new WaitForSeconds(effectDuration);
        returnToPool.Invoke(this);
    }
}
