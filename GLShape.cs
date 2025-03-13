using Silk.NET.OpenGL;
using StbImageSharp;


namespace GLShape
{
    public class GLShape
    {
        public uint _vao;
        public uint _vbo;
        public uint _ebo;
        public uint _shaderProgram;
        public GL _gl;

        public bool use_texture = false;
        public string image_texture = "";
        public uint _texture;

        public uint stride = 3 * sizeof(float);

        public float[] Vertices { get; set; } =
        {
            0.0f,  0.5f, 0.0f,
            0.5f, -0.5f, 0.0f,
            -0.5f, -0.5f, 0.0f
        };

        public uint[] Indices { get; set; } =
        {
            0u, 1u, 2u
        };

        public string VertexShaderSource { get; set; } = @"
            #version 330 core
            layout (location = 0) in vec3 aPos;

            void main()
            {
                gl_Position = vec4(aPos, 1.0);
            }";

        public string FragmentShaderSource { get; set; } = @"
            #version 330 core
            out vec4 FragColor;
            void main()
            {
                FragColor = vec4(1.0f, 1.0f, 1.0f, 1.0f);
            }";

        public GLShape(GL gL) 
        {
            _gl = gL;
        }

        public unsafe void Load()
        {
            _shaderProgram = CreateAndBindVAOVBOEBO();
        }

        public static unsafe void Clear(GL gl)
        {
            gl.BindBuffer(BufferTargetARB.ArrayBuffer, 0);
            gl.BindVertexArray(0);
            gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, 0);
            gl.BindTexture(TextureTarget.Texture2D, 0);
        }

        public unsafe void Render()
        {
            _gl.UseProgram(_shaderProgram);
            _gl.BindVertexArray(_vao);
            _gl.ActiveTexture(TextureUnit.Texture0); 
            _gl.BindTexture(TextureTarget.Texture2D, _texture);
            _gl.DrawElements(PrimitiveType.Triangles, (uint)Indices.Length, DrawElementsType.UnsignedInt, (void*)0);
            _gl.BindVertexArray(0);
        }

        private unsafe uint CreateAndBindVAOVBOEBO()
        {
            // 1. Create and Compile Shaders
            uint vertexShader = _gl.CreateShader(ShaderType.VertexShader);
            _gl.ShaderSource(vertexShader, VertexShaderSource);
            _gl.CompileShader(vertexShader);

            _gl.GetShader(vertexShader, ShaderParameterName.CompileStatus, out int vert_status);
            if (vert_status == 0)
            {
                throw new Exception("Vertex Shader Failed" + _gl.GetShaderInfoLog(vertexShader));
            }

            uint fragmentShader = _gl.CreateShader(ShaderType.FragmentShader);
            _gl.ShaderSource(fragmentShader, FragmentShaderSource);
            _gl.CompileShader(fragmentShader);

            _gl.GetShader(fragmentShader, ShaderParameterName.CompileStatus, out int frag_status);
            if (frag_status == 0)
            {
                throw new Exception("Fragment Shader Failed" + _gl.GetShaderInfoLog(fragmentShader));
            }

            // 2. Create and Link Program
            uint shaderProgram = _gl.CreateProgram();
            _gl.AttachShader(shaderProgram, vertexShader);
            _gl.AttachShader(shaderProgram, fragmentShader);
            _gl.LinkProgram(shaderProgram);

            _gl.GetProgram(shaderProgram, ProgramPropertyARB.LinkStatus, out int link_status);
            if (link_status == 0)
            {
                throw new Exception("Shader Program Failed");
            }

            // 3. Create VAO, VBO, EBO
            _vao = _gl.GenVertexArray();
            _gl.BindVertexArray(_vao);

            _vbo = _gl.GenBuffer();
            _gl.BindBuffer(BufferTargetARB.ArrayBuffer, _vbo);
            fixed (float* p = Vertices)
            {
                _gl.BufferData(BufferTargetARB.ArrayBuffer, (nuint)(Vertices.Length * sizeof(float)), p, BufferUsageARB.StaticDraw);
            }


            _ebo = _gl.GenBuffer();
            _gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, _ebo);
            fixed (uint* p = Indices)
            {
                _gl.BufferData(BufferTargetARB.ElementArrayBuffer, (nuint)(Indices.Length * sizeof(uint)), p, BufferUsageARB.StaticDraw);
            }

            // 4. Set Vertex Attributes (Example: Position Attribute)
            _gl.EnableVertexAttribArray(0);// 0 here is the (layout (location = 0) in vec3 aPos;
            _gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, stride, (void*)0); //same


            if (use_texture)
            {
                const uint texCoord = 1;
                _gl.EnableVertexAttribArray(texCoord);
                _gl.VertexAttribPointer(texCoord, 2, VertexAttribPointerType.Float, false, stride, (void*)(3 * sizeof(float)));
            }

            if (image_texture != "")
            {
                _texture = _gl.GenTexture();
                _gl.BindTexture(TextureTarget.Texture2D, _texture);
                
                ImageResult result = ImageResult.FromMemory(File.ReadAllBytes(image_texture), ColorComponents.RedGreenBlueAlpha);

                fixed (byte* p = result.Data)
                {
                    _gl.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgba,
                    (uint)result.Width, (uint)result.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, p);
                }

                int wrapMode = (int)TextureWrapMode.Repeat;
                int minFilter = (int)TextureMinFilter.Nearest;
                int magFilter = (int)TextureMagFilter.Nearest;
                
                _gl.TexParameterI(GLEnum.Texture2D, GLEnum.TextureWrapS, in wrapMode);
                _gl.TexParameterI(GLEnum.Texture2D, GLEnum.TextureWrapT, in wrapMode);
                _gl.TexParameterI(GLEnum.Texture2D, GLEnum.TextureMinFilter, in minFilter);
                _gl.TexParameterI(GLEnum.Texture2D, GLEnum.TextureMagFilter, in magFilter);       
                int location = _gl.GetUniformLocation(shaderProgram, "texture1");
                _gl.Uniform1(location, 0);     
            }

            Clear(_gl);
            _gl.DetachShader(shaderProgram, vertexShader);
            _gl.DetachShader(shaderProgram, fragmentShader);
            _gl.DeleteShader(vertexShader);
            _gl.DeleteShader(fragmentShader);

            return shaderProgram;
        }
    }
}