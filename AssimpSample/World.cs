// -----------------------------------------------------------------------
// <file>World.cs</file>
// <copyright>Grupa za Grafiku, Interakciju i Multimediju 2013.</copyright>
// <author>Srđan Mihić</author>
// <author>Aleksandar Josić</author>
// <summary>Klasa koja enkapsulira OpenGL programski kod.</summary>
// -----------------------------------------------------------------------
using System;
using Assimp;
using System.IO;
using System.Reflection;
using SharpGL.SceneGraph;
using SharpGL.SceneGraph.Primitives;
using SharpGL.SceneGraph.Quadrics;
using SharpGL.SceneGraph.Core;
using SharpGL;

namespace AssimpSample
{


    /// <summary>
    ///  Klasa enkapsulira OpenGL kod i omogucava njegovo iscrtavanje i azuriranje.
    /// </summary>
    public class World : IDisposable
    {

        #region Atributi

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
        private Disk disk;

        private double m_diskOffsetY;
        private double m_diskOffsetZ;

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
            set { Console.WriteLine(m_xRotation);  m_xRotation = value; }
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
            gl.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);

            float[] lineWidthRange = new float[2];
            float[] lineWidthGranularity = new float[1];
            gl.GetFloat(OpenGL.GL_LINE_WIDTH_RANGE, lineWidthRange);
            gl.GetFloat(OpenGL.GL_LINE_WIDTH_GRANULARITY, lineWidthGranularity);
            float lineWidth = lineWidthRange[0];
            gl.LineWidth(lineWidth);

            m_scene.LoadScene();
            m_scene.Initialize();

            cube = new Cube();
            disk = new Disk();
            disk.Loops = 120;
            disk.Slices = 50;
            disk.InnerRadius = 0.2f;
            disk.OuterRadius = 1.0f;

        }

        /// <summary>
        ///  Iscrtavanje OpenGL kontrole.
        /// </summary>
        public void Draw(OpenGL gl)
        {
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.LoadIdentity();
            gl.Perspective(m_fov, (double)m_width / (double)m_height, 1.0, 400.0);

            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.PushMatrix();

            //gl.LookAt(m_eyeX, m_eyeY, m_eyeZ, m_centerX, m_centerY, m_centerZ, m_upX, m_upY, m_upZ);

            gl.Translate(0.0f, 0.0f, -m_sceneDistance);
            gl.Rotate(m_xRotation, 1.0f, 0.0f, 0.0f);
            gl.Rotate(m_yRotation, 0.0f, 1.0f, 0.0f);

            gl.PushMatrix();
            gl.Translate(1, 0, 0);
            gl.Rotate(90, 1, 0, 0);
            gl.Rotate(-90, 0, 0, 1);
            gl.Scale(5, 5, 5);
            m_scene.Draw();
            gl.PopMatrix();

            //TODO: Draw table, podloga i disk
            gl.PushMatrix();
            gl.Scale(6, 2.0, 3.5);
            gl.Translate(0, m_onTableHeightY, 0);

            gl.Color(0, 0.43, 0.11);
            cube.Render(gl, RenderMode.Render);

            gl.Color(0.0, 0.29, 0.56);
            gl.Begin(OpenGL.GL_QUADS);
            gl.Vertex(-5, -1, -5);
            gl.Vertex(-5, -1, 5);
            gl.Vertex(5, -1, 5);
            gl.Vertex(5, -1, -5);
            gl.End();
            gl.PopMatrix();

            gl.PushMatrix();
            //Draw disk
            gl.Color(0.0, 1.0, 1.0);
            gl.Translate(2.9, m_onTableHeightY+0.6+m_diskOffsetY, 2.5+m_diskOffsetZ);
            gl.Scale(0.5, 0.5, 0.5);
            gl.Rotate(-90, 1, 0, 0);
            disk.CreateInContext(gl);
            disk.Render(gl, RenderMode.Render);
            gl.PopMatrix();

            DrawText(gl);

            gl.PopMatrix();
            gl.Flush();
        }

        private void DrawText(OpenGL gl)
        {
            gl.Viewport(0, 0, m_width, m_height);
            
            gl.DrawText(m_width - 140 - m_textMargin, 4 * 20 + m_textMargin, 1, 1, 0, "Tahoma", 10.0f, "Predmet: Racunarska grafika");
            gl.DrawText(m_width - 140 - m_textMargin, 4 * 20 + m_textMargin - 2, 1, 1, 0, "Tahoma", 10.0f, "_______________________");
            gl.DrawText(m_width - 85 - m_textMargin, 3 * 20 + m_textMargin, 1, 1, 0, "Tahoma", 10.0f, "Sk. god: 2020/21.");
            gl.DrawText(m_width - 85 - m_textMargin, 3 * 20 + m_textMargin - 2, 1, 1, 0, "Tahoma", 10.0f, "_____________");
            gl.DrawText(m_width - 59 - m_textMargin, 2 * 20 + m_textMargin, 1, 1, 0, "Tahoma", 10.0f, "Ime: Gergelj");
            gl.DrawText(m_width - 59 - m_textMargin, 2 * 20 + m_textMargin - 2, 1, 1, 0, "Tahoma", 10.0f, "_________");
            gl.DrawText(m_width - 56 - m_textMargin, 20 + m_textMargin, 1, 1, 0, "Tahoma", 10.0f, "Prezime: Kis");
            gl.DrawText(m_width - 56 - m_textMargin, 20 + m_textMargin - 2, 1, 1, 0, "Tahoma", 10.0f, "_________");
            gl.DrawText(m_width - 64 - m_textMargin, m_textMargin, 1, 1, 0, "Tahoma", 10.0f, "Sifra zad: 6.2");
            gl.DrawText(m_width - 64 - m_textMargin, m_textMargin - 2, 1, 1, 0, "Tahoma", 10.0f, "___________");
            
        }


        /// <summary>
        /// Podesava viewport i projekciju za OpenGL kontrolu.
        /// </summary>
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
