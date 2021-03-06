﻿using UnityEngine;
using System.Collections;

public class LeftRacket : MonoBehaviour
{
    public GameObject center;
    public GameObject otherPaddle;
    public ControlSettings ctrlSet;

    float getEllipseX(double angle)
    {
        return (float)(77 * System.Math.Cos(angle) + 100);
    }


    float getEllipseY(double angle)
    {
        return (float)(41 * System.Math.Sin(angle) + 100);
    }

    void Start()
    {
      	transform.position = new Vector2(getEllipseX(System.Math.PI),getEllipseY(System.Math.PI));
        Physics2D.IgnoreCollision(GetComponent<BoxCollider2D>(), otherPaddle.GetComponent<BoxCollider2D>());
    }

    float getX(float x)
    {
        // map pixel -> tranform coordinates
        return x / Screen.width * 180.0f + 10.0f;
    }

    float getY(float y)
    {
        // map pixel -> tranform coordinates
        return y / Screen.height * 100.0f + 50.0f;
    }
    

    float compute(float x, float y)
    {
        return ((x-100) * (x-100)) / (77 * 77) + ((y-100) * (y-100)) / (41 * 41);
    }

    Vector2 translate(Vector2 p, Vector2 q, float t)
    {
        return new Vector2(p.x + t * (q.x-p.x), p.y + t * (q.y - p.y));
    }

    void updateLocation(float x, float y)
    {
        if (x <= Screen.width / 2)
        {
            x = getX(x); y = getY(y);

            Debug.Log(x + " " + y);
            if (0 <= x && x <= 25 && 0 <= y && y <= 70) return;

            Vector2 p = new Vector2(x, y);
            Vector2 q = new Vector2(100, 100);

            if (compute(x, y) > 1)
            {
                float lo = 0, hi = 1;

                // binary search
                for (int i = 0; i < 50; i++)
                {
                    float mid = ((lo + hi)) / 2.0f;
                    Vector2 cur = translate(p, q, mid);
                    if (compute(cur.x, cur.y) > 1) lo = mid;
                    else hi = mid;
                }

                Vector3 touchDir = new Vector3(x, y, 0) - center.transform.position;
                float angle = Mathf.Atan2(touchDir.y, touchDir.x) * Mathf.Rad2Deg;
                Vector3 paddleDir = GetComponent<Rigidbody2D>().transform.position - center.transform.position;
                float angle2 = Mathf.Atan2(paddleDir.y, paddleDir.x) * Mathf.Rad2Deg;

                GetComponent<Rigidbody2D>().transform.RotateAround(GetComponent<Rigidbody2D>().position, Vector3.forward, 180 + (angle - angle2));

                GetComponent<Rigidbody2D>().position = translate(p, q, (lo + hi) / 2.0f);
            }
            else
            {
                // binary search
                float lo = 0, hi = 1;
                for (int i = 0; i < 50; i++)
                {
                    float mid = ((lo + hi)) / 2.0f;
                    Vector2 cur = translate(q, 1000.0f * (p - q) + q, mid);
                    if (compute(cur.x, cur.y) < 1) lo = mid;
                    else hi = mid;
                }

                Vector3 touchDir =  new Vector3(x, y, 0) - center.transform.position;
                float angle = Mathf.Atan2(touchDir.y, touchDir.x) * Mathf.Rad2Deg;
                Vector3 paddleDir = GetComponent<Rigidbody2D>().transform.position - center.transform.position;
                float angle2 = Mathf.Atan2(paddleDir.y, paddleDir.x) * Mathf.Rad2Deg;

                GetComponent<Rigidbody2D>().transform.RotateAround(GetComponent<Rigidbody2D>().position, Vector3.forward, 180 + (angle - angle2));

                GetComponent<Rigidbody2D>().position = translate(q, 1000.0f * (p - q) + q, (lo + hi) / 2.0f);

            }
        }
    }

    void FixedUpdate()
    {

        float x, y, dx, dy;
        // x and y are pixels

        bool touch = false;
        bool called = false;


            for (int i = 0; i < Input.touchCount; i++)
            {
                touch = true;
                x = Input.touches[i].position.x;
                y = Input.touches[i].position.y;

                if (x <= Screen.width / 2)
                {
                    updateLocation(x, y);
                    called = true;
                }
            }

            if (touch) return;

            x = Input.mousePosition.x;
            y = Input.mousePosition.y;
            dx = Input.GetAxis("Mouse X");
            dy = Input.GetAxis("Mouse Y");

            updateLocation(x, y);
        
    }
}
