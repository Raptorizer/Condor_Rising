using UnityEngine;

public class WindManager : MonoBehaviour
{
    public int windPower;
    public int windRandomPower;
    public Vector3 windDirection;
    public Vector3 windRandomDirection; 

    public bool isWindy;
    public bool isRandWindy;

    public static WindManager instance { get; private set; } = null;

     void Awake()
    {
        if(instance != null)
        {
            Debug.LogError($"Found Duplicate WindManager on {gameObject.name}");
            Destroy(gameObject);
            return;
        }
    }
     void Start()
    {
        SetWindValues();
    }

    private void Update()
    {
        if (windPower != 0 | windRandomPower != 0) isWindy = true;
        else isWindy = false;

        if (Input.GetKeyDown(KeyCode.R)) SetWindValues();

    }

    void SetWindValues()
    {
        windRandomPower = Random.Range(0, 9);
        windRandomDirection = new Vector3(Random.Range(-9, 9), Random.Range(-9, 9), Random.Range(-9, 9));
    }
}
