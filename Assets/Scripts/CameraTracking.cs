using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTracking : MonoBehaviour
{
    public Transform Player;

    Vector3 cameraOffset;

    // Start is called before the first frame update
    void Start()
    {
        cameraOffset = transform.position - Player.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Player.position + cameraOffset;
    }
}
