

namespace CMSXtream.Pages.DataEntry
{
    public class ClassGroup
    {
        public int CLS_ID { get; set; }
        public string CLS_NAME { get; set; }
        public double CLS_FEE { get; set; }
        public int IS_CLASS_FLG { get; set; }

        public ClassGroup(int id, string name, double amount, int isclass)
        {
            this.CLS_ID = id;
            this.CLS_NAME = name;
            this.CLS_FEE = amount;
            this.IS_CLASS_FLG = isclass;
        }
    }
}
