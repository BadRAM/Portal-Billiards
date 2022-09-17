using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Mime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Portal_Billiards;

public class Game1 : Game
{
    class Ball
    {
        public Vector2 Position;
        public Vector2 Velocity;
        public int size = 15;

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

    private List<Ball> _balls = new List<Ball>();
    
    private Texture2D _ballTexture;
    private Texture2D _brickTexture;
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        
        _graphics.PreferredBackBufferWidth = GameInfo.widthBound;  // set this value to the desired width of your window
        _graphics.PreferredBackBufferHeight = GameInfo.heightBound;   // set this value to the desired height of your window
        _graphics.ApplyChanges();
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here

        for (int i = 0; i < 100; i++)
        {
            _balls.Add(new Ball());
        }

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        
        Debug.WriteLine("#######");
        Debug.WriteLine("gonna load some file, Environment.CurrentDirectory = " + Environment.CurrentDirectory );
        string path = Path.GetFullPath(@"..\..\..\");
        Debug.WriteLine("Path.GetFullPath = " + path);
        Debug.WriteLine("#######");
        Console.WriteLine("Where does this go?");

        _ballTexture = Texture2D.FromFile(GraphicsDevice, path + "pinball.png");
        _brickTexture = Texture2D.FromFile(GraphicsDevice, path + "Brick.png");
        
        // using (var fileStream = new FileStream("Content/ball.png", FileMode.Open))
        // {
        //     ballTexture = Texture2D.FromStream(GraphicsDevice, fileStream);
        // }

        // TODO: use this.Content to load your game content here
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();


        // foreach (Ball b in _balls)
        // {
        //     b.Update(_balls);
        // }

        for (int i = 0; i < _balls.Count; i++)
        {
            _balls[i].Update(_balls, i);
        }
        
        for (int i = 0; i < _balls.Count; i++)
        {
            _balls[i].LateUpdate();
        }
        
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);

        // TODO: Add your drawing code here

        _spriteBatch.Begin();
        //_spriteBatch.Draw(_ballTexture, new Vector2(0, 0), Color.White);
        
        foreach (Ball b in _balls)
        {
            _spriteBatch.Draw(_ballTexture, b.Position, Color.White);
        }
        _spriteBatch.End();

        base.Draw(gameTime);
    }
}