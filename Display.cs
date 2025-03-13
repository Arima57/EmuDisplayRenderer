using System;
using Silk.NET.OpenGL;
using StbImageSharp;
using Silk.NET.Windowing;
using System.Drawing;
using Shape = GLShape.GLShape;
using Silk.NET.Input;
using Silk.NET.Maths;

namespace EmuDisplayRenderer;

public class Display
{

    public GL? _gl;
    private IWindow window;
    private Shape? ViewportQuad;

    private IInputContext? input;

    public delegate void HandleInput(IKeyboard keyboard, Key key, int keyCode);

    public event HandleInput? CustomInput;
    public Color ClearColor = Color.Black;

    public WindowOptions options = WindowOptions.Default with {Title = "EmuDisplayRenderer"};
    public Display(WindowOptions? options=null)
    {
        if (options is not null)
        {
            this.options = (WindowOptions) options;
        }

        window = Window.Create(this.options);
        window.Load += OnLoad;
        window.Update += OnUpdate;
        window.Render += OnRender;
        window.Resize += OnResize;

        window.Run();        



    }

    public void OnLoad()
    {
        _gl = window.CreateOpenGL();
        _gl.ClearColor(ClearColor);
        
        ViewportQuad = new Shape(_gl);


        ViewportQuad.Vertices = new float[]
        {
            // X,    Y,    Z,   Tex X, Tex Y
            -1.0f, -1.0f, 0.0f,  
            -1.0f,  1.0f, 0.0f,  
             1.0f, -1.0f, 0.0f,
             1.0f,  1.0f, 0.0f
        };
        ViewportQuad.Indices = new uint[]
        {
            0, 1, 2,
            1, 2, 3
        };

        ViewportQuad.Load();
        Shape.Clear(_gl);


        input = window.CreateInput();
        
        for (int i = 0; i < input.Keyboards.Count; i++)
        {
            input.Keyboards[i].KeyDown += InternalInputHandler;
        }

    }

    public void OnUpdate(double delta)
    {
        return;
    }

    public void OnRender(double delta)
    {
        if (_gl is null || ViewportQuad is null)
        {
            return;
        }

        _gl.Clear(ClearBufferMask.ColorBufferBit);
        ViewportQuad.Render();

    }

    public void OnResize(Vector2D<int> size)
    {
        if (_gl is null)
        {
            return;
        }

        double targetAspect = (double)options.Size.X / options.Size.Y;
        double currentAspect = (double)size.X / size.Y;
        
        Vector2D<int> viewport = size;
        
        if (currentAspect > targetAspect)
        {
            // Window is wider than needed
            viewport.X = (int)(size.Y * targetAspect);
            viewport.Y = size.Y;
        }
        else
        {
            // Window is taller than needed
            viewport.X = size.X;
            viewport.Y = (int)(size.X / targetAspect);
        }

        // Center the viewport
        Vector2D<int> offset = (size - viewport) / 2;
        
        _gl.Viewport(offset, viewport);
    }
    public void InternalInputHandler(IKeyboard keyboard, Key key, int keyCode)
    {
        CustomInput?.Invoke(keyboard, key, keyCode);
    }
}
