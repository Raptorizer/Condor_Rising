using UnityEngine;
using UnityEngine.SceneManagement;

public class TrajectoryLine : MonoBehaviour
{
    Scene simulationScene;
    PhysicsScene physicsScene;
    [SerializeField] Transform envrionmentParent;
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] int _maxPhysicsFrameIterations = 100;

    [Header("Prefabs")]
    [SerializeField] GameObject ghostBallPrefab; // Changed to accept a standard GameObject

    private GameObject ghostBallInstance;
    private Rigidbody ghostRb;

    private void Start()
    {
        CreatePhysicsScene();
    }

    void CreatePhysicsScene()
    {
        simulationScene = SceneManager.CreateScene("Simulation", new CreateSceneParameters(LocalPhysicsMode.Physics3D));
        physicsScene = simulationScene.GetPhysicsScene();

        foreach (Transform t in envrionmentParent)
        {
            var ghostEnviroObj = Instantiate(t.gameObject, t.transform.position, t.rotation);
            ghostEnviroObj.GetComponent<Renderer>().enabled = false;
            SceneManager.MoveGameObjectToScene(ghostEnviroObj, simulationScene);
        }

        // Instantiate standard GameObject and cache its Rigidbody
        ghostBallInstance = Instantiate(ghostBallPrefab);
        SceneManager.MoveGameObjectToScene(ghostBallInstance, simulationScene);

        ghostRb = ghostBallInstance.GetComponent<Rigidbody>();
        ghostBallInstance.SetActive(false);
    }

    public void SimulateTrajectory(Vector3 pos, Vector3 totalForce)
    {
        ghostBallInstance.SetActive(true);
        ghostBallInstance.transform.position = pos;

        ghostRb.linearVelocity = Vector3.zero;
        ghostRb.angularVelocity = Vector3.zero;

        // Apply the force directly to the Rigidbody component
        ghostRb.AddForce(totalForce);

        lineRenderer.positionCount = _maxPhysicsFrameIterations;

        for (int i = 0; i < _maxPhysicsFrameIterations; i++)
        {
            physicsScene.Simulate(Time.fixedDeltaTime);
            lineRenderer.SetPosition(i, ghostBallInstance.transform.position);
        }

        ghostBallInstance.SetActive(false);
    }
}