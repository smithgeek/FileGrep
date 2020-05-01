using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Windows.Forms;

namespace GrepWindows
{
    /// <summary>
    /// A bindable CheckedListBox
    /// </summary>
    [ToolboxBitmap(typeof(CheckedListBox))]
    public class CheckedListBoxBindable : CheckedListBox
    {
        private bool mCanCheck;
        public bool CheckOnCheckboxOnly { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public CheckedListBoxBindable()
        {
            CheckOnCheckboxOnly = true;
            mCanCheck = true;
            this.MouseDown += new MouseEventHandler(CheckedListBoxBindable_MouseDown);
            this.MouseUp += new MouseEventHandler(CheckedListBoxBindable_MouseUp);
            this.ItemCheck += new ItemCheckEventHandler(CheckedListBoxBindable_ItemCheck);
        }

        /// <summary>
        /// Handles when the mouse button is released.
        /// </summary>
        void CheckedListBoxBindable_MouseUp(object sender, MouseEventArgs e)
        {
            mCanCheck = true;
        }

        /// <summary>
        /// Handles when the mouse button is pressed down.  Checks if the click was on top
        /// of the checkbox or some other part of the list item.  If it's not on the checkbox
        /// the item is not checked.
        /// </summary>
        void CheckedListBoxBindable_MouseDown(object sender, MouseEventArgs e)
        {
            mCanCheck = false;
            Point loc = this.PointToClient(Cursor.Position);
            for (int i = 0; i < this.Items.Count; i++)
            {
                Rectangle rec = this.GetItemRectangle(i);
                rec.Width = 16; //checkbox itself has a default width of about 16 pixels

                if (rec.Contains(loc))
                {
                    mCanCheck = true;
                    bool newValue = !this.GetItemChecked(i);
                    this.SetItemChecked(i, newValue);//check 
                    mCanCheck = false;
                    break;
                }
            }
        }

        /// <summary>
        /// Handles when a list item is checked.  Updates the binding list.
        /// </summary>
        void CheckedListBoxBindable_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (!CheckOnCheckboxOnly || mCanCheck)
            {
                if (null != mBindingList)
                {
                    mCanCheck = false;
                    mBindingList[e.Index].Checked = e.NewValue == CheckState.Checked;
                    mCanCheck = true;
                }
            }
            else
            {
                e.NewValue = e.CurrentValue;
            }
        }

        /// <summary>
        /// Gets the item hight that should be used.
        /// </summary>
        public override int ItemHeight
        {
            get
            {
                return base.ItemHeight + 2;
            }
            set
            {
                base.ItemHeight = value;
            }
        }

        /// <summary>
        /// The binding list used to popuplate the list.
        /// </summary>
        private SerializableBindingList<CheckBoxModel> mBindingList;

        /// <summary>
        /// The public interface to the underlying binding list.
        /// </summary>
        public SerializableBindingList<CheckBoxModel> BindingList
        {
            get
            {
                return mBindingList;
            }
            set
            {
                this.Items.Clear();
                mBindingList = value;
                if (null != mBindingList)
                {
                    mBindingList.ListChanged += new ListChangedEventHandler(mBindingList_ListChanged);
                    int index = 0;
                    foreach (CheckBoxModel model in mBindingList)
                    {
                        this.Items.Add(model.Text);
                        this.SetItemChecked(index++, model.Checked);
                    }
                }
            }
        }

        /// <summary>
        /// Handles when the binding list changes somehow.  Makes sure the UI and list stay in sync.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mBindingList_ListChanged(object sender, ListChangedEventArgs e)
        {
            if (e.ListChangedType == ListChangedType.ItemAdded)
            {
                CheckBoxModel newCheckbox = mBindingList[e.NewIndex];
                this.Items.Add(newCheckbox.Text);
                this.SetItemChecked(e.NewIndex, newCheckbox.Checked);
            }
            else if (e.ListChangedType == ListChangedType.ItemChanged)
            {
                CheckBoxModel newCheckbox = mBindingList[e.NewIndex];
                this.Items[e.NewIndex] = newCheckbox.Text;
                this.SetItemChecked(e.NewIndex, newCheckbox.Checked);
            }
            else if (e.ListChangedType == ListChangedType.ItemDeleted)
            {
                this.Items.RemoveAt(e.NewIndex);
            }
            else if (e.ListChangedType == ListChangedType.ItemMoved)
            {
                this.Items.RemoveAt(e.OldIndex);
                this.Items.Insert(e.NewIndex, mBindingList[e.NewIndex].Text);
                this.SetItemChecked(e.NewIndex, mBindingList[e.NewIndex].Checked);
            }
            else if (e.ListChangedType == ListChangedType.Reset)
            {
                this.Items.Clear();
                int index = 0;
                foreach (CheckBoxModel model in mBindingList)
                {
                    this.Items.Add(model.Text);
                    this.SetItemChecked(index++, model.Checked);
                }
            }
        }
    }

    /// <summary>
    /// Model for checkbox list items.
    /// </summary>
    [Serializable]
    public class CheckBoxModel : BasePropertyChanged
    {
        /// <summary>
        /// The text to display.
        /// </summary>
        private String mText;

        /// <summary>
        /// Auto notify property text to display.
        /// </summary>
        public String Text
        {
            get { return mText; }
            set { SetField(ref mText, value, "Text"); }
        }

        /// <summary>
        /// If the item is checked.
        /// </summary>
        private bool mChecked;

        /// <summary>
        /// Auto notify preoprty checked item.
        /// </summary>
        public bool Checked
        {
            get { return mChecked; }
            set { SetField(ref mChecked, value, "Checked"); }
        }

        /// <summary>
        /// Constructor that sets the text and checked state.
        /// </summary>
        /// <param name="text">Text to display in list.</param>
        /// <param name="isChecked">If the item should initially be checked.</param>
        public CheckBoxModel(String text, bool isChecked)
        {
            Text = text;
            Checked = isChecked;
        }
    }
}
