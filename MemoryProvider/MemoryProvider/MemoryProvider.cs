using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Threading;
using MonoSoftware.Web.WAO.Base;

namespace MonoSoftware.Web.WAO.Providers
{
    /// <summary>
    /// ViewState Provider that stores view state in asp.net memory (asp.net process).
    /// </summary>
    public class MemoryProvider : BasePageStatePersister
    {
        #region Fields
        private static readonly object padLock = new object();
        private static readonly object padLockBG = new object();
        private static Thread bgWorker = null;
        private static MemoryItemList Items = new MemoryItemList(); 
        #endregion

        #region Properties
        private int _viewStateTimeout = 20;
        /// <summary>
        /// Get or set view state timeout in minutes
        /// </summary>
        public int ViewStateTimeout
        {
            get { return _viewStateTimeout; }
            set { _viewStateTimeout = value; }
        }

        private int _recycleTime = 60;
        /// <summary>
        /// Get or set recycle time in minutes
        /// </summary>
        public int RecycleTime
        {
            get { return _recycleTime; }
            set { _recycleTime = value; }
        }
        #endregion

        #region Ctor
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="page">The System.Web.UI.Page that the view state persistence mechanism is created for.</param>
        public MemoryProvider(Page page)
            : base(page)
        {
            //Static -> So only one instance is triggered per application 
            if (bgWorker == null)
                lock (padLockBG)
                {
                    bgWorker = new Thread(new ThreadStart(bgWorker_DoWork));
                    bgWorker.IsBackground = true;
                    bgWorker.Priority = ThreadPriority.Lowest;
                    bgWorker.Start();
                }
        }

        void bgWorker_DoWork()
        {
            while (true)
            {
                lock (padLock)
                {
                    List<MemoryItem> pendingRemoval = new List<MemoryItem>();
                    foreach (MemoryItem item in Items)
                    {
                        if (item.IsItemExpired())
                            pendingRemoval.Add(item);
                    }
                    foreach (MemoryItem item in pendingRemoval)
                    {
                        Items.Remove(item);
                        item.Dispose();
                    }
                }

                //Wait for recycleTime mins
                Thread.Sleep(TimeSpan.FromMinutes(RecycleTime));
            }
        }

        #endregion

        #region Methods
        /// <summary>
        /// Load page state.
        /// </summary>
        public override void Load()
        {
            Pair state = null;

            lock (padLock)
            {
                state = Items[base.LoadStateKey].Value;
            }

            if (state != null)
            {
                //Set view state and control state
                ViewState = state.First;
                ControlState = state.Second;
            }
        }

        /// <summary>
        /// Save page state.
        /// </summary>
        public override void Save()
        {
            //No processing needed if no state available
            if (ViewState == null & ControlState == null)
            {
                return;
            }

            //Save view state and control state separately
            Pair state = new Pair(ViewState, ControlState);

            //Add view state and control state to memory
            lock (padLock)
            {
                MemoryItem item = new MemoryItem(ViewStateTimeout, base.StateKey, state);
                Items.Add(item);
            }

            base.Save();
        }
        #endregion
    }
}
