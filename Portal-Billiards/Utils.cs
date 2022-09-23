using Microsoft.Xna.Framework;

namespace Portal_Billiards;

public static class Utils
{
    public static Rectangle SetFromTwoPoints(this Rectangle rect, Vector2 v1, Vector2 v2)
    {
        if (v1.X < v2.X)
        {
            rect.X = (int)v1.X;
            rect.Width = (int)(v2.X - v1.X);
        }
        else
        {
            rect.X = (int)v2.X;
            rect.Width = (int)(v1.X - v2.X);
        }

        if (v1.Y < v2.Y)
        {
            rect.Y = (int)v1.Y;
            rect.Height = (int)(v2.Y - v1.Y);
        }
        else
        {
            rect.Y = (int)v2.Y;
            rect.Height = (int)(v1.Y - v2.Y);
        }

        return rect;
    }
    
    public static Rectangle SetFromTwoPointsAndInflate(this Rectangle rect, Vector2 v1, Vector2 v2, int inflate)
    {
        if (v1.X < v2.X)
        {
            rect.X = (int)v1.X + inflate;
            rect.Width = (int)(v2.X - v1.X) + inflate;
        }
        else
        {
            rect.X = (int)v2.X + inflate;
            rect.Width = (int)(v1.X - v2.X) + inflate;
        }

        if (v1.Y < v2.Y)
        {
            rect.Y = (int)v1.Y + inflate;
            rect.Height = (int)(v2.Y - v1.Y) + inflate;
        }
        else
        {
            rect.Y = (int)v2.Y + inflate;
            rect.Height = (int)(v1.Y - v2.Y) + inflate;
        }

        return rect;
    }
}