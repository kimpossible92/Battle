using UnityEngine;

public class InputManager : MonoBehaviour
{
    public string PlayerID = "Player1";
    protected Vector3 _primaryMovement = Vector3.zero;
    protected Vector3 _secondaryMovement = Vector3.zero;
    public bool SmoothMovement = true;
    protected string _axisHorizontal;
    protected string _axisVertical;
    protected string _axisSecondaryHorizontal;
    protected string _axisSecondaryVertical;
    protected string _axisShoot;
    protected string _axisShootSecondary;
    public Vector3 PrimaryMovement { get { return _primaryMovement; } }
    private void Initialize()
    {
        _axisHorizontal = PlayerID + "_Horizontal";
        _axisVertical = PlayerID + "_Vertical";
        _axisSecondaryHorizontal = PlayerID + "_SecondaryHorizontal";
        _axisSecondaryVertical = PlayerID + "_SecondaryVertical";
        _axisShoot = PlayerID + "_ShootAxis";
        _axisShootSecondary = PlayerID + "_SecondaryShootAxis";
    }
    private void Awake()
    {
        Initialize();
    }
    private void Update()
    {
        transform.rotation = Quaternion.identity;
        if (SmoothMovement)
        {
            _primaryMovement.x = Input.GetAxis(_axisHorizontal);
            _primaryMovement.y = Input.GetAxis(_axisVertical);
        }
        else
        {
            _primaryMovement.x = Input.GetAxisRaw(_axisHorizontal);
            _primaryMovement.y = Input.GetAxisRaw(_axisVertical);
        }
    }
public static Vector2 getAxisVector()
        {
            return new Vector2(getHorInput(), getVerInput());
        }

        public static float getHorInput()
        {
            return Input.GetAxis("Horizontal");
        }

        public static float getVerInput()
        {
            return Input.GetAxis("Vertical");
        }

        //public static float getMagnitude()
        //{
            //return CommonMethods.CalculateMagnitude(getHorInput(), getVerInput());
        //}
}