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
    internal sealed class Format2SQLCommand
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 4130;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("862c1b50-2c84-49b0-ad31-9e9f56c00176");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly AsyncPackage package;

        /// <summary>
        /// Initializes a new instance of the <see cref="Format2SQLCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        /// <param name="commandService">Command service to add command to, not null.</param>
        private Format2SQLCommand(AsyncPackage package, OleMenuCommandService commandService)
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
        public static Format2SQLCommand Instance
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
            // Switch to the main thread - the call to AddCommand in Format2SQL's constructor requires
            // the UI thread.
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            OleMenuCommandService commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            Instance = new Format2SQLCommand(package, commandService);
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
            //string title = "Format2SQL";

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

                textSelection.Text = GenName(textSelection.Text);
            }
            catch (Exception)
            {

            }
        }

        private string GenName(string text)
        {
            StringBuilder result = new StringBuilder();
            if (!string.IsNullOrWhiteSpace(text))
            {
                result.Append(text[0]);
            }
            for (int i = 1; i < text.Length; i++)
            {
                if (text[i] >= 65 && text[i] <= (65 + 25))
                {
                    //大写之前加上_
                    result.Append("_");
                }

                //小写转大写
                if (text[i] >= 65 + 32 && text[i] <= (65 + 25 + 32))
                {
                    result.Append((char)(text[i] - 32));
                }
                else
                {
                    result.Append(text[i]);
                }


            }

            return result.ToString();
        }
    }
}
