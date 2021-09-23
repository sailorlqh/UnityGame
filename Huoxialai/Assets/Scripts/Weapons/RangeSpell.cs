using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeSpell : MonoBehaviour
{

    public GameObject Target;

    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
        if (Target != null)
        {
            Vector3 targetPos = Target.transform.position;
            transform.LookAt(targetPos);
            float distance2 = Vector3.Distance(targetPos, transform.position);
            if(distance2>1f)
            {
                transform.Translate(Vector3.forward * 30.0f * Time.deltaTime);

            }
            else
            {
                HitTarget();
            }
        }
    }

    void HitTarget()
    {
        Debug.Log("hit");
        Destroy(gameObject);
    }
}
