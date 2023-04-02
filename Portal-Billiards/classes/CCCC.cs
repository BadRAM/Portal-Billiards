using System;

namespace Portal_Billiards;

public static class CCCC // Circle Circle Collision Check utility functions
{
    // returns the time in s from present that the two balls passed will collide
    public static float Balls(Ball ball1, Ball ball2)
    {
                
        // do the cool maths
        float dx = ball1.Position.X - ball2.Position.X;
        float dy = ball1.Position.Y - ball2.Position.Y;
        float vx = ball1.Velocity.X - ball2.Velocity.X;
        float vy = ball1.Velocity.Y - ball2.Velocity.Y;
        int D = (int)(ball1.Size + ball2.Size);
        float predictionStatus = MathF.Pow(dx * vx + dy * vy, 2) - (vx*vx + vy*vy)*(-D*D + dx*dx + dy*dy);
        float predictedCollision;

        if (predictionStatus >= 0)
        {
            predictedCollision = (-(dx * vx + dy * vy) - MathF.Sqrt(predictionStatus)) / (vx * vx + vy * vy);
            return predictedCollision;
        }
        else
        {
            return -1;
        }

    }
}