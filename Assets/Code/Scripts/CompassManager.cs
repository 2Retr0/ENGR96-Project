using System;
using System.Collections.Generic;
using Code.Scripts.Pickup;
using Code.Scripts.Player;
using UnityEngine;
using UnityEngine.Events;

namespace Code.Scripts
{
    public class CompassManager : Singleton<CompassManager>
    {
        [SerializeField] private Camera playerCamera;
        [SerializeField] private GameObject player;
        [SerializeField] private GameObject canvas;
        [SerializeField] private GameObject uiSprite;
        [SerializeField] private GameObject uiSpriteSpeed;
        [SerializeField] private float borderThickness;


        private readonly Dictionary<Vector3, GameObject> pickups = new();
        
        private void Start()
        {
            if (!player) player = FindObjectOfType<PlayerController>().gameObject;
            if (!playerCamera) playerCamera = Camera.main;
            if (!canvas) canvas = FindObjectOfType<Canvas>().gameObject;
        }

        private void FixedUpdate()
        {
            // Constrain viewport to the border thickness
            var thickness = new Vector2(borderThickness * (1f / playerCamera.aspect), borderThickness);
            var viewport = new Rect(thickness, new Vector2(1f, 1f) - 2 * thickness);
            var viewportCenter = new Vector2(0.5f, 0.5f);

            var closestDistance = float.PositiveInfinity;
            GameObject closestPickup = null;
            foreach (var (position, sprite) in pickups)
            {
                var viewportPos = playerCamera.WorldToViewportPoint(position);
                var viewportIntersection = LiangBarskyIntersection(viewportCenter, viewportPos, viewport);

                if ((viewportPos.x is > 0f and < 1.0f) && (viewportPos.y is > 0f and < 1.0f))
                    sprite.transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
                else
                    sprite.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);

                var newX = viewportIntersection.x * Screen.width;
                var newY = viewportIntersection.y * Screen.height;
                sprite.transform.position = new Vector3(newX, newY, sprite.transform.position.z);

                var distance = Vector3.Distance(player.transform.position, position);
                if (!(distance < closestDistance)) continue;

                closestDistance = distance;
                closestPickup = sprite;
            }

            closestPickup!.transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
        }

        public void AddPosition(Vector3 position)
        {
            var sprite = Instantiate(uiSprite, canvas.transform);

            pickups.Add(position, sprite);
        }

        public void RemovePosition(Vector3 position)
        {
            //pickups.Remove(position);
            //GameObject.Destroy(pickups[position]);
            pickups[position].gameObject.SetActive(false);
            GameObject.Destroy(pickups[position]);
            pickups.Remove(position);
        }
        
        public void AddPositionSpeed(Vector3 position)
        {
            var sprite = Instantiate(uiSpriteSpeed, canvas.transform);

            pickups.Add(position, sprite);
        }
        
        

        /**
         * Line-Rectangle Intersection Test
         */
        private static Vector2 LiangBarskyIntersection(Vector2 p1, Vector2 p2, Rect r)
        {
            var t = 1f;
            // Iterate over the sides of the rectangle.
            float dx = p2.x - p1.x, dy = p2.y - p1.y;
            for (var i = 0; i < 4; i++)
            {
                float p = 0f, q = 0f;
                switch (i)
                {
                    case 0: p = -dx; q = p1.x - r.xMin; break; // Left side.
                    case 1: p =  dx; q = r.xMax - p1.x; break; // Bottom side.
                    case 2: p = -dy; q = p1.x - r.yMin; break; // Right side.
                    case 3: p =  dy; q = r.yMax - p1.x; break; // Top side.
                }

                var u = q / p;
                if (p > 0 && u < t) t = u;
            }

            return new Vector2(p1.x + t * dx, p1.x + t * dy);
        }
    }
}
