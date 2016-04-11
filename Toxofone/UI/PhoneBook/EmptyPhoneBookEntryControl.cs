namespace Toxofone.UI.PhoneBook
{
    using System;
    using System.Windows.Forms;

    public partial class EmptyPhoneBookEntryControl : UserControl
    {
        public EmptyPhoneBookEntryControl()
        {
            InitializeComponent();
            UpdateComponent();
        }

        public delegate void AddNewEntry();

        public event AddNewEntry OnAddNewEntry;

        private void UpdateComponent()
        {
            this.addEntry.PictureClick += this.OnAddEntryClick;
        }

        private void OnAddEntryClick(object sender, EventArgs e)
        {
            AddNewEntry handler = this.OnAddNewEntry;
            if (handler != null)
            {
                handler();
            }
        }
    }
}
