using System;
using System.Collections.Generic;
using System.Linq;

namespace func_rocket;

//То что получилось считается грязным решением или +- норм?

/*Я в метод GetLevel где поменьше кода для вычисления gravity передаю эту gravity в виде лямбды,
 где побольше - создаю отдельный метод. Это норм, или нужно единообразие?*/

//Почему задача противная? :D
public static class LevelsTask
{
    private static readonly Physics StandardPhysics = new();

    private static readonly Rocket StandardRocket = new Rocket(new Vector(200, 500),
        Vector.Zero, -0.5 * Math.PI);

    private static readonly Vector StandardTarget = new Vector(600, 200);
    private static readonly Vector LevelTarget = new Vector(700, 500);

    private static readonly Dictionary<string, Level> Levels = new()
    {
        {
            "Zero", GetLevel("Zero", null, null,
                (_, _) => Vector.Zero, null)
        },
        {
            "Heavy", GetLevel("Heavy", null, null,
                (_, _) => CreateGravityVector(0.9, Math.PI / 2), null)
        },
        {
            "Up", GetLevel("Up", null, LevelTarget,
                GetGravityForUpLevel(), null)
        },
        {
            "WhiteHole", GetLevel("WhiteHole", null, null,
                GetGravityForWhiteLevel(),
                null)
        },

        {
            "BlackHole", GetLevel("BlackHole", null, null,
                GetGravityForBlackLevel(),
                null)
        },

        {
            "BlackAndWhite", GetLevel("BlackAndWhite", null, null,
                GetGravityForBlackAndWhiteLevel(),
                null)
        },
    };

    public static IEnumerable<Level> CreateLevels()
    {
        return Levels.Select(level => level.Value);
    }

    private static Vector CreateGravityVector(double gravityForceValue,
        double directionAngleInRad)
    {
        return new Vector(gravityForceValue, 0).Rotate(directionAngleInRad);
    }

    private static Level GetLevel(string name, Rocket rocket, Vector target, Gravity gravity, Physics physics)
    {
        rocket ??= StandardRocket;
        target ??= StandardTarget;
        physics ??= StandardPhysics;
        return new Level(name, rocket, target,
            gravity, physics);
    }

    private static Gravity GetGravityForUpLevel()
    {
        return (sizeOfRoom, positionOfRocket) =>
        {
            var distanceFromBottom = sizeOfRoom.Y - positionOfRocket.Y;
            var length = 300 / (distanceFromBottom + 300.0);
            return CreateGravityVector(length, -Math.PI / 2);
        };
    }

    private static Gravity GetGravityForWhiteLevel()
    {
        return (_, positionOfRocket) =>
        {
            var targetDistance = positionOfRocket - StandardTarget;
            var length = 140 * targetDistance.Length / (targetDistance.Length * targetDistance.Length + 1);
            return CreateGravityVector(length, targetDistance.Angle);
        };
    }

    private static Gravity GetGravityForBlackLevel()
    {
        return (_, positionOfRocket) =>
        {
            var anomaly = (StandardRocket.Location + StandardTarget) / 2;
            var distanceToAnomaly = (positionOfRocket - anomaly).Length;
            var length = 300 * distanceToAnomaly / (distanceToAnomaly * distanceToAnomaly + 1);
            var angle = Math.PI + (positionOfRocket - anomaly).Angle;
            return CreateGravityVector(length, angle);
        };
    }
    
    private static Gravity GetGravityForBlackAndWhiteLevel()
    {
        return (sizeOfRoom, positionOfRocket) =>
        {
            var blackHoleVector = GetGravityForBlackLevel()(sizeOfRoom,
                positionOfRocket);
            var whiteHoleVector = GetGravityForWhiteLevel()(sizeOfRoom,
                positionOfRocket);
            return (blackHoleVector + whiteHoleVector) / 2;
        };
    }
}