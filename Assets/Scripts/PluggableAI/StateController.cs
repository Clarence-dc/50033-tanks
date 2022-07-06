using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class StateController : MonoBehaviour {

	public State currentState;
	public EnemyStats enemyStats;
	public Transform eyes;
	public State remainState;

	[HideInInspector] public NavMeshAgent navMeshAgent;
	[HideInInspector] public TankShooting tankShooting;
	[HideInInspector] public List<Transform> wayPointList;
	[HideInInspector] public int nextWayPoint;
	[HideInInspector] public Transform chaseTarget;
	[HideInInspector] public float stateTimeElapsed;

	private bool aiActive;

	Material DefaultIconMaterial;
	Material DefaultChassisMaterial;
	Material DefaultTurretMaterial;
	Material DefaultTracksLeftMaterial;
	Material DefaultTracksRightMaterial;
	public Material ChaseIconMaterial;
	public Material TankChaseMaterial;

	bool flashing = false;
	void Awake () 
	{
		tankShooting = GetComponent<TankShooting> ();
		navMeshAgent = GetComponent<NavMeshAgent> ();
		// find all default materials
		DefaultIconMaterial = gameObject.transform.Find("MiniMapIcon").GetComponent<MeshRenderer>().material;
		DefaultChassisMaterial = gameObject.transform.Find("TankRenderers").Find("TankChassis").GetComponent<MeshRenderer>().material;
		DefaultTurretMaterial = gameObject.transform.Find("TankRenderers").Find("TankTurret").GetComponent<MeshRenderer>().material;
		DefaultTracksLeftMaterial = gameObject.transform.Find("TankRenderers").Find("TankTracksLeft").GetComponent<MeshRenderer>().material;
		DefaultTracksRightMaterial = gameObject.transform.Find("TankRenderers").Find("TankTracksRight").GetComponent<MeshRenderer>().material;
	}

	public void SetupAI(bool aiActivationFromTankManager, List<Transform> wayPointsFromTankManager)
	{
		wayPointList = wayPointsFromTankManager;
		aiActive = aiActivationFromTankManager;
		if (aiActive) 
		{
			navMeshAgent.enabled = true;
		} else 
		{
			navMeshAgent.enabled = false;
		}
	}

	public void TransitionToState(State nextState)
	{
		if (nextState == remainState) return;
		currentState = nextState;
		// if in chase state and not already in FlashIcon coroutine
		if((currentState.name == "ChaseScanner" || currentState.name == "ChaseChaser") && !flashing) {
			StartCoroutine(FlashIcon());
		};

		OnExitState();
	}

	public bool CheckIfCountDownElapsed(float duration)
	{
		stateTimeElapsed += Time.deltaTime;
		return stateTimeElapsed >= duration;
	}

	void Update()
	{
		if (!aiActive) return;

		currentState.UpdateState(this);
	}

	void OnExitState()
	{
		stateTimeElapsed = 0;
	}

	void OnDrawGizmos()
	{
		if (currentState != null && eyes != null)
		{
			Gizmos.color = currentState.sceneGizmoColor;
			Gizmos.DrawWireSphere(eyes.position, enemyStats.lookSphereCastRadius);
		}
	}

	IEnumerator FlashIcon(){
			flashing = true;
			// while in chase state, flash
			while ((currentState.name == "ChaseScanner" || currentState.name == "ChaseChaser")) {
				// replace all default materials with chase materials
				gameObject.transform.Find("MiniMapIcon").GetComponent<MeshRenderer>().material = ChaseIconMaterial;
				gameObject.transform.Find("TankRenderers").Find("TankChassis").GetComponent<MeshRenderer>().material = TankChaseMaterial;
				gameObject.transform.Find("TankRenderers").Find("TankTurret").GetComponent<MeshRenderer>().material = TankChaseMaterial;
				gameObject.transform.Find("TankRenderers").Find("TankTracksLeft").GetComponent<MeshRenderer>().material = TankChaseMaterial;
				gameObject.transform.Find("TankRenderers").Find("TankTracksRight").GetComponent<MeshRenderer>().material = TankChaseMaterial;
				yield return new WaitForSeconds(1.0f);
				// replace all chase materials with default materials
				gameObject.transform.Find("MiniMapIcon").GetComponent<MeshRenderer>().material = DefaultIconMaterial;
				gameObject.transform.Find("TankRenderers").Find("TankChassis").GetComponent<MeshRenderer>().material = DefaultChassisMaterial;
				gameObject.transform.Find("TankRenderers").Find("TankTurret").GetComponent<MeshRenderer>().material = DefaultTurretMaterial;
				gameObject.transform.Find("TankRenderers").Find("TankTracksLeft").GetComponent<MeshRenderer>().material = DefaultTracksLeftMaterial;
				gameObject.transform.Find("TankRenderers").Find("TankTracksRight").GetComponent<MeshRenderer>().material = DefaultTracksRightMaterial;
				yield return new WaitForSeconds(1.0f);
			}
			flashing = false;
	}

}