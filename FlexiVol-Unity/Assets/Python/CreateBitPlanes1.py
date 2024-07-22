import UnityEngine 
import numpy as np
from PIL import Image, ImageDraw, ImageFont
# import h5py
# from mat4py import loadmat

# f = h5py.File('./Shaders/Materials/New Render Texture.mat', 'r')

# UnityEngine.Debug.Log("Hello")
image_array = np.zeros((600, 600, 3), dtype=np.uint8)
nombre_archivo = f'./Assets/Shaders/Materials/TextureAsPNG1.png'
# imagen = Image.open(nombre_archivo)
# array_imagen = np.array(imagen)

for bitplane in range(24):
    # data = loadmat('./Assets/Shaders/Materials/New Render Texture.mat')
    imagen = Image.open(nombre_archivo)
    imagen_blackandwhite = imagen.convert("L")
    imagen.close()
    array_imagen = np.array(imagen_blackandwhite)
    # UnityEngine.Debug.Log(array_imagen[:,60])

    # Escalar los valores de los píxeles a 0 o 255 y convertir a uint8
    array_imagen_binario = np.where(array_imagen < 10, np.uint8(0), np.uint8(255))
    image_array[:, :, bitplane // 8] |=  (array_imagen_binario & 1) << (bitplane % 8)
    img = Image.fromarray(image_array)
    img.save('./Assets/Shaders/Materials/bitPlanedImage.bmp')