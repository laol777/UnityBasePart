using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AnimationControlBot : MonoBehaviour {


    public Animator animator;


    [Range(0, 1)]public float idleOrActive;
    [Range(0, 1)]public float leftOrRight;

    public BulletContainer bulletContainer;

    List<Transform> bullets;

    Vector3 nearBullet;
    public float minDistance;

	void Update () {

        bullets = bulletContainer.GetBullet();
        minDistance = 100f;

        for (int i = 0; i < bullets.Count; ++i)
        {
            if (bullets[i] != null)
            {
                if (Vector3.Distance(bullets[i].position, transform.position) < minDistance)
                {
                    minDistance = Vector3.Distance(bullets[i].position, transform.position);
                    nearBullet = bullets[i].position;
                }
            }
        }


        idleOrActive = minDistance < 4f ? LerpValue(idleOrActive, 1f) : LerpValue(idleOrActive, 0f);
        leftOrRight = nearBullet.x > 0f ? LerpValue(leftOrRight, 0f) : LerpValue(leftOrRight, 1f);

        animator.SetFloat("actionOrIdle", idleOrActive);
        animator.SetFloat("leftOrRight", leftOrRight);
    }

    float LerpValue(float oldValue, float newValue)
    {
        return Mathf.Lerp(oldValue, newValue, 3f * Time.deltaTime);
    }

}
