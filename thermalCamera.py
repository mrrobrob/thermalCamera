# this is based on https://makersportal.com/blog/2020/6/8/high-resolution-thermal-camera-with-raspberry-pi-and-mlx90640
##################################
# MLX90640 Test with Raspberry Pi
##################################

import time,board,busio
import numpy as np
import adafruit_mlx90640

i2c = busio.I2C(board.SCL, board.SDA, frequency=400000) # setup I2C
mlx = adafruit_mlx90640.MLX90640(i2c) # begin MLX90640 with I2C comm
mlx.refresh_rate = adafruit_mlx90640.RefreshRate.REFRESH_16_HZ # set refresh rate

frame = np.zeros((24*32,)) # setup array for storing all 768 temperatures
while True:
    try:        
        mlx.getFrame(frame) # read MLX temperatures into frame var
        print("frame start")
        for x in frame:
            print(format(x))
        print("frame done")
    except ValueError:
        continue # if error, just read again
