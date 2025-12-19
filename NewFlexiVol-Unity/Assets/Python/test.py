import UnityEngine 
import numpy as np
from PIL import Image, ImageDraw, ImageFont
import h5py
from mat4py import loadmat

f = h5py.File('./Shaders/Materials/New Render Texture.mat', 'r')
print("Hello");
data = loadmat('./Shaders/Materials/New Render Texture.mat')

image_array = np.zeros((600, 600, 3), dtype=np.uint8)
for i in range(24):
	data = loadmat('./Shaders/Materials/New Render Texture.mat')
	array_imagen = np.array(data)

    # nombre_archivo = f'captura_{i+1:02d}.png'
    # imagen = Image.open(nombre_archivo)
    # # Convertir la imagen a escala de grises
    # imagen_gris = imagen.convert("L")
    # Convertir la imagen a un array numpy
    # array_imagen = np.array(imagen_gris)



    # Escalar los valores de los píxeles a 0 o 255 y convertir a uint8
    array_imagen_binario = np.where(array_imagen < 10, np.uint8(0), np.uint8(255))
    
    print(np.max(array_imagen))
    
    image_array[:, :, i // 8] |= (array_imagen_binario & 1) * (1 << (i % 8))
img = Image.fromarray(image_array)
img.save('binary_pattern_image.bmp')