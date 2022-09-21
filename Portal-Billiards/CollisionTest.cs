using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Mime;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;

namespace Portal_Billiards;

public class CollisionTest : Game
{
    
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    private Vector2 _ball1Origin = new Vector2(200, 400);
    private Vector2 _ball1StartVel = new Vector2(60, 0);
    private Vector2 _ball1Pos;
    private Vector2 _ball1Vel;
    private Vector2 _ball2Origin = new Vector2(400, 400);
    private Vector2 _ball2StartVel = new Vector2(0, 0);
    private Vector2 _ball2Pos;
    private Vector2 _ball2Vel;
    // private Vector2 _ball1Dest = new Vector2(600, 600);
    // private Vector2 _ball2Dest = new Vector2(640, 200);
    private List<int[]> _ball1Path = new List<int[]>();
    private List<int[]> _ball2Path = new List<int[]>();
    private bool _charted;
    private bool _hasCollided;


    private float _duration = 6;
    private float _currentTime = 0;

    // private List<int[]> _chart = new List<int[]>();
    private bool _skipButton = false;

    private float Dist()
    {
        return (int)Vector2.Distance(_ball1Pos, _ball2Pos);
    }
    
    private void DrawChart(List<int[]> chart, Color color)
    {
        for (int i = 0; i < chart.Count-1; i++)
        {
            _spriteBatch.DrawLine(chart[i][0], chart[i][1], chart[i+1][0], chart[i+1][1], color, 3);
        }
    }

    private void Reset()
    {
        _currentTime = 0;

        _ball1Pos = _ball1Origin;
        _ball2Pos = _ball2Origin;
        _ball1Vel = _ball1StartVel;
        _ball2Vel = _ball2StartVel;

        _hasCollided = false;
    }
    
    private void Randomize()
    {
        _ball1Path.Clear();
        _ball2Path.Clear();
        _charted = false;
        
        _ball1Origin = new Vector2(Random.Shared.Next(200, 300), Random.Shared.Next(350, 450));
        _ball2Origin = new Vector2(Random.Shared.Next(350, 450), Random.Shared.Next(350, 450));
        _ball1StartVel = new Vector2(Random.Shared.Next(0, 100), Random.Shared.Next(-10, 10));
        _ball2StartVel = new Vector2(Random.Shared.Next(-10, 10), Random.Shared.Next(-10, 10));
        
        Reset();
    }

    public CollisionTest()
    {
        _graphics = new GraphicsDeviceManager(this);
        IsMouseVisible = true;
        
        _graphics.PreferredBackBufferWidth = 800;  // set this value to the desired width of your window
        _graphics.PreferredBackBufferHeight = 800;   // set this value to the desired height of your window
        _graphics.ApplyChanges();
    }

    protected override void Initialize()
    {
        base.Initialize();

        Reset();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
    }

    protected override void Update(GameTime gameTime)
    {
        // Poll for current keyboard state
        KeyboardState state = Keyboard.GetState();
            
        // If they hit esc, exit
        if (state.IsKeyDown(Keys.Escape))
            Exit();

        if (state.IsKeyDown(Keys.N))
        {
            if (!_skipButton)
            {
                Randomize();
                
                _skipButton = true;
            }
        }
        else if (_skipButton) _skipButton = false;


        if (!state.IsKeyDown(Keys.Space))
        {
            _currentTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
        }
        
        if (_currentTime > _duration)
        {
            _charted = true;
            Reset();
        }
        
        // Physics update

        _ball1Pos += _ball1Vel * (float)gameTime.ElapsedGameTime.TotalSeconds;
        _ball2Pos += _ball2Vel * (float)gameTime.ElapsedGameTime.TotalSeconds;

        // collision test
        if (!_hasCollided && Dist() < 64)
        {
            _hasCollided = true;
            
            // Naive collision effect 1: add relative velocities
            // Vector2 ball1RVel = _ball2Vel - _ball1Vel;
            // Vector2 ball2RVel = _ball1Vel - _ball2Vel;
            // _ball1Vel += ball1RVel;
            // _ball2Vel += ball2RVel;
            
            // Naive collision effect 2: mirror vels across normal
            Vector2 n1 = _ball1Pos - _ball2Pos;
            n1.Normalize();
            _ball1Vel = Vector2.Reflect(_ball1Vel, n1);
            Vector2 n2 = _ball2Pos - _ball1Pos;
            n2.Normalize();
            _ball2Vel = Vector2.Reflect(_ball2Vel, n2);
        }

        if (!_charted)
        {
            _ball1Path.Add(new []{(int)_ball1Pos.X, (int)_ball1Pos.Y});
            _ball2Path.Add(new []{(int)_ball2Pos.X, (int)_ball2Pos.Y});
        }

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.DarkSlateGray);
        
        float t = _currentTime / _duration;
        
        _spriteBatch.Begin();
        
        
        // draw balls and their trajectories
        DrawChart(_ball1Path, Color.AliceBlue);
        DrawChart(_ball2Path, Color.DarkSalmon);
        _spriteBatch.DrawCircle(_ball1Pos, 32, 16, Color.CadetBlue, 32);
        _spriteBatch.DrawCircle(_ball2Pos, 32, 16, Color.Salmon, 32);

        
        _spriteBatch.End();

        base.Draw(gameTime);
    }
}