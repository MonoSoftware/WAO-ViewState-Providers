using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Threading;
using MonoSoftware.Web.WAO.Base;
using System.Web;

namespace MonoSoftware.Web.WAO.Providers
{
    /// <summary>
    /// ViewState Provider that stores view state in asp.net session.
    /// </summary>
    public class SessionProvider : BasePageStatePersister
    {
        #region Ctor
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="page">The System.Web.UI.Page that the view state persistence mechanism is created for.</param>
        public SessionProvider(Page page)
            : base(page)
        {
            
        }

        #endregion

        #region Methods
        /// <summary>
        /// Load page state.
        /// </summary>
        public override void Load()
        {
            Pair state = HttpContext.Current.Session[base.LoadStateKey] as Pair;

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

            //Add view state and control state to session
            HttpContext.Current.Session[base.StateKey] = state;

            base.Save();
        }
        #endregion
    }
}
