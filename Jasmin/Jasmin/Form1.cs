using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using System.Windows.Forms;

using Firebase;
using FireSharp;
using FireSharp.Config;
using FireSharp.EventStreaming;
using FireSharp.Extensions;
using FireSharp.Interfaces;
using FireSharp.Response;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Jasmin
{

    public partial class Form1 : Form
    {
       

        public Form1()
        {
            
            InitializeComponent();
            try
            {
                ostrm = new FileStream("./Redirect.txt", FileMode.OpenOrCreate, FileAccess.Write);
                writer = new StreamWriter(ostrm);
            }
            catch (Exception e)
            {
                Console.WriteLine("Cannot open Redirect.txt for writing");
                Console.WriteLine(e.Message);
                return;
            }
            Console.SetOut(writer);
        }

        private async void btnSend_ClickAsync(object sender, EventArgs e)
        {
            int id = 0;
            if (MessageList.Count > 0)
                id=Convert.ToInt32(MessageList.Count+1);
            
            var messageData = new MessageData
            {
                Id = id,
                Message = txtMesaj.Text,
                Name = CurrentUser.Name,
                Shoutout = false
            };

            SetResponse response = await client.SetAsync("Groups/"+CurrentGroup+"/"+messageData.Id, messageData);
            MessageData result = response.ResultAs<MessageData>();

            //MessageBox.Show("trimite mesaju" + result);

        }
        private List<Group> Groups = new List<Group>();
        private async void Form1_LoadAsync(object sender, EventArgs e)
        {

            client = new FireSharp.FirebaseClient(config);

            FirebaseResponse response = await client.GetAsync("Groups/");

            var obj = JObject.Parse(response.Body);
            foreach(var child in obj)
            {
                Groups.Add(new Group(child));
            }

           


        }
        private string[] splitSubstring(string target, string substring)
        {

            return target.Split(new string[] { substring }, StringSplitOptions.None);
        }
        private void writeMessage(MessageData mesaj)
        {
            lstMesaje.Items.Add(mesaj.Name+":"+mesaj.Message);
        }
        private void addMesage(ValueAddedEventArgs args)
        {
            if (u < 4)
            {
                u++;
                if (args.Path.Split('/').Length == 2)
                {
                    string path = args.Path.Split('/')[1];
                    switch (path)
                    {
                        case "Id":
                            LastMessage.Id = Convert.ToInt64(args.Data);
                            break;
                        case "Message":
                            LastMessage.Message = args.Data;
                            break;
                        case "Name":
                            LastMessage.Name = args.Data;
                            break;
                        case "Shoutout":
                            LastMessage.Shoutout = Convert.ToBoolean(args.Data);
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    string path = args.Path.Split('/')[2];
                    switch (path)
                    {
                        case "Id":
                            LastMessage.Id = Convert.ToInt64(args.Data);
                            break;
                        case "Message":
                            LastMessage.Message = args.Data;
                            break;
                        case "Name":
                            LastMessage.Name = args.Data;
                            break;
                        case "Shoutout":
                            LastMessage.Shoutout = Convert.ToBoolean(args.Data);
                            break;
                        default:
                            break;
                    }
                }
            }
            else
            {
                u = 1;

                MessageList.Add(LastMessage);
                lstMesaje.Invoke(new Action(() => this.writeMessage(LastMessage)));
                LastMessage = new MessageData();
                
                string path = args.Path.Split('/')[1];
                if (path == "Id")
                    LastMessage.Id = Convert.ToInt64(args.Data);
                if (path == "Message")
                    LastMessage.Message = args.Data;
                if (path == "Name")
                    LastMessage.Name = args.Data;
                if (path == "Shoutout")
                    LastMessage.Shoutout = Convert.ToBoolean(args.Data);
            }
        }
        

        private void button1_ClickAsync(object sender, EventArgs e)
        {
          

        }
        private void Form1_Disposed(object sender, System.EventArgs e)
        {
            FirebaseListener.Dispose();


            Console.SetOut(oldOut);
            writer.Close();
            ostrm.Close();
        }


        private string CurrentGroup = "GrupulMafiaHate";
        //Changing Console to a file
        FileStream ostrm;
        StreamWriter writer;
        TextWriter oldOut = Console.Out;
        //User Id to identify current user
        //User_Id CurrentUser = new User_Id() { Email = "marinaratoi@calutu.com", Password = "Parola", Cnp = "17896734543", Name = "Vasilica" };
        User_Id CurrentUser = new User_Id() { Email = "marinaratoi@calutu.com", Password = "Zeu", Cnp = "321241241", Name = "Bobo" };
        //event to lsiten to dbb
        EventStreamResponse FirebaseListener;
        //lsit of messages in the current conversation
        List<MessageData> MessageList = new List<MessageData>();
        //we use this last message to know what was the last Id
        MessageData LastMessage = new MessageData();
        //u will be used to read 4 lines at a time from listener and create MessageData objects
        short u = 1;
        //firebase configuration
        IFirebaseConfig config = new FirebaseConfig
        {
            AuthSecret = "kUkBSJNHvAhXv31pis29SDB11uD71edxr8x1TEwK",
            BasePath = "https://jasmin-chat-demo.firebaseio.com/"
        };
        //firebase client
        IFirebaseClient client;

        private async void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            FirebaseListener = await client.OnAsync("Groups/" + CurrentGroup + "/", (sender1, args, context) => {

                if (lstMesaje.InvokeRequired)
                {
                    this.addMesage(args);
                }
                else
                    lstMesaje.Items.Add(args.Data);
            });

            FirebaseResponse response = await client.GetAsync("Groups/"+CurrentGroup);
            try
            {
                List<MessageData> list = response.ResultAs<List<MessageData>>().ToList();
                foreach (var el in list)
                    if (el != null && list[0] != null && !el.Equals(list[0]))
                    {
                        MessageList.Add(el);
                        this.writeMessage(el);

                    }
            }
            catch { }
        }
    }
}
