import UnityEngine 
import numpy as np
from PIL import Image, ImageDraw, ImageFont


# UnityEngine.Debug.Log("Hello")
image_array = np.zeros((1140, 912, 3), dtype=np.uint8)

orderBitplane = [24,9,10,11,12,13,14,15,16,1,2,3,4,5,6,7,8,17,18,19,20,21,22,23]

for bitplane in range(0,24):
	nombre_archivo = f'./Assets/Shaders/Materials/BitPlanes/TextureAsPNG-'+str(bitplane)+'.png'
	imagen = Image.open(nombre_archivo)
	imagen_blackandwhite = imagen.convert("L")
	imagen.close()
	array_imagen = np.array(imagen_blackandwhite)
	array_imagen_binario = np.where(array_imagen < 10, np.uint8(0), np.uint8(255))
	# image_array[:, :, bitplane // 8] |=  (array_imagen_binario & 1) << (bitplane % 8)
	image_array[:, :, (orderBitplane[bitplane]-1) // 8] |=  (array_imagen_binario & 1) << ((orderBitplane[bitplane]-1) % 8)


	# img = Image.fromarray(image_array)
	# img.save('./Assets/Shaders/Materials/Construction/ReconstructedImage'+str(bitplane)+'.bmp')
	# img.close()



img = Image.fromarray(image_array)
img.save('./Assets/Shaders/Materials/ReconstructedImage.bmp')
img.close()
