// -----------------------------------------------------------------------
// <file>World.cs</file>
// <copyright>Grupa za Grafiku, Interakciju i Multimediju 2013.</copyright>
// <author>Srđan Mihić</author>
// <author>Aleksandar Josić</author>
// <summary>Klasa koja enkapsulira OpenGL programski kod.</summary>
// -----------------------------------------------------------------------
using System;
using Assimp;
using System.Drawing;
using System.IO;
using System.Reflection;
using SharpGL.SceneGraph;
using SharpGL.SceneGraph.Primitives;
using SharpGL.SceneGraph.Quadrics;
using SharpGL.SceneGraph.Core;
using SharpGL;
using System.Drawing.Imaging;

namespace AssimpSample
{


    /// <summary>
    ///  Klasa enkapsulira OpenGL kod i omogucava njegovo iscrtavanje i azuriranje.
    /// </summary>
    public class World : IDisposable
    {

        #region Atributi

        private OpenGL gl;
        /// <summary>
        ///	 Scena koja se prikazuje.
        /// </summary>
        private AssimpScene m_scene;

        /// <summary>
        ///	 Ugao rotacije sveta oko X ose.
        /// </summary>
        private float m_xRotation = 0.0f;

        /// <summary>
        ///	 Ugao rotacije sveta oko Y ose.
        /// </summary>
        private float m_yRotation = 0.0f;

        /// <summary>
        ///	 Udaljenost scene od kamere.
        /// </summary>
        private float m_sceneDistance = 20.0f;

        /// <summary>
        ///	 Sirina OpenGL kontrole u pikselima.
        /// </summary>
        private int m_width;

        /// <summary>
        ///	 Visina OpenGL kontrole u pikselima.
        /// </summary>
        private int m_height;

        private double m_onTableHeightY = -1.5;

        /// <summary>
        /// Field of view
        /// </summary>
        private readonly double m_fov = 45.0;

        private readonly int m_textMargin = 25;

        private float m_eyeX = 0.0f;
        private float m_eyeY = 0.0f;
        private float m_eyeZ = 0.0f;

        private float m_centerX = 0.0f;
        private float m_centerY = 0.0f;
        private float m_centerZ = -1.0f;

        private float m_upX = 0.0f;
        private float m_upY = 1.0f;
        private float m_upZ = 0.0f;

        private Cube cube;
        private Cube tray;
        private Disk disk;

        private double m_diskOffsetY = 1.3;
        private double m_diskOffsetZ;
        private double m_trayOffsetZ;

        private double m_computerOffsetX;
        private double m_computerScaleFactor = 1;

        private string[] m_text = new string[] { "Predmet: Racunarska grafika" , "Sk. god: 2020/21." , "Ime: Gergelj" , "Prezime: Kis" , "Sifra zad: 6.2" };
        private float[] m_textWidth = new float[] { 10.5f, 6.5f, 4.5f, 4.5f, 4.8f};
        private float m_ortho2dProjection = 10.0f;

        private float[] m_shearMatrix = {
            1.0f, 0.0f, 0.0f, 0.0f,
            0.5f, 1.0f, 0.0f, 0.0f,
            0.0f, 0.0f, 1.0f, 0.0f,
            0.0f, 0.0f, 0.0f, 1.0f
         };

        private float[] m_light1ambient = new float[] { 1.0f, 0.0f, 0.0f, 1.0f };

        public AnimationHandler animationHandler;
        private Boolean m_isBeingAnimated = false;

        private enum TextureObjects { Wood = 0, Carpet, Screen};
        private readonly int m_textureCount = Enum.GetNames(typeof(TextureObjects)).Length;
        private uint[] m_textures = null;
        private string[] m_textureFiles = { "..//..//images//wood.jpg", "..//..//images//carpet.jpg", "..//..//images//screen.jpg" };

        #endregion Atributi

        #region Properties

        /// <summary>
        ///	 Scena koja se prikazuje.
        /// </summary>
        public AssimpScene Scene
        {
            get { return m_scene; }
            set { m_scene = value; }
        }

        /// <summary>
        ///	 Ugao rotacije sveta oko X ose.
        /// </summary>
        public float RotationX
        {
            get { return m_xRotation; }
            set { m_xRotation = value; }
        }

        /// <summary>
        ///	 Ugao rotacije sveta oko Y ose.
        /// </summary>
        public float RotationY
        {
            get { return m_yRotation; }
            set { m_yRotation = value; }
        }

        /// <summary>
        ///	 Udaljenost scene od kamere.
        /// </summary>
        public float SceneDistance
        {
            get { return m_sceneDistance; }
            set { m_sceneDistance = value; }
        }

        /// <summary>
        ///	 Sirina OpenGL kontrole u pikselima.
        /// </summary>
        public int Width
        {
            get { return m_width; }
            set { m_width = value; }
        }

        /// <summary>
        ///	 Visina OpenGL kontrole u pikselima.
        /// </summary>
        public int Height
        {
            get { return m_height; }
            set { m_height = value; }
        }

        public double DiskOffsetY
        {
            get { return m_diskOffsetY; }
            set { m_diskOffsetY = value; }
        }

        public double DiskOffsetZ
        {
            get { return m_diskOffsetZ; }
            set { m_diskOffsetZ = value; }
        }

        public double TrayOffsetZ
        {
            get { return m_trayOffsetZ; }
            set { m_trayOffsetZ = value; }
        }

        public double ComputerOffsetX
        {
            get { return m_computerOffsetX; }
            set { m_computerOffsetX = value; }
        }

        public double ComputerScaleFactor
        {
            get { return m_computerScaleFactor; }
            set
            {
                if (value < 0.1)
                    m_computerScaleFactor = 0.1;
                else if (value > 2)
                    m_computerScaleFactor = 2;
                else
                    m_computerScaleFactor = value;
            }
        }

        public float[] Light1Ambient
        {
            get { return m_light1ambient; }
        }

        public Boolean IsBeingAnimated
        {
            get { return m_isBeingAnimated; }
            set { m_isBeingAnimated = value; }
        }

        #endregion Properties

        #region Konstruktori

        /// <summary>
        ///  Konstruktor klase World.
        /// </summary>
        public World(string scenePath, string sceneFileName, int width, int height, OpenGL gl)
        {
            this.m_scene = new AssimpScene(scenePath, sceneFileName, gl);
            this.m_width = width;
            this.m_height = height;
            this.gl = gl;
            m_textures = new uint[m_textureCount];
        }

        /// <summary>
        ///  Destruktor klase World.
        /// </summary>
        ~World()
        {
            this.Dispose(false);
        }

        #endregion Konstruktori

        #region Metode

        /// <summary>
        ///  Korisnicka inicijalizacija i podesavanje OpenGL parametara.
        /// </summary>
        public void Initialize(OpenGL gl)
        {
            gl.Enable(OpenGL.GL_DEPTH_TEST);
            gl.Enable(OpenGL.GL_CULL_FACE);

            gl.LookAt(m_eyeX, m_eyeY, m_eyeZ, m_centerX, m_centerY, m_centerZ, m_upX, m_upY, m_upZ);

            SetupMaterials(gl);
            SetupLighting(gl);
            SetupTextures(gl);

            m_scene.LoadScene();
            m_scene.Initialize();

            cube = new Cube();
            tray = new Cube();
            disk = new Disk
            {
                Loops = 120,
                Slices = 50,
                InnerRadius = 0.2f,
                OuterRadius = 1.0f
            };

            disk.NormalGeneration = Normals.Smooth;

            disk.Material = new SharpGL.SceneGraph.Assets.Material();
            disk.Material.Ambient = Color.FromArgb(32, 64, 64);
            disk.Material.Diffuse = Color.FromArgb(102, 102, 102);
            disk.Material.Specular = Color.FromArgb(198, 198, 198);
            disk.Material.Shininess = 76.8f;
            disk.Material.Bind(gl);

            gl.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);

        }

        /// <summary>
        ///  Iscrtavanje OpenGL kontrole.
        /// </summary>
        public void Draw(OpenGL gl)
        {
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

            gl.FrontFace(OpenGL.GL_CCW);
            gl.Viewport(0, 0, m_width, m_height);
            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.LoadIdentity();
            gl.Perspective(m_fov, (double)m_width / (double)m_height, 1.0, 400.0);

            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.LoadIdentity();
      
            gl.Translate(0.0f, 0.0f, -m_sceneDistance);
            gl.Rotate(m_xRotation, 1.0f, 0.0f, 0.0f);
            gl.Rotate(m_yRotation, 0.0f, 1.0f, 0.0f);

            DrawComputer();
            DrawTableAndGround();

            DrawProjected3DText(gl);
            gl.Flush();
        }

        private void DrawProjected3DText(OpenGL gl)
        {
            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.PushMatrix();
            gl.LoadIdentity();
            if (m_width <= m_height)
            {
                gl.Ortho2D(-m_ortho2dProjection, m_ortho2dProjection, -m_ortho2dProjection * m_height / m_width, m_ortho2dProjection * m_height / m_width);
            }
            else
            {
                gl.Ortho2D(-m_ortho2dProjection * m_width / m_height, m_ortho2dProjection * m_width / m_height, -m_ortho2dProjection, m_ortho2dProjection);
            }
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.PushMatrix();
            gl.LoadIdentity();
            // <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
            gl.Color(1.0, 1.0, 0);
            float ydif = 6.0f;
            float xdif = -0.5f;
            
            //pracenje ivice prozora
            if (m_width <= m_height)
                gl.Translate(m_ortho2dProjection + xdif, -m_ortho2dProjection * m_height / m_width + ydif, 0);
            else
                gl.Translate(m_ortho2dProjection * m_width / m_height + xdif, -m_ortho2dProjection + ydif, 0);

            for(int i =0; i<m_text.Length; i++)
            {
                gl.PushMatrix();
                gl.Translate(-m_textWidth[i], 0, 0); //pomera se od desne ivice prozora za sirinu teksta
                gl.MultMatrix(m_shearMatrix); //dodajem shear matricu na matricni stek (zbog efekta italic)
                gl.DrawText3D("Tahoma", 10, 5, 4, m_text[i]);
                gl.PopMatrix();
                gl.Translate(0, -0.2, 0);
                gl.PushMatrix();
                gl.Translate(-m_textWidth[i], 0, 0);
                gl.Begin(OpenGL.GL_LINES); //iscrtavam liniju ispod teksta
                gl.Vertex(0, 0);
                gl.Vertex(m_textWidth[i], 0);
                gl.End();
                gl.PopMatrix();
                gl.Translate(0, -1, 0);
            }

            // <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
            gl.PopMatrix();
            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.PopMatrix();
        }

        private void DrawTableAndGround()
        {
            gl.PushMatrix();

            gl.Scale(6, 2.0, 3.5);
            gl.Translate(0, m_onTableHeightY, 0);

            //TABLE
            gl.Color(0.63, 0.37, 0.06);

            gl.MatrixMode(OpenGL.GL_TEXTURE);
            gl.PushMatrix();
            //gl.Scale(5, 5, 5);
            gl.Rotate(90, 0, 0, 1);
            gl.Enable(OpenGL.GL_TEXTURE_2D);
            gl.TexEnv(OpenGL.GL_TEXTURE_ENV, OpenGL.GL_TEXTURE_ENV_MODE, OpenGL.GL_MODULATE);
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.Wood]);

            cube.Render(gl, RenderMode.Render);  //sto

            gl.Disable(OpenGL.GL_TEXTURE_2D);
            gl.PopMatrix();

            //GROUND
            gl.Color(0.0, 0.29, 0.56);

            gl.MatrixMode(OpenGL.GL_TEXTURE);
            gl.PushMatrix();
            gl.Scale(5, 5, 5);
            gl.Enable(OpenGL.GL_TEXTURE_2D);
            gl.TexEnv(OpenGL.GL_TEXTURE_ENV, OpenGL.GL_TEXTURE_ENV_MODE, OpenGL.GL_MODULATE);
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.Carpet]);

            gl.Begin(OpenGL.GL_QUADS); // podloga
            gl.Normal(0.0f, 1.0f, 0.0f);
            gl.TexCoord(0, 0);
            gl.Vertex(-5, -1, -5);
            gl.TexCoord(1, 0);
            gl.Vertex(-5, -1, 5);
            gl.TexCoord(1, 1);
            gl.Vertex(5, -1, 5);
            gl.TexCoord(0, 1);
            gl.Vertex(5, -1, -5);
            gl.End();

            gl.Disable(OpenGL.GL_TEXTURE_2D);
            gl.PopMatrix();
            
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.PopMatrix();
        }

        private void DrawComputer()
        {
            gl.PushMatrix();
            gl.Translate(m_computerOffsetX, 0.0f, 0.0f);
            gl.Scale(m_computerScaleFactor, m_computerScaleFactor, m_computerScaleFactor);

            DrawPC();
            if(animationHandler == null ? false : animationHandler.IsWorking())
                DrawScreen();
            DrawTray();
            DrawCD();

            gl.PopMatrix();
        }

        private void DrawPC()
        {
            gl.PushMatrix();
            //Iscrtaj kompjuter
            gl.Translate(1, 0, 0);
            gl.Rotate(90, 1, 0, 0);
            gl.Rotate(-90, 0, 0, 1);
            gl.Scale(5, 5, 5);
            m_scene.Draw();
            gl.PopMatrix();
        }

        private void DrawScreen()
        {
            gl.PushMatrix();
            gl.Color(1.0, 1.0, 1.0);
            gl.Scale(2.8, 1.3, 1);
            gl.Translate(-0.4, 1.2, -1.4);

            gl.MatrixMode(OpenGL.GL_TEXTURE);
            gl.PushMatrix();
            //gl.Scale(5, 5, 5);
            gl.Enable(OpenGL.GL_TEXTURE_2D);
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.Screen]);

            gl.Begin(OpenGL.GL_QUADS); // podloga
            gl.Normal(0.0f, 0.0f, 1.0f);
            gl.TexCoord(0, 0);
            gl.Vertex(-1, -1, 0);
            gl.TexCoord(1, 0);
            gl.Vertex(1, -1, 0);
            gl.TexCoord(1, 1);
            gl.Vertex(1, 1, 0);
            gl.TexCoord(0, 1);
            gl.Vertex(-1, 1, 0);
            gl.End();

            gl.Disable(OpenGL.GL_TEXTURE_2D);
            gl.PopMatrix();

            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.PopMatrix();
        }

        private void DrawTray()
        {
            gl.PushMatrix();
            //Iscrtaj tray
            gl.Color(1.0, 0.0, 0.0);
            gl.Translate(2.9, m_onTableHeightY + 2.6, 0.75 + m_trayOffsetZ);
            gl.Scale(0.55, 0.1, 0.6);
            cube.Render(gl, RenderMode.Render);
            gl.PopMatrix();
        }

        private void DrawCD()
        {
            gl.PushMatrix();
            //Iscrtaj disk
            gl.Color(0.0, 1.0, 1.0);
            gl.Translate(2.9, m_onTableHeightY + 2.6 + 0.11 + m_diskOffsetY, 0.75 + m_diskOffsetZ);
            gl.Scale(0.5, 0.5, 0.5);
            gl.Rotate(-90, 1, 0, 0);
            disk.CreateInContext(gl);
            disk.Render(gl, RenderMode.Render);
            gl.PopMatrix();
        }



        #region Materials
        private void SetupMaterials(OpenGL gl)
        {
            gl.ColorMaterial(OpenGL.GL_FRONT, OpenGL.GL_AMBIENT_AND_DIFFUSE);
            gl.Enable(OpenGL.GL_COLOR_MATERIAL);
        }
        #endregion Materials


        #region Lighting

        private void SetupLighting(OpenGL gl)
        {
            //float[] global_ambient = new float[] { 1.0f, 1.0f, 1.0f, 1.0f };
            //gl.LightModel(OpenGL.GL_LIGHT_MODEL_AMBIENT, global_ambient);

            float[] light0pos = new float[] { 0.0f, 10.0f, -m_sceneDistance, 1.0f };
            float[] light0ambient = new float[] { 1.0f, 1.0f, 1.0f, 1.0f };
            float[] light0diffuse = new float[] { 1.0f, 1.0f, 1.0f, 1.0f };
            float[] light0specular = new float[] { 1.0f, 1.0f, 1.0f, 1.0f };

            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_POSITION, light0pos);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_AMBIENT, light0ambient);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_DIFFUSE, light0diffuse);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_SPECULAR, light0specular);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_SPOT_CUTOFF, 180.0f);
            gl.Enable(OpenGL.GL_LIGHTING);
            gl.Enable(OpenGL.GL_LIGHT0);

            float[] light1pos = new float[] { 0.0f, 1.0f, -m_sceneDistance, 1.0f };
            float[] light1diffuse = new float[] { 1.0f, 0.0f, 0.0f, 1.0f };
            float[] light1specular = new float[] { 1.0f, 0.0f, 0.0f, 1.0f };
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_POSITION, light1pos);
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_AMBIENT, m_light1ambient);
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_DIFFUSE, light1diffuse);
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_SPECULAR, light1specular);
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_SPOT_CUTOFF, 30.0f);
            //gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_SPOT_EXPONENT, 15.0f);
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_SPOT_DIRECTION, new float[] { 0.0f, -1.0f, 0.0f });
            gl.Enable(OpenGL.GL_LIGHT1);

            // Ukljuci automatsku normalizaciju nad normalama
            gl.Enable(OpenGL.GL_NORMALIZE);

        }

        public void SetAmbientLighting(float val, int index)
        {
            if(index >= 0 && index <= 2)
            {
                if (val < 0)
                    val = 0;
                else if (val > 1)
                    val = 1;

                m_light1ambient[index] = val;
                gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_AMBIENT, m_light1ambient);
            }
        }

        #endregion Lighting
        public void Resize(OpenGL gl, int width, int height)
        {
            m_width = width;
            m_height = height;
            gl.Viewport(0, 0, m_width, m_height);
            gl.MatrixMode(OpenGL.GL_PROJECTION);      // selektuj Projection Matrix
            gl.LoadIdentity();
            gl.Perspective(m_fov, (float)m_width / m_height, 1.0f, 400.0f);

            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.LoadIdentity();                // resetuj ModelView Matrix
        }

        #region Textures
        private void SetupTextures(OpenGL gl)
        {
            //gl.Enable(OpenGL.GL_TEXTURE_2D);
            gl.TexEnv(OpenGL.GL_TEXTURE_ENV, OpenGL.GL_TEXTURE_ENV_MODE, OpenGL.GL_MODULATE);

            // Ucitaj slike i kreiraj teksture
            gl.GenTextures(m_textureCount, m_textures);
            for (int i = 0; i < m_textureCount; ++i)
            {
                // Pridruzi teksturu odgovarajucem identifikatoru
                gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[i]);

                // Ucitaj sliku i podesi parametre teksture
                Bitmap image = new Bitmap(m_textureFiles[i]);
                // rotiramo sliku zbog koordinantog sistema opengl-a
                image.RotateFlip(RotateFlipType.RotateNoneFlipY);
                Rectangle rect = new Rectangle(0, 0, image.Width, image.Height);
                // RGBA format (dozvoljena providnost slike tj. alfa kanal)
                BitmapData imageData = image.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadOnly,
                                                      System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                gl.Build2DMipmaps(OpenGL.GL_TEXTURE_2D, (int)OpenGL.GL_RGBA8, image.Width, image.Height, OpenGL.GL_BGRA, OpenGL.GL_UNSIGNED_BYTE, imageData.Scan0);
                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MIN_FILTER, OpenGL.GL_NEAREST);		// Nearest Filtering
                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MAG_FILTER, OpenGL.GL_NEAREST);		// Nearest Filtering
                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_S, OpenGL.GL_REPEAT);
                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_T, OpenGL.GL_REPEAT);

                image.UnlockBits(imageData);
                image.Dispose();
            }
        }
        #endregion Textures


        /// <summary>
        ///  Implementacija IDisposable interfejsa.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                m_scene.Dispose();
            }
        }

        #endregion Metode

        #region IDisposable metode

        /// <summary>
        ///  Dispose metoda.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable metode
    }
}
