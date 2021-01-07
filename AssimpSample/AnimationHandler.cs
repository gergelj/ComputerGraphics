using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;

namespace AssimpSample
{
    public enum AnimationState
    {
        READY,
        TRAY_OPEN_WITHOUT_CD,
        CD_IN,
        TRAY_CLOSE_WITH_CD,
        WORKING,
        TRAY_OPEN_WITH_CD,
        CD_OUT,
        TRAY_CLOSE_WITHOUT_CD,
        END
    }
    public class AnimationHandler
    {
        private AnimationState state;
        private World world;
        private DispatcherTimer trayOpenWithoutCdTimer = new DispatcherTimer();
        private DispatcherTimer cdInTimer = new DispatcherTimer();
        private DispatcherTimer trayCloseWithCdTimer = new DispatcherTimer();
        private DispatcherTimer workingTimer = new DispatcherTimer();
        private DispatcherTimer trayOpenWithCdTimer = new DispatcherTimer();
        private DispatcherTimer cdOutTimer = new DispatcherTimer();
        private DispatcherTimer trayCloseWithoutCdTimer = new DispatcherTimer();

        private EventHandler notifyAnimationProperty;
        public AnimationHandler(World w, EventHandler eventH)
        {
            this.state = AnimationState.READY;
            this.world = w;

            trayOpenWithoutCdTimer.Interval = TimeSpan.FromMilliseconds(20);
            trayOpenWithoutCdTimer.Tick += new EventHandler(TrayOpenWithoutCd);

            cdInTimer.Interval = TimeSpan.FromMilliseconds(20);
            cdInTimer.Tick += new EventHandler(CdIn);

            trayCloseWithCdTimer.Interval = TimeSpan.FromMilliseconds(20);
            trayCloseWithCdTimer.Tick += new EventHandler(TrayCloseWithCd);

            workingTimer.Interval = TimeSpan.FromSeconds(5);
            workingTimer.Tick += new EventHandler(Working);

            trayOpenWithCdTimer.Interval = TimeSpan.FromMilliseconds(20);
            trayOpenWithCdTimer.Tick += new EventHandler(TrayOpenWithCd);

            cdOutTimer.Interval = TimeSpan.FromMilliseconds(20);
            cdOutTimer.Tick += new EventHandler(CdOut);

            trayCloseWithoutCdTimer.Interval = TimeSpan.FromMilliseconds(20);
            trayCloseWithoutCdTimer.Tick += new EventHandler(TrayCloseWithoutCd);

            this.notifyAnimationProperty = eventH;
        }

        public void Start()
        {
            if(state == AnimationState.READY)
            {
                state = AnimationState.TRAY_OPEN_WITHOUT_CD;
                trayOpenWithoutCdTimer.Start();
                world.IsBeingAnimated = true;
                notifyAnimationProperty.Invoke(null, null);
            }
        }

        private void TrayOpenWithoutCd(object sender, EventArgs e)
        {
            if(state == AnimationState.TRAY_OPEN_WITHOUT_CD)
            {
                world.DiskOffsetZ += 0.05;
                world.TrayOffsetZ += 0.05;

                if(world.DiskOffsetZ >= 1.2)
                {
                    trayOpenWithoutCdTimer.Stop();
                    this.state = AnimationState.CD_IN;
                    cdInTimer.Start();
                }
            }
        }

        private void CdIn(object sender, EventArgs e)
        {
            if (state == AnimationState.CD_IN)
            {
                world.DiskOffsetY -= 0.05;

                if (world.DiskOffsetY <= 0)
                {
                    cdInTimer.Stop();
                    this.state = AnimationState.TRAY_CLOSE_WITH_CD;
                    trayCloseWithCdTimer.Start();
                }
            }
        }

        private void TrayCloseWithCd(object sender, EventArgs e)
        {
            if (state == AnimationState.TRAY_CLOSE_WITH_CD)
            {
                world.DiskOffsetZ -= 0.05;
                world.TrayOffsetZ -= 0.05;

                if (world.DiskOffsetZ <= 0)
                {
                    trayOpenWithCdTimer.Stop();
                    this.state = AnimationState.WORKING;
                    workingTimer.Start();
                }
            }
        }

        private void Working(object sender, EventArgs e)
        {
            if(state == AnimationState.WORKING)
            {
                workingTimer.Stop();
                this.state = AnimationState.TRAY_OPEN_WITH_CD;
                trayOpenWithCdTimer.Start();
            }
        }

        private void TrayOpenWithCd(object sender, EventArgs e)
        {
            if (state == AnimationState.TRAY_OPEN_WITH_CD)
            {
                world.DiskOffsetZ += 0.05;
                world.TrayOffsetZ += 0.05;

                if (world.DiskOffsetZ >= 1.2)
                {
                    trayOpenWithCdTimer.Stop();
                    this.state = AnimationState.CD_OUT;
                    cdOutTimer.Start();
                }
            }
        }

        private void CdOut(object sender, EventArgs e)
        {
            if (state == AnimationState.CD_OUT)
            {
                world.DiskOffsetY += 0.05;

                if (world.DiskOffsetY >= 1.3)
                {
                    cdOutTimer.Stop();
                    this.state = AnimationState.TRAY_CLOSE_WITHOUT_CD;
                    trayCloseWithoutCdTimer.Start();
                }
            }
        }

        private void TrayCloseWithoutCd(object sender, EventArgs e)
        {
            if (state == AnimationState.TRAY_CLOSE_WITHOUT_CD)
            {
                world.DiskOffsetZ -= 0.05;
                world.TrayOffsetZ -= 0.05;

                if (world.DiskOffsetZ <= 0)
                {
                    trayCloseWithoutCdTimer.Stop();
                    this.state = AnimationState.END;
                    world.IsBeingAnimated = false;
                    notifyAnimationProperty.Invoke(null, null);
                }
            }
        }

        public Boolean IsWorking() => state == AnimationState.WORKING;
        
    }
}
