namespace MercFACTUpload
{
    partial class ProjectInstaller
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.MercfactUploadInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.MercFactUpload = new System.ServiceProcess.ServiceInstaller();
            // 
            // MercfactUploadInstaller
            // 
            this.MercfactUploadInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.MercfactUploadInstaller.Password = null;
            this.MercfactUploadInstaller.Username = null;
            // 
            // MercFactUpload
            // 
            this.MercFactUpload.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.MercfactUploadInstaller});
            this.MercFactUpload.ServiceName = "MercFactUpload";
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.MercfactUploadInstaller, 
            this.MercFactUpload});

        }

        #endregion

        private System.ServiceProcess.ServiceInstaller MercFactUpload;
        public System.ServiceProcess.ServiceProcessInstaller MercfactUploadInstaller;
    }
}