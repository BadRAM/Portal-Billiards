using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;

namespace Portal_Billiards;

public class Game1 : Game
{
    
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private FontSystem _fontSystem;
    private FontSystem _smallFont;

    private Texture2D _ballTexture;
    private Texture2D _brickTexture;

    private List<Ball> Balls = new List<Ball>();
    private List<Ball> SunkBalls = new List<Ball>();
    private List<Vector2> _pockets = new List<Vector2>();

    private float _aimDir = 0f;
    private float _aimPow = 400f;

    private bool _spawnHeld;
    private bool _stepHeld;

    private string _debugText;
    
    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;

        int scale = 1;
        _graphics.PreferredBackBufferWidth = 960 * scale;  // set this value to the desired width of your window
        _graphics.PreferredBackBufferHeight = 720 * scale; // set this value to the desired height of your window
        _graphics.ApplyChanges();
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here
        
        base.Initialize();
        
        Balls.Add(new Ball(new Vector2(300,300), 0)); // Add Cueball to index 0
        Balls.Add(new Ball(new Vector2(700,300), 9));
        Balls.Add(new Ball(new Vector2(720, 315), 15));
        Balls.Add(new Ball(new Vector2(720,285), 3));
        Balls.Add(new Ball(new Vector2(740,330), 5));
        Balls.Add(new Ball(new Vector2(740, 300), 8)); // 8 ball
        Balls.Add(new Ball(new Vector2(740,270), 1));
        Balls.Add(new Ball(new Vector2(760,345), 2));
        Balls.Add(new Ball(new Vector2(760,315), 11));
        Balls.Add(new Ball(new Vector2(760,285), 4));
        Balls.Add(new Ball(new Vector2(760,255), 12));
        Balls.Add(new Ball(new Vector2(780,360), 7));
        Balls.Add(new Ball(new Vector2(780,330), 6));
        Balls.Add(new Ball(new Vector2(780, 300), 14));
        Balls.Add(new Ball(new Vector2(780,270), 13));
        Balls.Add(new Ball(new Vector2(780,240), 10));
        
        
        _pockets.Add(new Vector2(102, 102));
        _pockets.Add(new Vector2(102, 498));
        _pockets.Add(new Vector2(500, 100));
        _pockets.Add(new Vector2(500, 500));
        _pockets.Add(new Vector2(898, 102));
        _pockets.Add(new Vector2(898, 498));
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        
        string path = Path.GetFullPath(@"..\..\..\");

        // _ballTexture = Texture2D.FromFile(GraphicsDevice, path + "pinball.png");
        // _brickTexture = Texture2D.FromFile(GraphicsDevice, path + "Brick.png");
        
        _fontSystem = new FontSystem();
        _fontSystem.AddFont(File.ReadAllBytes(path + "fonts\\alagard.ttf"));

        _smallFont = new FontSystem();
        _smallFont.AddFont(File.ReadAllBytes(path + "fonts\\pixels.ttf"));
    }

    // returns a list of collision candidates as the indices of the balls they relate to
    private List<int[]> Prune(float delta)
    {
        // prune collisions
        List<int[]> collisions = new List<int[]>();
        for (int i = 0; i < Balls.Count; i++) // Non prune method, simply generates a list of all non duplicate collisions
        {
            for (int j = i+1; j < Balls.Count; j++)
            {
                if (Vector2.Distance(Balls[i].Position, Balls[j].Position) <= 
                    Balls[i].Size + Balls[i].Velocity.Length() * delta + 
                    Balls[j].Size + Balls[j].Velocity.Length() * delta)
                {
                    collisions.Add(new []{i, j});
                }
            }
        }
        return collisions;
    }

    private void PhysicsStep(float deltaTime)
    {
        _debugText = "";

        /* 
         * Known Physics Update Steps:
         * - apply forces
         * - prune collisions
         * - precise check for collisions
         * - apply collision effects
         * - step non colliding objects
         *
         * Known future requirements:
         * - different collider shapes
         *
         * Thoughts:
         *  when solving collisions, if a collision is detected, all balls will be advanced to that point in time and
         *  further collisions advance from that point. in this case, do I need to recheck pruned balls?
         * 
         */

        // Apply Forces
        foreach (Ball b in Balls)
        {
            b.Drag();
        }

        // prune collisions
        List<int[]> collisions = Prune(deltaTime);

        _debugText += $"{collisions.Count} collisions after pruning, ";

        int collisionsHandled = 0;
        float d = deltaTime;
        while (true)
        {
            // precise check for collisions
            int lowest = -1;
            float lowestVal = Single.PositiveInfinity;
            for (int i = 0; i < collisions.Count; i++)
            {
                float t = CCCC.Balls(Balls[collisions[i][0]], Balls[collisions[i][1]]);

                if (t < 0 || t > d)
                {
                    collisions.RemoveAt(i);
                    i--;
                    continue;
                }
                
                if (t < lowestVal)
                {
                    lowest = i;
                    lowestVal = t;
                }
            }

            if (lowest == -1) break;

            // move all balls forward to collision time
            foreach (Ball b in Balls)
            {
                b.Move(lowestVal);
            }
            
            // do collision
            Balls[collisions[lowest][0]].CollideWith(Balls[collisions[lowest][1]]);
            collisions.RemoveAt(lowest);
            collisionsHandled++;
            d -= lowestVal;
            collisions = Prune(d);
        }

        _debugText += $"{collisionsHandled} collisions handled.";
        
        // move all balls forward to end of frame positions now that no more collisions need to be done
        foreach (Ball b in Balls)
        {
            b.Move(d);
        }
        
        
        // Check if balls are sinking into pockets
        for (int i = 0; i < Balls.Count; i++)
        {
            for (int j = 0; j < _pockets.Count; j++)
            {
                if (Vector2.Distance(Balls[i].Position, _pockets[j]) < 20)
                {
                    Balls[i].Velocity = Vector2.Zero;
                    Balls[i].Position = new Vector2(400 + SunkBalls.Count * 24, 650);
                    SunkBalls.Add(Balls[i]);
                    Balls.RemoveAt(i);
                    break;
                }
            }
        }
    }

    protected override void Update(GameTime gameTime)
    {
        
        // Poll for current keyboard state
        KeyboardState state = Keyboard.GetState();
            
        // If they hit esc, exit
        if (state.IsKeyDown(Keys.Escape)) Exit();

        float fine = state.IsKeyDown(Keys.LeftShift) ? 0.5f : 2f;
        
        // control aim direction
        if (state.IsKeyDown(Keys.Right)) { _aimDir += gameTime.GetElapsedSeconds() * fine; }
        if (state.IsKeyDown(Keys.Left))  { _aimDir -= gameTime.GetElapsedSeconds() * fine; }
        
        // control aim power
        if (state.IsKeyDown(Keys.Up))   { _aimPow += gameTime.GetElapsedSeconds() * 80f * fine; }
        if (state.IsKeyDown(Keys.Down)) { _aimPow -= gameTime.GetElapsedSeconds() * 80f * fine; }

        if (Balls[0].Velocity == Vector2.Zero && state.IsKeyDown(Keys.Space))
        {
            Balls[0].Velocity = new Vector2(MathF.Cos(_aimDir), MathF.Sin(_aimDir)) * _aimPow;
        }
        
        if (!state.IsKeyDown(Keys.A) || (!_stepHeld && state.IsKeyDown(Keys.S)))
        {
            PhysicsStep(gameTime.GetElapsedSeconds());
        }
        _stepHeld = state.IsKeyDown(Keys.S);


        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        // _spriteBatch = new SpriteBatch(GraphicsDevice);
        // RenderTarget2D target = new RenderTarget2D(GraphicsDevice, 820, 615);
        // GraphicsDevice.SetRenderTarget(target);
        
        
        GraphicsDevice.Clear(Color.Black);

        // TODO: Add your drawing code here

        _spriteBatch.Begin();
        //_spriteBatch.Draw(_ballTexture, new Vector2(0, 0), Color.White);

        // draw the table and pockets
        _spriteBatch.DrawRectangle(50, 50, 900, 500, Color.Brown, 400);
        _spriteBatch.DrawRectangle(100, 100, 800, 400, Color.DarkGreen, 400);

        for (int i = 0; i < _pockets.Count; i++)
        {
            _spriteBatch.DrawCircle(_pockets[i], 18, 16, Color.Black, 18);
        }

        for (int i = 0; i < Balls.Count; i++)
        {
            Balls[i].Draw(_spriteBatch, _smallFont);
        }        
        
        for (int i = 0; i < SunkBalls.Count; i++)
        {
            SunkBalls[i].Draw(_spriteBatch, _smallFont);
        }

        if (Balls[0].Velocity == Vector2.Zero)
        {
            Vector2 aimDir = new Vector2(MathF.Cos(_aimDir), MathF.Sin(_aimDir));
            for (int i = 1; i <= 5; i++)
            {
                _spriteBatch.DrawCircle(Balls[0].Position + aimDir * _aimPow * i * 0.1f, 4, 8, Color.White, 1);
            }
        }
        
        SpriteFontBase font18 = _fontSystem.GetFont(30);
        _spriteBatch.DrawString(font18, $"Power: {Math.Round(_aimPow, 2)}", new Vector2(50, 550), Color.White);
        _spriteBatch.DrawString(font18, _debugText, new Vector2(50, 600), Color.White);
        SpriteFontBase font5 = _smallFont.GetFont(12);
        _spriteBatch.DrawString(font5, "1234567890", new Vector2(600, 600), Color.White);


        _spriteBatch.End();

        base.Draw(gameTime);
        
        //set rendering back to the back buffer
        GraphicsDevice.SetRenderTarget(null);

        // //render target to back buffer
        // _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);
        // _spriteBatch.Draw(target, new Rectangle(0, 0, GraphicsDevice.DisplayMode.Width, GraphicsDevice.DisplayMode.Height), Color.White);
        // _spriteBatch.End();
    }
}