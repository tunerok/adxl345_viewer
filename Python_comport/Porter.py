import serial

ser = None


def connect(serial_port_name, serial_port_speed):

    global ser

    port_name = 'COM' + serial_port_name
    ser = serial.Serial(port_name, serial_port_speed, timeout=0)

    print("Connected at port %s with speed %s " % (port_name, serial_port_speed))

def