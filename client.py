# A program running on a Raspberry Pi on the EV3 side of the Robot Sub
# that will forward commands from the user to the EV3 via bluetooth.
#
# It does this by constantly polling a server to see if the user has
# said to move the EV3 in any direction.
#
# A computer on the user side will send instructions to the server,
# which will let the EV3 side know when it makes a request (a GET request
# for /movement).

import http.client

# The actual server is robotsub.herokuapp.com

conn = http.client.HTTPConnection("robotsub.herokuapp.com")

while True:
    conn.request("GET", "/movement")
    response = conn.getresponse().read()

    if response != b"none":
        print(response)
