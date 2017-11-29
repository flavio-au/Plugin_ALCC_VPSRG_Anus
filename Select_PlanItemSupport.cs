using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;

namespace VMS.TPS
{
    /// <summary>
    /// A class to open a form with a list of items expressed on radiobutons to choose from.
    /// </summary>
    public class SelectBox
    {
        private GroupBox groupBox1;
        private RadioButton selectedrb;
        private Button getSelectedRB;
        private Form my_form;
        private IWin32Window window;
        private List<String> my_list;
        private String title;
        
        /// <summary>
        /// SelectBox constructor creates a new form instance an sets field values
        /// </summary>
        /// <param name="my_list"></param>
        /// <param name="title"></param>
        public SelectBox(List<String> my_list, String title)
        {
            my_form = new System.Windows.Forms.Form();
            this.my_list = my_list;
            this.title = title;
        }


        /// <summary>
        /// Get_Item() method returns the slected Radiobuton Text propertie
        /// Text properties are set after the list&ltString&gt Strings. 
        /// </summary>
        /// <returns></returns>
        public String Get_Item()
        {
            my_form.Controls.Add(InitializeRadioButtons(my_list,title));
            my_form.ShowDialog(window);

            return selectedrb.Text;
        }

        /// <summary>
        /// Private method for initializa radioButtons after the list&ltString&gt.
        /// String groupTitle defines de title of the group and get button.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="groupTitle"></param>
        /// <returns></returns>
        private GroupBox InitializeRadioButtons(List<String> list, String groupTitle)
        {
            //The default coordinate system for a Graphics object is CoordinateSystem.TEXT
            //the x and y values in a Point object increase, the object proceeds 
            // to the right horizontally and down vertically.

            List<RadioButton> rdbuttonList = new List<RadioButton>();
            groupBox1 = new System.Windows.Forms.GroupBox();
            int i = 0;
            foreach (String item in list )
            {
                rdbuttonList.Add(new System.Windows.Forms.RadioButton());
                rdbuttonList.Last().Location = new System.Drawing.Point(31, 20 + 25 * i);
                rdbuttonList.Last().Size = new System.Drawing.Size(210, 17);
                rdbuttonList.Last().Text = item;
                rdbuttonList.Last().CheckedChanged += new EventHandler(RadioButton_CheckedChanged);
                groupBox1.Controls.Add(rdbuttonList.Last());
                i++;
            }

            i++;
            getSelectedRB = new System.Windows.Forms.Button
            {
                Location = new System.Drawing.Point(10, 20 + 25 * i),
                Size = new System.Drawing.Size(240, 35),
                Text = "Get selected " + groupTitle,
                Enabled = false
            };
            getSelectedRB.Click += new EventHandler(GetSelectedRB_Click);

            groupBox1.Controls.Add(getSelectedRB);
            groupBox1.Location = new System.Drawing.Point(30, 30);
            groupBox1.Size = new System.Drawing.Size(270, 50 + 30 * i);
            groupBox1.Text = groupTitle;

            this.my_form.Location = new System.Drawing.Point(100, 100);
            this.my_form.ClientSize = new System.Drawing.Size(342, 200 + 25 * i);


            return groupBox1;
        }

        /// <summary>
        /// Method for getting checked radiobutton insuring the event originates on a radiobutton
        /// Ennables the getSelectedRB button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = sender as RadioButton;

            if (rb == null)
            {
                System.Windows.MessageBox.Show("Sender is not a RadioButton");
                return;
            }

            // Ensure that the RadioButton.Checked property
            // changed to true.
            if (rb.Checked)
            {
                // Keep track of the selected RadioButton by saving a reference
                // to it.
                selectedrb = rb;
                getSelectedRB.Enabled = true;
            }
        }

        /// <summary>
        /// GetButton Method that closes form only if a radiobutton has been selected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void GetSelectedRB_Click(object sender, EventArgs e)
        {
            if (!(selectedrb == null))
            {
                my_form.Close();
                my_form.Dispose();
            }
            
            
        }
    }

    public class SelectOneStruct
    {
        /// <summary>
        /// A class to open a form with a list of structures.Id s expressed on radiobutons to choose from.
        /// Returns a structure
        /// </summary>
        private List<String> my_list;
        private PlanningItem my_plan;
        private IEnumerable<Structure> set_of_structs;
        private String selected;
        private SelectBox selectDiag;
        private String title;

        /// <summary>
        /// Receives a Form title, PlanSetup and an IEnum(Structure) set of structures 
        /// and present a modal dialog for chosing only one out of the set
        /// </summary>
        /// <param name="title"></param>
        /// <param name="my_plan"></param>
        /// <param name="set_of_structs"></param>
        public SelectOneStruct(String title, PlanningItem my_plan, IEnumerable<Structure> set_of_structs)
        {
            this.title = title;
            this.my_plan = my_plan;
            this.set_of_structs = set_of_structs;
        }

        /// <summary>
        /// Method for getting the selected structure
        /// </summary>
        /// <returns></returns>
        public Structure Get_Selected()
        {

            //*************** Select structure
            my_list = new List<string>();
                foreach (Structure str in set_of_structs)
                { my_list.Add(str.Id); }
            selectDiag = new SelectBox(my_list, title);
            selected = selectDiag.Get_Item();

            if (my_plan is PlanSetup)
            {
                return ((PlanSetup) my_plan).StructureSet.Structures.Where(s => s.Id.Equals(selected)).First();
            }
            else
            {
                return ((PlanSum) my_plan).StructureSet.Structures.Where(s => s.Id.Equals(selected)).First();
            }
            
        }

    }



}
