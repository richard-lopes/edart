using System;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading;
using System.ComponentModel;
using System.Data.SqlClient;

namespace EdartEmailer
{

    public class Program
    {

        static bool mailSent = false;

        private static void SendCompletedCallback(object sender, AsyncCompletedEventArgs e)
        {
            // Get the unique identifier for this asynchronous operation.
             String token = (string) e.UserState;

            if (e.Cancelled)
            {
                 Console.WriteLine("[{0}] Send cancelled.", token);
            }
            if (e.Error != null)
            {
                 Console.WriteLine("[{0}] {1}", token, e.Error.ToString());
            } else
            {
                Console.WriteLine("Message sent.");
            }
            mailSent = true;
        }

        public static void Main(string[] args)
        {
            SqlConnection myConnection = new SqlConnection(
              "Data Source=localhost; " + 
              "Integrated Security=SSPI; " + 
              "Initial Catalog=edart"
              );

            try
            {
                myConnection.Open();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            try
            {
                SqlDataReader myReader = null;
                SqlCommand myCommand = new SqlCommand("select * from member",
                                                         myConnection);
                myReader = myCommand.ExecuteReader();
                while (myReader.Read())
                {

                    Console.WriteLine(myReader["name"].ToString());
                    Console.WriteLine(myReader["email"].ToString());
                    Console.WriteLine(myReader["email_frequency"].ToString());

//
// if email_date + frequency <= now send out an email
//

                    if (myReader["email_frequency"].ToString() == "1" || myReader["email_frequency"].ToString() == "2" || myReader["email_frequency"].ToString() == "3" || myReader["email_frequency"].ToString() == "4")
                    {
                        // Command line argument must the the SMTP host.
                        //            SmtpClient client = new SmtpClient(args[0]);
                        SmtpClient client = new SmtpClient("aumelcasarray.myob.myobcorp.net");
                        // Specify the e-mail sender. 
                        // Create a mailing address that includes a UTF8 character 
                        // in the display name.
                        MailAddress from = new MailAddress("andrew.mills@myob.com",
                           "Andrew Mills",
                        System.Text.Encoding.UTF8);
                        // Set destinations for the e-mail message.
                        MailAddress to = new MailAddress(myReader["email"].ToString());
                        // Specify the message content.
                        MailMessage message = new MailMessage(from, to);
                        message.Body = "Greetings, " + myReader["name"].ToString() + Environment.NewLine + Environment.NewLine;
                        message.Body += "This is a test e-mail message sent by Edart. ";
                        // Include some non-ASCII characters in body and subject. 
                        message.Body += Environment.NewLine;
                        message.BodyEncoding = System.Text.Encoding.UTF8;
                        switch (myReader["email_frequency"].ToString())
                        {
                            case "1":
                                message.Subject = "Daily ";
                                break;
                            case "2":
                                message.Subject = "Weekly ";
                                break;
                            case "3":
                                message.Subject = "Fortnightly ";
                                break;
                            case "4":
                                message.Subject = "Monthly ";
                                break;
                            default:
                                message.Subject = "";
                                break;
                        }
                        message.Subject += "Edart Update";
                        message.SubjectEncoding = System.Text.Encoding.UTF8;
                        // Set the method that is called back when the send operation ends.
                        client.SendCompleted += new SendCompletedEventHandler(SendCompletedCallback);
                        // The userState can be any object that allows your callback  
                        // method to identify this send operation. 
                        // For this example, the userToken is a string constant. 
                        string userState = "test message1";
                        client.SendAsync(message, userState);
//                        Console.WriteLine("Sending message... press c to cancel mail. Press any other key to exit.");
                        Console.WriteLine("Sending message...");
//                        string answer = Console.ReadLine();
                        // If the user canceled the send, and mail hasn't been sent yet, 
                        // then cancel the pending operation.
//                        if (answer.StartsWith("c") && mailSent == false)
//                        {
//                            client.SendAsyncCancel();
//                        }
                        // Clean up.
                        message.Dispose();
                        mailSent = false;
                    }
                    else
                    {
                        Console.WriteLine("Skipped");
                    }
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            } 
            
            try
            {
                myConnection.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            } 
            Console.WriteLine("Goodbye.");
        }

    }

}