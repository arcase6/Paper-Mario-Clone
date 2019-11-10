using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollMenuInput : MonoBehaviour
{
    private ScrollMenu ScrollMenuRef;
    // Start is called before the first frame update
    void Start()
    {
        this.ScrollMenuRef = GetComponent<ScrollMenu>();
    }

    // Update is called once per frame
    void Update()
    {
        ScrollMenuRef.ProcessInput();
    }
}
