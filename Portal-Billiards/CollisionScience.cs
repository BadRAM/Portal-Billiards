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

public class CollisionScience : Game
{
    
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    private Vector2 _ball1Pos = new Vector2(200, 250);
    // private Vector2 _ball1Vel = new Vector2(59.9f, 63.3f);
    private Vector2 _ball2Pos = new Vector2(220, 600);
    // private Vector2 _ball2Vel = new Vector2(41.1f, -85.5f);
    private Vector2 _ball1Dest = new Vector2(600, 600);
    private Vector2 _ball2Dest = new Vector2(640, 200);


    private float _duration = 6;
    private float _currentTime = 0;

    private List<int[]> _chart = new List<int[]>();
    private List<int[]> _xchart = new List<int[]>();
    private List<int[]> _ychart = new List<int[]>();
    private bool _skipButton = false;

    private float Dist(float t)
    {
        return (int)Vector2.Distance(Vector2.Lerp(_ball1Pos, _ball1Dest, t), Vector2.Lerp(_ball2Pos, _ball2Dest, t));
    }

    private void Chart()
    {
        // prechart new scene
        for (int i = 0; i < 200; i++)
        {
            _chart.Add(new []{i*4, (int)Dist((float)i/200)});
            _xchart.Add(new []{i*4, 
                (int)Math.Abs(Vector2.Lerp(_ball1Pos, _ball1Dest, (float)i/200).X - Vector2.Lerp(_ball2Pos, _ball2Dest, (float)i/200).X)});
            _ychart.Add(new []{i*4,
                (int)Math.Abs(Vector2.Lerp(_ball1Pos, _ball1Dest, (float)i/200).Y - Vector2.Lerp(_ball2Pos, _ball2Dest, (float)i/200).Y)});
        }
    }

    private void DrawChart(List<int[]> chart, int co, Color color)
    {
        for (int i = 0; i < chart.Count-1; i++)
        {
            _spriteBatch.DrawLine(chart[i][0], chart[i][1] + co, chart[i+1][0], chart[i+1][1] + co, color, 3);
        }
    }

    private void Randomize()
    {
        _currentTime = 0;
                
        _chart.Clear();
        _xchart.Clear();
        _ychart.Clear();
        _ball1Pos = new Vector2(Random.Shared.Next(50, 750), Random.Shared.Next(200, 750));
        _ball2Pos = new Vector2(Random.Shared.Next(50, 750), Random.Shared.Next(200, 750));
        _ball1Dest = new Vector2(Random.Shared.Next(50, 750), Random.Shared.Next(200, 750));
        _ball2Dest = new Vector2(Random.Shared.Next(50, 750), Random.Shared.Next(200, 750));
                
        Chart();
    }

    public CollisionScience()
    {
        _graphics = new GraphicsDeviceManager(this);
        IsMouseVisible = true;
        
        _graphics.PreferredBackBufferWidth = 800;  // set this value to the desired width of your window
        _graphics.PreferredBackBufferHeight = 800;   // set this value to the desired height of your window
        _graphics.ApplyChanges();
    }

    protected override void Initialize()
    {
        Chart();

        base.Initialize();
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

        
        // handle normal randomize button
        if (state.IsKeyDown(Keys.N))
        {
            if (!_skipButton)
            {
                Randomize();

                _skipButton = true;
            }
        }
        else if (_skipButton) _skipButton = false; // reset _skipbutton state when buttton is released.
        
        
        // handle super skip
        if (state.IsKeyDown(Keys.M))
        {
            bool reset = true;
            for (int i = 0; i < _chart.Count; i++)
            {
                if (_chart[i][1] <= 32)
                {
                    reset = false;
                    break;
                }
            }
            
            if (reset)
            {
                Randomize();
            }
        }


        if (!state.IsKeyDown(Keys.Space))
        {
            if (state.IsKeyDown(Keys.B))
            {
                _currentTime += (float)gameTime.ElapsedGameTime.TotalSeconds * 0.1f;
            }
            else
            {
                _currentTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
        }
        
        if (_currentTime > _duration)
        {
            _currentTime = 0;
        }

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);
        
        float t = _currentTime / _duration;
        
        _spriteBatch.Begin();
        
        
        // Draw distance graph
        const int cw = 800; // Chart Width
        const int co = 50; // Chart Offset

        
        DrawChart(_ychart, co, Color.Blue);
        DrawChart(_xchart, co, Color.DarkGreen);
        DrawChart(_chart, co, Color.Gray);
        

        // draw time cursor
        int cursor = (int)((_currentTime / _duration) * cw); 
        int cursory = co + (int)Dist(t);
        _spriteBatch.DrawCircle(cursor, cursory, 5, 16, Color.WhiteSmoke);
        _spriteBatch.DrawLine(cursor, 0, cursor, 800, Color.WhiteSmoke);

        // draw collision threshold line
        _spriteBatch.DrawLine(0, 82, 800, 82, Color.DarkCyan);
        
        
        // draw balls and their trajectories
        _spriteBatch.DrawLine(_ball1Pos, _ball1Dest, Color.AliceBlue, 2);
        _spriteBatch.DrawLine(_ball2Pos, _ball2Dest, Color.DarkSalmon, 2);
        _spriteBatch.DrawCircle(Vector2.Lerp(_ball1Pos, _ball1Dest, t), 16, 16, Color.CadetBlue, 3);
        _spriteBatch.DrawCircle(Vector2.Lerp(_ball2Pos, _ball2Dest, t), 16, 16, Color.Salmon, 3);

        _spriteBatch.End();

        base.Draw(gameTime);
    }
}