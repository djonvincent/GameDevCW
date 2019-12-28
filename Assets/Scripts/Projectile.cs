using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
   void Start() {
      Destroy(gameObject, 5f);
   }

   void OnTriggerEnter2D(Collider2D other) {
       Destroy(gameObject);
   }
} 
