using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Mime;
using System.Security.Cryptography.X509Certificates;
using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;

namespace Portal_Billiards;

/*
 * We need to change velocity into a linear equation representing their distance from eachother
 * 
 * one dimensional:
 *  deltaX = initialDispacementX + VelX1*t + VelX2*t
 *  
 */


// This program is a testbed for detecting collisions between pairs of moving circles
public class CollisionScience : Game
{
    
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private FontSystem _fontSystem;

    private Vector2 _ball1Pos = new Vector2(200, 250);
    // private Vector2 _ball1Vel = new Vector2(59.9f, 63.3f);
    private Vector2 _ball2Pos = new Vector2(220, 600);
    // private Vector2 _ball2Vel = new Vector2(41.1f, -85.5f);
    private Vector2 _ball1Dest = new Vector2(600, 600);
    private Vector2 _ball2Dest = new Vector2(640, 200);

    private int _ballRadius = 16;


    private float _duration = 6;
    private float _currentTime = 0;

    private List<int[]> _chart = new List<int[]>();
    private List<int[]> _mathsChart = new List<int[]>();
    private List<int[]> _xchart = new List<int[]>();
    private List<int[]> _ychart = new List<int[]>();

    private Vector2 _root = new Vector2();
    
    private bool _skipButton = false;

    private float _predictionStatus;
    private float _predictedCollision;

    private float Dist(float t)
    {
        return (int)Vector2.Distance(Vector2.Lerp(_ball1Pos, _ball1Dest, t), Vector2.Lerp(_ball2Pos, _ball2Dest, t));
    }
    

    private void Chart()
    {
        Vector2 b1p = new Vector2();
        Vector2 b2p = new Vector2();
        float x;
        float y;
        
        // prechart new scene
        for (int i = 0; i < 200; i++)
        {
            b1p = Vector2.Lerp(_ball1Pos, _ball1Dest, (float)i / 200);
            b2p = Vector2.Lerp(_ball2Pos, _ball2Dest, (float)i / 200);
            
            _chart.Add(new []{i*4, (int)Dist((float)i/200)});
            
            x = Math.Abs(b1p.X - b2p.X);
            _xchart.Add(new []{i*4, (int)x});

            y = Math.Abs(b1p.Y - b2p.Y);
            _ychart.Add(new []{i*4, (int)y});
            
            _mathsChart.Add(new []{i*4, (int)Math.Sqrt(x*x + y*y)});
        }

        // float d = 
        // if (expr)
        // {
        //     
        // }
        
        // do the cool maths
        float dx = _ball1Pos.X - _ball2Pos.X;
        float dy = _ball1Pos.Y - _ball2Pos.Y;
        Vector2 v1 = _ball1Dest - _ball1Pos;
        Vector2 v2 = _ball2Dest - _ball2Pos;
        float vx = v1.X - v2.X;
        float vy = v1.Y - v2.Y;
        int D = _ballRadius * 2;
        _predictionStatus = MathF.Pow(dx * vx + dy * vy, 2) - (vx*vx + vy*vy)*(-D*D + dx*dx + dy*dy);

        if (_predictionStatus >= 0)
        {
            _predictedCollision = (-(dx * vx + dy * vy) - MathF.Sqrt(_predictionStatus)) / (vx * vx + vy * vy);
        }
        else
        {
            _predictedCollision = -1;
        }

    }

    private void DrawChart(List<int[]> chart, int co, Color color, int thickness)
    {
        for (int i = 0; i < chart.Count-1; i++)
        {
            _spriteBatch.DrawLine(chart[i][0], chart[i][1] + co, chart[i+1][0], chart[i+1][1] + co, color, thickness);
        }
    }

    private void Randomize()
    {
        _currentTime = 0;
                
        _chart.Clear();
        _xchart.Clear();
        _ychart.Clear();
        _mathsChart.Clear();
        _ball1Pos = new Vector2(Random.Shared.Next(50, 750), Random.Shared.Next(200, 750));
        _ball2Pos = new Vector2(Random.Shared.Next(50, 750), Random.Shared.Next(200, 750));
        _ball1Dest = new Vector2(Random.Shared.Next(50, 750), Random.Shared.Next(200, 750));
        _ball2Dest = new Vector2(Random.Shared.Next(50, 750), Random.Shared.Next(200, 750));
                
        Chart();
    }

    private void BenchmarkRandomize()
    {
        _ball1Pos = new Vector2(Random.Shared.Next(50, 750), Random.Shared.Next(200, 750));
        _ball2Pos = new Vector2(Random.Shared.Next(50, 750), Random.Shared.Next(200, 750));
        _ball1Dest = new Vector2(Random.Shared.Next(50, 750), Random.Shared.Next(200, 750));
        _ball2Dest = new Vector2(Random.Shared.Next(50, 750), Random.Shared.Next(200, 750));
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
        
        // Benchmark Time!
        
        Debug.WriteLine("\n");
        Debug.WriteLine(" ---------------------- Benchmark Time! ----------------------\n");


        int tally = 0;
        Stopwatch sw = Stopwatch.StartNew();

        for (int i = 0; i < 1000000; i++)
        {
            BenchmarkRandomize();
            // tally += _ball1Dest.X > 400 ? 1 : 0;
        }
        
        
        sw.Stop();
        Debug.WriteLine($"Control: Time to generate 1 000 000 random collision situations: {sw.ElapsedMilliseconds}ms, tally (random 50% chance): {tally}");
        long control = sw.ElapsedMilliseconds;

        
        // Single axis overlap prune
        // tally = 0;
        // sw.Restart();
        //
        // int b1l = 0;
        // int b1r = 0;
        // int b2l = 0;
        // int b2r = 0;
        //
        // for (int i = 0; i < 1000000; i++)
        // {
        //     BenchmarkRandomize();
        //
        //
        //     if (_ball1Pos.X < _ball1Dest.X)
        //     {
        //         b1l = 
        //     }
        //
        //     if () tally++;
        // }
        //
        // sw.Stop();
        // Debug.WriteLine($"Time to prune 1 000 000 random collision situations via single axis overlap: {sw.ElapsedMilliseconds-control}ms, tally: {tally}");
        
        tally = 0;
        sw.Restart();

        Rectangle rect1 = new Rectangle();
        Rectangle rect2 = new Rectangle();

        for (int i = 0; i < 1000000; i++)
        {
            BenchmarkRandomize();
            
            // rect1.SetFromTwoPoints(_ball1Pos, _ball1Dest);
            // rect2.SetFromTwoPoints(_ball2Pos, _ball2Dest);
            // rect1.Inflate(64, 64);
            // rect2.Inflate(64, 64);
            
            rect1.SetFromTwoPointsAndInflate(_ball1Pos, _ball1Dest, 32);
            rect2.SetFromTwoPointsAndInflate(_ball2Pos, _ball2Dest, 32);
            
            
            if (rect1.Intersects(rect2)) tally++;
        }
        
        sw.Stop();
        Debug.WriteLine($"Time to prune 1 000 000 random collision situations via bounding box overlap: {sw.ElapsedMilliseconds-control}ms, tally: {tally}");

        tally = 0;
        sw.Restart();

        for (int i = 0; i < 1000000; i++)
        {
            BenchmarkRandomize();

            int dx = (int)_ball1Pos.X - (int)_ball2Pos.X;
            int dy = (int)_ball1Pos.Y - (int)_ball2Pos.Y;
            if (dx < 32 && dx > -32 && dy < 32 && dy > -32) tally++;
        }
        
        sw.Stop();
        Debug.WriteLine($"Time to test 1 000 000 ball bounding box overlaps: {sw.ElapsedMilliseconds-control}ms, tally: {tally}");
        
        tally = 0;
        sw.Restart();

        for (int i = 0; i < 1000000; i++)
        {
            BenchmarkRandomize();

            if (Vector2.Distance(_ball1Pos, _ball2Pos) < 32) tally++;
        }
        
        sw.Stop();
        Debug.WriteLine($"Time to test 1 000 000 circle-circle overlaps with vector2.distance: {sw.ElapsedMilliseconds-control}ms, tally: {tally}");

        tally = 0;
        sw.Restart();

        for (int i = 0; i < 1000000; i++)
        {
            if (Random.Shared.Next(100000) < 32) tally++;
        }
        
        sw.Stop();
        Debug.WriteLine($"Time to perform 1 000 000 random.shared.next operations: {sw.ElapsedMilliseconds}ms, tally: {tally}");
        
        tally = 0;
        sw.Restart();

        for (int i = 0; i < 1000000; i++)
        {
            if (Math.Sqrt(Random.Shared.Next(100000)) < 32) tally++;
        }
        
        sw.Stop();
        Debug.WriteLine($"Time to perform 1 000 000 math.sqrt operations: {sw.ElapsedMilliseconds}ms, tally: {tally}");
        
        tally = 0;
        sw.Restart();

        for (int i = 0; i < 1000000; i++)
        {
            if (Math.Abs(Random.Shared.Next(-10000, 10000)) < 32) tally++;
        }
        
        sw.Stop();
        Debug.WriteLine($"Time to perform 1 000 000 math.abs operations: {sw.ElapsedMilliseconds}ms, tally: {tally}");
        
        tally = 0;
        sw.Restart();

        for (int i = 0; i < 1000000; i++)
        {
            if (Math.Abs(Random.Shared.Next(-10000, 10000)) < 32) tally++;
        }
        
        sw.Stop();
        Debug.WriteLine($"Time to perform 1 000 000 math.abs operations: {sw.ElapsedMilliseconds}ms, tally: {tally}");
        
        Debug.WriteLine("\n");
        base.Initialize();
    }

    protected override void LoadContent()
    {
        string path = Path.GetFullPath(@"..\..\..\");

        _spriteBatch = new SpriteBatch(GraphicsDevice);
        
        _fontSystem = new FontSystem();
        //_fontSystem.AddFont(File.ReadAllBytes(path + "fonts\\ModernDOS8x14.ttf"));
        _fontSystem.AddFont(File.ReadAllBytes(path + "fonts\\alagard.ttf"));
    }

    protected override void Update(GameTime gameTime)
    {
        // Poll for current keyboard state
        KeyboardState state = Keyboard.GetState();
            
        // If they hit esc, exit
        if (state.IsKeyDown(Keys.Escape)) Exit();

        
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
            
            if (reset/* == _predictionStatus < 0*/)
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

        
        DrawChart(_ychart, co, Color.Blue, 2);
        DrawChart(_xchart, co, Color.DarkGreen, 2);
        DrawChart(_chart, co, Color.Gray, 3);
        DrawChart(_mathsChart, co, Color.Pink, 1);
        

        // draw time cursor
        int cursor = (int)((_currentTime / _duration) * cw); 
        int cursory = co + (int)Dist(t);
        _spriteBatch.DrawCircle(cursor, cursory, 5, 16, Color.WhiteSmoke);
        _spriteBatch.DrawLine(cursor, 0, cursor, 800, Color.WhiteSmoke);
        
        // Draw prediction target
        _spriteBatch.DrawLine(_predictedCollision * cw, 0,  _predictedCollision * cw, 800, Color.LimeGreen, 2);

        // draw collision threshold line
        _spriteBatch.DrawLine(0, 82, 800, 82, Color.DarkCyan);
        
        
        // draw balls and their trajectories
        _spriteBatch.DrawLine(_ball1Pos, _ball1Dest, Color.AliceBlue, 2);
        _spriteBatch.DrawLine(_ball2Pos, _ball2Dest, Color.DarkSalmon, 2);
        _spriteBatch.DrawCircle(Vector2.Lerp(_ball1Pos, _ball1Dest, t), 16, 16, Color.CadetBlue, 3);
        _spriteBatch.DrawCircle(Vector2.Lerp(_ball2Pos, _ball2Dest, t), 16, 16, Color.Salmon, 3);

        SpriteFontBase font18 = _fontSystem.GetFont(30);
        _spriteBatch.DrawString(font18, $"Discriminant: {_predictionStatus}\nPrediction: {_predictedCollision}", new Vector2(0, 0), Color.White);

        _spriteBatch.End();

        base.Draw(gameTime);
    }
}