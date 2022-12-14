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
using MonoGame.Extended.BitmapFonts;

namespace Portal_Billiards;

public class CollisionTest : Game
{
    
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private FontSystem _fontSystem;

    private int _mode = 3;
    private float _two = 2; // that weird constant that doesn't do the same thing in every algo
    //TODO: remove _two, it's only for science purposes

    private Vector2 _ball1Origin = new Vector2(200, 400);
    private Vector2 _ball1StartVel = new Vector2(60, 0);
    private Vector2 _ball1Pos;
    private Vector2 _ball1Vel;
    private float _ball1Mass = 50;
    private Vector2 _ball2Origin = new Vector2(400, 400);
    private Vector2 _ball2StartVel = new Vector2(0, 0);
    private Vector2 _ball2Pos;
    private Vector2 _ball2Vel;
    private float _ball2Mass = 50;
    // private Vector2 _ball1Dest = new Vector2(600, 600);
    // private Vector2 _ball2Dest = new Vector2(640, 200);
    private List<int[]> _ball1Path = new List<int[]>();
    private List<int[]> _ball2Path = new List<int[]>();
    private bool _charted;
    private bool _hasCollided;
    private float _timeToCollision;

    private float _skip;
    private int _skipCounter;
    private bool _stepHeld;

    private Vector2 _contactPoint = new Vector2(-100, -100);


    private float _duration = 3;
    private float _currentTime = 0;
    private float _deltaTime = 0;

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
        
        _ball1Origin = new Vector2(Random.Shared.Next(100, 200), Random.Shared.Next(350, 450));
        _ball2Origin = new Vector2(Random.Shared.Next(450, 550), Random.Shared.Next(350, 450));
        _ball1StartVel = new Vector2(Random.Shared.Next(0, 1000), Random.Shared.Next(-10, 10));
        _ball2StartVel = new Vector2(Random.Shared.Next(-10, 10), Random.Shared.Next(-10, 10));
        _ball1Mass = Random.Shared.Next(10, 50);
        _ball2Mass = Random.Shared.Next(10, 50);
        
        Reset();
    }

    private float CollisionCheck()
    {
        
        // do the cool maths
        float dx = _ball1Pos.X - _ball2Pos.X;
        float dy = _ball1Pos.Y - _ball2Pos.Y;
        float vx = _ball1Vel.X - _ball2Vel.X;
        float vy = _ball1Vel.Y - _ball2Vel.Y;
        int D = (int)(_ball1Mass + _ball2Mass);
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
        
        // Debug.WriteLine("#######");
        // Debug.WriteLine("gonna load some file, Environment.CurrentDirectory = " + Environment.CurrentDirectory );
        string path = Path.GetFullPath(@"..\..\..\");
        // Debug.WriteLine("Path.GetFullPath = " + path);
        // Debug.WriteLine("#######");
        // Console.WriteLine("Where does this go?");
        //
        // _ballTexture = Texture2D.FromFile(GraphicsDevice, path + "pinball.png");
        // _brickTexture = Texture2D.FromFile(GraphicsDevice, path + "Brick.png");
        // _bitmapFont = Content.Load<BitmapFont>("my-font");
        
        _fontSystem = new FontSystem();
        //_fontSystem.AddFont(File.ReadAllBytes(path + "fonts\\ModernDOS8x14.ttf"));
        _fontSystem.AddFont(File.ReadAllBytes(path + "fonts\\alagard.ttf"));
        
        //_fontSystem.AddFont(File.ReadAllBytes(@"Fonts/DroidSansJapanese.ttf"));
        //_fontSystem.AddFont(File.ReadAllBytes(@"Fonts/Symbola-Emoji.ttf"));
    }

    protected override void Update(GameTime gameTime)
    {
        // Poll for current keyboard state
        KeyboardState state = Keyboard.GetState();
            
        // If they hit esc, exit
        if (state.IsKeyDown(Keys.Escape))
            Exit();

        
        if (state.IsKeyDown(Keys.D1)) { _mode = 1; }
        if (state.IsKeyDown(Keys.D2)) { _mode = 2; }
        if (state.IsKeyDown(Keys.D3)) { _mode = 3; }
        if (state.IsKeyDown(Keys.D4)) { _mode = 4; }
        if (state.IsKeyDown(Keys.D5)) { _mode = 5; }
        if (state.IsKeyDown(Keys.Z)) { _two += gameTime.GetElapsedSeconds() * 0.5f; }
        else if (state.IsKeyDown(Keys.X)) { _two -= gameTime.GetElapsedSeconds() * 0.5f; }
        else { _two = MathF.Round(_two, 1); }
        if (state.IsKeyDown(Keys.C)) { _skip += gameTime.GetElapsedSeconds() * 2; }
        else if (state.IsKeyDown(Keys.V)) { _skip -= gameTime.GetElapsedSeconds() * 2; }
        else { _skip = (int)_skip; }
        _skip = MathF.Max(_skip, 1);
        
        

        if (state.IsKeyDown(Keys.N))
        {
            if (!_skipButton)
            {
                Randomize();
                _contactPoint = Vector2.Zero;
                _skipButton = true;
            }
        }
        else if (_skipButton) _skipButton = false;

        if (_skipCounter < (int)_skip)
        {
            _skipCounter++;
            return;
        }
        else
        {
            _skipCounter = 0;
        }

        if (!state.IsKeyDown(Keys.Space) || (!_stepHeld && state.IsKeyDown(Keys.S)))
        {
            _deltaTime = gameTime.GetElapsedSeconds() * (int)_skip;
            _currentTime += _deltaTime;
        }
        else
        {
            _deltaTime = 0;
        }
        _stepHeld = state.IsKeyDown(Keys.S);
        
        
        if (_currentTime > _duration)
        {
            _charted = true;
            Reset();
        }
        


        if (!_hasCollided)
        {
            _timeToCollision = CollisionCheck();
        }
        
        // collision test
        if (!_hasCollided && _timeToCollision > 0 && _timeToCollision < _deltaTime)
        {
            _hasCollided = true;

            _ball1Pos += _ball1Vel * _timeToCollision;
            _ball2Pos += _ball2Vel * _timeToCollision;

            _contactPoint = _ball1Pos + (_ball2Pos - _ball1Pos).NormalizedCopy() * _ball1Mass;

            switch (_mode)
            {
                case 1: // Naive collision effect 1: add relative velocities
                    Vector2 ball1RVel = _ball2Vel - _ball1Vel;
                    Vector2 ball2RVel = _ball1Vel - _ball2Vel;
                    _ball1Vel += ball1RVel;
                    _ball2Vel += ball2RVel;
                    break;
                
                case 2: // Naive collision effect 2: mirror vels across normal
                    Vector2 n1 = _ball1Pos - _ball2Pos;
                    n1.Normalize();
                    _ball1Vel = Vector2.Reflect(_ball1Vel, n1);
                    Vector2 n2 = _ball2Pos - _ball1Pos;
                    n2.Normalize();
                    _ball2Vel = Vector2.Reflect(_ball2Vel, n2);
                    break;

                case 3: // Naive collision effect 3: impulse of rvel projected along normal
                    Vector2 i1 = _ball2Vel - _ball1Vel;
                    Vector2 i2 = _ball1Vel - _ball2Vel;
                    i1 = i1.ProjectOnto(Vector2.Normalize(_ball1Pos - _ball2Pos));
                    i2 = i2.ProjectOnto(Vector2.Normalize(_ball2Pos - _ball1Pos));
                    _ball1Vel += _two * i1 * (_ball2Mass / (_ball1Mass + _ball2Mass));
                    _ball2Vel += _two * i2 * (_ball1Mass / (_ball1Mass + _ball2Mass));
                    
                    // case 1: b1 is 0% of combined mass, b1 is reflected
                    // case 2: b1 is 50% of combined mass, velocities are exchanged
                    // case 3: b1 is 100% of combined mass, it's velocity is unaffected
                    break;
                
                case 4: // steal from wiki
                    Vector2 v1 = _ball1Vel;
                    Vector2 v2 = _ball2Vel;
                    
                    _ball1Vel = (v1 * ((_ball1Mass - _ball2Mass) / (_ball1Mass + _ball2Mass)) + v2 * ((_two * _ball2Mass)/_ball1Mass + _ball2Mass));
                    _ball2Vel = (v2 * ((_ball2Mass - _ball1Mass) / (_ball1Mass + _ball2Mass)) + v1 * ((_two * _ball1Mass)/_ball1Mass + _ball2Mass));
                    break;
                
                case 5: // Jamesway (TM)
                    Vector2 CoolVel = _two*(_ball1Mass * _ball1Vel + _ball2Mass * _ball2Vel) / (_ball1Mass + _ball2Mass);

                    _ball1Vel = CoolVel - _ball1Vel;
                    _ball2Vel = CoolVel - _ball2Vel;
                    break;
            }
            
            _ball1Pos += _ball1Vel * (_deltaTime - _timeToCollision);
            _ball2Pos += _ball2Vel * (_deltaTime - _timeToCollision);
        }
        else
        {
            // Physics update

            _ball1Pos += _ball1Vel * _deltaTime;
            _ball2Pos += _ball2Vel * _deltaTime;
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
        _spriteBatch.DrawCircle(_ball1Pos, _ball1Mass, 32, Color.CadetBlue, 2);
        _spriteBatch.DrawCircle(_ball2Pos, _ball2Mass, 32, Color.Salmon, 2);
        
        
        SpriteFontBase font18 = _fontSystem.GetFont(30);
        _spriteBatch.DrawString(font18, $"Mode: {_mode}\nMass: {_ball1Mass} {_ball2Mass}\nVelocity: {_ball1Vel.Length()} {_ball2Vel.Length()}\nTwo: {_two}\nTime to collision: {_timeToCollision}\nSkip: {(int)_skip}", new Vector2(0, 0), Color.White);

        _spriteBatch.DrawCircle(_contactPoint, 5, 4, Color.White);

        _spriteBatch.End();

        base.Draw(gameTime);
    }
}