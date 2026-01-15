dotnet watch 🔥 Hot reload enabled. For a list of supported edits, see https://aka.ms/dotnet/hot-reload.
dotnet watch 💡 Press Ctrl+R to restart.
dotnet watch 🔨 Building C:\Projects\ccc\ccc.csproj ...
dotnet watch 🔨 Build succeeded: C:\Projects\ccc\ccc.csproj
Unhandled exception. System.Windows.Markup.XamlParseException: 'The invocation of the constructor on type 'CefSharp.Wpf.ChromiumWebBrowser' that matches the specified binding constraints threw an exception.' Line number '21' and line position '10'.
 ---> System.IO.FileNotFoundException: Could not load file or assembly 'C:\Projects\ccc\bin\Debug\net10.0-windows\runtimes\win-x64\lib\net6.0\CefSharp.Core.Runtime.dll'. The specified module could not be found.
File name: 'C:\Projects\ccc\bin\Debug\net10.0-windows\runtimes\win-x64\lib\net6.0\CefSharp.Core.Runtime.dll'
   at CefSharp.Cef.get_IsInitialized() in C:\projects\cefsharp\CefSharp.Core\Cef.cs:line 60
   at CefSharp.Cef.get_IsInitialized() in C:\projects\cefsharp\CefSharp.Core\Cef.cs:line 60
   at CefSharp.Wpf.ChromiumWebBrowser.InitializeCefInternal() in C:\projects\cefsharp\CefSharp\Internals\Partial\ChromiumWebBrowser.Partial.cs:line 522       
   at CefSharp.Wpf.ChromiumWebBrowser.NoInliningConstructor() in C:\projects\cefsharp\CefSharp.Wpf\ChromiumWebBrowser.cs:line 555
   at CefSharp.Wpf.ChromiumWebBrowser..ctor() in C:\projects\cefsharp\CefSharp.Wpf\ChromiumWebBrowser.cs:line 491
   at System.RuntimeType.CreateInstanceDefaultCtor(Boolean publicOnly, Boolean wrapExceptions)
   --- End of inner exception stack trace ---
   at System.Windows.Markup.XamlReader.RewrapException(Exception e, IXamlLineInfo lineInfo, Uri baseUri)
   at System.Windows.Markup.WpfXamlLoader.Load(XamlReader xamlReader, IXamlObjectWriterFactory writerFactory, Boolean skipJournaledProperties, Object rootObject, XamlObjectWriterSettings settings, Uri baseUri)
   at System.Windows.Markup.WpfXamlLoader.LoadBaml(XamlReader xamlReader, Boolean skipJournaledProperties, Object rootObject, XamlAccessLevel accessLevel, Uri baseUri)
   at System.Windows.Markup.XamlReader.LoadBaml(Stream stream, ParserContext parserContext, Object parent, Boolean closeStream)
   at System.Windows.Application.LoadBamlStreamWithSyncInfo(Stream stream, ParserContext pc)
   at System.Windows.Application.DoStartup()
   at System.Windows.Application.<.ctor>b__1_0(Object unused)
   at System.Windows.Threading.ExceptionWrapper.InternalRealCall(Delegate callback, Object args, Int32 numArgs)
   at System.Windows.Threading.ExceptionWrapper.TryCatchWhen(Object source, Delegate callback, Object args, Int32 numArgs, Delegate catchHandler)
   at System.Windows.Threading.DispatcherOperation.InvokeImpl()
   at MS.Internal.CulturePreservingExecutionContext.CallbackWrapper(Object obj)
   at System.Threading.ExecutionContext.RunInternal(ExecutionContext executionContext, ContextCallback callback, Object state)
--- End of stack trace from previous location ---
   at System.Threading.ExecutionContext.RunInternal(ExecutionContext executionContext, ContextCallback callback, Object state)
   at MS.Internal.CulturePreservingExecutionContext.Run(CulturePreservingExecutionContext executionContext, ContextCallback callback, Object state)
   at System.Windows.Threading.DispatcherOperation.Invoke()
   at System.Windows.Threading.Dispatcher.ProcessQueue()
   at System.Windows.Threading.Dispatcher.WndProcHook(IntPtr hwnd, Int32 msg, IntPtr wParam, IntPtr lParam, Boolean& handled)
   at MS.Win32.HwndWrapper.WndProc(IntPtr hwnd, Int32 msg, IntPtr wParam, IntPtr lParam, Boolean& handled)
   at MS.Win32.HwndSubclass.DispatcherCallbackOperation(Object o)
   at System.Windows.Threading.ExceptionWrapper.InternalRealCall(Delegate callback, Object args, Int32 numArgs)
   at System.Windows.Threading.ExceptionWrapper.TryCatchWhen(Object source, Delegate callback, Object args, Int32 numArgs, Delegate catchHandler)
   at System.Windows.Threading.Dispatcher.LegacyInvokeImpl(DispatcherPriority priority, TimeSpan timeout, Delegate method, Object args, Int32 numArgs)        
   at System.Windows.Threading.Dispatcher.Invoke(DispatcherPriority priority, Delegate method, Object arg)
   at MS.Win32.HwndSubclass.SubclassWndProc(IntPtr hwnd, Int32 msg, IntPtr wParam, IntPtr lParam)
   at MS.Win32.UnsafeNativeMethods.DispatchMessage(MSG& msg)
   at System.Windows.Threading.Dispatcher.PushFrameImpl(DispatcherFrame frame)
   at System.Windows.Application.RunDispatcher(Object ignore)
   at System.Windows.Application.RunInternal(Window window)
   at ccc.App.Main()
dotnet watch ❌ [ccc (net10.0-windows)] Exited with error code -532462766