using System;
using UnityEngine;

namespace SnowmansFolly.MonoBehaviours.Effects
{

    public sealed class ParallaxBackground : MonoBehaviour
    {
        [SerializeField] private Camera drivingCamera = default;

        [SerializeField] private float parallaxLoopWidth = 1f;

        [SerializeField] private float parallaxFloor = 0f;

        [SerializeField] private ParallaxLayer[] layers = default;

        [Serializable]
        private sealed class ParallaxLayer
        {
            public Transform imageTransform = default;
            public Vector2 scrollFactor = Vector2.one;
            public bool loopsVertically = false;
        }

        private void OnValidate()
        {
            if (parallaxLoopWidth < 0.005f)
                parallaxLoopWidth = 0.005f;
        }

        private void Update()
        {
            Vector2 scroll = drivingCamera.ViewportToWorldPoint(new Vector2(0f, 0f));
            if (scroll.y < parallaxFloor)
                scroll.y = parallaxFloor;

            foreach (ParallaxLayer layer in layers)
            {
                Vector2 localScroll = new Vector2
                {
                    x = (scroll.x * layer.scrollFactor.x).Wrapped(scroll.x - parallaxLoopWidth, scroll.x),
                    y = scroll.y * layer.scrollFactor.y
                };

                if (layer.loopsVertically)
                    localScroll.y = localScroll.y.Wrapped(scroll.y - parallaxLoopWidth, scroll.y);

                layer.imageTransform.position = localScroll;
            }
        }
    }
}
