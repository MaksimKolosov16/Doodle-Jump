namespace func_rocket;

public class ControlTask
{
    private const double CoefficientForRocketDirection = 1 / 3.0;
    private const double CoefficientForVelocityDirection = 2 / 3.0;
    public static Turn ControlRocket(Rocket rocket, Vector target)
    {

        var directionToTarget = target - rocket.Location;
        var directionOfRocketMoving = CoefficientForRocketDirection * rocket.Direction + 
                                CoefficientForVelocityDirection * rocket.Velocity.Angle;
        return directionOfRocketMoving < directionToTarget.Angle 
            ? Turn.Right 
            : Turn.Left;
    }
}