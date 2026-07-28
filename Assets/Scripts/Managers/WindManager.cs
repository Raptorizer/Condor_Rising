using UnityEngine;

public class WindManager : MonoBehaviour
{
    public int windPower;
    public int windRandomPower;
    //wind direction is -1/1 respective
    //X = Left/Right
    //Y = Up/Down
    //Z = Forward/Back
    public Vector3 windDirection;
    public Vector3 windRandomDirection; 

    public bool isWindy;
    public bool isRandWindy;

    public static WindManager instance { get; private set; } = null;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Debug.LogError($"Found Duplicate Wind Manager on {gameObject.name}");
            Destroy(gameObject);
            return;
        }
        instance = this;
    }
    void Start()
    {
        SetWindValues();
        if (windPower != 0 | windRandomPower != 0) isWindy = true;
        else isWindy = false;
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
        windRandomDirection = new Vector3(Random.Range(-0.9f, 0.9f), Random.Range(-0.9f, 0.9f), Random.Range(-0.9f, 0.9f));
    }
}
