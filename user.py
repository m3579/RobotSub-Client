import http.client
import urllib.parse
import msvcrt as visual_cpp

# The actual server is robotsub.herokuapp.com

conn = http.client.HTTPConnection("robotsub.herokuapp.com")

key_mapping = {"w": "forward", "a": "left", "s": "backward", "d": "right"}

while True:
    char = visual_cpp.getch().decode()

    if char == " ":

        params = urllib.parse.urlencode({"movement": "exit"})
        conn.request("POST", "/movement", params, {"Content-type": "application/x-www-form-urlencoded"})
        conn.getresponse().read()
        
        conn.close()
        break

    elif char not in key_mapping:
        continue
    
    params = urllib.parse.urlencode({"movement": key_mapping[char]})

    conn.request("POST", "/movement", params, {"Content-type": "application/x-www-form-urlencoded"})

    conn.getresponse().read()
