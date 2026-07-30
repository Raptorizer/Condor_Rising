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
            var ghostObj = Instantiate(t.gameObject, t.transform.position,t.rotation);
            ghostObj.GetComponent<Renderer>().enabled = false;
            SceneManager.MoveGameObjectToScene(ghostObj, simulationScene);
        }
    }
    public void SimulateTrajectory(BallController ball, Vector3 pos, Vector3 totalForce)
    {
        var ghostObj = Instantiate(ball, pos, Quaternion.identity);
        SceneManager.MoveGameObjectToScene(ghostObj.gameObject, simulationScene);

        ghostObj.Init(totalForce,true);

        lineRenderer.positionCount = _maxPhysicsFrameIterations;

        for (int i = 0; i < _maxPhysicsFrameIterations; i++)
        {
            physicsScene.Simulate(Time.fixedDeltaTime);
            lineRenderer.SetPosition(i, ghostObj.transform.position);
        }
        Destroy(ghostObj.gameObject);
    }
}
