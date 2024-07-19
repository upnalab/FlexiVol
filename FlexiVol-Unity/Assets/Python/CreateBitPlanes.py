import UnityEngine 
import numpy as np
from PIL import Image, ImageDraw, ImageFont
# import h5py
# from mat4py import loadmat

# f = h5py.File('./Shaders/Materials/New Render Texture.mat', 'r')

# UnityEngine.Debug.Log("Hello")
image_array = np.zeros((128, 128, 3), dtype=np.uint8)

nombre_archivo = f'./Assets/Shaders/Materials/TextureAsPNG.png'
imagen = Image.open(nombre_archivo)
array_imagen = np.array(imagen)

for bitplane in range(8):

    # data = loadmat('./Assets/Shaders/Materials/New Render Texture.mat')
    
    # Escalar los valores de los píxeles a 0 o 255 y convertir a uint8
    array_imagen_binario = np.where(array_imagen < 10, np.uint8(0), np.uint8(255))
    image_array[:, :, bitplane // 8] |=  (array_imagen_binario & 1) << (bitplane % 8)

img = Image.fromarray(image_array)
img.save('binary_pattern_image.bmp')