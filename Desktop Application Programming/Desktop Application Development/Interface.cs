using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using OHS_FBLA;
//NOTE: Due to a large amount of controls, they are named in this format: 
//[Screen Number (ex.s1,s2)] + [Component Type (ex. b = Button, x = CheckBox, g = GroupBox)] + [Shortened Description (NOTE: For date based Controls, acronyms are used))]

namespace OHS_FBLA
{
    public partial class Interface : Form
    {
        //Declare and Assign Variables
        int counter = 0, totalCost = 0; // counter is a generic use int, totalCost is one of the parameter for a Storage Object
        String code;
        private List<string> costs = new List<string>();// A expandable List of costs, contains the cost and source of cost, a parameter for Storage Object
        private String[] generalData = new String[8], specificData = new String[5];// two main pre-set arrays for directly storing user inputed data.
        
        public Interface()
        {
            InitializeComponent();
        }

        private void interfaceLoad(object sender, EventArgs e)//Runs everytime the Interface form loads
        {
                //Sets the current data for use with the Date of Arrival Box
            s2tMonthDOA.Text = "" + System.DateTime.Now.Month;
            s2tDayDOA.Text = "" + System.DateTime.Now.Day;
            s2tYearDOA.Text = "" + System.DateTime.Now.Year;
                //Retrieves the data for all the empty cages available for reservation
            Boolean[] cages = ((StorageInfo)Application.OpenForms[0]).getEmptyCage();
                //Hides all the tab pages, by unattaching them from the workSpace(TabControl). This is used for both streamlining the design process and guiding the user accurately. Creates a list then iterates through them using a Lambda Function
            new List<TabPage>{s2Register,s3Dog,s4Cat,s5Other,s6End}.ForEach(x => x.Parent = null);
                //Sets up the s2cCageNum drop down menu, only adds the empty cages that the user can select
            for (int k = 0; k < 100; k++)
            {
                if (!cages[k])
                {
                    s2cCageNum.Items.Add(k+1);
                }
            }
            workSpace.SelectedTab = s1Start;
            
            s1tCodeField.AutoCompleteCustomSource = ((StorageInfo)Application.OpenForms[0]).autoCaseIDList(new AutoCompleteStringCollection());
        }

        private void startupApp(object sender, EventArgs e)//Runs if this form (Interface) starts for the FIRST time
        {   
                //Hides (DOESN'T TERMINATE) the Mother form (StorageInfo) when this comes into view, StorageInfo is always the index 0 in OpenForms Collection
            Application.OpenForms[0].Hide();
        }

        private void returnBack(object sender, FormClosingEventArgs e)//Runs when the user hits the Close button, which causes this form (Interface) to terminate
        {
                //Re-Opens the Mother form, which was hiding before
            Application.OpenForms[0].Show();                     
        }

        private void checkCode(object sender, EventArgs e)//Runs when the user hits the s1bCodeCheck Button, checks if entered case number is valid
        {   
                //Checks if Text is not default value, Checks if entered value is actually assigned to an animal in the cages
            if (s1tCodeField.Text != "Enter " && s1tCodeField.Text != "")
            {
                if (((StorageInfo)Application.OpenForms[0]).checkCode(s1tCodeField.Text))
                {
                        //Hides this form and re-opens the mother form (StorageInfo)
                    this.Hide();
                    Application.OpenForms[0].Show();
                }
                else
                {
                        //If there was an error, then it calls a visual method(s1Blinker.Tick => invalidBlink) to indicate 'Invalid Case Numer'
                    s1tCodeField.Text = "";
                    s1InvalidCodeBlinker.Visible = true;
                        //Starts the timer (s1Blinker) which calls its Tick method (invalidBlink) every 250 milliseconds (Interval)
                    s1Blinker.Start();
                    MessageBox.Show("Enter valid NUMBER!");
                }
            }
            else
            {
                    //If there was an error, then it calls a visual method(s1Blinker.Tick => invalidBlink) to indicate 'Invalid Case Numer'
                s1tCodeField.Text = "";
                s1InvalidCodeBlinker.Visible = true;
                    //Starts the timer (s1Blinker) which calls its Tick method (invalidBlink) every 250 milliseconds (Interval)
                s1Blinker.Start();
                MessageBox.Show("Enter valid NUMBER!");
            }
            
        }
              
        private void invalidBlink(object sender, EventArgs e)//Causes s1InvalidCodeBlinker(Label) to alternate between red and orange for 1 second 
        {
                //Alternates between Red/DarkOrange everytime this method(invalidBlink) is called
            if(s1InvalidCodeBlinker.ForeColor == Color.Red)
            {
                s1InvalidCodeBlinker.ForeColor = Color.DarkOrange;
            }
            else
            {
                s1InvalidCodeBlinker.ForeColor = Color.Red;
            }
                //Terminates the timer and resests the counter
            if (counter == 3) 
            {
                counter = 0;
                s1Blinker.Stop();
            }     
                //Increments the counter so the method has a concrete end
            counter++;  
        }
//New Function: Implements the Watermark effect. The Watermark effect causes a textBox or editable control to display a temporary String. This String displays generic instructions, like what to input into the field. The Control clears the watermark text when the user focuses on the control and causes it to change colors to indicate general progress.The Control resets back to the Watermark text whenever the user leaves the Control blank.
        private void txtEnter(object sender, EventArgs e)
        {   
                //Intializes genericTxt to the TextBox that invoked the event
            TextBox genericTxt = (TextBox)sender;
                //Gets the parent(GroupBox), which contains partial input intructions as its label
            GroupBox genericParent = (GroupBox)genericTxt.Parent;
                //Creates a practical instruction from the partial instruction recieved from the parent, 
            if (genericTxt.Text == "Enter " + genericParent.Text)
            {
                genericTxt.Text = "";
                    //Changes the color of the parent to green to indicate progress (Green == Good/Done)
                genericTxt.ForeColor = Color.Crimson;
                genericParent.ForeColor = Color.Lime;
                
            }
            else if (genericTxt.Text == "ENTER " + genericParent.Text.ToUpper())//special case for case ID, due to Caps only chars
            {
                genericTxt.Text = "";
                //Changes the color of the parent to green to indicate progress (Green == Good/Done)
                genericTxt.ForeColor = Color.Crimson;
                genericParent.ForeColor = Color.Lime;

            }
        }
       
        private void txtLeave(object sender, EventArgs e)//Same Function as txtEnter above, but this time resets the field back to the Watermark text and reverts back to old colors if the user has not entered anything into the field, otherwise doesnt do anything
        {
            TextBox genericTxt = (TextBox)sender;
            GroupBox genericParent = (GroupBox)genericTxt.Parent;
                //Check if the field is blank
            if (genericTxt.Text.Length == 0)
            {
                genericTxt.Text = "Enter " + genericParent.Text;
                genericTxt.ForeColor = Color.CornflowerBlue;
                genericParent.ForeColor = Color.Crimson;
            }
        }
       
        private void dateEnter(object sender, EventArgs e)//Same methodology as in txtEnter, but this time is altered to accomodate for the three fields in Date
        {
            TextBox genericTxt = (TextBox)sender;
            GroupBox genericParent = (GroupBox)genericTxt.Parent;
            if (genericTxt.Text == "Month")
            {
                genericTxt.Text = "";
                genericTxt.ForeColor = Color.Crimson;
                genericParent.ForeColor = Color.Lime;
            }
            else if (genericTxt.Text == "Day")
            {
                genericTxt.Text = "";
                genericTxt.ForeColor = Color.Crimson;
                genericParent.ForeColor = Color.Lime;
            }
            else if (genericTxt.Text == "Year")
            {
                genericTxt.Text = "";
                genericTxt.ForeColor = Color.Crimson;
                genericParent.ForeColor = Color.Lime;
            }
        }

        private void dateLeave(object sender, EventArgs e)//Same methodology as in txtLeave but uses the TAG property of the field, instead of the parent label text
        {
            TextBox genericTxt = (TextBox)sender;
            GroupBox genericParent = (GroupBox)genericTxt.Parent;
            if (genericTxt.Text.Length == 0)
            {
                genericTxt.Text = genericTxt.Tag.ToString();
                genericTxt.ForeColor = Color.CornflowerBlue;
                genericParent.ForeColor = Color.Crimson;
            }
        }

        private void comboEnter(object sender, EventArgs e)//txtEnter modified for ComboBoxes 
        {
            ComboBox genericCmb = (ComboBox)sender;
            GroupBox genericParent = (GroupBox)genericCmb.Parent;
            if (genericCmb.Text == "Select " + genericParent.Text)
            {
                genericCmb.Text = "";
                genericCmb.ForeColor = Color.Crimson;
                genericParent.ForeColor = Color.Lime;
            }
        }

        private void comboLeave(object sender, EventArgs e)//txtLeave modified for ComboBoxes
        {
            ComboBox genericCmb = (ComboBox)sender;
            GroupBox genericParent = (GroupBox)genericCmb.Parent;
            if (genericCmb.Text.Length == 0)
            {
                genericCmb.Text = "Select " + genericParent.Text;
                genericCmb.ForeColor = Color.CornflowerBlue;
                genericParent.ForeColor = Color.Crimson;
                
            }
        }
        
        private void comboChange(object sender, EventArgs e)//logic error proofing solution, because this method handles two very important data components which can crash the app
        {
            ((ComboBox)sender).Parent.ForeColor = Color.Lime;
        }

        private void codeValid(object sender, EventArgs e)//ensures that case id for animal is unique to that animal
        {
            //Generates a random 5 digit integer to serve as the unique case number for the current animal
            s2gCaseID.ForeColor = Color.Lime;
            for (int k = 0; k < 100; k++)
            {
                s2gCaseID.Text = "Unique Animal ID";
                    //checks if generated code is currently assigned to. If it is a duplicate then it regenerates a random number.
                if (((StorageInfo)Application.OpenForms[0]).data[k] != null && ((StorageInfo)Application.OpenForms[0]).data[k].getCaseID().Equals(s2tCaseID.Text))
                {
                    s2gCaseID.ForeColor = Color.Crimson;
                    s2gCaseID.Text = "Case ID already in use";
                    break;
                }
                if (k == 99)//if the function doesn't break^, then code is set to entered value
                {
                    code = s2tCaseID.Text;
                }
            }
        }

        private void reformatCode(object sender, EventArgs e)//Reformats the case id after auto complete is used
        {
            if (s1tCodeField.Text.Contains(":"))
            {
                s1tCodeField.Text = s1tCodeField.Text.Remove(s1tCodeField.Text.IndexOf(':') - 1);
            }
        }
//End Function
        private void comboClick(object sender, MouseEventArgs e)//User Convenience Function (UCF). opens the dropdown list for comboboxes if the user clicks ANYWHERE on the combobox, instead of the tiny arrow in the corner of comboboxes
        {
            ComboBox genericCmb = (ComboBox)sender;
            genericCmb.DroppedDown = true;
        }
 
        private void numericInput(object sender, KeyPressEventArgs e)//A run time implementation for accepting only numberic input, for fields that require numbers only
        {   
                //Filters out incoming input based on Numbers(0-9), BackSpace, Tab key
            if (Char.IsDigit(e.KeyChar) || e.KeyChar == Convert.ToChar(Keys.Back) || e.KeyChar == Convert.ToChar(Keys.Tab))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

//New Function: Handles user usability and smoothens the flow of Program. Mainly enables GroupBoxes for additional input, whenever required.   
        private void s2gMCY_Selected(object sender, EventArgs e)//Runs if the user click on the YES groupbox
        {       
                //Un-hides all the components inside the YES groupbox
            s2gMCYChipCode.Visible = true;
            s2gMCYDOC.Visible = true;
            s2gMCYOwnerName.Visible = true;
                //Disables the NO groupbox and employs the same UCF function as in txtEnter
            s2gMicroChippedY.BackColor = Color.Green;
            s2gMicroChippedN.Enabled = false;
        }

        private void s2gMCN_Selected(object sender, EventArgs e)//Same thing as above method(s2gMCY_Selected), employs the same process for NO groupbox items
        {
            s2gMCNCD.Visible = true;
            s2gMCNChipCode.Visible = true;
            s2gMicroChippedN.BackColor = Color.Red;
            s2gMicroChippedY.Enabled = false;
        }

        private void testDateReveal(object sender, EventArgs e)//An UCF for enabling and accounting for Test represented by the radiobutton control
        {
                //Enables the corresponding groupbox, which is checked (if it is enabled) and added when compiling all the inputed data
            RadioButton firstControl = (RadioButton)sender;
            GroupBox innerContainer = (GroupBox)firstControl.Parent;
            GroupBox outerContainer = (GroupBox)innerContainer.Parent;
            GroupBox nextControl = (GroupBox)outerContainer.GetNextControl(firstControl, true);
                //If the correct radiobutton is checked/selected then enables the corresponding groupbox, allowing for it to be considered during data compilation
            if (firstControl.Checked)
            {
                if (firstControl.Tag == "Expand")//Special case for HeartWorm field under the Dog page
                {   
                        //Enables the groupbox
                    nextControl.Enabled = true;
                        //Enables the next groupbox, after skipping through the groupbox's textbox items (3 fields for date)
                    outerContainer.GetNextControl(outerContainer.GetNextControl(outerContainer.GetNextControl(outerContainer.GetNextControl(nextControl, true), true), true), true).Enabled = true;
                }
                else 
                { 
                nextControl.Enabled = true;
                }
            }
            else//Same process, but this time disables the corresponding controls when the user unchecks/de-selects the radiobutton
            {                
                if (firstControl.Tag == "Expand")
                {
                    nextControl.Enabled = false;
                    outerContainer.GetNextControl(outerContainer.GetNextControl(outerContainer.GetNextControl(outerContainer.GetNextControl(nextControl, true), true), true), true).Enabled = false;
                }
                else
                {
                    nextControl.Enabled = false;
                }
            }
        }

        private void customVaccineCheckBoxUI(object sender, EventArgs e)//Another UCF, performs the same function as above method (testDateReveal), but is tailored to fit the format of Vaccines in Dog and Cat Pages only
        {
            CheckBox firstControl = (CheckBox)sender;
            GroupBox parentContainer = (GroupBox)firstControl.Parent;
            Control nextControl = parentContainer.GetNextControl(firstControl, true);
                //Enables/Disables the corresponding groupbox to allow for user input
            if (firstControl.Checked)
            {
                nextControl.Enabled = true;
            }
            else
            {
                nextControl.Enabled = false;
            }
                
        }

        private void s5customVaccineUI(object sender, EventArgs e)//UCF that performs the same function as above method, but is implemented to work with textboxes in 'Other'page
        {
                //In 'Other' tabpage the vaccine types are to be entereed by the user. So if the textbox is focused by the user the corresponding groupbox is enabled to allow for date vaccinated input of the specific vaccine
            TextBox firstControl = (TextBox)sender;
            GroupBox parentContainer = (GroupBox)firstControl.Parent;
            Control nextControl = parentContainer.GetNextControl(firstControl, true);
            nextControl.Enabled = true;
        }
//End Function        

//New Super Function: Adds the appropriate tab page to the tab control (workSpace), then automatically progresses the screen onto the next appropriate tab page
        private void s1Done(object sender, EventArgs e)
        {   
                //Progresses the intital screen by changing the tab index on the TabControl
            s2Register.Parent = workSpace;
            workSpace.SelectedTab = s2Register;
        }

        private bool filledBox(List<GroupBox> items)//Checks every groupbox to see if any are both enabled and doesn't have a Lime forecolor
        {
            foreach(GroupBox item in items)
            {
                if (item.Enabled == true && item.ForeColor != Color.Lime)
                {
                    return false;
                }
            }
            return true;
        }

//New Function: Button Click > Save Data to either {generalData[],specificData[]} > Progress Screen Forward > Updates Cost and totalCost       
        private void s2Done(object sender, EventArgs e)
        {   
                  //Saves Data to appropriate UNIQUE index on the generalData[] array
                 //The Format is LABEL + USER INPUTED VALUE from the ENTIRE GROUPBOX, in SOME cases data is added based on if groupbox is enabled (handled by previous methods)
                //A '*' is added to represent the method 'Environment.NewLine', which skips to the next line in the string, promotes readable formating in multi-field groupboxes
                //Some attributes has a price tag, so the cost source is added first to cost(List) then the cost value is added. Therefore the cost List size is twice the amount of diffrent costs incurred. Also the cost value is added to the totalCost(int) variable.
                //Refreshes the Arrays just in case user goes back to change values then checks to see if all appropriate controls are filled in
            generalData = new String[8]; costs.Clear(); totalCost = 0;
            if (filledBox(new List<GroupBox> { s2gAnimalName, s2gAnimalType, s2gAnimalAge, s2gCageNum, s2gMCNCD, s2gMCNChipCode, s2gMCYChipCode, s2gMCYDOC, s2gMCYOwnerName, s2gRelinParty }) && (s2gMicroChippedY.BackColor == Color.Green || s2gMicroChippedN.BackColor == Color.Red))
            {
                generalData[0] = "Animal Name: " + s2tAnimalName.Text;
                generalData[1] = "Animal Type: " + s2cAnimalType.Text;
                generalData[2] = "Animal Age: " + s2cAnimalAge.Text;

                if (s2tMonthDOB.Text == "")//Special condition, if the exact Date of Birth is unknown.
                {
                    generalData[3] = "Unknown";
                }
                else
                {
                    generalData[3] = "Animal Date of Birth: " + s2tMonthDOB.Text + "/" + s2tDayDOB.Text + "/" + s2tYearDOB.Text;
                }

                generalData[4] = "Animal Date of Arrival: " + s2tMonthDOA.Text + "/" + s2tDayDOA.Text + "/" + s2tYearDOA.Text;

                if (s2gMicroChippedN.Enabled == false)
                {
                    generalData[5] = "Micro-Chip Code: " + s2tChipCodeY.Text + "*" + "Owner Name: " + s2tOwnerName.Text + "*" + "Date Contacted: " + s2tMonthDOC.Text + "/" + s2tDayDOC.Text + "/" + s2tYearDOC.Text;
                }
                else
                {
                    costs.Add("Micro-Chip");
                    costs.Add("" + 5 + ".00");
                    totalCost += 5;
                    generalData[5] = "Micro-Chipping Date: " + s2tMonthCD.Text
                        + "/" + s2tDayCD.Text + "/" + s2tYearCD.Text + "*" + "Micro-Chip Code: " + s2tChipCodeN.Text;
                }
                generalData[6] = "Relinquishing Party: " + s2tRelinParty.Text;
                generalData[7] = "Cage Number: " + s2cCageNum.SelectedItem.ToString();
                    //Redirects to the next tab page based on the specific type of animal selected, clears any other tabpages incase the user makes edits
                if (s2cAnimalType.SelectedItem.ToString().Equals("Dog"))
                {
                    if (workSpace.TabPages.Count > 3 && workSpace.TabPages[3] != s3Dog)
                    {
                        workSpace.TabPages[3].Controls.OfType<Control>().ToList().ForEach(x => clearControl(x));
                        workSpace.TabPages[3].Parent = null;
                        if (workSpace.TabPages.Count > 4 && workSpace.TabPages[4] == s6End)
                        {
                            workSpace.TabPages[4].Controls.OfType<Control>().ToList().ForEach(x => clearControl(x));
                            workSpace.TabPages[4].Parent = null;
                        }                       
                    }
                    s3Dog.Parent = workSpace;
                    workSpace.SelectedTab = s3Dog;
                }
                if (s2cAnimalType.SelectedItem.ToString().Equals("Cat"))
                {
                    if (workSpace.TabPages.Count > 3 && workSpace.TabPages[3] != s4Cat)
                    {
                        workSpace.TabPages[3].Controls.OfType<Control>().ToList().ForEach(x => clearControl(x));
                        workSpace.TabPages[3].Parent = null;
                        if (workSpace.TabPages.Count > 4 && workSpace.TabPages[4] == s6End)
                        {
                            workSpace.TabPages[4].Controls.OfType<Control>().ToList().ForEach(x => clearControl(x));
                            workSpace.TabPages[4].Parent = null;
                        } 
                    }
                    s4Cat.Parent = workSpace;
                    workSpace.SelectedTab = s4Cat;
                }
                if (s2cAnimalType.SelectedItem.ToString().Equals("Other"))
                {
                    if (workSpace.TabPages.Count > 3 && workSpace.TabPages[3] != s5Other)
                    {
                        workSpace.TabPages[3].Controls.OfType<Control>().ToList().ForEach(x => clearControl(x));
                        workSpace.TabPages[3].Parent = null;
                        if (workSpace.TabPages.Count > 4 && workSpace.TabPages[4] == s6End)
                        {
                            workSpace.TabPages[4].Controls.OfType<Control>().ToList().ForEach(x => clearControl(x));
                            (new List<GroupBox> { s5gDOV1, s5gDOV2, s5gDOV3, s5gDOV4, s5gDOV5 }).ForEach(x => x.Enabled = false);
                            workSpace.TabPages[4].Parent = null;
                        } 
                    }      
                    s5Other.Parent = workSpace;
                    workSpace.SelectedTab = s5Other;
                }
            }
            else
            {
                MessageBox.Show("Please COMPLETE all fields marked in RED, or Click on either Yes/No boxes under MICRO-CHIPPED");
            }

        }
        
        private void s3Done(object sender, EventArgs e)//Dog page
        {
                   //Saves data to appropriate UNIQUE index on the specificData[] array
                  //The Format is LABEL + USER INPUTED VALUE from the ENTIRE GROUPBOX, in SOME cases data is added based on if groupbox is enabled (handled by previous methods)
                 //Condenses data from EACH UNiQUE group to accomodate to the size of the specificData[] array
                //The format is however retained with the use of '*' which represents Environment.NewLine, which progresses to the next line in the string
                //Some attributes has a price tag, so the cost source is added first to cost(List) then the cost value is added. Therefore the cost List size is twice the amount of diffrent costs incurred. Also the cost value is added to the totalCost(int) variable.
                //Refreshes the Arrays just in case user goes back to change values then Checks if all fields are properly filled
            specificData = new String[5];
            if (costs.Count > 0 && costs[0] == "Micro-Chip")
            {
                costs.Clear();
                costs.Add("Micro-Chip");
                costs.Add("" + 5 + ".00");
                totalCost = 5;
            }
            else
            {
                costs.Clear();
                totalCost = 0;
            }
            if (filledBox(new List<GroupBox> { s3gBreed, s3gWeight, s3gCoatColor,s3gCoatType,s3gGender,s3gDONS,s3gDOFT,s3gDOHT, s3gDOHFT,s3gDOHRT,s3gRabiesDOV,s3gDistemperDOV,s3gBordetellaDOV}))
            {
                specificData[0] = "Breed: " + s3cBreed.Text + "*" +
                "Weight: " + s3cWeight.Text + " lbs" + "*" +
                "Coat Color: " + s3cCoatColor.Text
                + "*" + "Coat Type: " + s3cCoatType.Text;
                if (s3gDONS.Enabled == true)
                {
                    costs.Add("Neuter/Spay");
                    costs.Add("" + 75 + ".00");
                    totalCost += 75;
                    specificData[1] = "Gender: " + s3cGender.Text + "*" + "Neutered/Spayed Status: Needs to Neuter/Spay" + "*" +
                        "Neuter/Spay Schedule Date: " + s3tMonthDONS.Text
                        + "/" + s3tDayDONS.Text + "/" + s3tYearDONS.Text;
                }
                else
                {
                    specificData[1] = "Gender: " + s3cGender.Text + "*" + "Neutered/Spayed Status: Clear";
                }
                if (s3gDOFT.Enabled == true)
                {
                    costs.Add("Flea Treatment");
                    costs.Add("" + 10 + ".00");
                    totalCost += 10;
                    specificData[2] = "Flea Test: None" + "*" + "First Flea Treatment Date: " + s3tMonthDOFFT.Text
                        + "/" + s3tDayDOFFT.Text + "/" + s3tYearDOFFT.Text;
                }
                else
                {
                    specificData[2] = "Flea Test: Clear";
                }
                if (s3gDOHFT.Enabled == true)
                {
                    costs.Add("Heart-Worm Treatment");
                    costs.Add("" + 10 + ".00");
                    totalCost += 10;
                    specificData[3] = "Heart-Worm Test: Positive" + "*" + "Heart-Worm Test Date: " + s3tMonthDOHT.Text
                        + "/" + s3tDayDOHT.Text + "/" + s3YearDOHT.Text + "*" + "First Heart-Worm Medication Date: " + s3tMonthDOHFT.Text
                        + "/" + s3tDayDOHFT.Text + "/" + s3tYearDOHFT.Text + "*" + "Heart-Worm Re-Examination Date: " + s3tMonthDOHRT.Text
                        + "/" + s3tDayDOHRT.Text + "/" + s3tDOHRT.Text;
                }
                else
                {
                    specificData[3] = "Heart-Worm Test: Negative" + "*" + "Heart-Worm Test Date: " + s3tMonthDOHT.Text
                        + "/" + s3tDayDOHT.Text + "/" + s3YearDOHT.Text;
                }
                specificData[4] = "Vacinations: " + "*";

                if (s3gRabiesDOV.Enabled == true)
                {
                    costs.Add("Rabies Vaccine");
                    costs.Add("" + 30 + ".00");
                    totalCost += 30;
                    specificData[4] += "[Rabies vaccinated on: " + s3tMonthDORV.Text
                        + "/" + s3tDayDORV.Text + "/" + s3tYearDORV.Text + "]" + "*";
                }
                if (s3gDistemperDOV.Enabled == true)
                {
                    costs.Add("Distemper Vaccine");
                    costs.Add("" + 15 + ".00");
                    totalCost += 15;
                    specificData[4] += "[Distemper vaccinated on: " + s3tMonthDODV.Text
                        + "/" + s3tDayDODV.Text + "/" + s3tYearDODV.Text + "]" + "*";
                }
                if (s3gBordetellaDOV.Enabled == true)
                {
                    costs.Add("Bordetella Vaccine");
                    costs.Add("" + 15 + ".00");
                    totalCost += 15;
                    specificData[4] += "[Bordetella vaccinated on: " + s3tMonthDOBV.Text
                        + "/" + s3tDayDOBV.Text + "/" + s3tYearDOBV.Text + "]";
                }
                //ReConfigures the specificData[4] string if no vaccinations has been selected
                if (specificData[4].Equals("Vacinations: " + "*"))
                {
                    specificData[4] = "Vacinations: None";
                }
                //Progresses the tab page to the end 
                s6End.Parent = workSpace;
                workSpace.SelectedTab = s6End;
            }
            else
            {
                MessageBox.Show("Please COMPLETE all fields marked in RED");
            }
            
        }

        private void s4Done(object sender, EventArgs e)//Cat Page
        {
                //Saves data to appropriate UNIQUE index on the specificData[] array
                //The Format is LABEL + USER INPUTED VALUE from the ENTIRE GROUPBOX, in SOME cases data is added based on if groupbox is enabled (handled by previous methods)
                //Condenses data from EACH UNiQUE group to accomodate to the size of the specificData[] array
                //The format is however retained with the use of '*' which represents Environment.NewLine, which progresses to the next line in the string
                //Some attributes has a price tag, so the cost source is added first to cost(List) then the cost value is added. Therefore the cost List size is twice the amount of diffrent costs incurred. Also the cost value is added to the totalCost(int) variable.
                //Refreshes the arrays just in case user goes back to edit then checks if each appropriate field is correctly filled in
            specificData = new String[5];
            if (costs.Count > 0 && costs[0] == "Micro-Chip")
            {
                costs.Clear();
                costs.Add("Micro-Chip");
                costs.Add("" + 5 + ".00");
                totalCost = 5;
            }
            else
            {
                costs.Clear();
                totalCost = 0;
            }
            if (filledBox(new List<GroupBox> { s4gBreed, s4gWeight, s4gCoatColor, s4gCoatType, s4gGender, s4gDONS, s4gDOD, s4gDOFLT, s4gDORV }))
            {
                specificData[0] = "Breed: " + s4cBreed.Text + "*" +
                "Weight: " + s4cWeight.Text + " lbs" + "*" +
                "Coat Color: " + s4cCoatColor.Text
                + "*" + "Coat Type: " + s4cCoatType.Text;

                if (s4gDONS.Enabled == true)
                {
                    costs.Add("Spay/Neuter");
                    costs.Add("" + 75 + ".00");
                    totalCost += 75;
                    specificData[1] = "Gender: " + s4cGender.Text + "*" + "Neutered/Spayed Status: Needs to Neuter/Spay" + "*" +
                        "Neuter/Spay Schedule Date: " + s4tMonthDONS.Text
                        + "/" + s4tDayDONS.Text + "/" + s4tYearDONS.Text;
                }
                else
                {
                    specificData[1] = "Gender: " + s4cGender.Text + "*" + "Neutered/Spayed Status: Clear";
                }
                if (s4gDOD.Enabled == true)
                {
                    specificData[2] = "De-Clawed: False" + "*" + "De-Clawing Date: " + s4tMonthDOD.Text
                        + "/" + s4tDayDOD.Text + "/" + s4tYearDOD.Text;
                }
                else
                {
                    specificData[2] = "De-Clawed: True";
                }
                if (s4rFelLeuY.Checked)
                {
                    costs.Add("Feline Leukemia Test");
                    costs.Add("" + 15 + ".00");
                    totalCost += 15;
                    specificData[3] = "Feline Leukemia Test Result: Positive" + "*" + "Feline Leukemia Test Date: " + s4tMonthDOFLT.Text
                        + "/" + s4tDayDOFLT.Text + "/" + s4tYearDOFLT.Text;
                }
                else
                {
                    costs.Add("Feline Leukemia Test");
                    costs.Add("" + 15 + ".00");
                    totalCost += 15;
                    specificData[3] = "Feline Leukemia Test Result: Negative" + "*" + "Feline Leukemia Test Date: " + s4tMonthDOFLT.Text
                        + "/" + s4tDayDOFLT.Text + "/" + s4tYearDOFLT.Text;
                }
                //Cat page only has one selectable vaccine, so this process is handled differently than Dog page, but still same result if no vaccine is selected
                if (s4gDORV.Enabled == true)
                {
                    costs.Add("Rabies Vaccine");
                    costs.Add("" + 30 + ".00");
                    totalCost += 30;
                    specificData[4] = "[Rabies vaccinated on: " + s4tMonthDORV.Text
                        + "/" + s4tDayDORV.Text + "/" + s4tYearDORV.Text + "]" + "*";
                }
                else
                {
                    specificData[4] = "Vaccinations: None";
                }
                //Progresses the tab page to the end 
                s6End.Parent = workSpace;
                workSpace.SelectedTab = s6End; 
            }
            else
            {
                MessageBox.Show("Please COMPLETE all fields marked in RED!");
            }
                       
        }

        private void s5Done(object sender, EventArgs e)//Other Animal page
        {
                //Saves data to appropriate UNIQUE index on the specificData[] array, which has a reduced size of 4 compared to the others
                //The Format is LABEL + USER INPUTED VALUE from the ENTIRE GROUPBOX, in SOME cases data is added based on if groupbox is enabled (handled by previous methods)
                //Condenses data from EACH UNiQUE group to accomodate to the size of the specificData[] array
                //The format is however retained with the use of '*' which represents Environment.NewLine, which progresses to the next line in the string
                //Some attributes has a price tag, so the cost source is added first to cost(List) then the cost value is added. Therefore the cost List size is twice the amount of diffrent costs incurred. Also the cost value is added to the totalCost(int) variable.
                //Refreshes the arrays just in case user goes back to edit, then checks if each available field has been appropriately filled in
            specificData = new String[4];
            if (costs.Count > 0 && costs[0] == "Micro-Chip")
            {
                costs.Clear();
                costs.Add("Micro-Chip");
                costs.Add("" + 5 + ".00");
                totalCost = 5;
            }
            else
            {
                costs.Clear();
                totalCost = 0;
            }
            if (filledBox(new List<GroupBox> { s5gAnimalType, s5gWeight, s5gAnimalDesc, s5gDOV1, s5gDOV2, s5gDOV3, s5gDOV4, s5gDOV5 }))
            {
                specificData[0] = "General Animal Type: " + s5cAnimalType.Text;
                specificData[1] = "Weight: " + s5cWeight.Text + " lbs";
                specificData[2] = s5cAnimalType.Text + " Description: " + s5tAnimalDescription.Text;
                specificData[3] = "Vaccinations: " + "*";
                //Checks from the maximum of 5 inputable vaccines, if they have been inputted then adds the Vaccine name and date of administration
                if (s5gDOV1.Enabled == true)
                {
                    costs.Add(s5tVac1.Text + " Vaccine");
                    costs.Add("" + 15 + ".00");
                    totalCost += 15;
                    specificData[3] += "[" + s5tVac1.Text + " vaccinated on: " + s5tMonthDOV1.Text
                        + "/" + s5tDayDOV1.Text + "/" + s5tYearDOV1.Text + "]" + "*";
                }
                if (s5gDOV2.Enabled == true)
                {
                    costs.Add(s5tVac2.Text + " Vaccine");
                    costs.Add("" + 15 + ".00");
                    totalCost += 15;
                    specificData[3] += "[" + s5tVac2.Text + " vaccinated on: " + s5tMonthDOV2.Text
                        + "/" + s5tDayDOV2.Text + "/" + s5tYearDOV2.Text + "]" + "*";
                }
                if (s5gDOV3.Enabled == true)
                {
                    costs.Add(s5tVac3.Text + " Vaccine");
                    costs.Add("" + 15 + ".00");
                    totalCost += 15;
                    specificData[3] += "[" + s5tVac3.Text + " vaccinated on: " + s5tMonthDOV3.Text
                        + "/" + s5tDayDOV3.Text + "/" + s5tYearDOV3.Text + "]";
                }
                if (s5gDOV4.Enabled == true)
                {
                    costs.Add(s5tVac4.Text + " Vaccine");
                    costs.Add("" + 15 + ".00");
                    totalCost += 15;
                    specificData[3] += "[" + s5tVac4.Text + " vaccinated on: " + s5tMonthDOV4.Text
                        + "/" + s4tMonthDOFLT.Text + "/" + s5tYearDOV4.Text + "]";
                }
                if (s5gDOV5.Enabled == true)
                {
                    costs.Add(s5tVac5.Text + " Vaccine");
                    costs.Add("" + 15 + ".00");
                    totalCost += 15;
                    specificData[3] += "[" + s5tVac5.Text + " vaccinated on: " + s5tMonthDOV5.Text
                        + "/" + s5tDayDOV5.Text + "/" + s5tYearDOV5.Text + "]";
                }
                //Similar procces from the Dog page, adds possible vaccines, although if none found it resets the specificData[3] to a default value
                if (specificData[3].Equals("Vaccinations: " + "*"))
                {
                    specificData[3] = "Vaccinations: None";
                }
                //Progress to final page
                s6End.Parent = workSpace;
                workSpace.SelectedTab = s6End;
            }
            else
            {
                MessageBox.Show("Please COMPLETELY fill in all fields marked in RED!");
            }
            
        }
//End Function
        private void s6CompileData(object sender, EventArgs e)//Runs when s6bCompileData button is clicked, displays the collected data and randomly generated Case Number
        {    
                //Itterates through each value in either {generalData[], specificData[], costs} > Adds them to a String (with formating), then sets s6t{GeneralData, SpecificData, Costs}View Text property to the string.
                //Creates a variable that stores each String from the String[] array, for formatting purposes
            String output = "";
                //Add each String in generalData to the output String, then skips to the next line for formatting and readability
            foreach (String val in generalData)
            {   
                    //Splits the val (String) into a String[] array based on placement of '*' (Environment.NewLine), then replaces with the ACTUAL method on the string
                if(val.Split('*').Length == 1)//When there is no '*' found in val
                {
                    output += val + Environment.NewLine;
                }
                else
                {
                    foreach (String cut in val.Split('*'))
                    {
                        output += cut + Environment.NewLine;
                    }
                }
            }
                //Set the text value of s6tGeneralDataView to all the accumulated strings in output
            s6tGeneralDataView.Text = output;
            output = "";
            foreach (String val in specificData)
            {
                    //Splits the val (String) into a String[] array based on placement of '*' (Environment.NewLine), then replaces with the ACTUAL method on the string
                if (val.Split('*').Length == 1)//When there is no '*' found in val
                {
                    output += val + Environment.NewLine;
                }
                else
                {
                    foreach (String cut in val.Split('*'))
                    {
                        output += cut + Environment.NewLine;
                    }
                }
            }
                //Set the text value of s6tSpecifcDataView equal to output [all the accumulated strings in output with the formatting]
            s6tSpecificDataView.Text = output;
            output = "";
                //Formats the data in costs (List) with monetary symbols 
            for (int k = 0; k < costs.Count; k+= 2)//Since data was added to the costs List in pairs, the for loop increments by 2s, the format is cost source > cost amount
            {
                output += costs[k].ToString() + " $" + costs[k + 1].ToString() + Environment.NewLine;
            }
                //Attaches the totalCost variable at the end of output with appropriate formatting.
            s6tCostsView.Text = output + "TOTAL: $" + totalCost + ".00";
                //Sets the code variable
            s6lCodeView.Text = "CODE: " + code; 
        }
//End Function
        private void s6ExportData(object sender, EventArgs e)//Creates a new Storage object with the collected data and sends it to StorageInfo form/class, and then exits
        {
            ((StorageInfo)Application.OpenForms[0]).addData(new Storage(generalData, specificData, costs, totalCost, code), int.Parse(s2cCageNum.SelectedItem.ToString()) - 1);
                //back to the root form, where this current instance of Interface will be closed.
            Application.OpenForms[0].Show();
            this.Hide();
        }

        private void resetInterface(object sender, EventArgs e)//Runs when the resetButton tab page is clicked. Clears all fields and data and resets user interactions 
        {
            if (workSpace.SelectedTab == resetButton)
            {
                    //calls clearControl for each tab page attached to workSpace, and removes them from workSpace
                foreach (TabPage page in workSpace.TabPages)
                {
                    page.Controls.OfType<Control>().ToList().ForEach(x => clearControl(x));
                    page.Parent = null;
                }
                    //reset
                (new List<GroupBox> { s5gDOV1, s5gDOV2, s5gDOV3, s5gDOV4, s5gDOV5 }).ForEach(x => x.Enabled = false);
                resetButton.Parent = workSpace;
                s1Start.Parent = workSpace;
                workSpace.SelectedIndex = 1;
                s2tMonthDOA.Text = "" + System.DateTime.Now.Month;
                s2tDayDOA.Text = "" + System.DateTime.Now.Day;
                s2tYearDOA.Text = "" + System.DateTime.Now.Year;
                s2gAnimalDOA.ForeColor = Color.Lime;
                generalData = new String[8];
                specificData = new String[5];
                costs.Clear();
                totalCost = 0;
                code = "";
            }
        }

        private void clearControl(Control control)//Uses recursion to clear out all entered data in every field
        {
                //determines the type of control to evoke its specific methods
            if (control is GroupBox)//Recursive method, gets all of its child controls and calls this method on them.
            {
                if (((GroupBox)control).Tag == "reset")//specifically designed for the Yes/No Micro-Chip interface, reset them to match their original function
                {
                    ((GroupBox)control).Enabled = true;
                    ((GroupBox)control).BackColor = ((GroupBox)control).Parent.BackColor;
                    ((GroupBox)control).Controls.OfType<Control>().ToList().ForEach(x => x.Visible = false);
                    ((GroupBox)control).Controls.OfType<Control>().ToList().ForEach(x => clearControl(x));
                }                
                else
                {                   
                    ((GroupBox)control).ForeColor = Color.Crimson;
                    ((GroupBox)control).Controls.OfType<Control>().ToList().ForEach(x => clearControl(x));
                }
                
            }
            else if (control is TextBox)//clears and calls its leave method for the Watermark effect.
            {
                if (((TextBox)control).Tag == "Day" || ((TextBox)control).Tag == "Month" || ((TextBox)control).Tag == "Year")//For date format
                {
                    ((TextBox)control).Text = "";                    
                    dateLeave(control, new EventArgs());
                }
                else
                {
                    ((TextBox)control).Text = "";
                    txtLeave(control, new EventArgs());
                }                
            }
            else if (control is ComboBox)//clears and calls its leave method for the Watermark effect.
            {
                
                if(((ComboBox)control).DropDownStyle == ComboBoxStyle.DropDownList)//for the two special comboboxes used to enter Cage Number and Animal Type
                {
                    comboChange(control, new EventArgs());
                    ((ComboBox)control).SelectedItem = null;
                    ((ComboBox)control).Parent.ForeColor = Color.Crimson;
                }
                else
                {
                    ((ComboBox)control).Text = "";
                    comboLeave(control, new EventArgs());
                }
                
            }
            else if (control is RadioButton)
            {
                ((RadioButton)control).Checked = false;
            }
            else if (control is CheckBox)
            {
                ((CheckBox)control).Checked = false;
            }
            else if (control is RichTextBox)
            {
                ((RichTextBox)control).Text = "";
            }
        }        
    }

}