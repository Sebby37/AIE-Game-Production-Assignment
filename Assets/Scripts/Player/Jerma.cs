using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Written by Sebastian Cramond with advice from... him.
public class Jerma : MonoBehaviour
{
    [Header("Jerma")]
    public Sprite jerma;
    public Vector2 jermaSize;

    bool jermad = false;

    // FixedUpdate is called before Physics Updates
    private void FixedUpdate()
    {
        // Jerming if jerma'd
        if (jermad)
            Jerm();
    }

    // Update is called once per frame
    void Update()
    {
        // Jermaing if J is pressed
        if (Input.GetKeyDown(KeyCode.J))
            jermad = true;

        // Jerming if jerma'd
        if (jermad)
            Jerm();
    }

    // LateUpdate is called... later?
    private void LateUpdate()
    {
        // Jerming if jerma'd
        if (jermad)
            Jerm();
    }

    // I love it when Jerma said it's Jermin' time and Jerma'd all over The Skeld
    void Jerm()
    {
        // Getting IMPURE GameObjects
        List<GameObject> gameObjects = new List<GameObject>(GameObject.FindObjectsOfType<GameObject>());

        // Purifying the GameObjects
        foreach (GameObject impureGameObject in gameObjects)
        {
            // Continuing if the impure object is null
            if (impureGameObject == null)
                continue;
            
            // Getting the impure renderer
            SpriteRenderer renderer = impureGameObject.GetComponent<SpriteRenderer>();

            // If the renderer is null, we give it an impure renderer
            if (renderer == null)
                renderer = impureGameObject.AddComponent<SpriteRenderer>();

            // Purifying the renderer
            renderer.sprite = jerma;
            renderer.drawMode = SpriteDrawMode.Sliced;
            renderer.size = jermaSize;
        }
    }
}
