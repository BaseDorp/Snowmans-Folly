using System;
using UnityEngine;

public sealed class ParallaxBackground : MonoBehaviour
{
    [SerializeField] private Camera targetCamera = null;

    [SerializeField] private float parallaxLoopWidth = 1f;

    [SerializeField] private float parallaxFloor = 0f;

    [SerializeField] private ParallaxLayer[] layers = null;

    [Serializable]
    private sealed class ParallaxLayer
    {
        public Transform imageTransform = null;
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
        Vector2 scroll = targetCamera.ViewportToWorldPoint(new Vector2(0f, 0f));
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

            //while (localScroll.x > scroll.x)
            //    localScroll.x -= parallaxLoopWidth;
            //while (localScroll.x < scroll.x - parallaxLoopWidth)
            //    localScroll.x += parallaxLoopWidth;

            layer.imageTransform.position = localScroll;
        }
    }
}
