using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System;

namespace Portal_Billiards;

public class Ball
{
    public Vector2 Position;
    public Vector2 Velocity;
    public int size = 16;

    public Ball(Vector2 position, Vector2 velocity)
    {
        this.Position = position;
        this.Velocity = velocity;
    }

    public Ball()
    {
        this.Position = new Vector2(Random.Shared.Next(GameInfo.widthBound),
            Random.Shared.Next(GameInfo.heightBound));
        this.Velocity = new Vector2(Random.Shared.NextSingle() * 6 - 3, Random.Shared.NextSingle() * 6 - 3);
    }

    public void Update(List<Ball> toCollide, int index)
    {
        //check for collision with walls
        // if (Position.X < 0)
        // {
        //     Velocity.X = -Velocity.X;
        //     Position.X = -Position.X;
        // }
        // else if (Position.X > (gameInfo.widthBound - size))
        // {
        //     Position.X -= Position.X - (gameInfo.widthBound - size);
        //     Velocity.X = -Velocity.X;
        // }
        //
        // if (Position.Y < 0)
        // {
        //     Velocity.Y = -Velocity.Y;
        //     Position.Y = -Position.Y;
        // }
        // else if (Position.Y > (gameInfo.heightBound - size))
        // {
        //     Position.Y -= Position.Y - (gameInfo.heightBound - size);
        //     Velocity.Y = -Velocity.Y;
        // }

        Vector2 gravity = new Vector2(GameInfo.widthBound / 2, GameInfo.heightBound / 2) - Position;

        gravity = Vector2.Normalize(gravity) * 0.2f; //* MathF.Pow(gravity.Length(), -0.5f) * 1f;

        Velocity += gravity;


        // apply force from every ball
        for (int i = 0; i < toCollide.Count; i++)
        {
            if (toCollide[i].Position == Position)
            {
                continue; // this is probably us.
            }

            Vector2 repulse = Position - toCollide[i].Position;

            float strength = MathF.Pow(repulse.Length(), -1f);

            Velocity = Vector2.Lerp(Velocity, toCollide[i].Velocity,
                toCollide[i].Velocity.Length() * strength * 0.01f);

            repulse = Vector2.Normalize(repulse) * strength * 0.5f;

            Velocity += repulse;
        }

        // drag
        //Velocity *= 0.995f;

        // random jitter
        if (Random.Shared.Next(500) == 1)
        {
            Velocity.X += Random.Shared.Next(2) - 1;
            Velocity.Y += Random.Shared.Next(2) - 1;
        }

        // random jumps
        if (Random.Shared.Next(20000) == 1)
        {
            Velocity.X += Random.Shared.Next(50) - 25;
            Velocity.Y += Random.Shared.Next(50) - 25;
        }

        // // check for collision with every other ball
        // for (int i = index; i < toCollide.Count; i++)
        // {
        //     if(toCollide[i].Position == Position)
        //     {
        //         continue; // this is probably us.
        //     }
        //
        //     if (Vector2.Distance(toCollide[i].Position, Position) < 16)
        //     {
        //         Debug.WriteLine("Colliding!");
        //
        //         float m1 = Velocity.Length();
        //         float m2 = toCollide[i].Velocity.Length();
        //         
        //         Vector2 normal = Vector2.Normalize(Position - toCollide[i].Position);
        //
        //         Velocity = Vector2.Reflect(Velocity, normal);
        //         Velocity = Vector2.Normalize(Velocity) * m2;
        //         
        //         toCollide[i].Velocity = Vector2.Reflect(toCollide[i].Velocity, normal);
        //         toCollide[i].Velocity = Vector2.Normalize(toCollide[i].Velocity) * m1;
        //     }
        // }
    }

    public void LateUpdate()
    {
        Position += Velocity;
    }
}