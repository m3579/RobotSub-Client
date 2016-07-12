using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using MonoBrick.EV3;

namespace EV3Side
{
    /// <summary>
    /// The class containing the entry point into the EV3 side of
    /// Robot Sub
    /// </summary>
    class Program
    {
        /// <summary>
        /// The object representing the EV3
        /// </summary>
        private Brick<Sensor, Sensor, Sensor, Sensor> ev3;

        /// <summary>
        /// A string representing a COM port that the EV3 will
        /// be connected to via bluetooth.
        /// </summary>
        private string comPort;

        /// <summary>
        /// The IP address or domain name of the server that will
        /// inform us when we need to move the robot.
        /// </summary>
        private const string SERVER_ADDRESS = "robotsub.herokuapp.com";

        /// <summary>
        /// The GET URL for the GET requests that we will make to
        /// the server asking what movement the user on the other side
        /// has instructed the robot to do.
        /// </summary>
        private const string GET_REQUEST_URL = "/movement";

        /// <summary>
        /// The output port for the left motor of the robot
        /// </summary>
        private const MotorPort LEFT_PORT = MotorPort.OutA;

        /// <summary>
        /// The output port for the right motor of the robot
        /// </summary>
        private const MotorPort RIGHT_PORT = MotorPort.OutD;

        /// <summary>
        /// The speed that the robot will be moving at as a percentage
        /// </summary>
        private const sbyte SPEED = 50;

        // The following constants are strings representing
        // the ways that the robot can move. These same string values
        // are used throughout the EV3 side, user side, and the server.

        private const string MOVEMENT_FORWARD = "forward";
        private const string MOVEMENT_RIGHT = "right";
        private const string MOVEMENT_LEFT = "left";
        private const string MOVEMENT_BACKWARD = "backward";
        private const string MOVEMENT_NONE = "none";

        /// <summary>
        /// The entry point into the application (although the actual
        /// code for the entry point is in the Go() method.
        /// </summary>
        static void Main(string[] args)
        {
            new Program().Go();
        }

        /// <summary>
        /// A non-static method called in the Main function (in essence,
        /// the actual entry point into the application)
        /// </summary>
        private void Go()
        {
            //InitEV3();

            StartListeningToServer().Wait();
        }

        /// <summary>
        /// Create the EV3 object and start the bluetooth connection
        /// to it
        /// </summary>
        private void InitEV3()
        {
            //comPort = GetCOMPort();

            //ev3 = new Brick<Sensor, Sensor, Sensor, Sensor>(comPort);

            //try
            //{
            //    ev3.Connection.Open();

            //    ev3.Vehicle.LeftPort = LEFT_PORT;
            //    ev3.Vehicle.RightPort = RIGHT_PORT;

            //    ev3.Vehicle.ReverseLeft = false;
            //    ev3.Vehicle.ReverseRight = false;
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine();
            //    Console.WriteLine("ERROR ERROR ERROR ERROR");
            //    Console.WriteLine("Error: " + ex.Message);
            //    Console.WriteLine(ex.StackTrace);
            //    Console.WriteLine("Press any key to end...");
            //    Console.ReadKey();
            //}
        }

        /// <summary>
        /// A method to ask the user for the COM port that the EV3
        /// is connected to via bluetooth.
        /// </summary>
        /// <returns></returns>
        private string GetCOMPort()
        {
            // First, ask the user for the COM port
            do
            {
                Console.Write("Please enter the COM port that the EV3 is connected to in lowercase: ");
                comPort = Console.ReadLine();
            }
            // Check if what the user entered is a valid COM port
            // If it is not, then repeat
            while (!CheckCOMPort(comPort));

            return comPort;
        }

        /// <summary>
        /// Check to make sure the given COM port is an actual COM
        /// port
        /// </summary>
        /// <param name="comPort">The COM port to check</param>
        /// <returns></returns>
        private bool CheckCOMPort(string comPort)
        {
            // TODO: implement checking of the COM port

            if (comPort != "")
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Start making requests to the server that will inform this application
        /// when the person on the other side instructs the robot
        /// to move.
        /// </summary>
        private async Task StartListeningToServer()
        {
            // TODO: handle when the connection has closed

            using (HttpClient client = new HttpClient())
            {
                // The "current movement" variable in the server is only reset to "none" after
                // a GET. So if I closed this app before closing the user's command-entering-app
                // and the user entered a command, that command would be stored in the
                // server until I GETed it (for example, if after I closed this app, the user
                // entered a "forward" command, then even if I GETed from the server after a day that forward
                // command would still be there to run forward).
                // This GET is to clear the "current movement" variable in the server so that a command
                // entered after this app was closed does not take effect.
                await client.GetStringAsync("http://" + SERVER_ADDRESS + GET_REQUEST_URL);

                string currentMovement = MOVEMENT_NONE;

                while (true)
                {
                    string responseString = await client.GetStringAsync("http://" + SERVER_ADDRESS + GET_REQUEST_URL);

                    if (responseString != MOVEMENT_NONE)
                    {
                        Console.WriteLine(responseString);
                    }

                    // TODO: this is only for testing
                    continue;

                    switch (responseString)
                    {
                        case MOVEMENT_FORWARD:
                            {
                                if (currentMovement != MOVEMENT_FORWARD)
                                {
                                    ev3.Vehicle.Forward(SPEED);
                                }

                                break;
                            }


                        case MOVEMENT_RIGHT:
                            {
                                if (currentMovement != MOVEMENT_RIGHT)
                                {
                                    ev3.Vehicle.SpinRight(SPEED);
                                }

                                break;
                            }


                        case MOVEMENT_LEFT:
                            {
                                if (currentMovement != MOVEMENT_LEFT)
                                {
                                    ev3.Vehicle.SpinLeft(SPEED);
                                }

                                break;
                            }

                        case MOVEMENT_BACKWARD:
                            {
                                if (currentMovement != MOVEMENT_BACKWARD)
                                {
                                    ev3.Vehicle.Backward(SPEED);
                                }

                                break;
                            }

                        case MOVEMENT_NONE:
                            {
                                if (currentMovement != MOVEMENT_NONE)
                                {
                                    // TODO: see if this should be
                                    // ev3.Vehicle.Brake()
                                    ev3.Vehicle.Off();
                                }

                                break;
                            }

                        default:
                            {
                                Console.WriteLine("Unrecognized movement: " + responseString);

                                break;
                            }
                    }
                }
            }
        }
    }
}