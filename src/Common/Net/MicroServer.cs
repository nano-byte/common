// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using NanoByte.Common.Streams;

namespace NanoByte.Common.Net
{
    /// <summary>
    /// Provides a minimalistic HTTP web server that can provide only a single file. Useful for testing download code.
    /// </summary>
    public sealed class MicroServer : IDisposable
    {
        #region Constants
        /// <summary>The lowest port the server tries listening on.</summary>
        private const int MinimumPort = 50222;

        /// <summary>The highest port the server tries listening on.</summary>
        private const int MaxmimumPort = 50734;
        #endregion

        /// <summary>
        /// The URL under which the server root can be reached. Usually you should use <see cref="FileUri"/> instead.
        /// </summary>
        public Uri ServerUri { get; }

        /// <summary>
        /// The complete URL under which the server provides its file.
        /// </summary>
        public Uri FileUri { get; }

        /// <summary>
        /// The content of the file to be served under <see cref="FileUri"/>.
        /// </summary>
        public Stream FileContent { get; private set; }

        /// <summary>
        /// Wait for twenty seconds every time before finishing a response.
        /// </summary>
        public bool Slow { get; set; }

        private readonly string _resourceName;
        private readonly HttpListener _listener;

        /// <summary>
        /// Starts a HTTP web server that listens on a random port.
        /// </summary>
        /// <param name="resourceName">The HTTP resource name under which to provide the content.</param>
        /// <param name="fileContent">The content of the file to serve.</param>
        [SuppressMessage("Microsoft.Usage", "CA2234:PassSystemUriObjectsInsteadOfStrings")]
        public MicroServer([Localizable(false)] string resourceName, Stream fileContent)
        {
            _resourceName = resourceName;
            FileContent = fileContent;

            _listener = StartListening();
            ServerUri = new Uri(_listener.Prefixes.Last());
            FileUri = new Uri(ServerUri, resourceName);

            ThreadUtils.StartBackground(ListenLoop, name: "MicroServer.Listen");
        }

        private static HttpListener StartListening()
        {
            int port = MinimumPort;

            // Keep incrementing port number until we find a free one
            while (true)
            {
                try
                {
                    string prefix = "http://localhost:" + port++ + "/";
                    var listener = new HttpListener();
                    listener.Prefixes.Add(prefix);
                    listener.Start();
                    return listener;
                }
                catch (HttpListenerException) when (port <= MaxmimumPort)
                {}
                catch (SocketException) when (port <= MaxmimumPort)
                {}
            }
        }

        /// <summary>
        /// Stops listening for incoming HTTP connections.
        /// </summary>
        public void Dispose() => _listener.Close();

        /// <summary>
        /// Waits for HTTP requests and responds to them if they ask for "file".
        /// </summary>
        private void ListenLoop()
        {
            while (_listener.IsListening)
            {
                try
                {
                    var context = _listener.GetContext();
                    HandleRequest(context);
                    context.Response.OutputStream.Close();
                }
                #region Error handling
                catch (HttpListenerException)
                {
                    return;
                }
                catch (InvalidOperationException)
                {
                    return;
                }
                #endregion
            }
        }

        private void HandleRequest(HttpListenerContext context)
        {
            // Only return one specific file
            if (context.Request.RawUrl != "/" + _resourceName)
            {
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                return;
            }

            // Delay finishing the file transfer if Slow-mode is active
            if (Slow) Thread.Sleep(20000);

            switch (context.Request.HttpMethod)
            {
                case "GET":
                    context.Response.ContentLength64 = FileContent.Length;
                    FileContent.CopyToEx(context.Response.OutputStream);
                    break;

                case "PUT":
                    FileContent = new MemoryStream();
                    context.Request.InputStream.CopyToEx(FileContent);
                    break;

                default:
                    context.Response.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
                    break;
            }
        }
    }
}
