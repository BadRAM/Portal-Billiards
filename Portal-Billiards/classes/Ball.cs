using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Specialized;
using FontStashSharp;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using StbImageSharp;

namespace Portal_Billiards;

public class Ball
{
    public Vector2 Position;
    public Vector2 Velocity = Vector2.Zero;
    //public Color BallColor;
    public float Size = 11; // pixel radius of ball
    public float Mass = 10;
    public float Elasticity = 0.95f; // on a scale of 0-1, how much energy is conserved when this ball bounces?
    public int Number = 0; // Number on the ball
    private List<Color> Colors = new List<Color>() 
    {
        Color.White, // cue ball 
        Color.Yellow, Color.Blue, Color.Red, Color.Purple, Color.Orange, Color.Green, Color.Maroon, // solids
        Color.Black, // 8 ball
        Color.Yellow, Color.Blue, Color.Red, Color.Purple, Color.Orange, Color.Green, Color.Maroon // stripes
    };

    public Ball(Vector2 position, Vector2 velocity, Color color = new Color(), float size = 16, float mass = 16, float elasticity = 0.95f)
    {
        Position = position;
        Velocity = velocity;
        //BallColor = color;
        Size = size;
        Mass = mass;
        Elasticity = elasticity;
    }

    public Ball(Vector2 position, int number)
    {
        Position = position;
        //BallColor = color;
        Number = number;
    }

    public void Update(List<Ball> toCollide, int index)
    {
        // check for collision with walls
        if (Position.X < 100 + Size)
        {
            Velocity.X = -Velocity.X;
            Position.X += -Position.X + 100 + Size;
        }
        else if (Position.X > 900 - Size)
        {
            Position.X -= Position.X - (900 - Size);
            Velocity.X = -Velocity.X;
        }
        if (Position.Y < 100 + Size)
        {
            Velocity.Y = -Velocity.Y;
            Position.Y += -Position.Y + 100 + Size;
        }
        else if (Position.Y > 500 - Size)
        {
            Position.Y -= Position.Y - (500 - Size);
            Velocity.Y = -Velocity.Y;
        }

        
        // drag
        if (Velocity.Length() < 0.05f)
        {
            Velocity = Vector2.Zero;
        }
        else
        {
            Velocity = Velocity.NormalizedCopy() * (Velocity.Length() - 0.01f);
            Velocity *= 0.99f;
        }
    }

    public bool CollideWith(Ball target)
    {
        // // end now if both balls are stationary
        // if (Velocity == Vector2.Zero && target.Velocity == Vector2.Zero) { return false; }
        // end now if not in collision range
        float overlap = (Size + target.Size) - Vector2.Distance(Position, target.Position);
        if (overlap <= 0 ) { return false; }

        // Displace balls so they no longer overlap
        Vector2 norm = Vector2.Normalize(Position - target.Position);
        Position += norm * (overlap / 2);
        target.Position += -norm * (overlap / 2);
        
        // take average elasticity of members
        float elast = (Elasticity + target.Elasticity) / 2;
        
        // handle collision
        Vector2 i1 = target.Velocity - Velocity;
        Vector2 i2 = Velocity - target.Velocity;
        i1 = i1.ProjectOnto(Vector2.Normalize(Position - target.Position));
        i2 = i2.ProjectOnto(Vector2.Normalize(target.Position - Position));
        Velocity        += (1+elast) * i1 * (target.Mass / (Mass + target.Mass));
        target.Velocity += (1+elast) * i2 * (Mass        / (Mass + target.Mass));
        
        return true;
    }

    public void LateUpdate()
    {
        Position += Velocity;
    }

    public void Draw(SpriteBatch spriteBatch, FontSystem fontSystem)
    {
        spriteBatch.DrawCircle(Position.X, Position.Y, Size, 32, Colors[Number], Size);

        if (Number > 8)
        {
            spriteBatch.DrawCircle(Position.X, Position.Y, Size, 32, Color.White, 1);
        }
        
        spriteBatch.DrawCircle(Position.X, Position.Y, Size/2, 32, Color.White, Size/2);
        
        if (Number > 0)
        {
            SpriteFontBase font18 = fontSystem.GetFont(11.5f);
            spriteBatch.DrawString(font18, Number.ToString(), new Vector2((int)Position.X - (Number > 9 ? 4 : 2), (int)Position.Y - 8), Color.Black);
            
        }
    }
    
}