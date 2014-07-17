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
            string connectionString = "Data Source=localhost;Integrated Security=SSPI;Initial Catalog=Edart";
//            string connectionString = "Data Source=NZAKL009094N;Initial Catalog=Edart;User Id=sa;Password=1qaz!QAZ";
            SqlConnection myConnection = new SqlConnection(connectionString);

            var emailRunStarted = DateTime.Now;

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
                SqlCommand mySelectCommand = new SqlCommand("select * from [User]", myConnection);
                myReader = mySelectCommand.ExecuteReader();
                var messageSequence = 0;

                while (myReader.Read())
                {

                    var emailFrequency = Convert.ToInt32(myReader["EmailFrequency"].ToString());

                    Console.WriteLine("");
                    Console.WriteLine("========================================");

                    Console.WriteLine(myReader["Name"].ToString());
                    Console.WriteLine(myReader["Email"].ToString());
                    Console.WriteLine(emailFrequency);

                    var id = myReader["ID"].ToString();
                    var lastEmailSent = Convert.ToDateTime(myReader["EmailDate"].ToString());
                    var nextEmailDue = lastEmailSent;
                    bool validEmailFrequency = true;

                    string messageSubject;

                    messageSequence++;

                    switch (emailFrequency)
                    {
                        case 1:
                            messageSubject = "Today's Deals";
                            nextEmailDue = nextEmailDue.AddDays(1);
                            break;
                        case 2:
                            messageSubject = "This Week's Deals";
                            nextEmailDue = nextEmailDue.AddDays(7);
                            break;
                        case 3:
                            messageSubject = "This Fortnight's Deals";
                            nextEmailDue = nextEmailDue.AddDays(14);
                            break;
                        case 4:
                            messageSubject = "This Month's Deals";
                            nextEmailDue = nextEmailDue.AddMonths(1);
                            break;
                        case 5:
                            messageSubject = "a Deal";
                            break;
                        default:
                            messageSubject = "";
                            validEmailFrequency = false;
                            break;
                    }

                    if (validEmailFrequency == false)
                    {
                        Console.WriteLine("Skipping - Invalid Frequency");
                        continue;
                    }

                    if (emailFrequency == 5)
                    {
                        Console.WriteLine("Skipping - Opted Out");
                        continue;
                    }

                    Console.WriteLine("Email Run Started: {0} Next Email Due: {1}", emailRunStarted, nextEmailDue);

                    if (emailRunStarted.CompareTo(nextEmailDue) < 0)
                    {
                        Console.WriteLine("Skipping - Email Not Due - ({0})", nextEmailDue.ToString("dd/MM/yyyy"));
                        continue;
                    }

                        // Command line argument must the the SMTP host.
                        //            SmtpClient client = new SmtpClient(args[0]);
                        SmtpClient client = new SmtpClient("aumelcasarray.myob.myobcorp.net");
                        // Specify the e-mail sender. 
                        // Create a mailing address that includes a UTF8 character 
                        // in the display name.
//                        MailAddress from = new MailAddress("andrew.mills@myob.com", "Andrew Mills", System.Text.Encoding.UTF8);
                        MailAddress from = new MailAddress("honest.al@myob.com", "Honest Al", System.Text.Encoding.UTF8);
                        // Set destinations for the e-mail message.
                        MailAddress to = new MailAddress(myReader["Email"].ToString());
                        // Specify the message content.
                        MailMessage message = new MailMessage(from, to);
                        message.Body = "Ciao! " + myReader["Name"].ToString() + Environment.NewLine + Environment.NewLine;

                        SqlConnection myOfferConnection = new SqlConnection(connectionString);
                        try
                        {
                            myOfferConnection.Open();
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.ToString());
                        }

                        SqlDataReader myOfferReader = null;
                        SqlCommand myOfferCommand = new SqlCommand();
                        myOfferCommand.Connection = myOfferConnection;
                        myOfferCommand.CommandText =
                            "select u.Name, o.Title, o.Description, u.Location, u.Email " +
                            "from [Offer] o " +
                            "join [User] u on u.ID = o.UserID " +
                            "where o.ModifiedDate > @param1 " +
                            "and o.Status = 1";
                        Console.WriteLine("Command: {0}", myOfferCommand.CommandText);
//                        myOfferCommand.Parameters.AddWithValue("@param1", lastEmailSent.ToString("dd-MMM-yyyy hh:mm:ss"));
                        myOfferCommand.Parameters.AddWithValue("@param1", lastEmailSent);
                        Console.WriteLine("Parameter: {0}", myOfferCommand.Parameters[0].Value.ToString());
                        myOfferReader = myOfferCommand.ExecuteReader();

                        var firstDone = false;

                        // put offer output here.
                        while (myOfferReader.Read())
                        {
                            if (!firstDone)
                            {
                                message.Body += "Look at all the wonderful new things I have for you!";
                                message.Body += Environment.NewLine;
                                message.Body += Environment.NewLine;
                            }
                            firstDone = true;
                            message.Body += myOfferReader["Name"].ToString() + " has a " + myOfferReader["Title"].ToString();
                            message.Body += Environment.NewLine;
                            message.Body += myOfferReader["Description"].ToString();
                            message.Body += Environment.NewLine;
                            message.Body += myOfferReader["Location"].ToString() + " - " + myOfferReader["Email"].ToString();
                            message.Body += Environment.NewLine;
                            message.Body += Environment.NewLine;
                        }

                        myOfferConnection.Close();

                        message.Body += "Arrivederci," + Environment.NewLine + Environment.NewLine + "Al" + Environment.NewLine;

                        message.BodyEncoding = System.Text.Encoding.UTF8;
                        message.Subject = "Honest Al Has " + messageSubject + " For You!";
                        message.SubjectEncoding = System.Text.Encoding.UTF8;
                        // Set the method that is called back when the send operation ends.
                        client.SendCompleted += new SendCompletedEventHandler(SendCompletedCallback);
                        // The userState can be any object that allows your callback  
                        // method to identify this send operation. 
                        // For this example, the userToken is a string constant. 
                        string userState = "test message" + messageSequence.ToString();
                        Console.WriteLine("Sending message...");
                        client.Send(message);
                    SqlConnection myUpdateConnection = new SqlConnection(connectionString);
                    try
                    {
                        myUpdateConnection.Open();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.ToString());
                    }
                    SqlCommand myUpdateCommand = new SqlCommand();
                    myUpdateCommand.Connection = myUpdateConnection;
                    myUpdateCommand.CommandText = "update [User] set EmailDate = @param1 where id = @param2";
                    Console.WriteLine("Command: {0}", myUpdateCommand.CommandText);
                    myUpdateCommand.Parameters.AddWithValue("@param1", emailRunStarted);
                    myUpdateCommand.Parameters.AddWithValue("@param2", id);
                    myUpdateCommand.ExecuteNonQuery();
                    myUpdateConnection.Close();
                        //                        client.SendAsync(message, userState);
                        //                        Console.WriteLine("Sending message... press c to cancel mail. Press any other key to exit.");
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
            Console.WriteLine("");
            Console.WriteLine("Goodbye.");
        }

    }

}