using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] List<TextMeshProUGUI> debugVariables;
    BallController ballController;
    WindManager windManager;
    public static UIManager instance { get; private set; } = null;
    void Awake()
    {
        if (instance != null && instance != this)
        {
            Debug.LogError($"Found Duplicate UIManager on {gameObject.name}");
            Destroy(gameObject);
            return;
        }
        instance = this;
    }
    void Start()
    {
        if (windManager == null) windManager = WindManager.instance;
        if (ballController == null) ballController = BallController.instance;
    }

     void Update()
    {
        debugVariables[0].text = $"Ball Starting Position: {ballController.startingPos}";
        debugVariables[1].text = $"Ball current position: {ballController.currentPos}";
        debugVariables[2].text = $"Ball final position: {ballController.finalPos}";
        debugVariables[3].text = $"Ball has been hit?: {ballController.isHit}";
        debugVariables[4].text = $"Ball is moving?: {ballController.isMoving}";
        debugVariables[18].text = $"Ball is grounded?: {ballController.isGrounded}";
        debugVariables[17].text = $"Ball speed: {ballController.ballSpeedMagnitude}";
        debugVariables[5].text = $"Hit strength: {ballController.hitStrength}";
        debugVariables[6].text = $"Hit height: {ballController.hitHeight}";
        debugVariables[7].text = $"Hit count: {ballController.hitCount}";
        debugVariables[8].text = $"Hit spin strength: {ballController.spinPower}";
        debugVariables[9].text = $"Hit spin direction {ballController.spinDirection}";
        debugVariables[10].text = $"Ground type: {ballController.groundValue}";
        debugVariables[11].text = $"Wind strength: {windManager.windPower}";
        debugVariables[12].text = $"Wind direction: {windManager.windDirection}";
        debugVariables[13].text = $"Wind random strength {windManager.windRandomPower}";
        debugVariables[14].text = $"Wind random direction {windManager.windRandomDirection}";
        debugVariables[15].text = $"Is it Windy?: {windManager.isWindy}";
        debugVariables[16].text = $"Random wind?: {windManager.isRandWindy}";

        foreach(TextMeshProUGUI t in debugVariables)
        {
            //if no text is found, display error text
            t.text ??= "Error Displaying Text. Please Check the UI Manager script or Object!";
        }
    }
}
