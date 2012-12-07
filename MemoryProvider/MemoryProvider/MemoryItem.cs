using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;

namespace MonoSoftware.Web.WAO.Providers
{
    /// <summary>
    /// Memory item.
    /// </summary>
    internal class MemoryItem : IDisposable
    {
        #region Fields
		private DateTime timeoutTime = DateTime.MinValue; 
	    #endregion

        #region Properties
        
        private int _timeout = 20;
        /// <summary>
        /// Gets or sets timeout in minutes.
        /// </summary>
        public int Timeout
        {
            get { return _timeout; }
            set { _timeout = value; }
        }

        private string _key;
        /// <summary>
        /// Gets or sets key.
        /// </summary>
        public string Key
        {
            get { return _key; }
            set { _key = value; }
        }

        private Pair _value;
        /// <summary>
        /// Gets or sets value.
        /// </summary>
        public Pair Value
        {
            get { return _value; }
            set { _value = value; }
        }
	
        #endregion
        
        #region Constructor
        /// <summary>
        /// Constructor.
        /// </summary>
        public MemoryItem()
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="key">View state key</param>
        /// <param name="value">View state value</param>
        public MemoryItem(string key, Pair value)
        {
            _key = key;
            _value = value;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="timeoutTime">View state timeout time</param>
        /// <param name="key">View state key</param>
        /// <param name="value">View state value</param>
        public MemoryItem(int timeout, string key, Pair value)
            : this(key, value)
        {
            _timeout = timeout;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Added to view state used internally.
        /// </summary>
        internal void AddedToViewState()
        {
            timeoutTime = DateTime.Now.AddMinutes(Timeout);
        }

        /// <summary>
        /// Is item expired.
        /// </summary>
        /// <returns>True if expired, False if not</returns>
        public bool IsItemExpired()
        {
            return (DateTime.Now > timeoutTime);
        } 
        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            Key = null;
            Value = null;            
        }

        #endregion
    }
}
