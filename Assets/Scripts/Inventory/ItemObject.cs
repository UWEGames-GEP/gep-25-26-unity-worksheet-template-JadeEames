using System;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.Rendering.Universal;

namespace InventorySystem
{
    [RequireComponent(typeof(Rigidbody))]
    public class ItemObject : MonoBehaviour
    {
        [SerializeField] private InventoryItem item;
        public int count = 1;
        private Rigidbody rb;

        private bool pick_up_target_inrange = false;
        private Inventory pick_up_target_ref = null;
        private Transform target_transform = null;

        [SerializeField] private float pull_force = 10f;
        [SerializeField] private float min_distance = 0.5f;
        [SerializeField] private float max_force = 50f;
        [SerializeField] private float magnatise_delay = 2f;
        private float delay_timer = 0;
        
        public void setItemType(InventoryItem type)
        {
            item = type;

            if (rb == null)
            {
                rb = GetComponent<Rigidbody>();
            }

            rb.mass = item.weight;
        }

        private void Start()
        {
            rb = GetComponent<Rigidbody>();
            if (item != null)
            {
                rb.mass = item.weight;
            }
        }

        private void FixedUpdate()
        {
            if (pick_up_target_inrange)
            {
                delay_timer -= Time.deltaTime;

                if (delay_timer <= 0)
                {
                    MagnatiseTowardsTarget();
                }
            }
        }

        private void MagnatiseTowardsTarget()
        {
            Vector3 direction = target_transform.position - transform.position;
            float distance = direction.magnitude;

            if (distance < min_distance) return;

            direction.Normalize();
            float force = pull_force * (1f / Mathf.Max(distance, 0.1f));
            force = Mathf.Min(force, max_force);

            rb.AddForce(direction * force, ForceMode.Acceleration);
        }

        private void OnTriggerEnter(Collider collider)
        {
            if (collider.CompareTag("ItemMagnatiseRadius"))
            {
                pick_up_target_ref = collider.GetComponentInParent<Inventory>();
                if (pick_up_target_ref.CanPickupItem(item))
                {
                    pick_up_target_inrange = true;
                    target_transform = collider.transform;
                    delay_timer = magnatise_delay;
                }
            }

            if (collider.CompareTag("ItemMagnatiseTarget") && pick_up_target_ref.CanPickupItem(item))
            {
                int added = pick_up_target_ref.AddItems(item, count);
                count -= added;

                if (count <= 0)
                {
                    rb.isKinematic = true;
                    GetComponent<Collider>().enabled = false;
                    Destroy(gameObject);
                }
            }
        }

        private void OnTriggerExit(Collider collider)
        {
            if (collider.CompareTag("ItemMagnatiseRadius"))
            {
                pick_up_target_inrange = false;
                delay_timer = 0;
            }
        }
    }
}