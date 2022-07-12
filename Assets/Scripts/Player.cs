using Scripts;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

public class Player : MonoBehaviour
{
    private Plate Plate => Level.level.plate;
    private GUIHandler GUI => Level.level.gui;
    private Floor Floor => Level.level.floor;
    
    struct Timer
    {
        public float crrTime;
        public bool isEnabled;

        public void SetTimer(float time)
        {
            crrTime = time;
            isEnabled = true;
        }
        public void ProceedTimer()
        {
            if (isEnabled && !IsEnded()) crrTime -= Time.deltaTime;
        }

        public bool IsEnded()
        {
            return crrTime <= 0f && isEnabled;
        }
    }
    
    public float holdTime;
    
    private Timer timer;
    private Vector2 lastDeltaPosition;
    private int platesAmount;
    [SerializeField] private PlateTrimmer trimmer;

    public bool IsGameEnded => platesAmount == 0 || Floor.lastProgress == 100;
    
    private void Awake() => EnhancedTouchSupport.Enable();

    private void Start()
    {
        platesAmount = Level.level.preferences.platesAmount;
        GUI.resources.UpdatePlatesAmount(platesAmount);
        timer.SetTimer(holdTime);
    }

    private void OnEnable() => TouchSimulation.Enable();

    private void OnDisable() => TouchSimulation.Disable();

    void Update() => ProcessInput();

    private void ProcessInput()
    {
        if (IsGameEnded) return;
        
        Touch? touch = GetCurrentTouch();
        if (touch == null) return;

        switch (touch.Value.phase)
        {
            case TouchPhase.Began:
                EnterInputState();
                break;
            case TouchPhase.Moved:
                MoveInputState(touch.Value);
                break;
            case TouchPhase.Ended:
                FinishInputState();
                break;
        }
        
        lastDeltaPosition = touch.Value.screenPosition;
    }

    private void EnterInputState()
    {
        timer.ProceedTimer();
        
        if(timer.IsEnded()) HoldPlate();
    }
 
    private void MoveInputState(Touch touch)
    {
        timer.ProceedTimer();
        
        if(timer.IsEnded()) MovePlate(lastDeltaPosition - touch.screenPosition);
    }

    private void FinishInputState()
    {
        if(timer.IsEnded()) ReleasePlate();

        if (!timer.IsEnded()) DropPlate();
        
        timer.SetTimer(holdTime);
    }
   
    private void DropPlate()
    {
        Plate.Hide();
        
        trimmer.Set(Plate.PlateModel, Plate.PlatePosition);
        trimmer.PreparePlate();
        
        GUI.progress.SetPercentage(Floor.GetFillPercentage());

        DecreasePlatesStash();
    }

    private void DecreasePlatesStash()
    {
        platesAmount -= 1;
        GUI.resources.UpdatePlatesAmount(platesAmount);

        if (IsGameEnded)
        {
            GUI.levelComplete.Show(Floor.lastProgress);
        }
        else
        {
            Plate.ShowNext();
        }
    }

    private void ReleasePlate()
    {
        if (Plate.IsHolding) Plate.Release();
    }

    private void HoldPlate()
    {
        if (!Plate.IsHolding) Plate.Hold();
    }

    private void MovePlate(Vector3 pivotDelta)
    {
        HoldPlate();

        Plate.MovePivot(pivotDelta);
    }

    private static Touch? GetCurrentTouch()
    {
        if (Touch.activeFingers.Count == 0)
            return null;

        return Touch.activeFingers[0].currentTouch;
    }

    public void ResetPlayer()
    {
        platesAmount = Level.level.preferences.platesAmount;
        GUI.resources.UpdatePlatesAmount(platesAmount);
        timer.SetTimer(holdTime);
    }
}
