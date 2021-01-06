using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using SharpGL.SceneGraph;
using SharpGL;
using Microsoft.Win32;
using System.ComponentModel;

namespace AssimpSample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        #region Atributi

        /// <summary>
        ///	 Instanca OpenGL "sveta" - klase koja je zaduzena za iscrtavanje koriscenjem OpenGL-a.
        /// </summary>
        World m_world = null;

        //private double m_diskOffsetY = 0;
        //private double m_diskOffsetZ = 0;

        #endregion Atributi

        public double DiskYOffset
        {
            get
            {
                return m_world.DiskOffsetY;
                //return m_diskOffsetY;
            }
            set 
            {
                m_world.DiskOffsetY = value;
                //m_diskOffsetY = value;
                OnPropertyChanged("DiskYOffset");
            }
        }

        public double DiskZOffset
        {
            get
            {
                return m_world.DiskOffsetZ;
                //return m_diskOffsetZ;
            }
            set
            {
                m_world.DiskOffsetZ = value;
                //m_diskOffsetZ = value;
                OnPropertyChanged("DiskZOffset");
            }
        }

        public double ComputerXOffset
        {
            get
            {
                return m_world.ComputerOffsetX;
            }
            set
            {
                m_world.ComputerOffsetX = value;
                OnPropertyChanged("ComputerXOffset");
            }
        }

        public double ComputerScaleFactor
        {
            get
            {
                return m_world.ComputerScaleFactor;
            }
            set
            {
                m_world.ComputerScaleFactor = value;
                OnPropertyChanged("ComputerScaleFactor");
            }
        }

        public float LightingAmbientR
        {
            get { return m_world.Light1Ambient[0]; }
            set
            {
                float color = ValidColorValue(value);
                m_world.SetAmbientLighting(color, 0);
                OnPropertyChanged("LightingAmbientR");
            }
        }

        public float LightingAmbientG
        {
            get { return m_world.Light1Ambient[1]; }
            set
            {
                float color = ValidColorValue(value);
                m_world.SetAmbientLighting(color, 1);
                OnPropertyChanged("LightingAmbientG");
            }
        }

        public float LightingAmbientB
        {
            get { return m_world.Light1Ambient[2]; }
            set
            {
                float color = ValidColorValue(value);
                m_world.SetAmbientLighting(color, 2);
                OnPropertyChanged("LightingAmbientB");
            }
        }

        public Boolean IsNotBeingAnimated
        {
            get { return !m_world.IsBeingAnimated; }
        }

        #region Konstruktori

        public MainWindow()
        {
            // Inicijalizacija komponenti
            InitializeComponent();
            this.DataContext = this;

            // Kreiranje OpenGL sveta
            try
            {
                m_world = new World(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "3D Models\\computer"), "computador.dae", (int)openGLControl.ActualWidth, (int)openGLControl.ActualHeight, openGLControl.OpenGL);

            }
            catch (Exception e)
            {
                MessageBox.Show("Neuspesno kreirana instanca OpenGL sveta. Poruka greške: " + e.Message, "Poruka", MessageBoxButton.OK);
                this.Close();
            }
        }

        #endregion Konstruktori

        /// <summary>
        /// Handles the OpenGLDraw event of the openGLControl1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="SharpGL.SceneGraph.OpenGLEventArgs"/> instance containing the event data.</param>
        private void openGLControl_OpenGLDraw(object sender, OpenGLEventArgs args)
        {
            m_world.Draw(args.OpenGL);
        }

        /// <summary>
        /// Handles the OpenGLInitialized event of the openGLControl1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="SharpGL.SceneGraph.OpenGLEventArgs"/> instance containing the event data.</param>
        private void openGLControl_OpenGLInitialized(object sender, OpenGLEventArgs args)
        {
            m_world.Initialize(args.OpenGL);
        }

        /// <summary>
        /// Handles the Resized event of the openGLControl1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="SharpGL.SceneGraph.OpenGLEventArgs"/> instance containing the event data.</param>
        private void openGLControl_Resized(object sender, OpenGLEventArgs args)
        {
            m_world.Resize(args.OpenGL, (int)openGLControl.ActualWidth, (int)openGLControl.ActualHeight);
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (!IsNotBeingAnimated)
                return;

            switch (e.Key)
            {
                case Key.F10: this.Close(); break;
                case Key.W:
                    {
                        if (m_world.RotationX > 0 && m_world.RotationX <= 90)
                            m_world.RotationX -= 5.0f; break;
                    }
                case Key.S:
                    {
                        if(m_world.RotationX >= 0 && m_world.RotationX < 90)
                            m_world.RotationX += 5.0f; break;
                    }
                case Key.A: m_world.RotationY -= 5.0f; break;
                case Key.D: m_world.RotationY += 5.0f; break;
                case Key.Add: m_world.SceneDistance -= 2.0f; break;
                case Key.Subtract: m_world.SceneDistance += 2.0f; break;
                case Key.F4: this.Close(); break;
                case Key.F2:
                    OpenFileDialog opfModel = new OpenFileDialog();
                    bool result = (bool) opfModel.ShowDialog();
                    if (result)
                    {

                        try
                        {
                            World newWorld = new World(Directory.GetParent(opfModel.FileName).ToString(), Path.GetFileName(opfModel.FileName), (int)openGLControl.Width, (int)openGLControl.Height, openGLControl.OpenGL);
                            m_world.Dispose();
                            m_world = newWorld;
                            m_world.Initialize(openGLControl.OpenGL);
                        }
                        catch (Exception exp)
                        {
                            MessageBox.Show("Neuspesno kreirana instanca OpenGL sveta:\n" + exp.Message, "GRESKA", MessageBoxButton.OK );
                        }
                    }
                    break;
                case Key.C:
                    AnimationHandler animationHandler = new AnimationHandler(m_world, new EventHandler(UpdateAnimationPropery));
                    animationHandler.Start();
                    break;
            }
        }

        public void UpdateAnimationPropery(object sender, EventArgs e)
        {
            OnPropertyChanged("IsNotBeingAnimated");
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            m_world.Resize(openGLControl.OpenGL, (int)openGLControl.ActualWidth, (int)openGLControl.ActualHeight);
        }

        private void IncreaseDiskY(object sender, RoutedEventArgs e)
        {
            DiskYOffset += 0.1;
        }

        private void DecreaseDiskY(object sender, RoutedEventArgs e)
        {
            DiskYOffset -= 0.1;
        }

        private void IncreaseDiskZ(object sender, RoutedEventArgs e)
        {
            DiskZOffset += 0.1;
        }

        private void DecreaseDiskZ(object sender, RoutedEventArgs e)
        {
            DiskZOffset -= 0.1;
        }

        private void IncreaseComputerX(object sender, RoutedEventArgs e)
        {
            ComputerXOffset += 0.5;
        }

        private void DecreaseComputerX(object sender, RoutedEventArgs e)
        {
            ComputerXOffset -= 0.5;
        }

        private void IncreaseComputerScale(object sender, RoutedEventArgs e)
        {
            ComputerScaleFactor += 0.1;
        }

        private void DecreaseComputerScale(object sender, RoutedEventArgs e)
        {
            ComputerScaleFactor -= 0.1;
        }
        private void SetAmbientLighting(object sender, RoutedEventArgs e)
        {

        }

        private float ValidColorValue(float toValidate)
        {
            if (toValidate < 0)
                return 0;
            else if (toValidate > 1)
                return 1;
            else
                return toValidate;
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #endregion

    }
}
