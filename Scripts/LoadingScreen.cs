using UnityEngine;

public class LoadingScreen : MonoBehaviour
{

    public GameObject loadingCircle;

    public void Start()
    {
        GameManager.instance.WaterIncreased += close;
    }

    private void close(object o, float f)
    {
        GameManager.instance.WaterIncreased -= close;
        Destroy(this.gameObject);
    }

    private void FixedUpdate()
    {
        loadingCircle.transform.Rotate(0,0,-360/4*Time.deltaTime);
    }

}
