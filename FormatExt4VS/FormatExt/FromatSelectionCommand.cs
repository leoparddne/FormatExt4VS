using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.ComponentModel.Design;
using System.Globalization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

namespace FormatExt
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class FromatSelectionCommand
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 4129;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("862c1b50-2c84-49b0-ad31-9e9f56c00176");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly AsyncPackage package;

        /// <summary>
        /// Initializes a new instance of the <see cref="FromatSelectionCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        /// <param name="commandService">Command service to add command to, not null.</param>
        private FromatSelectionCommand(AsyncPackage package, OleMenuCommandService commandService)
        {
            this.package = package ?? throw new ArgumentNullException(nameof(package));
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            var menuCommandID = new CommandID(CommandSet, CommandId);
            var menuItem = new MenuCommand(this.Execute, menuCommandID);
            commandService.AddCommand(menuItem);
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static FromatSelectionCommand Instance
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private Microsoft.VisualStudio.Shell.IAsyncServiceProvider ServiceProvider
        {
            get
            {
                return this.package;
            }
        }

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static async Task InitializeAsync(AsyncPackage package)
        {
            // Switch to the main thread - the call to AddCommand in FromatSelectionCommand's constructor requires
            // the UI thread.
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            OleMenuCommandService commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            Instance = new FromatSelectionCommand(package, commandService);
        }

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private async void Execute(object sender, EventArgs e)
        {
            //ThreadHelper.ThrowIfNotOnUIThread();
            //string message = string.Format(CultureInfo.CurrentCulture, "Inside {0}.MenuItemCallback()", this.GetType().FullName);
            //string title = "FromatSelectionCommand";

            //// Show a message box to prove we were here
            //VsShellUtilities.ShowMessageBox(
            //    this.package,
            //    message,
            //    title,
            //    OLEMSGICON.OLEMSGICON_INFO,
            //    OLEMSGBUTTON.OLEMSGBUTTON_OK,
            //    OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);

            try
            {
                //await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();


                //var service = await this.ServiceProvider.GetServiceAsync<SVsCmdNameMapping, IVsCmdNameMapping>();
                //service.MapNameToGUIDID("Edit.FormatSelection", out Guid pguidCmdGroup, out uint pdwCmdID);
                //var cmdId = Convert.ToInt32(pdwCmdID);
                //var commandID = new CommandID(pguidCmdGroup, cmdId);


                //var commandService = await this.ServiceProvider.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;

                //commandService.GlobalInvoke(commandID);

                //2.
                //TextSelection selectedText = _vs.ActiveDocument.Selection as TextSelection; //获取选择的文本对象

                ////string copyInfo = AddInHelper.Read();   //读取版权配置信息

                ////copyInfo = copyInfo.Replace("@time", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));//替换时间点位符

                //selectedText.Text = copyInfo.ToLower();   //覆盖选择文本

                string selectedText = string.Empty;
                //DTE dte = this.GetService(typeof(DTE) as DTE;

                var dte = await this.ServiceProvider.GetServiceAsync(typeof(DTE)) as DTE;

                Document doc = dte.ActiveDocument;

                var x = doc.Selection.ToString();

                TextSelection textSelection = doc.Selection as TextSelection;

                //VsShellUtilities.ShowMessageBox(
                //    this.package,
                //    textSelection.Text,
                //    x,
                //    OLEMSGICON.OLEMSGICON_INFO,
                //    OLEMSGBUTTON.OLEMSGBUTTON_OK,
                //    OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);

                textSelection.Text= GenName(textSelection.Text);
            }
            catch (Exception)
            {
                //TODO
            }

        }

        /// <summary>
        /// 驼峰
        /// </summary>
        /// <param name="rawStr"></param>
        /// <returns></returns>
        string GenName(string rawStr, string prevMask = "")
        {
            var tmp = rawStr;
            //移除前缀
            if (!string.IsNullOrWhiteSpace(prevMask))
            {
                if (tmp.StartsWith(prevMask))
                {
                    tmp = tmp.Substring(prevMask.Length);
                }
            }

            StringBuilder result = new StringBuilder();
            bool start = true;

            for (int i = 0; i < tmp.Length; i++)
            {
                if (tmp[i] == '_')
                {
                    start = true;
                    continue;
                }

                //首字母不变
                if (start)
                {
                    result.Append(tmp[i]);
                    start = false;
                    continue;
                }

                //大小转小写
                if (tmp[i] >= 65 && tmp[i] <= (65 + 25))
                {
                    result.Append((char)(tmp[i] + 32));
                }
                else
                {
                    //其他字符原样输出
                    result.Append(tmp[i]);
                }
            }



            return result.ToString();
        }
    }
}
