namespace Soaring.AdminWeb.Wrappers {
    public class UtilWrapper {
        private UtilWrapper _instance = null;
        private UtilWrapper () { }
        public UtilWrapper Instance {
            get {
                if (this._instance == null) {
                    this._instance = new UtilWrapper ();
                }
                return this._instance;
            }
        }
        public string GetFilePath(string path){
            return path;
        }
    }
}