import http.server
import urllib.parse
import time

HOST_NAME = "127.0.0.1"
PORT_NUMBER = 9000

current_movement = b"none"

class RequestHandler(http.server.BaseHTTPRequestHandler):

    def do_HEAD(self):
        self.send_response(200)
        self.send_header("Content-type", "text/plain")
        self.end_headers()


    def do_GET(self):
        global current_movement
        
        self.send_response(200)
        self.send_header("Content-type", "text/plain")
        self.end_headers()

        self.wfile.write(current_movement)
        current_movement = b"none"
        

    def do_POST(self):
        global current_movement
        
        self.send_response(200)
        self.send_header("Content-type", "text/plain")
        self.end_headers()

        raw_data = self.rfile.read(int(self.headers["Content-length"]))
        args = urllib.parse.parse_qs(raw_data)

        current_movement = args[b"movement"][0]
        
    
httpd = http.server.HTTPServer((HOST_NAME, PORT_NUMBER), RequestHandler)

print(time.asctime(), "Server Starts - %s:%s" % (HOST_NAME, PORT_NUMBER))

try:
    httpd.serve_forever()
except KeyboardInterrupt:
    pass

httpd.server_close()
print(time.asctime(), "Server Stops - %s:%s" % (HOST_NAME, PORT_NUMBER))
