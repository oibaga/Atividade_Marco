using UnityEngine;

public class Object_Interact : MonoBehaviour
{
    [SerializeField] private Transform objToInspect;
    [SerializeField] private GameObject cameraOnTop;
    [SerializeField] private float rotationSpeed = 100f;

    private GameObject currentInspectInstance;

    public void StartInspection(GameObject newInspectInstance)
    {
        SpawnNewObjectToInspect(newInspectInstance);

        FindFirstObjectByType<PlayerMoviment>().BlockMoviment();

        cameraOnTop.SetActive(true);
    }

    public void StopInspection()
    {
        cameraOnTop.SetActive(false);

        FindFirstObjectByType<PlayerMoviment>().UnlockMoviment();

        ResetTransform(currentInspectInstance);

        Destroy(currentInspectInstance.gameObject);

        currentInspectInstance = null;
    }

    private void Update()
    {
        if (objToInspect == null) return;

        if ( FindFirstObjectByType<PlayerMoviment>().isInspecting )
        {
            float horizontal = 0f;
            float vertical = 0f;

            if (Input.GetKey(KeyCode.A)) horizontal = -1f;
            if (Input.GetKey(KeyCode.D)) horizontal = 1f;
            if (Input.GetKey(KeyCode.W)) vertical = 1f;
            if (Input.GetKey(KeyCode.S)) vertical = -1f;

            float rotationX = vertical * rotationSpeed * Time.deltaTime;
            float rotationY = horizontal * rotationSpeed * Time.deltaTime;

            Quaternion rotation = Quaternion.Euler(rotationX, 0, -rotationY);
            currentInspectInstance.transform.rotation *= rotation;
        }
    }

    private void SpawnNewObjectToInspect(GameObject newObject)
    {
        if (currentInspectInstance) Destroy( currentInspectInstance );
        currentInspectInstance = Instantiate( newObject );

        currentInspectInstance.transform.SetParent( objToInspect );

        ResetTransform( currentInspectInstance );

        currentInspectInstance.SetActive( true );
    }

    private void ResetTransform(GameObject inObject)
    {
        if ( inObject )
        {
            inObject.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            inObject.transform.localScale = Vector3.one;
        }
    }
}
