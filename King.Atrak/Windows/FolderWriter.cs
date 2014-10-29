﻿namespace King.ATrak.Windows
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Folder Writer
    /// </summary>
    public class FolderWriter : IDataWriter
    {
        #region Members
        /// <summary>
        /// Folder to
        /// </summary>
        protected readonly string to = null;
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="to">To</param>
        public FolderWriter(string to)
        {
            if (string.IsNullOrWhiteSpace(to))
            {
                throw new ArgumentException("to");
            }

            this.to = to;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Initialize Folder
        /// </summary>
        public virtual async Task<bool> Initialize()
        {
            if (!Directory.Exists(this.to))
            {
                Directory.CreateDirectory(this.to);

                Trace.TraceInformation("Created directory: '{0}'.", this.to);
            }

            return await Task.FromResult(true);
        }

        /// <summary>
        /// Store Items
        /// </summary>
        /// <param name="items">Items</param>
        public virtual async Task Store(IEnumerable<IStorageItem> items)
        {
            foreach (var item in items)
            {
                var path = item.RelativePath.Replace("/", "\\");
                var folders = path.Split('\\');
                if (1 < folders.Count())
                {
                    var pathcreate = this.to;
                    for (var i = 0; i < folders.Count(); i++)
                    {
                        Directory.CreateDirectory(pathcreate);
                        pathcreate = string.Format("{0}\\{1}", pathcreate, folders.ElementAt(i));
                    }
                }

                await item.LoadMD5();
                await item.Load();

                path = Path.Combine(this.to, path);
                Trace.TraceInformation("Writing to file: '{0}'.", path);
                File.WriteAllBytes(path, item.Data);
            }
        }
        #endregion
    }
}