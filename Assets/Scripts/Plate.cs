using Parabox.CSG;
using UnityEngine;
using Random = UnityEngine.Random;

public class Plate : MonoBehaviour
{
    private enum Axis { X, Z }
    private enum HoldState { Holded, Realesed }

    public GameObject platePrefab;
    public Vector2 movingRangeX;
    public Vector2 movingRangeZ;
    public Vector2 pivotRangeX;
    public Vector2 pivotRangeZ;
    
    private float speed => Level.level.preferences.plateSpeed;
    private Vector3 moveDirection;
    private HoldState holdState = HoldState.Realesed;
    private Axis crrAxis;
    private Vector3 pivotPosition;
    private readonly float pivotHeight = 3f;

    public Model PlateModel => new Model(platePrefab);

    public Vector3 PlatePosition => transform.position;
    
    public bool IsHolding => holdState == HoldState.Holded;

    private void Start()
    {
        platePrefab = Instantiate(platePrefab, platePrefab.transform.position, Quaternion.identity, transform);
        
        SetAxis((Axis)Random.Range(0,2));
    }

    private void Update() => Move();

    private void Move()
    {
        if (IsHolding) return;

        Vector3 target = transform.position + moveDirection * speed;
        
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

        if (transform.position.x >= movingRangeX.y || transform.position.x <= movingRangeX.x) moveDirection *= -1;
        else if (transform.position.z >= movingRangeZ.y || transform.position.z <= movingRangeZ.x) moveDirection *= -1;
    }

    public void Hold() => holdState = HoldState.Holded;

    public void Release() => holdState = HoldState.Realesed;

    public void Hide()
    {
        Hold();
        platePrefab.SetActive(false);
    }

    public void ShowNext()
    {
        SetAxis((Axis)Random.Range(0,2));
        platePrefab.SetActive(true);
        
        Release();
    }
    
    private void SetAxis(Axis axis)
    {
        crrAxis = axis;
        
        if (crrAxis == Axis.X)
            pivotPosition = new Vector3(
                Random.Range(pivotRangeX.x, pivotRangeX.y),
                pivotHeight,
                pivotRangeZ.x);
        else
            pivotPosition = new Vector3(
                pivotRangeX.x,
                pivotHeight,
                Random.Range(pivotRangeZ.x, pivotRangeZ.y));

        transform.position = pivotPosition;
        moveDirection = (crrAxis == Axis.X) ? Vector3.forward : Vector3.right;
    }

    private void SetPivot(Vector3 platePivotPosition, Axis axis)
    {
        Vector3 position = transform.position;

        if (axis == Axis.X)
        {
            int celled = (int) (platePivotPosition.x / 0.5f);
            position.x = celled * 0.5f;
        }
        else
        {
            int celled = (int) (platePivotPosition.z / 0.5f);
            position.z = celled * 0.5f;
        }

        transform.position = position;
    }
    
    public void MovePivot(Vector2 pivotDelta)
    {
        if (pivotDelta == Vector2.zero) return;

        float translateDelta = ((crrAxis == Axis.X) ? pivotDelta.x : pivotDelta.y) * 0.1f;
        float baseCoordinate = (crrAxis == Axis.X) ? pivotPosition.x : pivotPosition.z;
        Vector2 baseAxisBorders = (crrAxis == Axis.X) ? pivotRangeX : pivotRangeZ;

        baseCoordinate += translateDelta;
        baseCoordinate = Mathf.Clamp(baseCoordinate, baseAxisBorders.x, baseAxisBorders.y);

        if (crrAxis == Axis.X)
            pivotPosition.x = baseCoordinate;
        else
            pivotPosition.z = baseCoordinate;

        SetPivot(pivotPosition, crrAxis);
    }
    /*private void OnDrawGizmosSelected()
    {

        {
            // Draw moveAxisBorder box
            Gizmos.color = Color.black;
            Vector3 center = new Vector3(
                (movingRangeX.y + movingRangeX.x) / 2f,
                pivotHeight,
                (movingRangeZ.y + movingRangeZ.x) / 2f);

            Vector3 sizes = new Vector3(
                Mathf.Abs(movingRangeX.y) + Mathf.Abs(movingRangeX.x),
                0.25f,
                Mathf.Abs(movingRangeZ.y) + Mathf.Abs(movingRangeZ.x));

            Gizmos.DrawWireCube(center, sizes);
        }

        {
            // Draw axisBorder box
            Gizmos.color = Color.white;
            Vector3 center = new Vector3(
                (pivotRangeX.y + pivotRangeX.x) / 2f,
                pivotHeight,
                (pivotRangeZ.y + pivotRangeZ.x) / 2f);

            Vector3 sizes = new Vector3(
                Mathf.Abs(pivotRangeX.y) + Mathf.Abs(pivotRangeX.x),
                0.25f,
                Mathf.Abs(pivotRangeZ.y) + Mathf.Abs(pivotRangeZ.x));

            Gizmos.DrawWireCube(center, sizes);
        }
    }*/
}
