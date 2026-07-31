using UnityEngine;
using UnityEngine.SceneManagement;

public class TrajectoryLine : MonoBehaviour
{
    Scene simulationScene;
    PhysicsScene physicsScene;
    [SerializeField] Transform envrionmentParent;
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] int _maxPhysicsFrameIterations = 100;
    private void Start()
    {
      CreatePhysicsScene();  
    }
    void CreatePhysicsScene()
    {
        simulationScene = SceneManager.CreateScene("Simulation", new CreateSceneParameters(LocalPhysicsMode.Physics3D));
        physicsScene = simulationScene.GetPhysicsScene();

        foreach(Transform t in envrionmentParent)
        {
            var ghostEnviroObj = Instantiate(t.gameObject, t.transform.position,t.rotation);
            ghostEnviroObj.GetComponent<Renderer>().enabled = false;
            SceneManager.MoveGameObjectToScene(ghostEnviroObj, simulationScene);
        }
    }
    public void SimulateTrajectory(FauxBallController ball, Vector3 pos, Vector3 totalForce)
    {
        var ghostBallObj = Instantiate(ball, pos, Quaternion.identity);
        SceneManager.MoveGameObjectToScene(ghostBallObj.gameObject, simulationScene);

        ghostBallObj.Init(totalForce,true);

        lineRenderer.positionCount = _maxPhysicsFrameIterations;

        for (int i = 0; i < _maxPhysicsFrameIterations; i++)
        {
            physicsScene.Simulate(Time.fixedDeltaTime);
            lineRenderer.SetPosition(i, ghostBallObj.transform.position);
        }
        Destroy(ghostBallObj.gameObject);
    }
}
